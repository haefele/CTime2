using Caliburn.Micro;

namespace CTime2.Services.Navigation
{
    public interface ICTimeNavigationService
    {
        NavigateHelper<TViewModel> For<TViewModel>();
        void ClearBackStack();
    }
}