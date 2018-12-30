using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CellularAutomaton
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9]+");
        private Map _map;
        private int _width;
        private int _height;
        private DispatcherTimer _timer;
        private void TextBoxIntInputValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        public SettingWindow(Map map, DispatcherTimer timer)
        {
            InitializeComponent();
            _map = map;
            _timer = timer;
            _width = _map.Width;
            _height = _map.Height;

            WidthTextBox.Text = _width.ToString();
            HeightTextBox.Text = _height.ToString();
            IntervalTextBox.Text = ((int)_timer.Interval.TotalMilliseconds).ToString();
        }

        private void SettingWindow_OnClosing(object sender, CancelEventArgs e)
        {
            
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _map.Resize(int.Parse(WidthTextBox.Text), int.Parse(HeightTextBox.Text));
            _timer.Interval = new TimeSpan(0, 0, 0, 0, int.Parse(IntervalTextBox.Text));
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
