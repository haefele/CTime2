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
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        #endregion

        #region Constructors
        public CTimeStampHelper(IApplicationStateService applicationStateService, ICTimeService cTimeService)
        {
            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
        }
        #endregion

        public async Task Stamp(ICTimeStampHelperCallback callback, TimeState timeState)
        {
            await this._applicationStateService.RestoreStateAsync();

            if (this._applicationStateService.GetCurrentUser() == null)
            {
                _logger.Debug(() => "User is not logged in.");
                await callback.OnNotLoggedIn();

                return;
            }
            
            bool checkedIn = await this._cTimeService.IsCurrentlyCheckedIn(this._applicationStateService.GetCurrentUser().Id);

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

            _logger.Debug(() => "Saving the timer.");
            await this._cTimeService.SaveTimer(
                this._applicationStateService.GetCurrentUser(),
                timeState);

            _logger.Debug(() => "Finished voice command.");
            await callback.OnSuccess(timeState);
        } 
    }
}