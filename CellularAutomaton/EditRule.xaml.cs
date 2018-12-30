using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
    /// Interaction logic for EditRule.xaml
    /// </summary>
    public partial class EditRule : Window
    {
        private Rule _rule;
        private StatusList _statusList;
        private static readonly Regex _regex = new Regex("[^0-9]+");

        public EditRule(Rule rule, StatusList statusList)
        {
            _rule = rule;
            _statusList = statusList;
            InitializeComponent();
            InitializeCheckTextBox();
            InitializeCanvas();
        }

        private void InitializeCheckTextBox()
        {
            RuleNameTextBox.Text = _rule.DescriptionString;

            RuleLeftDimensionTextBox.Text = _rule.Left.ToString();
            RuleRightDimensionTextBox.Text = _rule.Right.ToString();
            RuleTopDimensionTextBox.Text = _rule.Top.ToString();
            RuleBottomDimensionTextBox.Text = _rule.Bottom.ToString();

            RuleRotationCheckBox_0.IsChecked = _rule.AllowRotateType0;
            RuleRotationCheckBox_1.IsChecked = _rule.AllowRotateType1;
            RuleRotationCheckBox_2.IsChecked = _rule.AllowRotateType2;
            RuleRotationCheckBox_3.IsChecked = _rule.AllowRotateType3;
            RuleRotationCheckBox_4.IsChecked = _rule.AllowRotateType4;
            RuleRotationCheckBox_5.IsChecked = _rule.AllowRotateType5;
            RuleRotationCheckBox_6.IsChecked = _rule.AllowRotateType6;
            RuleRotationCheckBox_7.IsChecked = _rule.AllowRotateType7;

            RuleNameTextBox.TextChanged += RuleNameTextBox_TextChanged;
            RuleTopDimensionTextBox.TextChanged += RuleTopDimensionTextBox_TextChanged;
            RuleBottomDimensionTextBox.TextChanged += RuleBottomDimensionTextBox_TextChanged;
            RuleLeftDimensionTextBox.TextChanged += RuleLeftDimensionTextBox_TextChanged;
            RuleRightDimensionTextBox.TextChanged += RuleRightDimensionTextBox_TextChanged;

            RuleRotationCheckBox_0.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_1.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_2.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_3.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_4.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_5.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_6.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_7.Checked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_0.Unchecked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_1.Unchecked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_2.Unchecked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_3.Unchecked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_4.Unchecked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_5.Unchecked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_6.Unchecked += RuleRotationCheckBox_Changed;
            RuleRotationCheckBox_7.Unchecked += RuleRotationCheckBox_Changed;
        }

        private void InitializeCanvas()
        {
            RefreshRuleCanvas();
            RefreshRuleResultCanvas();
        }

        private void RefreshRuleCanvas()
        {
            RuleCanvas.Children.Clear();

            // Default Fit to Screen Transforms
            Size size = RuleCanvas.RenderSize;
            double canvasRatio = size.Width / size.Height;
            double mapRatio = (_rule.Left + _rule.Right + 1) / (double)(_rule.Top + _rule.Bottom + 1);
            double fixedScale = 1.0;
            double fixedOffsetX = 0.0;
            double fixedOffsetY = 0.0;
            if (mapRatio > canvasRatio)
            {
                fixedScale = size.Width / ((_rule.Left + _rule.Right + 1) * 10.0) * 0.9;
            }
            else
            {
                fixedScale = size.Height / ((_rule.Top + _rule.Bottom + 1) * 10.0) * 0.9;
            }

            fixedOffsetX = (size.Width - (10 * (_rule.Left + _rule.Right + 1)) * fixedScale) / 2.0;
            fixedOffsetY = (size.Height - (10 * (_rule.Top + _rule.Bottom + 1)) * fixedScale) / 2.0;

            // Create transform objects
            var fixedRenderTransform = new TransformGroup();
            var fixedTranslateTransform = new TranslateTransform(fixedOffsetX, fixedOffsetY);
            var fixedScaleTransform = new ScaleTransform(fixedScale, fixedScale, 0, 0);
            fixedRenderTransform.Children.Add(fixedScaleTransform);
            fixedRenderTransform.Children.Add(fixedTranslateTransform);

            // Generate cells
            for (int i = 0; i <= _rule.Left + _rule.Right; ++i)
                for (int j = 0; j <= _rule.Top + _rule.Bottom; ++j)
                {
                    Rectangle cell = new Rectangle();
                    TransformGroup cellTransform = new TransformGroup();
                    cellTransform.Children.Add(new TranslateTransform(10 * i, 10 * j));
                    cellTransform.Children.Add(fixedRenderTransform);
                    cell.RenderTransform = cellTransform;
                    cell.Width = 10;
                    cell.Height = 10;
                    cell.Fill = new SolidColorBrush(_statusList[_rule[i - _rule.Left, j - _rule.Top]].DisplayColor);
                    cell.Name = "_" + i + "_" + j + "_";
                    cell.MouseRightButtonDown += Cell_OnMouseRightButtonDown;
                    RuleCanvas.Children.Add(cell);
                }

            // Draw grid
            for (int i = 0; i <= _rule.Left + _rule.Right + 1; ++i)
            {
                Line line = new Line();
                line.RenderTransformOrigin = new Point(0, 0);
                line.RenderTransform = fixedRenderTransform;
                line.X1 = 10 * i;
                line.X2 = 10 * i;
                line.Y1 = 0;
                line.Y2 = 10 * (_rule.Bottom + _rule.Top + 1);
                line.StrokeThickness = 0.5;
                line.Stroke = Brushes.LightGray;
                RuleCanvas.Children.Add(line);
            }

            for (int i = 0; i <= _rule.Bottom + _rule.Top + 1; ++i)
            {
                Line line = new Line();
                line.RenderTransformOrigin = new Point(0, 0);
                line.RenderTransform = fixedRenderTransform;
                line.X1 = 0;
                line.X2 = 10 * (_rule.Left + _rule.Right + 1);
                line.Y1 = 10 * i;
                line.Y2 = 10 * i;
                line.StrokeThickness = 0.5;
                line.Stroke = Brushes.LightGray;
                RuleCanvas.Children.Add(line);
            }

            // Indicate current cell
            Line crossLine1 = new Line();
            crossLine1.RenderTransformOrigin = new Point(0, 0);
            crossLine1.RenderTransform = fixedRenderTransform;
            crossLine1.X1 = (_rule.Left) * 10;
            crossLine1.Y1 = (_rule.Top) * 10;
            crossLine1.X2 = (_rule.Left + 1) * 10;
            crossLine1.Y2 = (_rule.Top + 1) * 10;
            crossLine1.StrokeThickness = 0.5;
            crossLine1.Stroke = Brushes.LightGray;
            RuleCanvas.Children.Add(crossLine1);

            Line crossLine2 = new Line();
            crossLine2.RenderTransformOrigin = new Point(0, 0);
            crossLine2.RenderTransform = fixedRenderTransform;
            crossLine2.X1 = (_rule.Left + 1) * 10;
            crossLine2.Y1 = (_rule.Top) * 10;
            crossLine2.X2 = (_rule.Left) * 10;
            crossLine2.Y2 = (_rule.Top + 1) * 10;
            crossLine2.StrokeThickness = 0.5;
            crossLine2.Stroke = Brushes.LightGray;
            RuleCanvas.Children.Add(crossLine2);

            for (int i = 0; i < 8; ++i)
            {
                RefreshRuleRotateCanvas(i);
            }
        }

        private void RefreshRuleResultCanvas()
        {
            RuleResultCanvas.Children.Clear();

            // Default Fit to Screen Transforms
            Size size = RuleResultCanvas.RenderSize;
            double canvasRatio = size.Width / size.Height;
            double mapRatio = 1;
            double fixedScale = 1.0;
            double fixedOffsetX = 0.0;
            double fixedOffsetY = 0.0;
            if (mapRatio > canvasRatio)
            {
                fixedScale = size.Width / (10.0) * 0.9;
            }
            else
            {
                fixedScale = size.Height / (10.0) * 0.9;
            }

            fixedOffsetX = (size.Width - (10) * fixedScale) / 2.0;
            fixedOffsetY = (size.Height - (10) * fixedScale) / 2.0;

            // Create transform objects
            var fixedRenderTransform = new TransformGroup();
            var fixedTranslateTransform = new TranslateTransform(fixedOffsetX, fixedOffsetY);
            var fixedScaleTransform = new ScaleTransform(fixedScale, fixedScale, 0, 0);
            fixedRenderTransform.Children.Add(fixedScaleTransform);
            fixedRenderTransform.Children.Add(fixedTranslateTransform);

            Rectangle cell = new Rectangle();
            cell.RenderTransform = fixedRenderTransform;
            cell.Width = 10;
            cell.Height = 10;
            cell.Fill = new SolidColorBrush(_statusList[_rule.Result].DisplayColor);
            cell.Name = "ResultCell";
            cell.MouseRightButtonDown += Cell_OnMouseRightButtonDown;
            RuleResultCanvas.Children.Add(cell);

            // Indicate current cell
            Line crossLine1 = new Line();
            crossLine1.RenderTransformOrigin = new Point(0, 0);
            crossLine1.RenderTransform = fixedRenderTransform;
            crossLine1.X1 = 0;
            crossLine1.Y1 = 0;
            crossLine1.X2 = 10;
            crossLine1.Y2 = 10;
            crossLine1.StrokeThickness = 0.5;
            crossLine1.Stroke = Brushes.LightGray;
            RuleResultCanvas.Children.Add(crossLine1);

            Line crossLine2 = new Line();
            crossLine2.RenderTransformOrigin = new Point(0, 0);
            crossLine2.RenderTransform = fixedRenderTransform;
            crossLine2.X1 = 10;
            crossLine2.Y1 = 0;
            crossLine2.X2 = 0;
            crossLine2.Y2 = 10;
            crossLine2.StrokeThickness = 0.5;
            crossLine2.Stroke = Brushes.LightGray;
            RuleResultCanvas.Children.Add(crossLine2);
        }

        private void RefreshRuleRotateCanvas(int index)
        {
            Canvas ruleRotateCanvas;
            TransformGroup rotateTransform = new TransformGroup();
            switch (index)
            {
                case 0:
                    ruleRotateCanvas = RuleRotationCanvas_0;
                    break;
                case 1:
                    ruleRotateCanvas = RuleRotationCanvas_1;
                    break;
                case 2:
                    ruleRotateCanvas = RuleRotationCanvas_2;
                    break;
                case 3:
                    ruleRotateCanvas = RuleRotationCanvas_3;
                    break;
                case 4:
                    ruleRotateCanvas = RuleRotationCanvas_4;
                    break;
                case 5:
                    ruleRotateCanvas = RuleRotationCanvas_5;
                    break;
                case 6:
                    ruleRotateCanvas = RuleRotationCanvas_6;
                    break;
                case 7:
                    ruleRotateCanvas = RuleRotationCanvas_7;
                    break;
                default:
                    MessageBox.Show("WTF?");
                    return;
            }

            ruleRotateCanvas.Children.Clear();


            // Default Fit to Screen Transforms
            Size size = ruleRotateCanvas.RenderSize;
            rotateTransform.Children.Add(new ScaleTransform(index > 3 ? -1 : 1, 1, size.Width / 2, size.Height / 2));
            rotateTransform.Children.Add(new RotateTransform(90.0 * (index % 4), size.Width / 2, size.Height / 2));
            double canvasRatio = size.Width / size.Height;
            double mapRatio = (_rule.Left + _rule.Right + 1) / (double)(_rule.Top + _rule.Bottom + 1);
            double fixedScale = 1.0;
            double fixedOffsetX = 0.0;
            double fixedOffsetY = 0.0;
            if (mapRatio > canvasRatio)
            {
                fixedScale = size.Width / ((_rule.Left + _rule.Right + 1) * 10.0) * 0.9;
            }
            else
            {
                fixedScale = size.Height / ((_rule.Top + _rule.Bottom + 1) * 10.0) * 0.9;
            }

            fixedOffsetX = (size.Width - (10 * (_rule.Left + _rule.Right + 1)) * fixedScale) / 2.0;
            fixedOffsetY = (size.Height - (10 * (_rule.Top + _rule.Bottom + 1)) * fixedScale) / 2.0;

            // Create transform objects
            var fixedRenderTransform = new TransformGroup();
            var fixedTranslateTransform = new TranslateTransform(fixedOffsetX, fixedOffsetY);
            var fixedScaleTransform = new ScaleTransform(fixedScale, fixedScale, 0, 0);
            fixedRenderTransform.Children.Add(fixedScaleTransform);
            fixedRenderTransform.Children.Add(fixedTranslateTransform);
            fixedRenderTransform.Children.Add(rotateTransform);

            // Generate cells
            for (int i = 0; i <= _rule.Left + _rule.Right; ++i)
                for (int j = 0; j <= _rule.Top + _rule.Bottom; ++j)
                {
                    Rectangle cell = new Rectangle();
                    TransformGroup cellTransform = new TransformGroup();
                    cellTransform.Children.Add(new TranslateTransform(10 * i, 10 * j));
                    cellTransform.Children.Add(fixedRenderTransform);
                    cell.RenderTransform = cellTransform;
                    cell.Width = 10;
                    cell.Height = 10;
                    cell.Fill = new SolidColorBrush(_statusList[_rule[i - _rule.Left, j - _rule.Top]].DisplayColor);
                    ruleRotateCanvas.Children.Add(cell);
                }

            // Indicate current cell
            Line crossLine1 = new Line();
            crossLine1.RenderTransformOrigin = new Point(0, 0);
            crossLine1.RenderTransform = fixedRenderTransform;
            crossLine1.X1 = (_rule.Left) * 10;
            crossLine1.Y1 = (_rule.Top) * 10;
            crossLine1.X2 = (_rule.Left + 1) * 10;
            crossLine1.Y2 = (_rule.Top + 1) * 10;
            crossLine1.StrokeThickness = 0.5;
            crossLine1.Stroke = Brushes.LightGray;
            ruleRotateCanvas.Children.Add(crossLine1);

            Line crossLine2 = new Line();
            crossLine2.RenderTransformOrigin = new Point(0, 0);
            crossLine2.RenderTransform = fixedRenderTransform;
            crossLine2.X1 = (_rule.Left + 1) * 10;
            crossLine2.Y1 = (_rule.Top) * 10;
            crossLine2.X2 = (_rule.Left) * 10;
            crossLine2.Y2 = (_rule.Top + 1) * 10;
            crossLine2.StrokeThickness = 0.5;
            crossLine2.Stroke = Brushes.LightGray;
            ruleRotateCanvas.Children.Add(crossLine2);
        }

        private void TextBoxIntInputValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void RuleNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _rule.DescriptionString = RuleNameTextBox.Text;
        }

        private void RuleTopDimensionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _rule.Top = RuleTopDimensionTextBox.Text.Length == 0 ? 0 : int.Parse(RuleTopDimensionTextBox.Text);

            RefreshRuleCanvas();
        }

        private void RuleLeftDimensionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _rule.Left = RuleLeftDimensionTextBox.Text.Length == 0 ? 0 : int.Parse(RuleLeftDimensionTextBox.Text);

            RefreshRuleCanvas();
        }

        private void RuleBottomDimensionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _rule.Bottom = RuleBottomDimensionTextBox.Text.Length == 0 ? 0 : int.Parse(RuleBottomDimensionTextBox.Text);

            RefreshRuleCanvas();
        }

        private void RuleRightDimensionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _rule.Right = RuleRightDimensionTextBox.Text.Length == 0 ? 0 : int.Parse(RuleRightDimensionTextBox.Text);

            RefreshRuleCanvas();
        }

        private void RuleRotationCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox source = (CheckBox)e.Source;
            int rotateMode = int.Parse(source.Name.Split('_')[1]);

            switch (rotateMode)
            {
                case 0:
                    _rule.AllowRotateType0 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType0;
                    break;
                case 1:
                    _rule.AllowRotateType1 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType1;
                    break;
                case 2:
                    _rule.AllowRotateType2 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType2;
                    break;
                case 3:
                    _rule.AllowRotateType3 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType3;
                    break;
                case 4:
                    _rule.AllowRotateType4 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType4;
                    break;
                case 5:
                    _rule.AllowRotateType5 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType5;
                    break;
                case 6:
                    _rule.AllowRotateType6 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType6;
                    break;
                case 7:
                    _rule.AllowRotateType7 = source.IsChecked ?? false;
                    source.IsChecked = _rule.AllowRotateType7;
                    break;
                default:
                    MessageBox.Show("WTF?");
                    break;
            }
        }

        private void RuleCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            RefreshRuleCanvas();
        }

        private void RuleResultCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            RefreshRuleResultCanvas();
        }

        private void RuleRotationCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            Canvas source = (Canvas)e.Source;
            int index = int.Parse(source.Name.Split('_')[1]);

            RefreshRuleRotateCanvas(index);
        }

        private void Cell_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Show status panel
            Rectangle cell = (Rectangle)e.Source;
            int index0 = 0, index1 = 0;
            //MessageBox.Show(cell.Name);

            //MessageBox.Show(cell.Name);

            PresentationSource source =
                PresentationSource.FromVisual(this);
            double dpiX = 0;
            double dpiY = 0;
            if (source?.CompositionTarget != null)
            {
                dpiX =
                    96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY =
                    96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            var mousePosition = PointToScreen(e.GetPosition(this));


            if (cell.Name != "ResultCell")
            {
                index0 = int.Parse(cell.Name.Split('_')[1]);
                index1 = int.Parse(cell.Name.Split('_')[2]);
            }

            StatusWindow statusWindow = new StatusWindow(_statusList, cell, true)
            {
                Left = mousePosition.X * 96.0 / dpiX,
                Top = mousePosition.Y * 96.0 / dpiY
            };
            statusWindow.ShowDialog();

            if (cell.Name == "ResultCell")
            {
                _rule.Result = _statusList.Find(((SolidColorBrush)cell.Fill).Color);
            }
            else
            {
                _rule[index0 - _rule.Left, index1 - _rule.Top] = _statusList.Find(((SolidColorBrush)cell.Fill).Color);
                for (int i = 0; i < 8; ++i)
                    RefreshRuleRotateCanvas(i);
            }
        }
    }
}
