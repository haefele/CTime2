using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.Statistics;
using CTime2.Core.Strings;
using Microsoft.Toolkit.Uwp.Notifications;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Events;
using UwCore.Services.ApplicationState;

namespace CTime2.Core.Services.Tile
{
    [AutoSubscribeEvents]
    public class TileService : ITileService, IHandleWithTask<ApplicationResumed>, IHandleWithTask<UserStamped>, IHandleWithTask<ShellModeEntered>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IStatisticsService _statisticsService;

        public TileService(IApplicationStateService applicationStateService, ICTimeService cTimeService, IStatisticsService statisticsService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(statisticsService, nameof(statisticsService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._statisticsService = statisticsService;
        }
        
        public async Task UpdateLiveTileAsync()
        {
            var updateManager = TileUpdateManager.CreateTileUpdaterForApplication();

            var currentUser = this._applicationStateService.GetCurrentUser();
            if (currentUser == null)
            {
                updateManager.Clear();
                return;
            }

            var timesTodayTask = this._cTimeService.GetTimes(currentUser.Id, DateTime.Today, DateTime.Today);
            var currentTimeTask = this._cTimeService.GetCurrentTime(currentUser.Id);

            await Task.WhenAll(timesTodayTask, currentTimeTask);

            var timesToday = TimesByDay.Create(await timesTodayTask).FirstOrDefault();
            var currentTime = await currentTimeTask;

            TileContent content = new TileContent
            {
                Visual = new TileVisual
                {
                    Branding = TileBranding.NameAndLogo,
                    TileMedium = this.GetTileMedium(currentTime, timesToday),
                    TileWide = this.GetTileWide(currentTime, timesToday),
                },
            };

            var notification = new TileNotification(content.GetXml());

            updateManager.Update(notification);
        }
        
        private TileBinding GetTileWide(Time currentTime, TimesByDay today)
        {
            var group = new AdaptiveGroup();

            foreach (var part in this.GetTileParts(currentTime, today).Take(2))
            {
                group.Children.Add(part);
            }
            
            return new TileBinding
            {
                Content = new TileBindingContentAdaptive
                {
                    Children =
                    {
                        group
                    }
                }
            };
        }

        private TileBinding GetTileMedium(Time currentTime, TimesByDay today)
        {
            var content = new TileBindingContentAdaptive();

            foreach (var part in this.GetTileParts(currentTime, today))
            {
                content.Children.Add(new AdaptiveGroup { Children = { part }});
            }

            return new TileBinding { Content = content };
        }

        #region Tile Parts
        private IEnumerable<AdaptiveSubgroup> GetTileParts(Time currentTime, TimesByDay today)
        {
            var parts = new Func<Time, TimesByDay, AdaptiveSubgroup>[]
            {
                this.CreateRunningTimeGroup,
                this.CreateTimesTodayGroup,
            };

            foreach (var part in parts)
            {
                var group = part(currentTime, today);

                if (group != null)
                    yield return group;
            }
        }

        private AdaptiveSubgroup CreateRunningTimeGroup(Time currentTime, TimesByDay today)
        {
            if (currentTime.State.IsEntered() == false)
                return null;

            var statistics = this._statisticsService.CalculateCurrentTime(currentTime);

            var group = new AdaptiveSubgroup
            {
                Children =
                {
                    new AdaptiveText
                    {
                        Text = CTime2CoreResources.Get("LiveTile.WorkTime")
                    },
                    new AdaptiveText
                    {
                        Text = $"> {statistics.WorkTime.Hours} h {this.RoundDownTo5(statistics.WorkTime.Minutes)} min",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
            };

            if (statistics.OverTime != null)
            {
                group.Children.Add(new AdaptiveText
                {
                    Text = CTime2CoreResources.Get("LiveTile.OverTime")
                });
                group.Children.Add(new AdaptiveText
                {
                    Text = $"> {statistics.OverTime.Value.Hours} h {this.RoundDownTo5(statistics.OverTime.Value.Minutes)} min",
                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                });
            }

            if (statistics.BreakTime != null)
            {
                group.Children.Add(new AdaptiveText
                {
                    Text = CTime2CoreResources.Get("LiveTile.Break")
                });
                group.Children.Add(new AdaptiveText
                {
                    Text = $"> {statistics.BreakTime.Value.Hours} h {this.RoundDownTo5(statistics.BreakTime.Value.Minutes)} min",
                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                });
            }

            if (statistics.PreferredBreakTimeEnd != null)
            {
                group.Children.Add(new AdaptiveText
                {
                    Text = CTime2CoreResources.Get("LiveTile.BreakEnd")
                });
                group.Children.Add(new AdaptiveText
                {
                    Text = $"{statistics.PreferredBreakTimeEnd.Value:t}",
                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                });
            }

            return group;
        }

        private AdaptiveSubgroup CreateTimesTodayGroup(Time currentTime, TimesByDay today)
        {
            var group = new AdaptiveSubgroup
            {
                Children =
                {
                    new AdaptiveText
                    {
                        Text = CTime2CoreResources.Get("LiveTile.TimesToday")
                    }
                }
            };

            foreach (var time in today.Times)
            {
                group.Children.Add(new AdaptiveText
                {
                    Text = $"{time.ClockInTime?.ToString("t") ?? "?"} - {time.ClockOutTime?.ToString("t") ?? "?"}",
                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                });
            }

            return group;
        }
        
        private int RoundDownTo5(int value)
        {
            return (value / 5) * 5;
        }
        #endregion

        #region Events
        Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            return this.UpdateLiveTileAsync();
        }
        Task IHandleWithTask<UserStamped>.Handle(UserStamped message)
        {
            return this.UpdateLiveTileAsync();
        }
        Task IHandleWithTask<ShellModeEntered>.Handle(ShellModeEntered message)
        {
            return this.UpdateLiveTileAsync();
        }
        #endregion
    }
}