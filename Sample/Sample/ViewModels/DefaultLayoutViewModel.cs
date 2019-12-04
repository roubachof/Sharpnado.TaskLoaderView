using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Sample.Domain;
using Sample.Infrastructure;
using Sample.Navigation;

using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class DefaultLayoutViewModel : ANavigableViewModel
    {
        private readonly IRetroGamingService _retroGamingService;

        private List<Game> _games;

        public DefaultLayoutViewModel(INavigationService navigationService, IRetroGamingService retroGamingService, ErrorEmulator errorEmulator)
            : base(navigationService)
        {
            _retroGamingService = retroGamingService;

            RefreshCommand = new Command(() => Load(null));

            ErrorEmulator = new ErrorEmulatorVm(errorEmulator,  () => Load(null));
        }

        public ICommand RefreshCommand { get; }

        public ErrorEmulatorVm ErrorEmulator { get; }

        public List<Game> Games
        {
            get => _games;
            set => SetAndRaise(ref _games, value);
        }

        public override void Load(object parameter)
        {
            TaskMonitor.Create(InitializeMock);
        }

        private async Task InitializeMock()
        {
            Games = await Task.Run(
                () => new List<Game>
                    {
                        new Game(
                            1,
                            "https://images.igdb.com/igdb/image/upload/t_cover_big/co1tnp.jpg",
                            new DateTime(1984, 2, 10),
                            new List<Genre> { new Genre(1, "Puzzle") },
                            new List<Company> { new Company(1, "USSR") },
                            "Tetris",
                            87.1122),
                        new Game(
                            2,
                            "https://images.igdb.com/igdb/image/upload/t_cover_big/tsi99e7cbu6pituhjfwe.jpg",
                            new DateTime(1988, 4, 1),
                            new List<Genre> { new Genre(1, "Action") },
                            new List<Company> { new Company(1, "Elite") },
                            "Thundercats",
                            null),
                    });
        }
    }
}
