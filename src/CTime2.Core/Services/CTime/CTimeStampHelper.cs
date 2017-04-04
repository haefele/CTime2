using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using UwCore.Common;
using UwCore.Logging;
using UwCore.Services.ApplicationState;
using UwCore.Services.Clock;

namespace CTime2.Core.Services.CTime
{
    public class CTimeStampHelper
    {
        #region Logger
        private static readonly ILog Logger = LogManager.GetLog(typeof(CTimeStampHelper));
        #endregion

        #region Fields
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IClock _clock;
        #endregion

        #region Constructors
        public CTimeStampHelper(IApplicationStateService applicationStateService, ICTimeService cTimeService, IClock clock)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(clock, nameof(clock));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._clock = clock;
        }
        #endregion

        public async Task Stamp(ICTimeStampHelperCallback callback, TimeState timeState)
        {
            Guard.NotNull(callback, nameof(callback));

            await this._applicationStateService.RestoreStateAsync();

            if (this._applicationStateService.GetCurrentUser() == null)
            {
                Logger.Info("User is not logged in.");
                await callback.OnNotLoggedIn();

                return;
            }
            
            bool checkedIn = await this._cTimeService.IsCurrentlyCheckedIn(this._applicationStateService.GetCurrentUser().Id);

            if (checkedIn && timeState.IsEntered())
            {
                if (callback.SupportsQuestions())
                {
                    Logger.Info("User wants to check-in. But he is already. Asking him if he wants to check-out instead.");
                    var checkOutResult = await callback.OnAlreadyCheckedInWannaCheckOut();

                    if (checkOutResult == false)
                    {
                        Logger.Info("User does not want to check-out. Doing nothing.");
                        await callback.OnDidNothing();

                        return;
                    }

                    timeState = TimeState.Left;
                }
                else
                {
                    Logger.Info("User wants to check-in. But he is already. Doing nothing.");
                    await callback.OnAlreadyCheckedIn();

                    return;
                }
            }

            if (checkedIn == false && timeState.IsLeft())
            {
                if (callback.SupportsQuestions())
                {
                    Logger.Info("User wants to check-out. But he is already. Asking him if he wants to check-in instead.");
                    var checkInResult = await callback.OnAlreadyCheckedOutWannaCheckIn();

                    if (checkInResult == false)
                    {
                        Logger.Info("User does not want to check-in. Doing nothing.");
                        await callback.OnDidNothing();

                        return;
                    }

                    timeState = TimeState.Entered;
                }
                else
                {
                    Logger.Info("User wants to check-out. But he is already. Doing nothing.");
                    await callback.OnAlreadyCheckedOut();

                    return;
                }
            }

            Logger.Info("Saving the timer.");
            await this._cTimeService.SaveTimer(
                this._applicationStateService.GetCurrentUser(),
                this._clock.Now().DateTime,
                timeState);

            Logger.Info("Finished voice command.");
            await callback.OnSuccess(timeState);
        } 
    }
}