using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

using Sample.Domain;
using Sample.Infrastructure;
using Sample.Navigation;
using Sample.Services;

using Sharpnado.Presentation.Forms;

using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class RetroGamesViewModel : ANavigableViewModel
    {
        private readonly IRetroGamingService _retroGamingService;

        private GamePlatform _platform;

        public RetroGamesViewModel(INavigationService navigationService, IRetroGamingService retroGamingService, ErrorEmulator errorEmulator)
            : base(navigationService)
        {
            _retroGamingService = retroGamingService;

            ErrorEmulatorViewModel = new ErrorEmulatorViewModel(errorEmulator, () => Loader.Load(InitializeAsync));

            // TaskStartMode = Auto
            // Loader = new TaskLoaderNotifier<List<Game>>(InitializeAsync);

            // TaskStartMode = Manual (Default mode)
            Loader = new TaskLoaderNotifier<List<Game>>();
        }

        public TaskLoaderNotifier<List<Game>> Loader { get; }

        public ErrorEmulatorViewModel ErrorEmulatorViewModel { get; }

        public override void OnNavigated(object parameter)
        {
            _platform = (GamePlatform)parameter;

            // TaskStartMode = Manual (Default mode)
            Loader.Load(InitializeAsync);
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
