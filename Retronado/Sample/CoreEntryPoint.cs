// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreEntryPoint.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The core entry point.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Sample.Domain;
using Sample.Infrastructure;
using Sample.Navigation;
using Sample.Navigation.Impl;
using Sample.ViewModels;
using Xamarin.Forms;

namespace Sample
{
    /// <summary>
    /// The core entry point.
    /// </summary>
    public class CoreEntryPoint
    {
        private static readonly Assembly ProjectAssembly = typeof(CoreEntryPoint).GetTypeInfo().Assembly;

        public async Task RegisterDependenciesAsync()
        {
            await Task.Run(RegisterDependencies);
        }

        public void RegisterDependencies()
        {
            var container = DependencyContainer.Instance;

            container.RegisterSingleton(
                () => new Lazy<NavigationPage>(() => (NavigationPage)Application.Current.MainPage));

            container.RegisterSingleton<IViewLocator, ViewLocator>();
            container.RegisterSingleton<IRetroGamingService, RetroGamingService>();
            container.RegisterSingleton<INavigationService, FormsNavigationService>();
            container.RegisterSingleton<ErrorEmulator>();
            container.Register<RetroGamesViewModel>();
            container.Register<CommandsPageViewModel>();

            

            // Register all views by convention
            foreach (var pageType in
                ProjectAssembly.ExportedTypes.Where(
                    type =>
                    type.Namespace.StartsWith("Sample.Views") && !type.GetTypeInfo().IsAbstract
                    && type.Name.EndsWith("Page")))
            {
                container.Register(pageType);
            }

            // Register all view models by convention
            foreach (var viewModelType in
                ProjectAssembly.ExportedTypes.Where(
                    type =>
                    type.Namespace.StartsWith("Sample.ViewModels")
                    && !type.GetTypeInfo().IsAbstract && type.Name.EndsWith("Vm")))
            {
                container.Register(viewModelType);
            }
        }
    }
}