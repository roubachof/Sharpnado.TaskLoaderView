// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INavigationService.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;

using Sample.ViewModels;
using Sample.Views;

namespace Sample.Navigation
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the bindable page matching the given navigable view model type.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <param name="parameter">
        /// The parameter passed to the view model Load method.
        /// </param>
        /// <param name="modalNavigation">
        /// True if we want a modal navigation.
        /// </param>
        /// <param name="clearStack">
        /// Navigate and clears the stack history (the new view become the new navigation root)
        /// </param>
        /// <param name="animated">
        /// If true animate the navigation.
        /// </param>
        /// <returns>
        /// </returns>
        Task NavigateToViewModelAsync<TViewModel>(object parameter = null, bool modalNavigation = false, bool clearStack = false, bool animated = true)
            where TViewModel : ANavigableViewModel;

        Task NavigateToViewAsync<TView>(
            object parameter = null,
            bool modalNavigation = false,
            bool clearStack = false,
            bool animated = true)
            where TView : class, IBindablePage;

        /// <summary>
        /// Navigation from menu means: reset the stack, and then add the new page.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model to navigate to.
        /// </typeparam>
        /// <returns>
        /// </returns>
        Task NavigateFromMenuToAsync<TViewModel>()
            where TViewModel : ANavigableViewModel;

        /// <summary>
        /// Closes the current bindable page.
        /// </summary>
        /// <returns>
        /// </returns>
        Task<IBindablePage> NavigateBackAsync(object parameter = null);
    }
}