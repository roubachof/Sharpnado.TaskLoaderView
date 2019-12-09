// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewLocator.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Sample.ViewModels;
using Sample.Views;

namespace Sample.Navigation
{
    /// <summary>
    /// Service responsible for locating the correct view from the ViewModel infos.
    /// The service is currently also responsible for the creation of the view and the view model if needed.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Builds the view matching the given view model type.
        /// Builds the view model and bind it to the created view.
        /// Loads the view model.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <returns>
        /// </returns>
        IBindablePage GetViewFor<TViewModel>()
            where TViewModel : ANavigableViewModel;

        IBindablePage GetView<TView>()
            where TView : class, IBindablePage;

        /// <summary>
        /// Gets the view type matching the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <returns>
        /// </returns>
        Type GetViewTypeFor<TViewModel>()
            where TViewModel : ANavigableViewModel;
    }
}