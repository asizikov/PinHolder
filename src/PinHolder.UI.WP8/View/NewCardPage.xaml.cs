﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using PinHolder.Lifecycle;
using PinHolder.Navigation;

namespace PinHolder.View
{
    public sealed partial class NewCardPage
    {
        public NewCardPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string parameter;
            if (NavigationContext.QueryString.TryGetValue(Keys.Id, out parameter))
            {
                int id;
                if (Int32.TryParse(parameter, out id))
                {
                    DataContext = ViewModelLocator.GetEditCardViewModel(id);
                    return;
                }
            }
            DataContext = ViewModelLocator.GetNewCardViewModel();

            if (!this.LoadState<bool>("hasRestore")) return;

            name.Text = this.LoadState<string>("name");
            description.Text = this.LoadState<string>("descripton");

            int index = 0;
            foreach (UIElement child in digits.Children)
            {
                var border = child as Border;
                if (border == null) return; //something went wrong. Let's sckip it

                var tb = border.Child as TextBox;
                if (tb == null) return; //something went wrong. Let's sckip it

                tb.Text = this.LoadState<string>(index.ToString(CultureInfo.InvariantCulture));
                index++;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.SaveState("hasRestore", false);
                return;
            }

            this.SaveState("hasRestore", true);
            this.SaveState("name", name.Text);
            this.SaveState("descripton", description.Text);

            int index = 0;
            foreach (UIElement child in digits.Children)
            {
                var border = child as Border;
                if (border == null) return; //something went wrong. Let's sckip it

                var tb = border.Child as TextBox;
                if (tb == null) return; //something went wrong. Let's sckip it

                this.SaveState(index.ToString(CultureInfo.InvariantCulture), tb.Text);
                index++;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            if (tb.Text.Length > 0)
            {
                tb.Text = tb.Text[0].ToString(CultureInfo.InvariantCulture);
                Focus();
            }
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;
            // Update the binding source
            BindingExpression bindingExpr = textBox.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpr == null) return;
            bindingExpr.UpdateSource();
        }
    }
}