using System;
using Caliburn.Micro;

namespace CTime2.Services.Navigation
{
    public interface ICTimeNavigationService
    {
        void Navigate(Type viewModelType);
        void ClearBackStack();
    }
}