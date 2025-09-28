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
        private bool _isMenuVisible = false;
        private bool _isAnimating = false;

        private DieselLocomotive _locomotive = new DieselLocomotive();

        private Logger _logger = new Logger();
        private EngineControlUnit _engineControlUnit;

        public MainWindow()
        {
            InitializeComponent();

            SideMenu.RenderTransform = new TranslateTransform(-SideMenu.Width, 0);
            MainContent.Content = _logger;
            MainGrid.Background = _logger.LoggerBackground;

            _engineControlUnit = new EngineControlUnit(_locomotive);

            this.PreviewMouseLeftButtonDown += MainWindow_PreviewMouseLeftButtonDown;

            foreach (var msg in DieselMessagesCatalog.Messages)
            {
                _logger.AddLog(msg);
            }
        }

        private void MainWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isMenuVisible)
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
            if (_isAnimating) return;
            _isAnimating = true;

            Overlay.Visibility = Visibility.Visible;
            Overlay.IsHitTestVisible = true;

            var transform = (TranslateTransform)SideMenu.RenderTransform;
            var sideMenuAnimation = new DoubleAnimation
            {
                By = SideMenu.Width,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            sideMenuAnimation.Completed += (s, e) => { _isAnimating = false; };
            transform.BeginAnimation(TranslateTransform.XProperty, sideMenuAnimation);

            var overlayAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            Overlay.BeginAnimation(UIElement.OpacityProperty, overlayAnimation);

            _isMenuVisible = true;
        }

        private void HideMenu()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var transform = (TranslateTransform)SideMenu.RenderTransform;
            var sideMenuAnimation = new DoubleAnimation
            {
                By = -SideMenu.Width,
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
                _isAnimating = false;
            };
            Overlay.BeginAnimation(UIElement.OpacityProperty, overlayAnimation);

            _isMenuVisible = false;
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_isMenuVisible)
                HideMenu();
            else
                ShowMenu();
        }

        private void Overlay_Click(object sender, MouseButtonEventArgs e)
        {
            if (!_isAnimating)
                HideMenu();
        }

        private void ShowLogger_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _logger;
            MainGrid.Background = _logger.LoggerBackground;
        }

        private void ShowEngineControlUnit_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _engineControlUnit;
        }
    }
}