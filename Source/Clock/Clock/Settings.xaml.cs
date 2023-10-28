using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Clock
{
    public partial class Settings : Window
    {
        public class AppState
        {
            public String? ImagePath { get; set; } = null;
            public Boolean IsClockColorsInverted { get; set; } = false;
            public Boolean IsDisplayDigital { get; set; } = false;
            public Boolean IsDisplayClockNumbers { get; set; } = true;
            public Boolean IsDisplayTicks { get; set; } = true;
            public int WindowSize { get; set; } = 250;
            public FontFamily FontFamily { get; set; } = new FontFamily("Segoe UI");
            public double FontSize { get; set; } = 72;
        }

        public class StorageSettings
        {
            public AppState? AppState { get; set; } = new();
            private String _documentsPath { get; set; }

            public StorageSettings()
            {
                CreateSaveDirectory();
                LoadFromSaveDirectory();
            }

            public void SaveSettings()
            {
                SaveToSettingsDirectory(MainWindow.MainWindowInstance.StorageSettings.AppState);
            }

            public void CreateSaveDirectory()
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string folderName = "Clock PRO";
                this._documentsPath = Path.Combine(documentsPath, folderName);

                if (!Directory.Exists(this._documentsPath))
                    Directory.CreateDirectory(this._documentsPath);
            }

            public void LoadFromSaveDirectory()
            {
                if (!File.Exists(Path.Combine(this._documentsPath, "ClockPROSettings.json"))) return;
                String? json = File.ReadAllText(Path.Combine(this._documentsPath, "ClockPROSettings.json"));
                this.AppState = JsonConvert.DeserializeObject<AppState>(json);
            }

            public void ApplySettings()
            {
                if (this.AppState == null) return;
                MainWindow.MainWindowInstance.SettingFontSize(this.AppState.FontSize);
                MainWindow.MainWindowInstance.SettingFontFamily(this.AppState.FontFamily);
                MainWindow.MainWindowInstance.SettingColorsInverted(this.AppState.IsClockColorsInverted);
                MainWindow.MainWindowInstance.SettingDisplayDigital(this.AppState.IsDisplayDigital);
                MainWindow.MainWindowInstance.SettingDisplayClockNumbers(this.AppState.IsDisplayClockNumbers);
                MainWindow.MainWindowInstance.SettingDisplayTicks(this.AppState.IsDisplayTicks);
                MainWindow.MainWindowInstance.SettingWindowSize(this.AppState.WindowSize);
                MainWindow.MainWindowInstance.SettingFrameImage(this.AppState.ImagePath);
            }

            public void SaveToSettingsDirectory(AppState settings)
            {
                File.WriteAllText(Path.Combine(this._documentsPath, "ClockPROSettings.json"), JsonConvert.SerializeObject(this.AppState));
            }

        }

        public Settings()
        {
            InitializeComponent();
            ClockBox.Text = MainWindow.MainWindowInstance.Width.ToString();
        }

        private void OnFontChangeClicked(object sender, RoutedEventArgs e)
        {
            FontPicker fontPicker = new FontPicker();
            fontPicker.ShowDialog();
        }

        private void OnFrameChangeClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                MainWindow.MainWindowInstance.SettingFrameImage(openFileDialog.FileName);
                MainWindow.MainWindowInstance.StorageSettings.SaveSettings();
            }
        }

        private void ColorSwitchCheckboxClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MainWindowInstance.SettingColorsInverted(ColorSwitchCheckbox.IsChecked.Value);
        }

        private void DisplayDigitalCheckboxClicked(object sender, RoutedEventArgs e)
        {
            DisplayTicksCheckbox.IsChecked = false;
            MainWindow.MainWindowInstance.StorageSettings.AppState.IsDisplayTicks = false;
            DisplayClockNumbersCheckbox.IsChecked = false;
            MainWindow.MainWindowInstance.StorageSettings.AppState.IsDisplayClockNumbers = false;

            MainWindow.MainWindowInstance.SettingDisplayDigital(DisplayDigitalCheckbox.IsChecked.Value);
        }

        private void DisplayClockNumbersCheckboxClicked(object sender, RoutedEventArgs e)
        {
            DisplayDigitalCheckbox.IsChecked = false;
            MainWindow.MainWindowInstance.StorageSettings.AppState.IsDisplayDigital = false;

            MainWindow.MainWindowInstance.SettingDisplayClockNumbers(DisplayClockNumbersCheckbox.IsChecked.Value);
        }
        private void DisplayTicksCheckboxClicked(object sender, RoutedEventArgs e)
        {
            DisplayDigitalCheckbox.IsChecked = false;
            MainWindow.MainWindowInstance.StorageSettings.AppState.IsDisplayDigital = false;

            MainWindow.MainWindowInstance.SettingDisplayTicks(DisplayTicksCheckbox.IsChecked.Value);
        }

        private void ApplyClockBoxClicked(object sender, RoutedEventArgs e)
        {
            int size = int.Parse(ClockBox.Text);
            MainWindow.MainWindowInstance.SettingWindowSize(size);
        }
    }
}
