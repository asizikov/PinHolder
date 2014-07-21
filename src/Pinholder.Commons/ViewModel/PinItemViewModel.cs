using System;

namespace PinHolder.ViewModel
{
    public sealed class PinItemViewModel : UnsafeBaseViewModel
    {
        private string _pin;

        private readonly Action _valueUpdatedCallback;

        public PinItemViewModel(Action updatedCallback )
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
                    _valueUpdatedCallback();
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