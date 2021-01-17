using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Capture;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ruler
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        bool IsDragging = false;

        PointerPoint StartPoint;

        private void DrawCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            RegionBorder.Width = 0;
            RegionBorder.Height = 0;
            if (!IsGuideDragging)
            {
                IsDragging = true;
                StartPoint = e.GetCurrentPoint(DrawCanvas);
                RegionBorder.Width = 1;
                RegionBorder.Height = 1;

                RegionBorder.Visibility = Visibility.Visible;
                Canvas.SetLeft(RegionBorder, StartPoint.Position.X);
                Canvas.SetTop(RegionBorder, StartPoint.Position.Y);
            }
            else
            {
                IsDragging = false;
            }
        }


        private void Grid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint NewPos = e.GetCurrentPoint(sender as Grid);
            XText.Text = Math.Round(NewPos.Position.X, 0).ToString();
            YText.Text = Math.Round(NewPos.Position.Y, 0).ToString();
        }

        private void DrawCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (IsDragging)
            {
                PointerPoint NewPos = e.GetCurrentPoint(DrawCanvas);
                double Width = NewPos.Position.X - StartPoint.Position.X;
                double Height = NewPos.Position.Y - StartPoint.Position.Y;

 
                
                if (Width > 0)
                {
                    RegionBorder.Width = Width;
                    WText.Text = Math.Round(Width, 0).ToString();
                }

                if (Height > 0)
                {
                    RegionBorder.Height = Height;
                    HText.Text = Math.Round(Height, 0).ToString();
                }
            }
        }

        private void DrawCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            IsDragging = false;
            //RegionBorder.Visibility = Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundGridShadow.Receivers.Add(DropshadowGrid);
            FloatingShadow.Receivers.Add(ImageHolderGrid);
        }

        private void HorizontalGuideBtn_Click(object sender, RoutedEventArgs e)
        {
            HorizontalRuler H = new HorizontalRuler();
            H.Width = 2000;
            H.ManipulationStarted += Ruler_ManipulationStarted;
            H.ManipulationCompleted += Ruler_ManipulationCompleted;
            H.ManipulationDelta += Ruler_ManipulationDelta;
            H.ManipulationMode = ManipulationModes.TranslateY;
            H.PointerEntered += Ruler_PointerEntered;
            H.PointerExited += Ruler_PointerExited;
            DrawCanvas.Children.Add(H);
            Canvas.SetTop(H, 120);
        }

        private void Ruler_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void Ruler_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender.GetType() == typeof(VerticalRuler))
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeWestEast, 0);
            }
            else if (sender.GetType() == typeof(HorizontalRuler))
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeNorthSouth, 0);
            }
        }

        bool IsGuideDragging = false;
        private void Ruler_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (IsGuideDragging)
            {
                if (sender.GetType() == typeof(VerticalRuler))
                {
                    VerticalRuler V = sender as VerticalRuler;
                    Canvas.SetLeft(V, Canvas.GetLeft(V) + e.Delta.Translation.X);
                }
                else if (sender.GetType() == typeof(HorizontalRuler))
                {
                    HorizontalRuler H = sender as HorizontalRuler;
                    Canvas.SetTop(H, Canvas.GetTop(H) + e.Delta.Translation.Y);
                }
            }
        }

        private void Ruler_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            IsGuideDragging = false;
        }

        private void Ruler_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            IsGuideDragging = true;
        }

        private void VerticalGuideBtn_Click(object sender, RoutedEventArgs e)
        {
            VerticalRuler V = new VerticalRuler();
            V.Height = 2000;
            V.ManipulationStarted += Ruler_ManipulationStarted;
            V.ManipulationCompleted += Ruler_ManipulationCompleted;
            V.ManipulationDelta += Ruler_ManipulationDelta;
            V.ManipulationMode = ManipulationModes.TranslateX;
            V.PointerEntered += Ruler_PointerEntered;
            V.PointerExited += Ruler_PointerExited;
            DrawCanvas.Children.Add(V);
            Canvas.SetLeft(V, 120);
        }

        private void MargueTool_Checked(object sender, RoutedEventArgs e)
        {
            if (MargueToolBtn.IsChecked == true)
            {
                RegionBorder.Opacity = 1;
            }
            else
            {
                RegionBorder.Opacity = 0;
            }
        }

        int compactViewId;
        private async void ShowCompactView()
        {
            CoreApplicationView compactView = CoreApplication.CreateNewView();
            await compactView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                compactViewId = ApplicationView.GetForCurrentView().Id;
                frame.Navigate(typeof(CompactOverlayPage));
                Window.Current.Content = frame;
                Window.Current.Activate();
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            });

           

            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(120, 120);
            bool viewShown = await ApplicationViewSwitcher.TryShowAsViewModeAsync(compactViewId, ApplicationViewMode.CompactOverlay, compactOptions);

            string X = XText.Text;
            string Y = YText.Text;
            string W = WText.Text;
            string H = HText.Text;
            await compactView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = (Frame)Window.Current.Content;
                var compactOverlay = (CompactOverlayPage)frame.Content;
                compactOverlay.SetMeasurements(X,Y,W,H);
            });
         

        }

        private void OverlayButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCompactView();
        }
    }
}
