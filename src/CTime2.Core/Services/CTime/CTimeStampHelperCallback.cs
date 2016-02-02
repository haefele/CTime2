using System;
using System.Threading.Tasks;
using CTime2.Core.Data;

namespace CTime2.Core.Services.CTime
{
    public class CTimeStampHelperCallback : ICTimeStampHelperCallback
    {
        private readonly Func<Task> _onNotLoggedIn;
        private readonly Func<bool> _supportsQuestions;
        private readonly Func<Task> _onDidNothing;
        private readonly Func<Task<bool>> _onAlreadyCheckedInWannaCheckOut;
        private readonly Func<Task> _onAlreadyCheckedIn;
        private readonly Func<Task<bool>> _onAlreadyCheckedOutWannaCheckIn;
        private readonly Func<Task> _onAlreadyCheckedOut;
        private readonly Func<TimeState, Task> _onSuccess;

        public CTimeStampHelperCallback(
            Func<Task> onNotLoggedIn, 
            Func<bool> supportsQuestions, 
            Func<Task> onDidNothing, 
            Func<Task<bool>> onAlreadyCheckedInWannaCheckOut, 
            Func<Task> onAlreadyCheckedIn,
            Func<Task<bool>> onAlreadyCheckedOutWannaCheckIn,
            Func<Task> onAlreadyCheckedOut,
            Func<TimeState, Task> onSuccess)
        {
            this._onNotLoggedIn = onNotLoggedIn;
            this._supportsQuestions = supportsQuestions;
            this._onDidNothing = onDidNothing;
            this._onAlreadyCheckedInWannaCheckOut = onAlreadyCheckedInWannaCheckOut;
            this._onAlreadyCheckedIn = onAlreadyCheckedIn;
            this._onAlreadyCheckedOutWannaCheckIn = onAlreadyCheckedOutWannaCheckIn;
            this._onAlreadyCheckedOut = onAlreadyCheckedOut;
            this._onSuccess = onSuccess;
        }

        public Task OnNotLoggedIn() => this._onNotLoggedIn();

        public bool SupportsQuestions() => this._supportsQuestions();

        public Task OnDidNothing() => this._onDidNothing();

        public Task<bool> OnAlreadyCheckedInWannaCheckOut() => this._onAlreadyCheckedInWannaCheckOut();

        public Task OnAlreadyCheckedIn() => this._onAlreadyCheckedIn();

        public Task<bool> OnAlreadyCheckedOutWannaCheckIn() => this._onAlreadyCheckedOutWannaCheckIn();

        public Task OnAlreadyCheckedOut() => this._onAlreadyCheckedOut();

        public Task OnSuccess(TimeState timeState) => this._onSuccess(timeState);
    }
}