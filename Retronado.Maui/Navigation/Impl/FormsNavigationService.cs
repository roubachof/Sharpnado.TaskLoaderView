﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormsNavigationService.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The forms navigation service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Sample.ViewModels;
using Sample.Views;

namespace Sample.Navigation.Impl
{
    /// <summary>
    /// The forms navigation service.
    /// </summary>
    public class FormsNavigationService : INavigationService
    {
        /// <summary>
        /// The lazy forms navigation.
        /// </summary>
        private readonly Lazy<NavigationPage> _lazyFormsNavigation;

        /// <summary>
        /// The view locator.
        /// </summary>
        private readonly IViewLocator _viewLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormsNavigationService"/> class.
        /// </summary>
        /// <param name="lazyFormsNavigation">
        /// The lazy forms navigation.
        /// </param>
        /// <param name="viewLocator">
        /// The view locator.
        /// </param>
        public FormsNavigationService(Lazy<NavigationPage> lazyFormsNavigation, IViewLocator viewLocator)
        {
            _lazyFormsNavigation = lazyFormsNavigation;
            _viewLocator = viewLocator;
        }

        /// <summary>
        /// The navigation page.
        /// </summary>
        private NavigationPage NavigationPage => _lazyFormsNavigation.Value;

        /// <summary>
        /// The forms navigation.
        /// </summary>
        private INavigation FormsNavigation => _lazyFormsNavigation.Value.Navigation;

        public async Task NavigateToViewModelAsync<TViewModel>(
            object parameter = null,
            bool modalNavigation = false,
            bool clearStack = false,
            bool animated = true)
            where TViewModel : ANavigableViewModel
        {
            if (clearStack)
            {
                var viewType = _viewLocator.GetViewTypeFor<TViewModel>();
                var rootPage = FormsNavigation.NavigationStack.First();
                if (viewType != rootPage.GetType())
                {
                    var newRootView = (Page)_viewLocator.GetViewFor<TViewModel>();

                    // Make the new view the root of our navigation stack
                    FormsNavigation.InsertPageBefore(newRootView, rootPage);
                    rootPage = newRootView;
                }

                // Then we want to go back to root page and clear the stack
                await NavigationPage.PopToRootAsync(animated);
                ((ANavigableViewModel)rootPage.BindingContext).OnNavigated(parameter);
                return;
            }

            var view = _viewLocator.GetViewFor<TViewModel>();

            if (modalNavigation)
            {
                await FormsNavigation.PushModalAsync((Page)view, animated);
            }
            else
            {
                await NavigationPage.PushAsync((Page)view, animated);
            }

            ((ANavigableViewModel)view.BindingContext).OnNavigated(parameter);
        }

        public async Task NavigateToViewAsync<TView>(
            object parameter = null,
            bool modalNavigation = false,
            bool clearStack = false,
            bool animated = true)
            where TView : class, IBindablePage
        {
            if (clearStack)
            {
                var viewType = typeof(TView);
                var rootPage = FormsNavigation.NavigationStack.First();
                if (viewType != rootPage.GetType())
                {
                    var newRootView = (Page)_viewLocator.GetView<TView>();

                    // Make the new view the root of our navigation stack
                    FormsNavigation.InsertPageBefore(newRootView, rootPage);
                    rootPage = newRootView;
                }

                // Then we want to go back to root page and clear the stack
                await NavigationPage.PopToRootAsync(animated);
                ((ANavigableViewModel)rootPage.BindingContext).OnNavigated(parameter);
                return;
            }

            var view = _viewLocator.GetView<TView>();

            if (modalNavigation)
            {
                await FormsNavigation.PushModalAsync((Page)view, animated);
            }
            else
            {
                await NavigationPage.PushAsync((Page)view, animated);
            }

            ((ANavigableViewModel)view.BindingContext).OnNavigated(parameter);
        }

        public async Task NavigateFromMenuToAsync<TViewModel>()
            where TViewModel : ANavigableViewModel
        {
            var view = _viewLocator.GetViewFor<TViewModel>();
            await NavigationPage.PushAsync((Page)view);
            ((ANavigableViewModel)view.BindingContext).OnNavigated(null);

            foreach (
                var page in
                FormsNavigation.NavigationStack
                    .Take(FormsNavigation.NavigationStack.Count - 1)
                    .Skip(1))
            {
                FormsNavigation.RemovePage(page);
            }
        }

        public async Task<IBindablePage> NavigateBackAsync(object parameter = null)
        {
            var page = (IBindablePage)await NavigationPage.PopAsync();
            return page;
        }
    }
}