using System;
using System.Collections.Generic;

using Sample.Infrastructure;
using Sample.Localization;

namespace Sample.ViewModels
{
    public class ErrorEmulatorVm : Bindable
    {
        private readonly ErrorEmulator _errorEmulator;
        private readonly Action _onErrorTypeChanged;

        private int _selectedIndex;

        public ErrorEmulatorVm(ErrorEmulator errorEmulator, Action onErrorTypeChanged)
        {
            _errorEmulator = errorEmulator;
            _onErrorTypeChanged = onErrorTypeChanged;

            ErrorTypes = new List<string>
            {
                SampleResources.ErrorType_None,
                SampleResources.ErrorType_Unknown,
                SampleResources.ErrorType_Network,
                SampleResources.ErrorType_Server,
                SampleResources.ErrorType_NoData,
            };
        }

        public IReadOnlyList<string> ErrorTypes { get; }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (SetAndRaise(ref _selectedIndex, value))
                {
                    _errorEmulator.ErrorType = (ErrorType)_selectedIndex;
                    _onErrorTypeChanged();
                }
            }
        }
    }
}
