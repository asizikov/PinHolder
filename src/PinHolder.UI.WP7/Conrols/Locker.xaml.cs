using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PinHolder.Conrols
{
    public partial class Locker
    {

        public static readonly DependencyProperty AcceptedProperty =
            DependencyProperty.Register("Accepted", typeof (bool), typeof (Locker), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof (string), typeof (Locker), new PropertyMetadata(default(string)));

        public string Password
        {
            get { return (string) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public bool Accepted
        {
            get { return (bool) GetValue(AcceptedProperty); }
            set { SetValue(AcceptedProperty, value); }
        }

        private readonly TextBox[] _inputs = new TextBox[5];


        public Locker()
        {
            InitializeComponent();

            _inputs[0] = zero;
            _inputs[1] = one;
            _inputs[2] = two;
            _inputs[3] = three;
            _inputs[4] = four;

            foreach (var textBox in _inputs)
            {
                textBox.TextChanged += TbOnTextChanged;
            }

            SizeChanged += OnSizeChanged;

        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var h = sizeChangedEventArgs.NewSize.Height;
            if (h > 0)
            {
                _inputs[0].Focus();
            }
        }

        private void TbOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var textBox = sender as TextBox;
            if(textBox == null) return;

            int number; 
            if(!Int32.TryParse(textBox.Tag as String, out number)) return;

            if (number < _inputs.Length - 1)
            {
                _inputs[++number].Focus();
            }
            else
            {
                CheckPassword();
            }
        }

        private void CheckPassword()
        {
            var sb = new StringBuilder(5);
            foreach (var tb in _inputs)
            {
                sb.Append(tb.Text);
            }

            Accepted = Password == sb.ToString();
        }

    }
}
