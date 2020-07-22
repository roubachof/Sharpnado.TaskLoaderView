using System;
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

            RandomGameLoaderCommand = new TaskLoaderCommand<object, Game>(_ => GetRandomGame());
        }

        public TaskLoaderCommand<object, Game> RandomGameLoaderCommand { get; }

        private async Task<Game> GetRandomGame()
        {
            await Task.Delay(TimeSpan.FromSeconds(4));

            if (DateTime.Now.Millisecond % 2 == 0)
            {
                throw new NetworkException();
            }

            return await _retroGamingService.GetRandomGame();
        }
    }
}
