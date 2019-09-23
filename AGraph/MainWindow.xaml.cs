using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int VertecsCount { get; set; }
        public static int Radius { get; set; }
        public bool IsVertexSelected { get; set; }
        public Vector PrevVertex { get; set; }
        public Button PrevButton { get; set; }
        private UndirectedGenericGraph<string> _graph;
        //private readonly DispatcherTimer _uiElementUpdate = new DispatcherTimer();
        public bool IsOnGreedyColoringState { get; set; }
        private Queue<Vertex<string>> _queuebButtons;
        private List<Button> _glowingButtons = new List<Button>();
        public int BfsLevelIndex { get; set; }
        public int[] BfsLevelArray { get; set; }
        readonly Collection<string> _linesRegisteredName = new Collection<string>();
        private List<string> DfsQueue = new List<string>();
        public int DfsLevelIndex { get; set; }

        private int[,] _adjacencyMatrix;
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            SetButtonColorAnimation();
            VisibilityAnimation.SetIsActive(GridGreedyColoringInfo, true);
            VisibilityAnimation.SetIsActive(GridGraphOptions, true);
            //UiDis.Tick += GlowingVertices;
        }
        private void BuildAdjacencyMatrix()
        {
            if (_graph.Vertices.Count <= 1)
                return;
            _adjacencyMatrix = new int[_graph.Vertices.Count - 1, _graph.Vertices.Count - 1];
            for (var i = 65; i <= 63 + _graph.Vertices.Count; i++)
            {
                for (var j = 66; j <= 64 + _graph.Vertices.Count; j++)
                {
                    if (i == j)
                        _adjacencyMatrix[i - 65, j - 66]=0;
                    else if (_graph.Vertices.Single(v => v.Name == ((char)i).ToString()).Neighbors
                        .SingleOrDefault(n => n.Name == ((char)j).ToString()) != null)
                        _adjacencyMatrix[i - 65, j - 66] = 1;
                    else
                        _adjacencyMatrix[i - 65, j - 66] = 100;
                }
            }
        }

        private void ExecuteFloyedWarshall()
        {
            for (int k = 0; k < _graph.Vertices.Count - 1; k++)
            {
                for (int i = 0; i < _graph.Vertices.Count - 1; i++)
                {
                    for (int j = 0; j < _graph.Vertices.Count - 1; j++)
                    {
                        if (_adjacencyMatrix[i, j] > _adjacencyMatrix[i, k] + _adjacencyMatrix[k, j])
                            _adjacencyMatrix[i, j] = _adjacencyMatrix[i, k] + _adjacencyMatrix[k, j];
                    }
                }
            }
        }
        private void SetButtonColorAnimation()
        {
            var animation = new ColorAnimation
            {
                To = Colors.DarkOrchid,
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true,
                Duration = new Duration(TimeSpan.FromSeconds(0.8))
            };
            BtnStartGreedyAlgorithm.Background = new SolidColorBrush(Colors.CornflowerBlue);
            BtnStartGreedyAlgorithm.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
        private void WindowsMain_Loaded(object sender, RoutedEventArgs e)
        {
            int windowsContentSize;
            if (WindowsMain.Height < WindowsMain.Width - 200)
                windowsContentSize = (int)WindowsMain.Height - 130;
            else
                windowsContentSize = (int)WindowsMain.Width - 200;
            Radius = windowsContentSize - windowsContentSize % 100;
        }
        private void BtnCreateGraph_Click(object sender, RoutedEventArgs e)
        {
            if (TxtVerticesNumber.Text.Length < 1)
                return;
            foreach (var child in GridGraph.Children)
            {
                if (child is Label label)
                    label.Content = string.Empty;
                if (child is Button button)
                    button.BeginAnimation(BackgroundProperty, null);
                else if (child is Line line)
                {
                    var gradientStopAnimationStoryboard = new Storyboard();
                    gradientStopAnimationStoryboard.Begin(line);
                }
            }
            GridGraph.Children.Clear();
            IsVertexSelected = false;
            _queuebButtons = new Queue<Vertex<string>>();
            var vertices = new List<Vertex<string>>();
            VertecsCount = int.Parse(TxtVerticesNumber.Text);
            for (int i = 1; i <= VertecsCount; i++)
            {
                var bottonYposition = Math.Sin(Math.PI * (360 / VertecsCount * i) / 180.0) * (Radius - 70);
                var buttonXposition = Math.Cos(Math.PI * (360 / VertecsCount * i) / 180.0) * (Radius - 70);
                var labelYposition = Math.Sin(Math.PI * (360 / VertecsCount * i) / 180.0) * (Radius);
                var labelXposition = Math.Cos(Math.PI * (360 / VertecsCount * i) / 180.0) * (Radius);
                var charId = ((char)(64 + i)).ToString();
                GridGraph.Children
                    .Add(NewVertex(charId,
                    new Thickness(0, 0, buttonXposition, bottonYposition)));
                GridGraph.Children
                    .Add(new Label
                    {
                        Name = charId,
                        FontSize = 20,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, labelXposition, labelYposition)
                    });
                vertices.
                    Add(new Vertex<string>(charId));
            }
            _graph = new UndirectedGenericGraph<string>(vertices);
            if (GridGraphOptions.Visibility != Visibility.Visible)
                GridGraphOptions.Visibility = Visibility.Visible;
        }
        public Button NewVertex(string name, Thickness margin)
        {
            var vertexRighClickMenu = new ContextMenu();
            var menuItemSimulateBFS = new MenuItem { Header = "Simulate BFS" };
            menuItemSimulateBFS.Click += MenuItemSimulateBFS_Click;
            var menuItemSimulateDFS = new MenuItem { Header = "Simulate DFS" };
            menuItemSimulateDFS.Click += MenuItemSimulateDFS_Click;
            vertexRighClickMenu.Items.Add(menuItemSimulateBFS);
            vertexRighClickMenu.Items.Add(menuItemSimulateDFS);
            var vertex = new Button
            {
                Name = name,
                Style = Resources["Vertex"] as Style,
                Width = 36,
                Margin = margin,
                Height = 36,
                Content = name,
                Foreground = Brushes.White,
                FontSize = 22,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                ContextMenu = vertexRighClickMenu
            };
            Extensions.VertexState(vertex, (int)VertexState.Normal);
            vertex.Click += Vertex_Click;
            return vertex;
        }
        private void MenuItemSimulateDFS_Click(object sender, RoutedEventArgs e)
        {
            var btnStartVertex = ((sender as MenuItem).Parent as ContextMenu)
                .PlacementTarget as Button;
            var vertexStart = _graph.Vertices.Single(v => v.Name == btnStartVertex.Name);
            DfsLevelIndex = 0;
            SaveDfsToList(vertexStart, vertexStart);
            DfsColorTheButton();
        }
        private void DfsColorTheButton()
        {
            if (DfsLevelIndex == 0)
                DfsLevelIndex++;
            var btnRoot = GridGraph.Children.OfType<Button>()
                .Single(b => b.Name == DfsQueue[DfsLevelIndex][0].ToString());
            if (!Extensions.IsPassed(btnRoot))
            {
                Extensions.IsPassed(btnRoot, true);
                var colorAnimationButton = new ColorAnimation
                {
                    From = Colors.Black,
                    To = Colors.Red,
                    Duration = TimeSpan.FromSeconds(1)
                };
                colorAnimationButton.Completed += (sender, e) => DfsColorAnimationButton_Completed(sender, e, DfsQueue[DfsLevelIndex]);
                btnRoot.Background = new SolidColorBrush(Colors.Black);
                btnRoot.Background
                    .BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationButton);
                GridGraph.Children.OfType<Label>()
                    .Single(l => l.Name == DfsQueue[DfsLevelIndex][0].ToString())
                    .Content = DfsLevelIndex;
            }
            else
            {
                DfsColorTheLine(DfsQueue[DfsLevelIndex]);
            }
        }
        private void DfsColorTheLine(string lineName)
        {
            var line = GridGraph.Children.OfType<Line>()
    .Single(l => l.Name.Contains(lineName[0].ToString()) &&
                 l.Name.Contains(lineName[1].ToString()));
            //if (Extensions.IsPassed(line))
            //{
            //    if (DfsLevelIndex >= DfsQueue.Count)
            //        return;
            //    DfsLevelIndex++;
            //    DfsColorTheButton();
            //    return;
            //}
            var btnStart = GridGraph.Children.OfType<Button>().Single(b => b.Name == lineName[0].ToString());
            var btnDistination = GridGraph.Children.OfType<Button>().Single(b => b.Name == lineName[1].ToString());
            if (_linesRegisteredName.Contains(lineName))
            {
                line.Stroke = Brushes.Black;
                line.UnregisterName($"GradientStop1{line.Name}");
                line.UnregisterName($"GradientStop2{line.Name}");
            }
            else
                _linesRegisteredName.Add(line.Name);
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(btnStart.Margin.Right, btnStart.Margin.Bottom);
            gradientBrush.EndPoint = new Point(btnDistination.Margin.Right, btnDistination.Margin.Bottom);
            GradientStop stop1 = new GradientStop(Colors.Red, 0.0);
            GradientStop stop2 = new GradientStop(Colors.Black, 0.0);
            line.RegisterName($"GradientStop1{line.Name}", stop1);
            line.RegisterName($"GradientStop2{line.Name}", stop2);
            //Extensions.IsPassed(line, true);
            gradientBrush.GradientStops.Add(stop1);
            gradientBrush.GradientStops.Add(stop2);
            line.Stroke = gradientBrush;
            var offsetAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 15,
                Duration = TimeSpan.FromSeconds(4),
            };
            offsetAnimation.Completed += DfsAnimateLineColoring_Completed;
            Storyboard.SetTargetName(offsetAnimation, $"GradientStop2{line.Name}");
            Storyboard.SetTargetProperty(offsetAnimation,
                new PropertyPath(GradientStop.OffsetProperty));
            var gradientStopAnimationStoryboard = new Storyboard();
            gradientStopAnimationStoryboard.Children.Add(offsetAnimation);
            gradientStopAnimationStoryboard.Begin(line);

        }
        private void DfsColorAnimationButton_Completed(object sender, EventArgs eventArgs, string lineName)
        {
            DfsColorTheLine(lineName);
        }
        private void DfsAnimateLineColoring_Completed(object sender, EventArgs e)
        {
            if (DfsLevelIndex >= DfsQueue.Count - 1)
                return;
            DfsLevelIndex++;
            DfsColorTheButton();
        }
        private void SaveDfsToList(Vertex<string> vertex, Vertex<string> prevVertex)
        {
            DfsQueue.Add(prevVertex.Name + vertex.Name);
            vertex.IsVisited = true;
            foreach (var neighber in vertex.Neighbors.Where(n => n.IsVisited == false))
            {
                SaveDfsToList(neighber, vertex);
            }
        }
        private void MenuItemSimulateBFS_Click(object sender, RoutedEventArgs e)
        {
            BfsLevelIndex = 0;
            foreach (var child in GridGraph.Children)
            {
                switch (child)
                {
                    case Button button:
                        button.Background = Brushes.Black;
                        break;
                    case Line line:
                        line.Stroke = Brushes.Black;
                        break;
                }
                Extensions.IsPassed(child as UIElement, false);
            }
            var btnStartVertex = ((sender as MenuItem).Parent as ContextMenu)
                    .PlacementTarget as Button;
            BfsLevelArray = _graph
                .GetVerticesCountOfEachLevel(_graph.Vertices
                    .Single(v => v.Name == btnStartVertex.Name));
            ChangeButtonBackGroundAnimatial(btnStartVertex);
        }
        private void ChangeButtonBackGroundAnimatial(Button button)
        {
            if (Extensions.IsPassed(button))
                return;
            var colorAnimationButton = new ColorAnimation
            {
                From = Colors.Black,
                To = Colors.Red,
                Duration = TimeSpan.FromSeconds(1)
            };
            colorAnimationButton.Completed += (sender, e) => BfsColorAnimationButton_Completed(sender, e, button);
            button.Background = new SolidColorBrush(Colors.Black);
            button.Background
                .BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationButton);
            BfsLevelArray[BfsLevelIndex]--;
            GridGraph.Children.OfType<Label>().Single(l => l.Name == button.Name).Content = BfsLevelIndex + 1;
            if (BfsLevelArray[BfsLevelIndex] == 0)
                BfsLevelIndex++;
            //if (BtnRemaindedToIndex == -1)
            //{
            //    GridGraph.Children.OfType<Label>().Single(l => l.Name == button.Name).Content = ++BfsCounter;
            //    BfsCounter++;
            //}
            //else if (BtnRemaindedToIndex > 0)
            //{
            //    GridGraph.Children.OfType<Label>().Single(l => l.Name == button.Name).Content = BfsCounter;
            //    BtnRemaindedToIndex--;
            //    if(BtnRemaindedToIndex==0)
            //        BfsCounter++;
            //}
            Extensions.IsPassed(button, true);
        }
        private void ChangeLinesBackGroundAnimatial(Button buttonRoot)
        {
            var linesShouldEffect = GridGraph.Children.OfType<Line>().Where(l => l.Name.Contains(buttonRoot.Name));
            foreach (var line in linesShouldEffect)
            {
                if (Extensions.IsPassed(line))
                    continue;
                var distinationVertexName = line.Name.Replace(buttonRoot.Name, string.Empty);
                var btndistinationVertex =
                    GridGraph.Children.OfType<Button>()
                        .Single(b => b.Name == distinationVertexName);
                if (Extensions.IsPassed(btndistinationVertex))
                    continue;
                if (_linesRegisteredName.Contains(line.Name))
                {
                    line.Stroke = Brushes.Black;
                    line.UnregisterName($"GradientStop1{line.Name}");
                    line.UnregisterName($"GradientStop2{line.Name}");
                }
                else
                    _linesRegisteredName.Add(line.Name);
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(buttonRoot.Margin.Right, buttonRoot.Margin.Bottom);
                gradientBrush.EndPoint = new Point(btndistinationVertex.Margin.Right, btndistinationVertex.Margin.Bottom);
                GradientStop stop1 = new GradientStop(Colors.Red, 0.0);
                GradientStop stop2 = new GradientStop(Colors.Black, 0.0);
                line.RegisterName($"GradientStop1{line.Name}", stop1);
                line.RegisterName($"GradientStop2{line.Name}", stop2);
                Extensions.IsPassed(line, true);
                gradientBrush.GradientStops.Add(stop1);
                gradientBrush.GradientStops.Add(stop2);
                line.Stroke = gradientBrush;
                var offsetAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 15,
                    Duration = TimeSpan.FromSeconds(4),
                };
                offsetAnimation.Completed += (sender, e) => OffsetAnimation_Completed(sender, e, btndistinationVertex); ;
                Storyboard.SetTargetName(offsetAnimation, $"GradientStop2{line.Name}");
                Storyboard.SetTargetProperty(offsetAnimation,
                    new PropertyPath(GradientStop.OffsetProperty));
                var gradientStopAnimationStoryboard = new Storyboard();
                gradientStopAnimationStoryboard.Children.Add(offsetAnimation);
                gradientStopAnimationStoryboard.Begin(line);
            }
        }
        private void BfsColorAnimationButton_Completed(object sender, EventArgs e, Button buttonSender)
        {
            ChangeLinesBackGroundAnimatial(buttonSender);
        }
        private void OffsetAnimation_Completed(object sender, EventArgs e, Button buttonDistanation)
        {
            ChangeButtonBackGroundAnimatial(buttonDistanation);
        }
        //private void MenuItemGreedyColoring_Click(object sender, RoutedEventArgs e)
        //{
        //    var btnStartNode = ((sender as MenuItem).Parent as ContextMenu)
        //        .PlacementTarget as Button;
        //    _graph.GreedyColoring(_graph.Vertices.Single(v => v.Name == btnStartNode.Name), GridGraph);
        //}
        private void Vertex_Click(object sender, RoutedEventArgs e)
        {
            var currentButton = (Button)sender;
            if (IsOnGreedyColoringState)
            {
                if (Extensions.VertexState(currentButton) != (int)VertexState.ReadyForGreedyColoring) return;
                currentButton.BitmapEffect = null;
                currentButton.BorderBrush = Brushes.Black;
                Extensions.VertexState(currentButton, (int)VertexState.Normal);
                GridGraph.Children.OfType<Label>()
                        .Single(l => l.Name == currentButton.Name).Content =
                    _queuebButtons.Count + 1;
                _queuebButtons.Enqueue(_graph.Vertices.Single(v => v.Name == currentButton.Name));
                if (GridGraph.Children.OfType<Button>()
                    .All(b => Extensions.VertexState(b) !=
                              (int)VertexState.ReadyForGreedyColoring))
                {
                    IsOnGreedyColoringState = false;
                    GridGreedyColoringInfo.Visibility = Visibility.Hidden;
                    _graph.GreedyColoring(_queuebButtons, GridGraph);
                    _queuebButtons.Clear();
                }
            }
            else
            {
                var currentPoint = currentButton
                .TransformToAncestor(this).Transform(new Point(0, 0));
                if (IsVertexSelected)
                {
                    if (Equals(PrevButton, currentButton))
                    {
                        MessageBox.Show("Just simple graph allowed.");
                        IsVertexSelected = false;
                        currentButton.BorderBrush = Brushes.Black;
                        currentButton.BitmapEffect = null;
                        return;
                    }
                    //Add edge to graph
                    var firstVertex = _graph.Vertices
                        .Single(g => g.Name == PrevButton.Name);
                    var secendVertex = _graph.Vertices
                        .Single(g => g.Name == currentButton.Name);
                    if (_graph.AddPair(firstVertex, secendVertex))
                    {
                        //draw a line in visual graph
                        var line = new Line
                        {
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 2.0,
                            Name = $"{PrevButton.Name[0]}{currentButton.Name[0]}",
                            X1 = PrevVertex.X,
                            X2 = currentPoint.X + currentButton.ActualWidth / 2,
                            Y1 = PrevVertex.Y,
                            Y2 = currentPoint.Y + currentButton.ActualHeight / 2
                        };
                        Panel.SetZIndex(line, -10);
                        GridGraph.Children.Add(line);
                    }
                    PrevButton.BitmapEffect = null;
                    PrevButton.BorderBrush = Brushes.Black;
                    IsVertexSelected = false;
                }
                else
                {
                    var selectedVertexEffect = new DropShadowBitmapEffect
                    {
                        Color = Colors.Aqua,
                        Direction = 320,
                        ShadowDepth = 0,
                        Softness = 1,
                        Opacity = 1
                    };
                    currentButton.BitmapEffect = selectedVertexEffect;
                    currentButton.BorderBrush = Brushes.Aqua;
                    PrevButton = currentButton;
                    PrevVertex = new Vector(
                        currentPoint.X + currentButton.ActualWidth / 2,
                        currentPoint.Y + currentButton.ActualHeight / 2);
                    IsVertexSelected = true;
                }
            }
        }
        private void BtnIsGraphConnected_OnClick(object sender, RoutedEventArgs e)
        {
            if (_graph.IsConnectedGraph())
                MessageBox.Show("The graph is connected.");
            else
                MessageBox.Show("The graph is NOT connected.");
        }
        private void BtnIsGraphEulerian_OnClick(object sender, RoutedEventArgs e)
        {
            if (_graph.IsEulerianGraph())
                MessageBox.Show("The graph is an eulerian graph.");
            else
                MessageBox.Show("The graph is NOT an eulerian graph.");
        }
        public static bool IsTxtVerticesTextValid(string str)
        {
            return int.TryParse(str, out int i) && i >= 1 && i <= 20;
        }
        private void VertexTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTxtVerticesTextValid(((TextBox)sender).Text + e.Text);
        }
        private void TxtVerticesNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtVerticesNumber.Text.Length == 0)
                LblVerticesTextBoxPlaceHolder.Visibility = Visibility.Visible;
            else
                LblVerticesTextBoxPlaceHolder.Visibility = Visibility.Hidden;
        }
        private void BtnStartOptimalColoring_OnClick(object sender, RoutedEventArgs e)
        {
            ResetColoringAndLabels();
            GridGreedyColoringInfo.Visibility = Visibility.Hidden;
            _graph.OptimalColoring(GridGraph);
        }
        private readonly DispatcherTimer UiDis = new DispatcherTimer();
        private void BtnStartGreedyColoring_OnClick(object sender, RoutedEventArgs e)
        {
            ResetColoringAndLabels();
            //_glowingButtons = GridGraph.Children.OfType<Button>().ToList();
            //UiDis.Interval = TimeSpan.FromMilliseconds(10);
            //UiDis.Stop();
            //UiDis.Start();
            GlowVertices(!IsOnGreedyColoringState);
        }
        private void BtnStartGreedyAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            _graph.GreedyColoring(_queuebButtons, GridGraph);
            _queuebButtons.Clear();
            IsOnGreedyColoringState = false;
            GridGreedyColoringInfo.Visibility = Visibility.Hidden;
            foreach (var buttonGlowing in GridGraph.Children.OfType<Button>().Where(b => b.BorderBrush != Brushes.Black))
            {
                buttonGlowing.BorderBrush = Brushes.Black;
                buttonGlowing.BitmapEffect = null;
            }
        }
        private void GlowVertices(bool light)
        {
            if (light)
            {
                var readyToSelectEffect = new DropShadowBitmapEffect
                {
                    Color = Colors.Blue,
                    Direction = 320,
                    ShadowDepth = 0,
                    Softness = 1,
                    Opacity = 1
                };
                foreach (var btnVertex in GridGraph.Children.OfType<Button>())
                {
                    btnVertex.BitmapEffect = readyToSelectEffect;
                    btnVertex.BorderBrush = new SolidColorBrush(Color.FromRgb(155, 144, 239));
                    Extensions.VertexState(btnVertex, (int)VertexState.ReadyForGreedyColoring);
                }
                IsOnGreedyColoringState = true;
                GridGreedyColoringInfo.Visibility = Visibility.Visible;
            }
            else
            {
                foreach (var btnVertex in GridGraph.Children.OfType<Button>())
                {
                    btnVertex.BitmapEffect = null;
                    Extensions.VertexState(btnVertex, (int)VertexState.Normal);
                }
                GridGreedyColoringInfo.Visibility = Visibility.Hidden;
                IsOnGreedyColoringState = false;
                _queuebButtons.Clear();
            }
        }
        //private double opa;
        //private bool rising = true;
        //private void GlowingVertices(object sender, EventArgs e)
        //{
        //    if (rising)
        //    {
        //        if (opa >= 1)
        //            rising = false;
        //        opa += .05;
        //    }
        //    else
        //    {
        //        if (opa <= 0)
        //            rising = true;
        //        opa -= .05;
        //    }
        //    var readyToSelectEffect = new DropShadowBitmapEffect
        //    {
        //        Color = Colors.Blue,
        //        Direction = 320,
        //        ShadowDepth = 0,
        //        Softness = 1,
        //        Opacity = opa
        //    };
        //    foreach (var btnVertex in _glowingButtons)
        //    {
        //        btnVertex.BitmapEffect = readyToSelectEffect;
        //    }
        //    IsOnGreedyColoringState = true;
        //}
        private void ResetColoringAndLabels()
        {
            foreach (var buttonVertex in GridGraph.Children.OfType<Button>())
            {
                buttonVertex.Background = Brushes.Black;
            }
            foreach (var labelVertex in GridGraph.Children.OfType<Label>())
            {
                labelVertex.Content = string.Empty;
            }
            foreach (var buttonSelected in GridGraph.Children.OfType<Button>().Where(b => b.BorderBrush != Brushes.Black))
            {
                buttonSelected.BorderBrush = Brushes.Black;
                buttonSelected.BitmapEffect = null;
            }
            IsVertexSelected = false;
        }
        #region Line From One POint To Mouse Position
        //private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var canvas = (Canvas)sender;

        //    if (canvas.CaptureMouse())
        //    {
        //        var startPoint = e.GetPosition(canvas);
        //        var line = new Line
        //        {
        //            Stroke = Brushes.Blue,
        //            StrokeThickness = 3,
        //            X1 = startPoint.X,
        //            Y1 = startPoint.Y,
        //            X2 = startPoint.X,
        //            Y2 = startPoint.Y,
        //        };

        //        canvas.Children.Add(line);
        //    }
        //}
        //private void Canvas_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var canvas = (Canvas)sender;

        //    if (canvas.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        var line = canvas.Children.OfType<Line>().LastOrDefault();

        //        if (line != null)
        //        {
        //            var endPoint = e.GetPosition(canvas);
        //            line.X2 = endPoint.X;
        //            line.Y2 = endPoint.Y;
        //        }
        //    }
        //}
        //private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    ((Canvas)sender).ReleaseMouseCapture();
        //}
        //private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState == MouseButtonState.Pressed)
        //        _currentPoint = e.GetPosition(this);
        //}

        //private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    { 
        //        Line line = new Line();

        //        line.Stroke = SystemColors.WindowFrameBrush;
        //        line.StrokeThickness = 15;
        //        line.X1 = _currentPoint.X - 135;
        //        line.Y1 = _currentPoint.Y;
        //        line.X2 = e.GetPosition(this).X - 135;
        //        line.Y2 = e.GetPosition(this).Y;

        //        _currentPoint = e.GetPosition(this);

        //        paintSurface.Children.Add(line);
        //    }

        //}
        #endregion
        #region Mouse Path Line
        //public bool IsMouseDraged { get; set; } = false;
        //private void Canvas_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (IsMouseDraged)
        //        polyline.Points.Add(new Point(LimitToRange(e.GetPosition(canvas).X
        //            , 135,(int)WindowsMain.ActualWidth-135),
        //            e.GetPosition(canvas).Y));
        //}

        //private void Canvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState == MouseButtonState.Pressed)
        //        IsMouseDraged = true;
        //}

        //private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    IsMouseDraged = false;
        //}
        //public static double LimitToRange(
        //    double value, int inclusiveMinimum, int inclusiveMaximum)
        //{
        //    if (value < inclusiveMinimum) { return inclusiveMinimum; }
        //    if (value > inclusiveMaximum) { return inclusiveMaximum; }
        //    return value;
        //}
        #endregion
        private void BtnIsGrapfBipartite_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_graph.IsGraph2Colorable().ToString());
        }

        private void BtnBuildAdjacencyMatrix_OnClick(object sender, RoutedEventArgs e)
        {
            BuildAdjacencyMatrix();
            ExecuteFloyedWarshall();
            for (int i = 0; i < _graph.Vertices.Count - 1; i++)
            {
                for (int j = 0; j < _graph.Vertices.Count - 1; j++)
                {
                    Console.WriteLine($"{(char)(i + 65)} to {(char)(j + 66)} {_adjacencyMatrix[i, j]}");
                }
            }
        }
    }
}
