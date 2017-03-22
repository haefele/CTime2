using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Caliburn.Micro;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.CTime.RequestCache;
using CTime2.Core.Services.GeoLocation;
using UwCore.Services.ApplicationState;
using EmpNotificationService = CTime2.Core.Services.EmployeeNotification.EmployeeNotificationService;

namespace CTime2.EmployeeNotificationService
{
    public sealed class CTime2EmployeeNotificationService : IBackgroundTask
    {
        #region Logger
        private static readonly ILog Logger = LogManager.GetLog(typeof(CTime2EmployeeNotificationService));
        #endregion

        #region Fields
        private BackgroundTaskDeferral _deferral;
        #endregion

        #region Methods
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                this._deferral = taskInstance.GetDeferral();

                taskInstance.Canceled += (s, e) => this.Close();

                var applicationStateService = new ApplicationStateService();
                await applicationStateService.RestoreStateAsync();

                var eventAggregator = new EventAggregator();
                var geoLocationService = new GeoLocationService();
                var requestCache = new NullCTimeRequestCache();
                var ctimeService = new CTimeService(requestCache, eventAggregator, applicationStateService, geoLocationService);
                var employeeNotificationService = new EmpNotificationService(applicationStateService, ctimeService);

                await employeeNotificationService.SendNotificationsAsync();

                await applicationStateService.SaveStateAsync();
            }
            catch (Exception exception)
            {
                Logger.Warn("Exception occured in the employee-notification service.");
                Logger.Error(exception);
            }
            finally
            {
                this.Close();
            }
        }
        #endregion

        #region Private Methods
        private void Close()
        {
            this._deferral.Complete();
        }
        #endregion
    }
}
