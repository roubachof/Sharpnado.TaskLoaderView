// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLocator.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Sample.Infrastructure;
using Sample.ViewModels;
using Sample.Views;

namespace Sample.Navigation.Impl
{
    public class ViewLocator : IViewLocator
    {
        private static readonly List<(Type ViewModelType, Type ViewType)> ViewLocatorDictionary = new List<(Type, Type)>
            {
                (typeof(RetroGamesViewModel), typeof(DefaultViewsPage)),
                (typeof(RetroGamesViewModel), typeof(DefaultViewsSkeletonPage)),
                (typeof(RetroGamesViewModel), typeof(UserViewsPage)),
                (typeof(RetroGamesViewModel), typeof(LottieViewsPage)),
            };

        public IBindablePage GetViewFor<TViewModel>()
            where TViewModel : ANavigableViewModel
        {
            var viewModel = DependencyContainer.Instance.GetInstance<TViewModel>();
            var view =
                (IBindablePage)DependencyContainer.Instance.GetInstance(FindViewByViewModel(typeof(TViewModel)));
            view.BindingContext = viewModel;
            return view;
        }

        public IBindablePage GetView<TView>()
            where TView : class, IBindablePage
        {
            var view =
                (IBindablePage)DependencyContainer.Instance.GetInstance(typeof(TView));
            var viewModel = DependencyContainer.Instance.GetInstance(FindViewModelByView(typeof(TView)));
            view.BindingContext = viewModel;
            return view;
        }

        /// <summary>
        /// Gets the view type matching the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <returns>
        /// </returns>
        public Type GetViewTypeFor<TViewModel>()
            where TViewModel : ANavigableViewModel
        {
            return FindViewByViewModel(typeof(TViewModel));
        }

        private static Type FindViewModelByView(Type viewType)
        {
            foreach (var pair in ViewLocatorDictionary)
            {
                if (pair.ViewType == viewType)
                {
                    return pair.ViewModelType;
                }
            }

            return null;
        }

        private static Type FindViewByViewModel(Type viewModelType)
        {
            foreach (var pair in ViewLocatorDictionary)
            {
                if (pair.ViewModelType == viewModelType)
                {
                    return pair.ViewType;
                }
            }

            return null;
        }
    }
}