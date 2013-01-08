using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PinHolder.Controls
{


    public partial class PageLock : UserControl
    {
        private enum LockerState
        {
            WaitForPassword,
            InputingPassword,
            IncorrectPassword,
            PasswordCorrect
        }

        private readonly StringBuilder _inputStringBuilder = new StringBuilder();
        private readonly StringBuilder _fakeStringBuilder = new StringBuilder();
        private readonly Dictionary<Button, Action> _handlers = new Dictionary<Button, Action>();
        private LockerState _currentState;

        public PageLock()
        {
            InitializeComponent();
            CurrentState = LockerState.WaitForPassword;

            _handlers.Add(One, () => Append("1"));
            _handlers.Add(Two, () => Append("2"));
            _handlers.Add(Three, () => Append("3"));
            _handlers.Add(Four, () => Append("4"));
            _handlers.Add(Five, () => Append("5"));
            _handlers.Add(Six, () => Append("6"));
            _handlers.Add(Seven, () => Append("7"));
            _handlers.Add(Eight, () => Append("8"));
            _handlers.Add(Nine, () => Append("9"));
            _handlers.Add(Zero, () => Append("0"));
            _handlers.Add(Del, Delete);
            _handlers.Add(Enter, () => CheckState(true));
        }

        private void Delete()
        {
            if (_fakeStringBuilder.Length <= 0) return;
            _fakeStringBuilder.Remove(_fakeStringBuilder.Length - 1, 1);
            _inputStringBuilder.Remove(_inputStringBuilder.Length - 1, 1);
            Password.Text = _fakeStringBuilder.ToString();
            CheckState(false);
        }

        private void Append(string digit)
        {
            _inputStringBuilder.Append(digit);
            _fakeStringBuilder.Append("•");
            Password.Text = _fakeStringBuilder.ToString();
            CheckState(false);
        }

        private string _originalPassword = "123";

        private int PasswordLength
        {
            get { return _originalPassword.Length; }
        }

        private LockerState CurrentState
        {
            get { return _currentState; }
            set
            {
                switch (value)
                {
                    case LockerState.WaitForPassword:
                        Password.Text = "enter password";
                        Del.Visibility = Visibility.Collapsed;
                        VisualStateManager.GoToState(this, "WaitForPassword", false);
                        break;
                    case LockerState.InputingPassword:
                        Del.Visibility = Visibility.Visible;
                        break;
                    case LockerState.IncorrectPassword:
                        Password.Text = "incorrect password";
                        VisualStateManager.GoToState(this, "IsPasswordIncorrect", false);
                        _fakeStringBuilder.Clear();
                        _inputStringBuilder.Clear();
                        break;
                    case LockerState.PasswordCorrect:
                        VisualStateManager.GoToState(this, "IsPasswordCorrect", false);
                        break;
                        
                }
                _currentState = value;
            }
        }


        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (!_handlers.ContainsKey(button)) return;

            var action = _handlers[button];
            if (action != null)
            {
                action();
            }

        }

        private void CheckState(bool fromEnter)
        {
            CurrentState = _inputStringBuilder.Length <= 0 ? LockerState.WaitForPassword : LockerState.InputingPassword;

            if (fromEnter)
            {
                var input = _inputStringBuilder.ToString();
                CurrentState = input == _originalPassword ? LockerState.PasswordCorrect : LockerState.IncorrectPassword;
            }
        }
    }
}
