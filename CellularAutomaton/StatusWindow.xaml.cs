using System;
using System.Collections.Generic;
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

namespace CellularAutomaton
{
    /// <summary>
    /// Interaction logic for StatusWindow.xaml
    /// </summary>
    public partial class StatusWindow : Window
    {
        private StatusList _statusList;
        private Style _blackButtonStyle;
        private Rectangle _source;
        private bool _showAnyStatus;

        public StatusWindow(StatusList statusList, Rectangle source, bool showAnyStatus)
        {
            _statusList = statusList;
            _source = source;
            _showAnyStatus = showAnyStatus;
            _blackButtonStyle = Application.Current.Resources["BlackButton"] as Style;
            InitializeComponent();
        }

        private void RefreshStatusPanel()
        {
            StatusPanel.Children.Clear();
            for (int i = _showAnyStatus ? -1 : 0; i < _statusList.Length; ++i)
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions[0].Width = new GridLength(30);
                grid.ColumnDefinitions[1].Width = new GridLength(200);

                Label newLabel = new Label();
                newLabel.Background = new SolidColorBrush(_statusList[i].DisplayColor);
                newLabel.Height = 20;
                newLabel.Margin = new Thickness(5);
                if (((SolidColorBrush) newLabel.Background).Color == Colors.Black)
                {
                    newLabel.BorderBrush = Brushes.Gray;
                    newLabel.BorderThickness = new Thickness(0.5);
                }

                newLabel.Name = "Label_" + (i < 0 ? "A" : i.ToString());
                newLabel.MouseLeftButtonDown += StatusLabel_OnMouseLeftButtonDown;
                newLabel.ToolTip = "Edit Status";
                Grid.SetColumn(newLabel, 0);
                grid.Children.Add(newLabel);

                Button newButton = new Button();
                newButton.Content = _statusList[i].Description;
                newButton.Margin = new Thickness(5);
                newButton.Style = _blackButtonStyle;
                newButton.Name = "Status_" + (i < 0 ? "A" : i.ToString());
                newButton.Click += StatusButton_Click;
                Grid.SetColumn(newButton, 1);
                grid.Children.Add(newButton);

                StatusPanel.Children.Add(grid);
            }
        }

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            Button source = (Button)e.Source;
            string idStr = source.Name.Split('_')[1];
            int id = idStr == "A" ? -1 : int.Parse(idStr);

            _source.Fill = new SolidColorBrush(_statusList[id].DisplayColor);
            Close();
        }

        private void StatusPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            RefreshStatusPanel();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddStatusButton_Click(object sender, RoutedEventArgs e)
        {
            Status newStatus = new Status("New Status", Colors.Black);
            EditStatus window = new EditStatus(_statusList, newStatus);
            window.ShowDialog();

            bool succeeded = _statusList.Add(newStatus);
            while (!succeeded)
            {
                MessageBox.Show("The description or color of the new status is duplicated!");
                window = new EditStatus(_statusList, newStatus);
                window.ShowDialog();
                succeeded = _statusList.Add(newStatus);
            }

            RefreshStatusPanel();
        }

        private void StatusLabel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label source = (Label)e.Source;
            string idStr = source.Name.Split('_')[1];
            int id = idStr == "A" ? -1 : int.Parse(idStr);

            if (id == -1) return;

            EditStatus window = new EditStatus(_statusList, _statusList[id]);
            window.ShowDialog();

            bool succeeded = _statusList.Find(_statusList[id].DisplayColor) >= 0 && _statusList.Find(_statusList[id].Description) >= 0;
            while (!succeeded)
            {
                MessageBox.Show("The description or color of the new status is duplicated!");
                window = new EditStatus(_statusList, _statusList[id]);
                window.ShowDialog();
                succeeded = _statusList.Find(_statusList[id].DisplayColor) >= 0 && _statusList.Find(_statusList[id].Description) >= 0;
            }

            RefreshStatusPanel();
        }
    }
}
