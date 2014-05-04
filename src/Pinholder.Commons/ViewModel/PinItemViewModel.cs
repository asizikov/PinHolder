using System;
using Curacao.Mvvm.ViewModel;

namespace PinHolder.ViewModel
{
    public sealed class PinItemViewModel : UnsafeBaseViewModel
    {
        private string _pin;

        private readonly Action<PinItemViewModel> _valueUpdatedCallback;

        public PinItemViewModel(Action<PinItemViewModel> updatedCallback )
        {
            _valueUpdatedCallback = updatedCallback;
        }

        public PinItemViewModel(string pin)
        {
            Pin = pin;
        }

        public string Pin
        {
            get { return _pin; }
            set
            {
                if (value == _pin) return;
                _pin = value;
                OnPropertyChanged("Pin");
                if (_valueUpdatedCallback != null)
                {
                    _valueUpdatedCallback(this);
                }
            }
        }

        public void SetSilently(string value)
        {
            _pin = value;
        }

        public void RaiseChanged()
        {
            OnPropertyChanged("Pin");
        }
    }
}