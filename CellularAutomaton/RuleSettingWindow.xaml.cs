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
    /// Interaction logic for RuleSettingWindow.xaml
    /// </summary>
    public partial class RuleSettingWindow : Window
    {
        private List<Rule> _rules;
        private StatusList _status;
        private Style _blackButtonStyle;
        public RuleSettingWindow(List<Rule> rules, StatusList status)
        {
            InitializeComponent();
            _rules = rules;
            _status = status;

            _blackButtonStyle = Application.Current.Resources["BlackButton"] as Style;
            RefreshList();
        }

        private void RefreshList()
        {
            RuleButtonStackPanel.Children.Clear();
            for (int i = 0; i < _rules.Count; ++i)
            {
                Grid grid = new Grid { Height = 100 };
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions[1].Width = new GridLength(50);
                grid.ColumnDefinitions[2].Width = new GridLength(50);

                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                Button newRuleButton = new Button
                {
                    Name = "Rule_" + i,
                    Content = _rules[i].DescriptionString,
                    Margin = new Thickness(5),
                    Style = _blackButtonStyle,
                    ToolTip = "Edit Rule " + _rules[i].DescriptionString
                };
                Grid.SetRowSpan(newRuleButton, 2);
                grid.Children.Add(newRuleButton);
                newRuleButton.Click += RuleButton_Click;

                Button newAddButton = new Button
                {
                    Name = "Add_" + i,
                    Content = "\xE710",
                    Margin = new Thickness(5),
                    Style = _blackButtonStyle,
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    ToolTip = "Add a New Rule after This One"
                };
                Grid.SetRow(newAddButton, 0);
                Grid.SetColumn(newAddButton, 1);
                grid.Children.Add(newAddButton);
                newAddButton.Click += AddButton_Click;

                Button newRemoveButton = new Button
                {
                    Name = "Remove_" + i,
                    Content = "\xE738",
                    Margin = new Thickness(5),
                    Style = _blackButtonStyle,
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    ToolTip = "Remove This Rule"
                };
                Grid.SetRow(newRemoveButton, 1);
                Grid.SetColumn(newRemoveButton, 1);
                grid.Children.Add(newRemoveButton);
                newRemoveButton.Click += RemoveButton_Click;
                if (_rules.Count == 1) newRemoveButton.IsEnabled = false;

                Button newUpButton = new Button
                {
                    Name = "Up_" + i,
                    Content = "\xE70E",
                    Margin = new Thickness(5),
                    Style = _blackButtonStyle,
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    ToolTip = "Move up"
                };
                Grid.SetRow(newUpButton, 0);
                Grid.SetColumn(newUpButton, 2);
                grid.Children.Add(newUpButton);
                newUpButton.Click += UpButton_Click;
                if (i == 0) newUpButton.IsEnabled = false;

                Button newDownButton = new Button
                {
                    Name = "Down_" + i,
                    Content = "\xE70D",
                    Margin = new Thickness(5),
                    Style = _blackButtonStyle,
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    ToolTip = "Move Down"
                };
                Grid.SetRow(newDownButton, 1);
                Grid.SetColumn(newDownButton, 2);
                grid.Children.Add(newDownButton);
                newDownButton.Click += DownButton_Click;
                if (i == _rules.Count - 1) newDownButton.IsEnabled = false;

                RuleButtonStackPanel.Children.Add(grid);
            }
        }

        private void RuleButton_Click(object sender, RoutedEventArgs e)
        {
            Button ruleButton = (Button)e.Source;
            int index = int.Parse(ruleButton.Name.Split('_')[1]);
            Rule rule = _rules[index];
            //MessageBox.Show(rule.DescriptionString + " " + ruleButton.Name);
            // TODO
            EditRule window = new EditRule(rule, _status);
            window.ShowDialog();

            RefreshList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Button addButton = (Button)e.Source;
            int index = int.Parse(addButton.Name.Split('_')[1]);
            Rule rule = _rules[index];
            // TODO
            //MessageBox.Show(rule.DescriptionString + " " + addButton.Name);
            Rule newRule = new Rule("New Rule", 0, 0, 0, 0);
            _rules.Insert(index + 1, newRule);

            RefreshList();

            EditRule window = new EditRule(newRule, _status);
            window.ShowDialog();

            RefreshList();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button removeButton = (Button)e.Source;
            int index = int.Parse(removeButton.Name.Split('_')[1]);
            Rule rule = _rules[index];
            
            _rules.RemoveAt(index);
            RefreshList();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            Button upButton = (Button)e.Source;
            int index = int.Parse(upButton.Name.Split('_')[1]);
            Rule rule = _rules[index];

            _rules.RemoveAt(index);
            _rules.Insert(index - 1, rule);
            RefreshList();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            Button downButton = (Button)e.Source;
            int index = int.Parse(downButton.Name.Split('_')[1]);
            Rule rule = _rules[index];

            _rules.RemoveAt(index);
            _rules.Insert(index + 1, rule);
            RefreshList();
        }
    }
}
