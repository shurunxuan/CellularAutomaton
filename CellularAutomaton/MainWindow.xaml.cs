using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace CellularAutomaton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Map of cellulars
        private Map _map;
        // Rules 
        private List<Rule> _rules;
        // Statuses
        private StatusList _status;
        // All cells
        private Rectangle[,] _cells;
        // Default Fit to Screen Transforms
        private TransformGroup _fixedRenderTransform;
        private ScaleTransform _fixedScaleTransform;
        private TranslateTransform _fixedTranslateTransform;
        // Parameters of Fit to Screen Transforms
        private double _fixedScale = 1.0;
        private double _fixedOffsetX = 0.0;
        private double _fixedOffsetY = 0.0;
        // Scale & Translate Transforms Modified by Mouse Events
        private TransformGroup _renderTransform;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;
        // Parameters of Mouse Transforms
        private double _scale = 1.0;
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;
        private Point _currentMousePosition;
        // Flag of Keeping Fit to Screen
        private bool _fitScreen = true;
        // Timer
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            _map = new Map(10, 10);
            _rules = new List<Rule>(0);
            _status = new StatusList();

            InitializeComponent();

            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //_dispatcherTimer.Start();
        }

        private void ResetButton_Click(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(cell.Name);
            Rectangle newCell = new Rectangle();
            newCell.Fill = Brushes.Black;
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

            var mousePosition = this.PointToScreen(e.GetPosition(this));

            StatusWindow statusWindow = new StatusWindow(_status, newCell, false)
            {
                Left = mousePosition.X * 96.0 / dpiX,
                Top = mousePosition.Y * 96.0 / dpiY
            };
            statusWindow.ShowDialog();

            for (int i = 0; i < _map.Width; ++i)
                for (int j = 0; j < _map.Height; ++j)
                {
                    _map[i, j] = _status.Find(((SolidColorBrush)newCell.Fill).Color);
                }

            for (int i = 0; i < _map.Width; ++i)
                for (int j = 0; j < _map.Height; ++j)
                {
                    _cells[i, j].Fill = new SolidColorBrush(_status[_map[i, j]].DisplayColor);
                }
        }

        // Calculate the parameters of Fit to Screen transform
        private void CalculateFitTransform()
        {
            Size size = MainCanvas.RenderSize;
            double canvasRatio = size.Width / size.Height;
            double mapRatio = _map.Width / (double)_map.Height;
            if (mapRatio > canvasRatio)
            {
                _fixedScale = size.Width / (_map.Width * 10.0) * 0.95;
            }
            else
            {
                _fixedScale = size.Height / (_map.Height * 10.0) * 0.95;
            }
            _fixedOffsetX = (size.Width - (10 * _map.Width) * _fixedScale) / 2.0;
            _fixedOffsetY = (size.Height - (10 * _map.Height) * _fixedScale) / 2.0;
        }

        // Set the parameters to fixed transform objects
        private void SetFixedTransforms()
        {
            _fixedTranslateTransform.X = _fixedOffsetX;
            _fixedTranslateTransform.Y = _fixedOffsetY;
            _fixedScaleTransform.ScaleX = _fixedScale;
            _fixedScaleTransform.ScaleY = _fixedScale;
        }

        // Set the parameters to mouse transform objects
        private void SetTransforms()
        {
            _translateTransform.X = _offsetX;
            _translateTransform.Y = _offsetY;
            _scaleTransform.ScaleX = _scale;
            _scaleTransform.ScaleY = _scale;
        }

        // Reset the mouse transform objects
        private void ResetTransforms()
        {
            _offsetX = 0;
            _offsetY = 0;
            _scale = 1;
            SetTransforms();
        }

        private void ReconstructGrid()
        {
            _cells = new Rectangle[_map.Width, _map.Height];
            MainCanvas.Children.Clear();
            CalculateFitTransform();

            // Create transform objects
            _fixedRenderTransform = new TransformGroup();
            _fixedTranslateTransform = new TranslateTransform(_fixedOffsetX, _fixedOffsetY);
            _fixedScaleTransform = new ScaleTransform(_fixedScale, _fixedScale, 0, 0);
            _fixedRenderTransform.Children.Add(_fixedScaleTransform);
            _fixedRenderTransform.Children.Add(_fixedTranslateTransform);

            _renderTransform = new TransformGroup();
            _translateTransform = new TranslateTransform();
            _scaleTransform = new ScaleTransform();
            _renderTransform.Children.Add(_scaleTransform);
            _renderTransform.Children.Add(_translateTransform);

            _fixedRenderTransform.Children.Add(_renderTransform);

            // Generate cells
            for (int i = 0; i < _map.Width; ++i)
                for (int j = 0; j < _map.Height; ++j)
                {
                    Rectangle cell = new Rectangle();
                    TransformGroup cellTransform = new TransformGroup();
                    cellTransform.Children.Add(new TranslateTransform(10 * i, 10 * j));
                    cellTransform.Children.Add(_fixedRenderTransform);
                    cell.RenderTransform = cellTransform;
                    cell.Width = 10;
                    cell.Height = 10;
                    cell.Fill = new SolidColorBrush(_status[_map[i, j]].DisplayColor);
                    cell.Name = "_" + j + "_" + i + "_";
                    cell.MouseRightButtonDown += Cell_OnMouseRightButtonDown;
                    //cell.MouseMove += MainCanvas_OnMouseMove;
                    MainCanvas.Children.Add(cell);
                    _cells[i, j] = cell;
                }

            // Draw grid
            for (int i = 0; i <= _map.Width; ++i)
            {
                Line line = new Line();
                line.RenderTransformOrigin = new Point(0, 0);
                line.RenderTransform = _fixedRenderTransform;
                line.X1 = 10 * i;
                line.X2 = 10 * i;
                line.Y1 = 0;
                line.Y2 = 10 * _map.Height;
                line.StrokeThickness = 0.5;
                line.Stroke = Brushes.Gray;
                MainCanvas.Children.Add(line);
            }

            for (int i = 0; i <= _map.Height; ++i)
            {
                Line line = new Line();
                line.RenderTransformOrigin = new Point(0, 0);
                line.RenderTransform = _fixedRenderTransform;
                line.X1 = 0;
                line.X2 = 10 * _map.Width;
                line.Y1 = 10 * i;
                line.Y2 = 10 * i;
                line.StrokeThickness = 0.5;
                line.Stroke = Brushes.Gray;
                MainCanvas.Children.Add(line);
            }
        }

        private void MainCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            ReconstructGrid();
        }

        private void Cell_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // ShowDialog status panel
            Rectangle cell = (Rectangle)e.Source;
            int index0 = int.Parse(cell.Name.Split('_')[1]);
            int index1 = int.Parse(cell.Name.Split('_')[2]);

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

            var mousePosition = MainCanvas.PointToScreen(e.GetPosition(MainCanvas));

            StatusWindow statusWindow = new StatusWindow(_status, cell, false)
            {
                Left = mousePosition.X * 96.0 / dpiX,
                Top = mousePosition.Y * 96.0 / dpiY
            };
            statusWindow.ShowDialog();

            _map[index0, index1] = _status.Find(((SolidColorBrush)cell.Fill).Color);
        }

        private void MainCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize == new Size()) return;
            if (_map == null) return;
            if (!_fitScreen) return;
            // Keep Fit to Screen
            CalculateFitTransform();
            SetFixedTransforms();
            ResetTransforms();
        }

        private void MainCanvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Scale with mouse wheel
            _fitScreen = false;

            double scale = 1 + e.Delta / 250.0;
            _offsetX = (_offsetX - _currentMousePosition.X) * scale + _currentMousePosition.X;
            _offsetY = (_offsetY - _currentMousePosition.Y) * scale + _currentMousePosition.Y;
            _scale *= scale;
            SetTransforms();
        }

        private void FitToScreenButton_Click(object sender, RoutedEventArgs e)
        {
            // Fit to Screen
            _fitScreen = true;

            CalculateFitTransform();
            SetFixedTransforms();
            ResetTransforms();
        }

        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(MainCanvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Move transforms
                _fitScreen = false;
                _offsetX += position.X - _currentMousePosition.X;
                _offsetY += position.Y - _currentMousePosition.Y;
                SetTransforms();
            }
            _currentMousePosition = position;

            e.Handled = true;
        }

        private void MainCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void MainCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MainCanvas);
        }

        private void EditRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_rules.Count == 0)
            {
                Rule newRule = new Rule("New Rule", 0, 0, 0, 0);
                _rules.Add(newRule);
            }

            RuleSettingWindow window = new RuleSettingWindow(_rules, _status);
            window.ShowDialog();
        }

        private Task NextFrame()
        {
            _map.Evolve(_rules);
            for (int i = 0; i < _map.Width; ++i)
                for (int j = 0; j < _map.Height; ++j)
                {
                    _cells[i, j].Fill = new SolidColorBrush(_status[_map[i, j]].DisplayColor);
                }
            return Task.Delay(0);
        }

        private async void NextFrameButton_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
            await NextFrame();
        }

        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            await NextFrame();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Cellular Automaton Game File (*.cag)|*.cag";
            if (saveFileDialog.ShowDialog() == true)
            {
                string saveFileText = "";
                for (int i = 0; i < _status.Length; ++i)
                {
                    string statusText = "s#";
                    statusText += _status[i].Description + "#";
                    statusText += _status[i].DisplayColor.R + "#";
                    statusText += _status[i].DisplayColor.G + "#";
                    statusText += _status[i].DisplayColor.B + "#";
                    saveFileText += statusText + "\n";
                }

                foreach (var rule in _rules)
                {
                    string ruleText = "r#";
                    ruleText += rule.DescriptionString + "#";
                    ruleText += rule.Top + "#";
                    ruleText += rule.Bottom + "#";
                    ruleText += rule.Left + "#";
                    ruleText += rule.Right + "#";
                    ruleText += rule.Result + "#";
                    ruleText += rule.RotateMode + "#";
                    for (int i = -rule.Left; i <= rule.Right; ++i)
                        for (int j = -rule.Top; j <= rule.Bottom; ++j)
                            ruleText += rule[i, j] + "#";

                    saveFileText += ruleText + "\n";
                }

                string mapText = "m#";
                mapText += _map.Width + "#";
                mapText += _map.Height + "#";
                mapText += (int)_map.Type + "#\n";
                for (int i = 0; i < _map.Width; ++i)
                {
                    for (int j = 0; j < _map.Height; ++j)
                    {
                        mapText += _map[j, i] + "#";
                    }

                    mapText += "\n";
                }

                saveFileText += mapText + "\n";
                File.WriteAllText(saveFileDialog.FileName, saveFileText);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Cellular Automaton Game File (*.cag)|*.cag";
            _rules = new List<Rule>();
            _status = new StatusList(false);
            if (openFileDialog.ShowDialog() == true)
            {
                string[] allLines = File.ReadAllLines(openFileDialog.FileName);
                int mapLineLeft = 0;
                foreach (var line in allLines)
                {
                    string trim = line.Trim(' ', '\t');
                    if (trim.StartsWith("$")) continue;
                    if (trim.Length == 0) continue;

                    if (mapLineLeft > 0)
                    {
                        string[] split = trim.Split('#');
                        for (int j = 0; j < _map.Height; ++j)
                            _map[_map.Width - mapLineLeft, j] = int.Parse(split[j]);
                        --mapLineLeft;
                        if (mapLineLeft == 0)
                            ReconstructGrid();
                        continue;
                    }

                    if (trim.StartsWith("s"))
                    {
                        // Status Line
                        string[] split = trim.Split('#');

                        Status newStatus = new Status(split[1], Color.FromRgb(byte.Parse(split[2]), byte.Parse(split[3]), byte.Parse(split[4])));
                        _status.Add(newStatus);
                    }
                    else if (trim.StartsWith("r"))
                    {
                        // Rule Line
                        string[] split = trim.Split('#');

                        Rule newRule = new Rule(split[1], int.Parse(split[2]), int.Parse(split[3]), int.Parse(split[4]), int.Parse(split[5]), short.Parse(split[7]));
                        newRule.Result = int.Parse(split[6]);
                        int s = 8;
                        for (int i = -newRule.Left; i <= newRule.Right; ++i)
                            for (int j = -newRule.Top; j <= newRule.Bottom; ++j)
                            {
                                newRule[i, j] = int.Parse(split[s]);
                                ++s;
                            }

                        _rules.Add(newRule);
                    }
                    else if (trim.StartsWith("m"))
                    {
                        string[] split = trim.Split('#');

                        _map = new Map(int.Parse(split[1]), int.Parse(split[2]), (Map.EdgeType)int.Parse(split[3]));
                        mapLineLeft = _map.Width;
                    }
                }

            }
            CalculateFitTransform();
            SetFixedTransforms();
            ResetTransforms();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            Map dummyMap = new Map(_map.Width, _map.Height);

            SettingWindow window = new SettingWindow(dummyMap, _dispatcherTimer);
            window.ShowDialog();

            if (dummyMap.Width != _map.Width || dummyMap.Height != _map.Height)
            {
                _map = new Map(dummyMap.Width, dummyMap.Height);
                ReconstructGrid();
            }
        }
    }
}
