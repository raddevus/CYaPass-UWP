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



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CYaPass
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// Pre
    public sealed partial class MainPage : Page
    {
        
        private SiteKeys allSites = new SiteKeys();
        private List<Point> userShape = new List<Point>();
        private LineSegments LineSegments = new LineSegments();
        private Point previousPoint;
        private bool previousPointExists;
        private Point MouseLoc;
        private List<Point> allPosts;
        private int postWidth = 10;
        private int centerPoint = 50;
        private int offset;
        private byte[] saltBytes;
        private StringBuilder pwdBuilder;

        public MainPage()
        {
            this.InitializeComponent();
            offset =  postWidth / 2;

            GenerateAllPosts();
            DrawGridLines();
            DrawPosts();
        }

        private void CalculateGeometricSaltValue()
        {
            LineSegments.PostPoints = String.Empty;
            LineSegments.PostValue = 0;

            foreach (LineSegment l in LineSegments)
            {
                for (int x = 0; x < allPosts.Count; x++)
                {
                    if (l.Start.X == allPosts[x].X && l.Start.Y == allPosts[x].Y)
                    {
                        //System.Diagnostics.Debug.Print(string.Format("START x : {0}", x));
                        LineSegments.AddOn(x);
                    }
                    if (l.End.X == allPosts[x].X && l.End.Y == allPosts[x].Y)
                    {
                        //System.Diagnostics.Debug.Print(string.Format("END x : {0}", x));
                        LineSegments.AddOn(x);
                    }
                }
            }
            //System.Diagnostics.Debug.Print(String.Format("Value : {0}", LineSegments.PostValue));
            //System.Diagnostics.Debug.Print(LineSegments.PostPoints);
        }

        private String GenCrypto(string clearText)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(clearText, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

        private void ComputeHashBytes()
        {
            
            var selItemText = SiteListBox.SelectedItem.ToString();
            
            string clearTextMessage = LineSegments.PostValue.ToString();
            clearTextMessage += selItemText;

            passwordTextBox.Text = GenCrypto(clearTextMessage);
         
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Draw(e.GetPosition(MainCanvas));
        }

        private bool HitTest(ref Point p)
        {
            foreach (Point Pt in allPosts)
            {
                if ((p.X >= (Pt.X + offset) - postWidth) && (p.X <= (Pt.X + offset) + postWidth))
                {
                    if ((p.Y >= (Pt.Y + offset) - postWidth) && (p.Y <= (Pt.Y + offset) + postWidth))
                    {
                        p = Pt;
                        return true;
                    }
                }
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

            DrawLine(currentPoint.X, currentPoint.Y);
            if (IsNewPoint(currentPoint))
            {
                if (userShape.Count > 0)
                {
                    if (IsNewLineSegment(new LineSegment(userShape[userShape.Count - 1], currentPoint))) // never allow a duplicate line segment in the list
                    {
                        LineSegments.Add(new LineSegment(userShape[userShape.Count - 1], currentPoint));
                    }
                }
                userShape.Add(currentPoint);
                DrawHighlight();
                if (SiteListBox.SelectedItems.Count > 0 && userShape.Count > 1)
                {
                    //GenPassword();
                }
            }
            previousPointExists = true;
            previousPoint = currentPoint;

        }


        private bool IsNewPoint(Point currentPoint)
        {
            if (userShape.Count > 0)
            {
                if (userShape.Count > 1)
                {
                    if (!((currentPoint.X == userShape[userShape.Count - 1].X && currentPoint.Y == userShape[userShape.Count - 1].Y)
                        || (currentPoint.X == userShape[userShape.Count - 2].X || currentPoint.Y == userShape[userShape.Count - 2].Y)))
                    {
                        return true;
                    }
                }
                if (!(currentPoint.X == userShape[userShape.Count - 1].X && currentPoint.Y == userShape[userShape.Count - 1].Y))
                {
                    return true;
                }
            }
            else { return true; }
            return false;
        }

        private bool IsNewLineSegment(LineSegment l)
        {
            LineSegment duplicate = LineSegments.Find(ls => (ls.Start.X == l.Start.X && ls.Start.Y == l.Start.Y) && (ls.End.X == l.End.X && ls.End.Y == l.End.Y) || (ls.End.X == l.Start.X && ls.End.Y == l.Start.Y) && (ls.Start.X == l.End.X && ls.Start.Y == l.End.Y));
            if (duplicate == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Draw(Point m)
        {
            Brush blackBrush = new SolidColorBrush(Colors.Black);
            Brush greenBrush = new SolidColorBrush(Colors.Green);
            MainCanvas.Children.Clear();
            DrawGridLines();
            DrawPosts();

            int mX = (int)m.X;
            int mY = (int)m.Y;
            Ellipse el = new Ellipse();
            el.Width = 15;
            el.Height = 15;
            el.SetValue(Canvas.LeftProperty, (Double)mX);
            el.SetValue(Canvas.TopProperty, (Double)mY);
            el.Fill = blackBrush;
            Line l = new Line();
            l.Fill = greenBrush;
            //l.Width = 5;

            l.Stroke = greenBrush;
            l.StrokeThickness = 3;

            l.X1 = 0;
            l.Y1 = 0;
            l.X2 = mX;
            l.Y2 = mY;

            MainCanvas.Children.Add(el);
            MainCanvas.Children.Add(l);
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
            el.SetValue(Canvas.LeftProperty, (Double)userShape[0].X - offset);
            el.SetValue(Canvas.TopProperty, (Double)userShape[0].Y - offset);
            MainCanvas.Children.Add(el);
        }

        private void DrawLine(Double x, Double y)
        {
            if (!previousPointExists) { return; }
            previousPointExists = true;
            Line l = new Line();
            l.StrokeThickness = 5;
            Brush b = new SolidColorBrush(Colors.Green);
            l.Stroke = b;
            l.X1 = previousPoint.X + offset;
            l.Y1 = previousPoint.Y + offset;
            l.X2 = x + offset;
            l.Y2 = y + offset;
            MainCanvas.Children.Add(l);
        }

        private void GeneratePassword()
        {
            if (SiteListBox.SelectedItem == null || 
                SiteListBox.SelectedIndex == 0 ||
                SiteListBox.SelectedItem.ToString() == String.Empty) { return; }
            if (userShape.Count > 1)
            {
                CalculateGeometricSaltValue();
                ComputeHashBytes();
            }
            var dataPackage = new DataPackage();
            dataPackage.SetText(passwordTextBox.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void MainCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectNewPoint();   
            GeneratePassword();
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
            userShape.Clear();
            LineSegments.Clear();
            MainCanvas.Children.Clear();
            previousPointExists = false;
            DrawGridLines();
            DrawPosts();
            passwordTextBox.Text = String.Empty;
            Clipboard.SetContent(null);
        }

        private void SiteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeneratePassword();
        }
    }
}
