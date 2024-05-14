using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace MouseTracking
{
    public partial class MainWindow : Window
    {
        private readonly ConfigSettings _settings;
        private Point _lastMousePosition;
        DoubleAnimation animation = new DoubleAnimation();

        public MainWindow()
        {
            InitializeComponent();
            _lastMousePosition = new Point();
            _settings = ConfigLoader.LoadConfigSettings("ConfigSettings.xml");
            appCircle.Fill = _settings.DefaultColor;
            appCircle.Width = _settings.FromSize;
            appCircle.Height = _settings.FromSize;
            // Initialize the timer for periodic updates


            // Hide the default cursor
            Cursor = Cursors.None;

            // Subscribe to the MouseMove event
            MouseHook.Start();
            MouseHook.MouseMove += OnGlobalMouseMove;
            MouseHook.MouseClickAction += InteractiveEllipse_MouseDown;
            animation.Completed += RevertCircleSizeAndColor;
        }

        private void InteractiveEllipse_MouseDown(object sender, MouseClickEventArgs e)
        {

            switch (e.Action)
            {
                case MouseAction.LMBDown:
                    appCircle.Fill = _settings.LMBClickColor;

                    // Create and start a motion animation for the ellipse
                    animation.From = _settings.FromSize;  // Initial size
                    animation.To = _settings.ToSize;    // Target size
                    animation.Duration = new Duration(TimeSpan.FromSeconds(_settings.Duration));  // Animation duration

                    // Set the center point for scaling (optional)
                    appCircle.RenderTransformOrigin = new Point(0.5, 0.5);

                    // Apply the animation to the Width and Height properties of the ellipse
                    appCircle.BeginAnimation(Ellipse.WidthProperty, animation);
                    appCircle.BeginAnimation(Ellipse.HeightProperty, animation);
                    break;
                case MouseAction.LMBUp:
                    

                    break;
                case MouseAction.RMBDown:
                    appCircle.Fill = _settings.RMBClickColor;

                    // Create and start a motion animation for the ellipse
                    animation.From = _settings.FromSize;  // Initial size
                    animation.To = _settings.ToSize;     // Target size
                    animation.Duration = new Duration(TimeSpan.FromSeconds(_settings.Duration));  // Animation duration

                    // Set the center point for scaling (optional)
                    appCircle.RenderTransformOrigin = new Point(0.5, 0.5);

                    // Apply the animation to the Width and Height properties of the ellipse
                    appCircle.BeginAnimation(Ellipse.WidthProperty, animation);
                    appCircle.BeginAnimation(Ellipse.HeightProperty, animation);
                    break;
                case MouseAction.RMBUp:
                    
                    break;
            }
        }

        public void RevertCircleSizeAndColor(object sender, EventArgs e)
        {
            appCircle.BeginAnimation(Ellipse.WidthProperty, null);
            appCircle.Fill = _settings.DefaultColor;
            appCircle.Width = _settings.FromSize;
            appCircle.Height = _settings.FromSize;

            // Create and start a motion animation for the ellipse
            animation.From = _settings.FromSize;  // Initial size
            animation.To = _settings.FromSize;    // Target size
            animation.Duration = new Duration(TimeSpan.FromSeconds(0));  // Animation duration

            // Set the center point for scaling (optional)
            appCircle.RenderTransformOrigin = new Point(0.5, 0.5);

            // Apply the animation to the Width and Height properties of the ellipse
            appCircle.BeginAnimation(Ellipse.WidthProperty, animation);
            appCircle.BeginAnimation(Ellipse.HeightProperty, animation);
        }

        private void OnGlobalMouseMove(object sender, Point e)
        {
            appCircle.Margin = new Thickness(e.X + _settings.FromSize + _settings.MoveX, e.Y + _settings.FromSize + _settings.MoveY, 0, 0);
            _lastMousePosition = e;
        }
    }
}
