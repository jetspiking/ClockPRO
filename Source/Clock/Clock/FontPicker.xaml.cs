using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Clock
{
    public partial class FontPicker : Window
    {
        public FontPicker()
        {
            InitializeComponent();

            if (MainWindow.MainWindowInstance.StorageSettings.AppState == null) return;
            this.FontFamilyComboBox.SelectedItem = MainWindow.MainWindowInstance.StorageSettings.AppState.FontFamily;
            this.FontSizeSlider.Value = MainWindow.MainWindowInstance.StorageSettings.AppState.FontSize;
        }

        private void OnApplyClicked(object sender, RoutedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null)
            {
                MainWindow.MainWindowInstance.SettingFontFamily((FontFamily)FontFamilyComboBox.SelectedItem);
                MainWindow.MainWindowInstance.SettingFontSize(FontSizeSlider.Value);
            }
            MainWindow.MainWindowInstance.UpdateGeneral();
        }
    }

}
