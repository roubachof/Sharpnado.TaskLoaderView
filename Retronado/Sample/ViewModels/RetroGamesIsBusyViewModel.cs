using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

using Sample.Domain;
using Sample.Infrastructure;
using Sample.Resources.Localization;
using Sample.Navigation;
using Sample.Services;

using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class RetroGamesIsBusyViewModel : ANavigableViewModel
    {
        private readonly IRetroGamingService _retroGamingService;

        private GamePlatform _platform;
        private bool _isBusy;
        private bool _isRefreshing;
        private bool _hasError;
        private bool _hasRefreshError;
        private string _errorMessage;
        private string _errorImageUrl;

        private List<Game> _games;

        public RetroGamesIsBusyViewModel(
            INavigationService navigationService,
            IRetroGamingService retroGamingService,
            ErrorEmulator errorEmulator)
            : base(navigationService)
        {
            _retroGamingService = retroGamingService;

            ErrorEmulatorViewModel = new ErrorEmulatorViewModel(errorEmulator, () => Load());

            RefreshCommand = new Command(() => Load(true));
            ReloadCommand = new Command(() => Load());
        }

        public ICommand RefreshCommand { get; }

        public ICommand ReloadCommand { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetAndRaise(ref _isBusy, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetAndRaise(ref _isRefreshing, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetAndRaise(ref _hasError, value);
        }

        public bool HasRefreshError
        {
            get => _hasRefreshError;
            set => SetAndRaise(ref _hasRefreshError, value);
        }

        public string ErrorImageUrl
        {
            get => _errorImageUrl;
            set => SetAndRaise(ref _errorImageUrl, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetAndRaise(ref _errorMessage, value);
        }

        public List<Game> Games
        {
            get => _games;
            set => SetAndRaise(ref _games, value);
        }

        public ErrorEmulatorViewModel ErrorEmulatorViewModel { get; }

        public override void OnNavigated(object parameter)
        {
            _platform = (GamePlatform)parameter;

            Load();
        }

        private async void Load(bool isRefreshing = false)
        {
            IsBusy = !isRefreshing;
            IsRefreshing = isRefreshing;
            HasError = false;
            HasRefreshError = false;
            ErrorMessage = string.Empty;
            List<Game> items = new List<Game>();

            if (!isRefreshing)
            {
                Games = new List<Game>();
            }

            try
            {
                items = await InitializeAsync();
            }
            catch (NetworkException)
            {
                ErrorImageUrl = "Sample.Images.the_internet.png";
                ErrorMessage = SampleResources.Error_Network;
            }
            catch (ServerException)
            {
                ErrorImageUrl = "Sample.Images.server.png";
                ErrorMessage = SampleResources.Error_Business;
            }
            catch (Exception)
            {
                ErrorImageUrl = "Sample.Images.richmond.png";
                ErrorMessage = SampleResources.Error_Unknown;
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
                HasError = !isRefreshing && ErrorMessage != string.Empty;
                HasRefreshError = isRefreshing && ErrorMessage != string.Empty;
            }

            if (!HasRefreshError)
            {
                Games = items;
            }
        }

        private async Task<List<Game>> InitializeAsync()
        {
            var result = _platform == GamePlatform.Computer
                ? await _retroGamingService.GetAtariAndAmigaGames()
                : await _retroGamingService.GetNesAndSmsGames();

            return result;
        }
    }
}
