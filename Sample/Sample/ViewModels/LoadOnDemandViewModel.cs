using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Sample.Domain;
using Sample.Services;

using Sharpnado.Presentation.Forms;

using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class LoadOnDemandViewModel : Bindable
    {
        private readonly IRetroGamingService _retroGamingService;

        public LoadOnDemandViewModel(IRetroGamingService retroGamingService)
        {
            _retroGamingService = retroGamingService;

            RandomGameLoader = new TaskLoaderNotifier<Game>();

            LoadRandomGameCommand = new Command(
                () => { RandomGameLoader.Load(GetRandomGame); });
        }

        public TaskLoaderNotifier<Game> RandomGameLoader { get; }

        public ICommand LoadRandomGameCommand { get; }

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
