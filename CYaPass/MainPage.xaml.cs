using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace CYaPass
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// Pre
    public sealed partial class MainPage : Page
    {
        
        private SiteKeys allSites = new SiteKeys();

        private Point MouseLoc;
        private List<Point> allPosts;
        private int postWidth = 10;
        private int centerPoint = 50;
        private int offset;
        StringBuilder pwdBuilder;
        private int minvalue = 0;
        private int maxvalue = 100;
        private int currentValue;
        private int startValue = 32;
        private UserPath us = new UserPath();
        private int hitTestIdx;

        public MainPage()
        {
            this.InitializeComponent();
            offset =  postWidth / 2;

            GenerateAllPosts();
            DrawGridLines();
            DrawPosts();

            NUDTextBox.Text = startValue.ToString();
            currentValue = startValue;
        }
        
        private void GenCrypto(string clearText)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(clearText, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            
            pwdBuilder = new StringBuilder(CryptographicBuffer.EncodeToHexString(hashed));
            
        }

        private void AddUpperCaseLetter(StringBuilder pwd)
        {
            if (pwd.Length <= 0) { return; }
            int index = -1;
            string target = string.Empty;
            foreach (Char c in pwd.ToString())
            {
                index++;
                if (Char.IsLetter(c))
                {
                    target = c.ToString();
                    break;
                }
            }
            pwd[index] = Convert.ToChar(target.ToUpper());
        }

        private string AddSpecialChars(string pwd)
        {
            if (pwd == String.Empty) { return String.Empty; }
            if (specialCharsTextBox.Text == string.Empty)
            {
                return pwd;
            }
            string temp = pwd;
            int charOffset = 2;
            pwd = temp.Substring(0, charOffset);
            pwd += specialCharsTextBox.Text;
            pwd += temp.Substring(2, temp.Length - charOffset);
            return pwd;
        }

        private void ComputeHashString()
        {
            
            var selItemText = SiteListBox.SelectedItem.ToString();

            string clearTextMessage = (us.PointValue + 28).ToString();
            clearTextMessage += selItemText;

            GenCrypto(clearTextMessage);
         
        }
        
        private bool HitTest(ref Point p)
        {
            int loopcount = 0;
            foreach (Point Pt in allPosts)
            {
                if ((p.X >= (Pt.X + offset) - postWidth) && (p.X <= (Pt.X + offset) + postWidth))
                {
                    if ((p.Y >= (Pt.Y + offset) - postWidth) && (p.Y <= (Pt.Y + offset) + postWidth))
                    {
                        p = Pt;
                        hitTestIdx = loopcount;
                        return true;
                    }
                }
                loopcount++;
            }

            return false;
        }

        async void LoadAllSites()
        {
            bool result = await allSites.Read();
            if (result)
            {
                allSites = (SiteKeys)JsonConvert.DeserializeObject(allSites.allJson, allSites.GetType());
            }
            foreach (SiteKey s in allSites)
            {
                SiteListBox.Items.Add(s);
            }
        }

        private void GenerateAllPosts()
        {
            allPosts = new List<Point>();
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    allPosts.Add(new Point((centerPoint * x) - (postWidth / 2), (centerPoint * y) - (postWidth / 2)));

                }
            }
        }

        private void SelectNewPoint()
        {
            Point currentPoint = new Point(MouseLoc.X, MouseLoc.Y);
            if (!HitTest(ref currentPoint))
            {
                return;
            }

            us.append(currentPoint, hitTestIdx);
            passwordTextBox.Text = String.Format("{0}", us.PointValue);

        }

        private void DrawPosts()
        {
            Brush b = new SolidColorBrush(Colors.Red);
            foreach (Point Pt in allPosts)
            {
                Ellipse el = new Ellipse();
                el.Fill = b;
                el.Width = postWidth;
                el.Height = postWidth;
                el.SetValue(Canvas.LeftProperty, (Double)Pt.X);
                el.SetValue(Canvas.TopProperty, (Double)Pt.Y);
                MainCanvas.Children.Add(el);
            }
        }

        private void DrawGridLines()
        {
            int numOfCells = 5;
            int cellSize = 50;

            Brush b = new SolidColorBrush(Colors.Gray);
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);

            for (int y = 0; y <= numOfCells; ++y)
            {
                Line l = new Line();
                l.StrokeThickness = 2;

                l.Stroke = b;
                l.X1 = 0;
                l.Y1 = y * cellSize;
                l.X2 = numOfCells * cellSize;
                l.Y2 = y * cellSize;
                MainCanvas.Children.Add(l);
            }

            for (int x = 0; x <= numOfCells; ++x)
            {
                Line l = new Line();
                l.StrokeThickness = 2;
                l.Stroke = b;
                l.X1 = x * cellSize;
                l.Y1 = 0;
                l.X2 = x * cellSize;
                l.Y2 = numOfCells * cellSize;
                MainCanvas.Children.Add(l);
            }
        }


        private void DrawHighlight()
        {
            Ellipse el = new Ellipse();
            Brush b = new SolidColorBrush(Colors.Orange);
            el.Stroke = b;
            el.Width = postWidth + 10;
            el.Height = postWidth + 10;
            if (us.allPoints.Count > 0)
            {
                el.SetValue(Canvas.LeftProperty, (Double)us.allPoints[0].X - offset);
                el.SetValue(Canvas.TopProperty, (Double)us.allPoints[0].Y - offset);
            }
            MainCanvas.Children.Add(el);
        }

        void DrawUserShape()
        {
            foreach (Segment s in us.allSegments)
            {
                DrawLine(s.Begin, s.End);
            }
        }

        private void DrawLine(Point begin, Point end)
        {
            Line l = new Line();
            l.StrokeThickness = 5;
            Brush b = new SolidColorBrush(Colors.Green);
            l.Stroke = b;
            l.X1 = begin.X + offset;
            l.Y1 = begin.Y + offset;
            l.X2 = end.X + offset;
            l.Y2 = end.Y + offset;
            MainCanvas.Children.Add(l);
        }

        private void GeneratePassword()
        {
            if (SiteListBox.SelectedItem == null ||
                SiteListBox.SelectedIndex < 0 ||
                SiteListBox.SelectedItem.ToString() == String.Empty) { return; }
            ComputeHashString();

            if ((bool)addUppercaseCheckbox.IsChecked)
            {
                AddUpperCaseLetter(pwdBuilder);
            }
            String pwd = pwdBuilder.ToString();

            if ((bool)addSpecialCharscheckBox.IsChecked)
            {
                pwd = AddSpecialChars(pwd);
            }

            if ((bool)setMaxLengthCheckBox.IsChecked)
            {
                // Math.Min insures we don't overflow bounds of string
                pwd = pwd.Substring(0, Math.Min((int)currentValue, pwd.Length));
            }
            passwordTextBox.Text = pwd;
            var dataPackage = new DataPackage();
            dataPackage.SetText(pwd);
            Clipboard.SetContent(dataPackage);
        }

        private void MainCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectNewPoint();
            DrawHighlight();
            DrawUserShape();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllSites();
        }

        private void MainCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            MouseLoc = e.GetCurrentPoint(MainCanvas).Position;//e.GetPosition(MainCanvas);
        }

        private async void AddSiteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SiteKeyDialog();
            
            ContentDialogResult cdr = await dialog.ShowAsync();
            if (cdr == ContentDialogResult.Primary)
            {
                String newKey = dialog.siteKey;
                allSites.Add(new CYaPass.SiteKey(newKey));
                SiteListBox.Items.Add(new SiteKey(newKey));
                await allSites.Save();
            }
        }

        private async void DeleteSiteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //int itemIdx = SiteListBox.SelectedIndex - 1;
                allSites.Remove((SiteKey)SiteListBox.SelectedItem);
                SiteListBox.Items.Remove(SiteListBox.SelectedItem);
                
                await allSites.Save();
            }
            catch (Exception ex)
            {
              
            }
        }

        private void ClearGridButton_Click(object sender, RoutedEventArgs e)
        {
            us = new UserPath();
            MainCanvas.Children.Clear();
            DrawGridLines();
            DrawPosts();
            passwordTextBox.Text = String.Empty;
            Clipboard.SetContent(null);
        }

        private void SiteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (us.allSegments.Count > 0 && SiteListBox.SelectedIndex >= 0)
            {
                GeneratePassword();
            }
        }

        private void addUppercaseCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (us.allSegments.Count > 0 && SiteListBox.SelectedIndex >= 0)
            {
                GeneratePassword();
            }
        }

        private void addSpecialCharscheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (us.allSegments.Count > 0 && SiteListBox.SelectedIndex >= 0)
            {
                GeneratePassword();
            }
        }

        private void specialCharsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (us.allSegments.Count > 0 && SiteListBox.SelectedIndex >= 0)
            {
                GeneratePassword();
            }
        }

        private void setMaxLengthCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (us.allSegments.Count > 0 && SiteListBox.SelectedIndex >= 0)
            {
                GeneratePassword();
            }
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

        private void NUDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (us.allSegments.Count > 0 && SiteListBox.SelectedIndex >= 0)
            {
                GeneratePassword();
            }
        }
    }
}
