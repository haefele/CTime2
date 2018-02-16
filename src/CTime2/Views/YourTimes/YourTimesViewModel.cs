using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using CTime2.Core.Data;
using CTime2.Core.Extensions;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.Email;
using CTime2.Core.Services.Sharing;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Clock;

namespace CTime2.Views.YourTimes
{
    public class YourTimesViewModel : UwCoreScreen
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly ISharingService _sharingService;
        private readonly IClock _clock;
        private readonly IEmailService _emailService;

        private TimesByDay _selectedDayForReportMissingTime;
        private readonly ObservableAsPropertyHelper<ReactiveList<TimesByDay>> _timesHelper;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public TimesByDay SelectedDayForReportMissingTime
        {
            get { return this._selectedDayForReportMissingTime; }
            set { this.RaiseAndSetIfChanged(ref this._selectedDayForReportMissingTime, value); }
        }

        public ReactiveList<TimesByDay> Times => this._timesHelper.Value;

        public DateTimeOffset StartDate
        {
            get { return this._startDate; }
            set { this.RaiseAndSetIfChanged(ref this._startDate, value); }
        }

        public DateTimeOffset EndDate
        {
            get { return this._endDate; }
            set { this.RaiseAndSetIfChanged(ref this._endDate, value); }
        }

        public Parameters Parameter { get; set; }

        public UwCoreCommand<ReactiveList<TimesByDay>> LoadTimes { get; }
        public UwCoreCommand<Unit> Share { get; }
        public UwCoreCommand<Unit> ReportMissingTimes { get; }
        public UwCoreCommand<Unit> ReportMissingTimeForSelectedDay { get; }

        public YourTimesViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, ISharingService sharingService, IClock clock, IEmailService emailService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(sharingService, nameof(sharingService));
            Guard.NotNull(clock, nameof(clock));
            Guard.NotNull(emailService, nameof(emailService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._sharingService = sharingService;
            this._clock = clock;
            this._emailService = emailService;

            this.WhenAnyValue(f => f.StartDate, f => f.EndDate)
                .Select(f => CTime2Resources.GetFormatted("MyTimes.TitleFormat", this.StartDate, this.EndDate))
                .Subscribe(name => this.DisplayName = name);

            this.LoadTimes = UwCoreCommand.Create(this.LoadTimesImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.Times"))
                .HandleExceptions()
                .TrackEvent("LoadTimes");
            this.LoadTimes.ToProperty(this, f => f.Times, out this._timesHelper);

            this.Share = UwCoreCommand.Create(this.ShareImpl)
                .HandleExceptions()
                .TrackEvent("ShareMyTimes");

            var canReportMissingTimes = this.WhenAnyValue(f => f.Times)
                .Where(f => f != null)
                .Where(f => f.Any(d => d.IsMissing))
                .Any();
            this.ReportMissingTimes = UwCoreCommand.Create(canReportMissingTimes, this.ReportMissingTimesImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.ReportMissingTimes"))
                .HandleExceptions()
                .TrackEvent("ReportMissingTimes");

            var canReportMissingTimesSelectedDay = this.WhenAnyValue(f => f.SelectedDayForReportMissingTime)
                .Select(f => f != null);
            this.ReportMissingTimeForSelectedDay = UwCoreCommand.Create(canReportMissingTimesSelectedDay, this.ReportMissingTimeForSelectedDayImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.ReportMissingTimes"))
                .HandleExceptions()
                .TrackEvent("ReportMissingTimesSelectedDay");

            this.StartDate = this._clock.Now().StartOfMonth();
            this.EndDate = this._clock.Now().WithoutTime();
        }

        protected override void SaveState(IApplicationStateService applicationStateService)
        {
            base.SaveState(applicationStateService);

            applicationStateService.Set(nameof(this.StartDate), this.StartDate, ApplicationState.Temp);
            applicationStateService.Set(nameof(this.EndDate), this.EndDate, ApplicationState.Temp);
        }

        protected override void RestoreState(IApplicationStateService applicationStateService)
        {
            base.RestoreState(applicationStateService);

            if (applicationStateService.HasValueFor(nameof(this.StartDate), ApplicationState.Temp))
                this.StartDate = applicationStateService.Get<DateTimeOffset>(nameof(this.StartDate), ApplicationState.Temp);

            if (applicationStateService.HasValueFor(nameof(this.EndDate), ApplicationState.Temp))
                this.EndDate = applicationStateService.Get<DateTimeOffset>(nameof(this.EndDate), ApplicationState.Temp);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            if (this.Parameter != null)
            {
                this.StartDate = this.Parameter.StartDate;
                this.EndDate = this.Parameter.EndDate;
            }
            
            await this.LoadTimes.ExecuteAsync();
        }
        
        private async Task<ReactiveList<TimesByDay>> LoadTimesImpl()
        {
            var today = this._clock.Today();
            var workDays = this._applicationStateService.GetWorkDays();

            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);
            return new ReactiveList<TimesByDay>(TimesByDay.Create(times, workDays, today));
        }
        
        private Task ShareImpl()
        {
            this._sharingService.Share(this.DisplayName, package =>
            {
                var message = new StringBuilder();
                foreach (TimesByDay timeByDay in this.Times.EmptyIfNull()) //If an error occured while loading the times, they might be NULL
                {
                    message.AppendLine(timeByDay.ToString());
                    message.AppendLine();
                }

                package.SetText(message.ToString());
            });

            return Task.CompletedTask;
        }

        private async Task ReportMissingTimesImpl()
        {
            await this.ReportMissingTimesInternal(this.Times.ToArray());
        }

        private async Task ReportMissingTimeForSelectedDayImpl()
        {
            await this.ReportMissingTimesInternal(this.SelectedDayForReportMissingTime);
        }

        private async Task ReportMissingTimesInternal(params TimesByDay[] times)
        {
            var missingDays = times
                .Where(f => f.IsMissing)
                .Select(f => f.Day.ToString("d"));

            var email = new EmailMessage();
            email.Subject = CTime2Resources.Get("ReportMissingDaysEmail.Subject");
            email.Body = Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, missingDays);

            var missingDaysEmailReceiver = this._applicationStateService.GetMissingDaysEmailReceiver();
            if (string.IsNullOrWhiteSpace(missingDaysEmailReceiver) == false)
                email.To.Add(new EmailRecipient(missingDaysEmailReceiver));

            await this._emailService.SendEmailAsync(email);
        }

        #region Internal
        public class Parameters
        {
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
        }
        #endregion
    }
}