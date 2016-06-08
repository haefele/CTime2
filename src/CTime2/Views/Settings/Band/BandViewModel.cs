using System;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.Band;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Application;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace CTime2.Views.Settings.Band
{
    public class BandViewModel : ReactiveScreen, IHandleWithTask<ApplicationResumed>
    {
        private readonly IBandService _bandService;

        private BandViewModelState _state;

        public BandViewModelState State
        {
            get { return this._state; }
            set { this.RaiseAndSetIfChanged(ref this._state, value); }
        }

        public ReactiveCommand<Unit> ToggleTile { get; }
        public ReactiveCommand<Unit> Reload { get; }

        public BandViewModel(IBandService bandService, IEventAggregator eventAggregator)
        {
            Guard.NotNull(bandService, nameof(bandService));

            this._bandService = bandService;

            this.DisplayName = "Band";

            var canToggleTile = this.WhenAnyValue(f => f.State, state => state != BandViewModelState.NotConnected);
            this.ToggleTile = ReactiveCommand.CreateAsyncTask(canToggleTile, _ => this.ToggleTileImpl());
            this.ToggleTile.AttachLoadingService(() => this.State == BandViewModelState.Installed
                    ? CTime2Resources.Get("Loading.RemoveTileFromBand")
                    : CTime2Resources.Get("Loading.AddTileToBand"));
            this.ToggleTile.AttachExceptionHandler();

            this.Reload = ReactiveCommand.CreateAsyncTask(_ => this.ReloadImpl());
            this.Reload.AttachLoadingService(CTime2Resources.Get("Loading.Band"));
            this.Reload.AttachExceptionHandler();

            eventAggregator.SubscribeScreen(this);
        }

        protected override async void OnActivate()
        {
            this.State = BandViewModelState.Loading;

            await this.Reload.ExecuteAsyncTask();
        }
        
        private async Task ToggleTileImpl()
        {
            if (this.State == BandViewModelState.NotConnected)
                return;

            if (this.State == BandViewModelState.Installed)
            {
                await this._bandService.UnRegisterBandTileAsync();
            }
            else
            {
                await this._bandService.RegisterBandTileAsync();
            }

            await this.ReloadImpl();
        }

        private async Task ReloadImpl()
        {
            var bandInfo = await this._bandService.GetBand();

            if (bandInfo == null)
            {
                this.State = BandViewModelState.NotConnected;
                return;
            }

            if (await this._bandService.IsBandTileRegisteredAsync())
            {
                this.State = BandViewModelState.Installed;
            }
            else
            {
                this.State = BandViewModelState.NotInstalled;
            }
        }

        async Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            await this.Reload.ExecuteAsyncTask();
        }
    }
}