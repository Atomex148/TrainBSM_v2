using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading;
using TrainBSM_v2.AppAppearance;

namespace TrainBSM_v2
{
    public partial class MainWindow : Window
    {
        private bool isMenuVisible = false;
        private bool isAnimating = false;

        private Logger logger = new Logger();
        private DataGrid table = new DataGrid();

        public MainWindow()
        {
            InitializeComponent();

            SideMenu.RenderTransform = new TranslateTransform(-SideMenu.Width, 0);
            MainContent.Content = logger;
            MainGrid.Background = logger.LoggerBackground;

            table.ItemsSource = new[]
            {
                new { ID = 1, Name = "Item1" },
                new { ID = 2, Name = "Item2" },
            };
            this.PreviewMouseLeftButtonDown += MainWindow_PreviewMouseLeftButtonDown;

            foreach (var msg in DieselMessagesCatalog.Messages)
            {
                logger.AddLog(msg);
            }
        }

        private void MainWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isMenuVisible)
            {
                if (!IsClickInsideElement(SideMenu, e))
                {
                    HideMenu();
                }
            }
        }

        private bool IsClickInsideElement(FrameworkElement element, MouseButtonEventArgs e)
        {
            Point clickPos = e.GetPosition(element);
            return clickPos.X >= 0 && clickPos.X <= element.ActualWidth &&
                   clickPos.Y >= 0 && clickPos.Y <= element.ActualHeight;
        }

        private void ShowMenu()
        {
            if (isAnimating) return;
            isAnimating = true;

            Overlay.Visibility = Visibility.Visible;
            Overlay.IsHitTestVisible = true;

            var transform = (TranslateTransform)SideMenu.RenderTransform;
            var sideMenuAnimation = new DoubleAnimation
            {
                From = transform.X,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            sideMenuAnimation.Completed += (s, e) => { isAnimating = false; };
            transform.BeginAnimation(TranslateTransform.XProperty, sideMenuAnimation);

            var overlayAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            Overlay.BeginAnimation(UIElement.OpacityProperty, overlayAnimation);

            isMenuVisible = true;
        }

        private void HideMenu()
        {
            if (isAnimating) return;
            isAnimating = true;

            var transform = (TranslateTransform)SideMenu.RenderTransform;
            var sideMenuAnimation = new DoubleAnimation
            {
                From = transform.X,
                To = -SideMenu.Width,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            transform.BeginAnimation(TranslateTransform.XProperty, sideMenuAnimation);

            var overlayAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            overlayAnimation.Completed += (s, e) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
                Overlay.IsHitTestVisible = false;
                isAnimating = false;
            };
            Overlay.BeginAnimation(UIElement.OpacityProperty, overlayAnimation);

            isMenuVisible = false;
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (isMenuVisible)
                HideMenu();
            else
                ShowMenu();
        }

        private void Overlay_Click(object sender, MouseButtonEventArgs e)
        {
            if (!isAnimating)
                HideMenu();
        }

        private void ShowLogger_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = logger;
            MainGrid.Background = logger.LoggerBackground;
        }

        private void ShowTable_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = table;
        }
    }
}