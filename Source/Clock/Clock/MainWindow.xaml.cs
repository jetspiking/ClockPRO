using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Clock.Settings;

namespace Clock
{
    public partial class MainWindow : Window
    {
        public static MainWindow MainWindowInstance;

        public StorageSettings? StorageSettings;

        private DispatcherTimer _timer;
        private List<TextBlock> _clockNumbers = new List<TextBlock>();
        private List<Line> TickMarks = new List<Line>();

        public MainWindow()
        {
            InitializeComponent();

            MainWindow.MainWindowInstance = this;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _timer.Tick += UpdateClock;
            _timer.Start();

            // Generate tick marks
            GenerateTickMarks(12, 100, 140, 6); // Hour ticks
            GenerateTickMarks(60, 180, 200, 4); // Minute ticks

            this.StorageSettings = new();
            AddClockNumbers();
            UpdateGeneral();
            this.StorageSettings.ApplySettings();

        }

        private void UpdateClock(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            DigitalClockText.Text = currentTime.ToString("HH:mm:ss");

            double seconds = currentTime.Second;
            double minutes = currentTime.Minute + seconds / 60.0;
            double hours = currentTime.Hour % 12 + minutes / 60.0;

            SetClockHand(SecondHand, seconds, 60, 160);
            SetClockHand(MinuteHand, minutes, 60, 170);
            SetClockHand(HourHand, hours, 12, 120);
        }

        private void SetClockHand(Line hand, double value, double maxValue, double length)
        {
            double angle = (Math.PI / 2) - (2 * Math.PI * value / maxValue);
            hand.X2 = 250 + length * Math.Cos(angle);
            hand.Y2 = 250 - length * Math.Sin(angle);
        }

        private void GenerateTickMarks(int count, double startLength, double endLength, double thickness)
        {
            for (int i = 0; i < count; i++)
            {
                double angle = (Math.PI / 2) - (2 * Math.PI * i / count);
                Line tick = new Line()
                {
                    X1 = 250 + startLength * Math.Cos(angle),
                    Y1 = 250 - startLength * Math.Sin(angle),
                    X2 = 250 + endLength * Math.Cos(angle),
                    Y2 = 250 - endLength * Math.Sin(angle),
                    Stroke = Brushes.Black,
                    StrokeThickness = thickness
                };

                TickMarks.Add(tick);  // Add to the list
                ClockCanvas.Children.Add(tick);
            }
        }

        private void AddClockNumbers()
        {
            for (int i = 1; i <= 12; i++)
            {
                TextBlock number = new TextBlock
                {
                    Text = i.ToString(),
                    FontSize = 32,
                    FontFamily = DigitalClockText.FontFamily,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Width = 40,
                    Height = 40,
                    TextAlignment = TextAlignment.Center
                };

                _clockNumbers.Add(number);  // Add the TextBlock to the list
                ClockCanvas.Children.Add(number);
            }
        }


        private void PositionClockNumbers()
        {
            int radius = 215;

            for (int i = 0; i < _clockNumbers.Count; i++)  // Use the list for iteration
            {
                // Calculate angle similar to the tick marks
                double angle = (Math.PI / 2) - (2 * Math.PI * (i + 1) / 12);

                // Calculate x and y position
                double x = 250 + radius * Math.Cos(angle);
                double y = 250 - radius * Math.Sin(angle);

                TextBlock number = _clockNumbers[i];  // Retrieve the TextBlock from the list

                number.FontFamily = DigitalClockText.FontFamily;

                // Adjust for the TextBlock's size to center it
                x -= number.Width / 2;
                y -= number.Height / 2;

                if (i == 9 || i == 10) x -= 5;


                Canvas.SetLeft(number, x);
                Canvas.SetTop(number, y);

            }
        }


        private void UpdateVisibilityClockNumbers()
        {
            SolidColorBrush brush = this.StorageSettings.AppState.IsClockColorsInverted ? Brushes.White : Brushes.Black;
            foreach (var child in ClockCanvas.Children)
            {
                if (child is TextBlock textBlock && int.TryParse(textBlock.Text, out _))
                {
                    textBlock.Visibility = this.StorageSettings.AppState.IsDisplayClockNumbers ? Visibility.Visible : Visibility.Collapsed;
                    textBlock.Foreground = brush;
                }
            }
        }

        private void ShowHideTickMarks()
        {
            foreach (Line tick in TickMarks)
            {
                tick.Visibility = this.StorageSettings.AppState.IsDisplayTicks ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.ColorSwitchCheckbox.IsChecked = this.StorageSettings.AppState.IsClockColorsInverted;
            settings.DisplayDigitalCheckbox.IsChecked = this.StorageSettings.AppState.IsDisplayDigital;
            settings.DisplayClockNumbersCheckbox.IsChecked = this.StorageSettings.AppState.IsDisplayClockNumbers;
            settings.DisplayTicksCheckbox.IsChecked = this.StorageSettings.AppState.IsDisplayTicks;
            settings.ShowDialog();
            this.StorageSettings.SaveSettings();
        }

        public void UpdateGeneral()
        {
            UpdateClockVisibility();
            UpdateVisibilityClockNumbers();
            PositionClockNumbers();
            ShowHideTickMarks();
            UpdateFrameImage();
        }

        private void UpdateClockVisibility()
        {
            SolidColorBrush brush = this.StorageSettings.AppState.IsClockColorsInverted ? Brushes.White : Brushes.Black;
            DigitalClockText.Foreground = brush;
            foreach (Line tick in ClockCanvas.Children.OfType<Line>())
            {
                if (tick.Name != "SecondHand")
                    tick.Stroke = brush;
            }
            DigitalClockText.Visibility = this.StorageSettings.AppState.IsDisplayDigital ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateFrameImage()
        {
            if (!File.Exists(this.StorageSettings.AppState.ImagePath)) return;
            this.FrameImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(this.StorageSettings.AppState.ImagePath));
        }

        public void SettingFontSize(double fontSize)
        {
            this.DigitalClockText.FontSize = fontSize;

            this.StorageSettings.AppState.FontSize = fontSize;
            UpdateGeneral();
        }

        public void SettingFontFamily(FontFamily fontFamily)
        {
            this.DigitalClockText.FontFamily = fontFamily;

            this.StorageSettings.AppState.FontFamily = fontFamily;
            UpdateGeneral();
        }


        public void SettingFrameImage(String path)
        {
            this.StorageSettings.AppState.ImagePath = path;
            UpdateGeneral();
        }

        public void SettingColorsInverted(Boolean inverted)
        {
            this.StorageSettings.AppState.IsClockColorsInverted = inverted;
            UpdateGeneral();
        }

        public void SettingDisplayDigital(Boolean digital)
        {
            this.StorageSettings.AppState.IsDisplayDigital = digital;
            UpdateGeneral();
        }

        public void SettingDisplayClockNumbers(Boolean clockNumbers)
        {
            this.StorageSettings.AppState.IsDisplayClockNumbers = clockNumbers;
            UpdateGeneral();
        }

        public void SettingDisplayTicks(Boolean ticks)
        {
            this.StorageSettings.AppState.IsDisplayTicks = ticks;
            UpdateGeneral();
        }

        public void SettingWindowSize(int size)
        {
            MainWindow.MainWindowInstance.Width = size;
            MainWindow.MainWindowInstance.Height = size;

            this.StorageSettings.AppState.WindowSize = size;
            UpdateGeneral();
        }
    }
}
