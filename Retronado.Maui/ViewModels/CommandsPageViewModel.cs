using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Sample.Domain;
using Sample.Navigation;
using Sample.Services;
using Sharpnado.TaskLoaderView;

namespace Sample.ViewModels
{
    public class CommandsPageViewModel : ANavigableViewModel
    {
        private readonly IRetroGamingService _retroGamingService;

        public CommandsPageViewModel(INavigationService navigationService, IRetroGamingService retroGamingService)
            : base(navigationService)
        {
            _retroGamingService = retroGamingService;

            Loader = new TaskLoaderNotifier<Game>();

            BuyGameCommand = new TaskLoaderCommand(BuyGame);
            PlayTheGameCommand = new TaskLoaderCommand(PlayTheGame);

            CompositeNotifier = CompositeTaskLoaderNotifier.ForCommands()
                .WithLoaders(Loader)
                .WithCommands(BuyGameCommand, PlayTheGameCommand)
                .Build();
        }

        public CompositeTaskLoaderNotifier CompositeNotifier { get; }

        public TaskLoaderCommand BuyGameCommand { get; }

        public TaskLoaderCommand PlayTheGameCommand { get; }

        public TaskLoaderNotifier<Game> Loader { get; }

        public string LoadingText { get; set; }

        public override void OnNavigated(object parameter)
        {
            var cts = new CancellationTokenSource();

            // For testing the TaskMonitorConfiguration.ConsiderCanceledAsFaulted = true setting
            // cts.Cancel();

            Loader.Load(isRefreshing => GetRandomGame(cts.Token, isRefreshing));
        }

        private async Task<Game> GetRandomGame(CancellationToken token, bool isRefreshing)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), token);

            if (isRefreshing)
            {
                throw new InvalidOperationException();
            }

            if (DateTime.Now.Second % 2 == 0)
            {
                throw new InvalidOperationException();
            }

            return await _retroGamingService.GetRandomGame(true);
        }

        private async Task BuyGame()
        {
            LoadingText = "Proceeding to payment";
            RaisePropertyChanged(nameof(LoadingText));

            await Task.Delay(2000);
            throw new LocalizedException($"Sorry, we only accept DogeCoin...{Environment.NewLine}BTW GameStop are still opened");
        }

        private async Task PlayTheGame()
        {
            LoadingText = "Loading the game...";
            RaisePropertyChanged(nameof(LoadingText));

            await Task.Delay(2000);
            throw new LocalizedException("AHAHAHA! Yeah right...");
        }
    }
}
