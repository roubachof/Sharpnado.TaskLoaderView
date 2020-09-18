using System;
using System.Threading;
using System.Threading.Tasks;

using Sample.Domain;
using Sample.Services;

using Sharpnado.Presentation.Forms;

namespace Sample.ViewModels
{
    public class LoadOnDemandViewModel : Bindable
    {
        private readonly IRetroGamingService _retroGamingService;

        public LoadOnDemandViewModel(IRetroGamingService retroGamingService)
        {
            _retroGamingService = retroGamingService;

            var cts = new CancellationTokenSource();

            // For testing the TaskMonitorConfiguration.ConsiderCanceledAsFaulted = true setting
            // cts.Cancel();

            RandomGameLoaderCommand = new TaskLoaderCommand<object, Game>(_ => GetRandomGame(cts.Token));
        }

        public TaskLoaderCommand<object, Game> RandomGameLoaderCommand { get; }

        private async Task<Game> GetRandomGame(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(4), token);

            if (DateTime.Now.Millisecond % 2 == 0)
            {
                throw new NetworkException();
            }

            return await _retroGamingService.GetRandomGame();
        }
    }
}
