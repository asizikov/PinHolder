using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PinHolder.Annotations;

namespace PinHolder.Controls
{
    public partial class ConfirmNumericPasswordBox : UserControl
    {

        [NotNull] private readonly Dictionary<TextBox, StringBuilder> _stringBuilders = new Dictionary<TextBox, StringBuilder>();
        [NotNull] private readonly Dictionary<TextBox, TextBlock> _textBlocks = new Dictionary<TextBox, TextBlock>();
        [NotNull] private readonly Dictionary<TextBox, TextBlock> _tips = new Dictionary<TextBox, TextBlock>();

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof (string), typeof (ConfirmNumericPasswordBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SetPasswordTextProperty =
            DependencyProperty.Register("SetPasswordText", typeof (string), typeof (ConfirmNumericPasswordBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ConfirmPasswordTextProperty =
            DependencyProperty.Register("ConfirmPasswordText", typeof (string), typeof (ConfirmNumericPasswordBox), new PropertyMetadata(default(string)));

        public ConfirmNumericPasswordBox()
        {
            InitializeComponent();

            _stringBuilders.Add(firstReal,  new StringBuilder());
            _stringBuilders.Add(secondReal, new StringBuilder());

            _textBlocks.Add(firstReal, firstFake);
            _textBlocks.Add(secondReal, secondFake);

            _tips.Add(firstReal, FirstTip);
            _tips.Add(secondReal, SecondTip);
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public string ConfirmPasswordText
        {
            get { return (string)GetValue(ConfirmPasswordTextProperty); }
            set { SetValue(ConfirmPasswordTextProperty, value); }
        }

        public string SetPasswordText
        {
            get { return (string)GetValue(SetPasswordTextProperty); }
            set { SetValue(SetPasswordTextProperty, value); }
        }


        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            HandleInput(tb, e);
        }

        private void HandleInput([NotNull] TextBox sender, KeyEventArgs e)
        {
            if (e.Key == Key.Unknown) return;

            if(!_stringBuilders.ContainsKey(sender)) return;
            if (!_textBlocks.ContainsKey(sender)) return;
            if (!_tips.ContainsKey(sender)) return;
            
            
            var fakeString = _stringBuilders[sender];
            var textBlock = _textBlocks[sender];
            var tip = _tips[sender];

            if (e.Key == Key.Back)
            {
                if (fakeString.Length <= 0)
                    return;

                fakeString.Remove(fakeString.Length - 1, 1);
            }
            else
            {
                fakeString.Append("*");
            }

            tip.Visibility = fakeString.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
            textBlock.Text = fakeString.ToString();
        }

    }
}

