using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CYaPass
{
    public sealed partial class NumericUpDown : UserControl
    {
        int minvalue = 0,
            maxvalue = 100,
            startvalue = 10;
        public int  currentValue;
        public NumericUpDown()
        {
            InitializeComponent();
            NUDTextBox.Text = startvalue.ToString();
            currentValue = startvalue;
        }

        private void NUDButtonUP_Click(object sender, RoutedEventArgs e)
        {
            UpButtonClickHandler();
        }

        private void UpButtonClickHandler()
        {
            int number;
            if (NUDTextBox.Text != "")
            {
                number = Convert.ToInt32(NUDTextBox.Text);
                currentValue = number;
            }
            else { currentValue = number = 0; }
            if (number < maxvalue)
            {
                currentValue = ++number;
                NUDTextBox.Text = Convert.ToString(currentValue);
            }
        }

        private void NUDButtonDown_Click(object sender, RoutedEventArgs e)
        {
            DownButtonClickHandler();
        }

        private void DownButtonClickHandler()
        {
            int number;
            if (NUDTextBox.Text != "") { number = Convert.ToInt32(NUDTextBox.Text); }
            else { number = 0; }
            if (number > minvalue)
            {
                currentValue--;
                NUDTextBox.Text = Convert.ToString(currentValue);
            }
        }

        private void NUDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number = 0;
            if (NUDTextBox.Text != "")
            {
                if (!int.TryParse(NUDTextBox.Text, out number))
                {
                    currentValue = startvalue;
                    NUDTextBox.Text = startvalue.ToString();
                }
            }
            if (number > maxvalue)
            {
                currentValue = maxvalue;
                NUDTextBox.Text = maxvalue.ToString();
            }
            if (number < minvalue)
            {
                currentValue = minvalue;
                NUDTextBox.Text = minvalue.ToString();
            }
            NUDTextBox.SelectionStart = NUDTextBox.Text.Length;

        }

        private void NUDTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                UpButtonClickHandler();
            }

            if (e.Key == Windows.System.VirtualKey.Down)
            {
                DownButtonClickHandler();
            }
        }
    }   
}
