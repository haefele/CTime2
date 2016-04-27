using System;
using JetBrains.Annotations;

namespace CTime2.Core.Common
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _actionToExecuteOnDispose;

        public DisposableAction([CanBeNull]Action actionToExecuteOnDispose)
        {
            this._actionToExecuteOnDispose = actionToExecuteOnDispose;
        }

        public void Dispose()
        {
            this._actionToExecuteOnDispose?.Invoke();
        }
    }
}