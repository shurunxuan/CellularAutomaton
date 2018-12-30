using System;
using System.Collections.Generic;
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

namespace CellularAutomaton
{
    /// <summary>
    /// Interaction logic for EditStatus.xaml
    /// </summary>
    public partial class EditStatus : Window
    {
        private StatusList _statusList;
        private Status _status;
        private static readonly Regex _regex = new Regex("[^0-9]+");

        private void TextBoxIntInputValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }


        public EditStatus(StatusList statusList, Status status)
        {
            _statusList = statusList;
            _status = status;
            InitializeComponent();

            NameTextBox.Text = status.Description;
            RTextBox.Text = status.DisplayColor.R.ToString();
            GTextBox.Text = status.DisplayColor.G.ToString();
            BTextBox.Text = status.DisplayColor.B.ToString();

            NameTextBox.TextChanged += NameTextBox_TextChanged;
            RTextBox.TextChanged += RGBTextBox_TextChanged;
            GTextBox.TextChanged += RGBTextBox_TextChanged;
            BTextBox.TextChanged += RGBTextBox_TextChanged;
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _status.Description = NameTextBox.Text;
        }

        private void RGBTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte r = RTextBox.Text.Length == 0 ? (byte)0 : byte.Parse(RTextBox.Text);
            byte g = GTextBox.Text.Length == 0 ? (byte)0 : byte.Parse(GTextBox.Text);
            byte b = BTextBox.Text.Length == 0 ? (byte)0 : byte.Parse(BTextBox.Text);

            _status.DisplayColor = Color.FromRgb(r, g, b);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
