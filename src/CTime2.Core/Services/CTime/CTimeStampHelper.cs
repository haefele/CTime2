using System;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using UwCore.Logging;
using UwCore.Services.ApplicationState;

namespace CTime2.Core.Services.CTime
{
    public class CTimeStampHelper
    {
        #region Logger
        private static readonly Logger _logger = LoggerFactory.GetLogger<CTimeStampHelper>();
        #endregion

        #region Fields
        private readonly IApplicationStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        #endregion

        #region Constructors
        public CTimeStampHelper(IApplicationStateService sessionStateService, ICTimeService cTimeService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
        }
        #endregion

        public async Task Stamp(ICTimeStampHelperCallback callback, TimeState timeState)
        {
            await this._sessionStateService.RestoreStateAsync();

            if (this._sessionStateService.GetCurrentUser() == null)
            {
                _logger.Debug(() => "User is not logged in.");
                await callback.OnNotLoggedIn();

                return;
            }
            
            var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.GetCurrentUser().Id);
            bool checkedIn = currentTime != null && currentTime.State.IsEntered();

            if (checkedIn && timeState.IsEntered())
            {
                if (callback.SupportsQuestions())
                {
                    _logger.Debug(() => "User wants to check-in. But he is already. Asking him if he wants to check-out instead.");
                    var checkOutResult = await callback.OnAlreadyCheckedInWannaCheckOut();

                    if (checkOutResult == false)
                    {
                        _logger.Debug(() => "User does not want to check-out. Doing nothing.");
                        await callback.OnDidNothing();

                        return;
                    }

                    timeState = TimeState.Left;
                }
                else
                {
                    _logger.Debug(() => "User wants to check-in. But he is already. Doing nothing.");
                    await callback.OnAlreadyCheckedIn();

                    return;
                }
            }

            if (checkedIn == false && timeState.IsLeft())
            {
                if (callback.SupportsQuestions())
                {
                    _logger.Debug(() => "User wants to check-out. But he is already. Asking him if he wants to check-in instead.");
                    var checkInResult = await callback.OnAlreadyCheckedOutWannaCheckIn();

                    if (checkInResult == false)
                    {
                        _logger.Debug(() => "User does not want to check-in. Doing nothing.");
                        await callback.OnDidNothing();

                        return;
                    }

                    timeState = TimeState.Entered;
                }
                else
                {
                    _logger.Debug(() => "User wants to check-out. But he is already. Doing nothing.");
                    await callback.OnAlreadyCheckedOut();

                    return;
                }
            }

            if (timeState.IsLeft())
            {
                _logger.Debug(() => "User is checking out. Updating the TimeState to make him check out what he previously checked in (Normal, Trip or Home-Office).");
                if (currentTime.State.IsTrip())
                {
                    _logger.Debug(() => "User checked-in a trip. Update the TimeState to make him check out a trip.");
                    timeState = timeState | TimeState.Trip;
                }

                if (currentTime.State.IsHomeOffice())
                {
                    _logger.Debug(() => "User checked-in home-office. Update the TimeState to make him check out home-office.");
                    timeState = timeState | TimeState.HomeOffice;
                }
            }

            _logger.Debug(() => "Saving the timer.");
            await this._cTimeService.SaveTimer(
                this._sessionStateService.GetCurrentUser().Id, 
                DateTime.Now, 
                this._sessionStateService.GetCurrentUser().CompanyId, 
                timeState);

            _logger.Debug(() => "Finished voice command.");
            await callback.OnSuccess(timeState);
        } 
    }
}