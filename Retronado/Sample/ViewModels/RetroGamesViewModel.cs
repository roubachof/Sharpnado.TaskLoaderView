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

        private readonly ErrorEmulator _errorEmulator;

        private GamePlatform _platform;

        public RetroGamesViewModel(INavigationService navigationService, IRetroGamingService retroGamingService, ErrorEmulator errorEmulator)
            : base(navigationService)
        {
            _retroGamingService = retroGamingService;
            _errorEmulator = errorEmulator;

            RefreshCommand = new Command(() => Load(null));

            ErrorEmulatorViewModel = new ErrorEmulatorViewModel(errorEmulator,  () => Loader.Load(InitializeAsync));

            // TaskStartMode = Auto
            // Loader = new TaskLoaderNotifier<List<Game>>(InitializeAsync);

            // TaskStartMode = Manual (Default mode)
            Loader = new TaskLoaderNotifier<List<Game>>();
        }

        public TaskLoaderNotifier<List<Game>> Loader { get; }

        public ICommand RefreshCommand { get; }

        public ErrorEmulatorViewModel ErrorEmulatorViewModel { get; }

        public override void Load(object parameter)
        {
            _platform = (GamePlatform)parameter;

            // TaskStartMode = Manual (Default mode)
            Loader.Load(InitializeAsync);
        }

        private async Task<List<Game>> InitializeAsync()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var result = _platform == GamePlatform.Computer
                             ? await _retroGamingService.GetAtariAndAmigaGames()
                             : await _retroGamingService.GetNesAndSmsGames();

            watch.Stop();
            var remainingWaitingTime = TimeSpan.FromSeconds(4) - watch.Elapsed;
            if (remainingWaitingTime > TimeSpan.Zero)
            {
                // Sometimes the api is too good x)
                await Task.Delay(remainingWaitingTime);
            }

            switch (_errorEmulator.ErrorType)
            {
                case ErrorType.Unknown:
                    throw new InvalidOperationException();

                case ErrorType.Network:
                    throw new NetworkException();

                case ErrorType.Server:
                    throw new ServerException();

                case ErrorType.NoData:
                    return new List<Game>();

                case ErrorType.ErrorOnRefresh:
                    if (DateTime.Now.Second % 2 == 0)
                    {
                        throw new NetworkException();
                    }

                    throw new ServerException();
            }

            return result;
        }

        private async Task<List<Game>> InitializeMock()
        {
            await Task.Delay(3000);

            switch (_errorEmulator.ErrorType)
            {
                case ErrorType.Unknown:
                    throw new InvalidOperationException();

                case ErrorType.Network:
                    throw new NetworkException();

                case ErrorType.Server:
                    throw new ServerException();

                case ErrorType.NoData:
                    return new List<Game>();

                case ErrorType.ErrorOnRefresh:
                    if (DateTime.Now.Second % 2 == 0)
                    {
                        throw new NetworkException();
                    }

                    throw new InvalidOperationException();
            }

            return await Task.Run(
                () => new List<Game>
                    {
                        new Game(
                            1,
                            "https://images.igdb.com/igdb/image/upload/t_cover_big/co1tnp.jpg",
                            null,
                            new DateTime(1984, 2, 10),
                            new List<Genre> { new Genre(1, "Puzzle") },
                            new List<Company> { new Company(1, "USSR") },
                            "Tetris",
                            87.1122),
                        new Game(
                            2,
                            "https://images.igdb.com/igdb/image/upload/t_cover_big/tsi99e7cbu6pituhjfwe.jpg",
                            null,
                            new DateTime(1988, 4, 1),
                            new List<Genre> { new Genre(1, "Action") },
                            new List<Company> { new Company(1, "Elite") },
                            "Thundercats",
                            null),
                    });
        }
    }
}
