using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly DispatcherTimer _uiElementUpdate = new DispatcherTimer();
        public bool IsOnGreedyColoringState { get; set; }
        private Queue<Vertex<string>> _queuebButtons;
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

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
            GraidGraph.Children.Clear();
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
                GraidGraph.Children
                    .Add(NewVertex(charId,
                    new Thickness(0, 0, buttonXposition, bottonYposition)));
                GraidGraph.Children
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
            BtnIsGraphConnected.IsEnabled = true;
            BtnIsGraphEulerian.IsEnabled = true;
            BtnStartGreedyColoring.IsEnabled = true;
            BtnStartOptimalColoring.IsEnabled = true;
        }
        public Button NewVertex(string name, Thickness margin)
        {
            var vertexRighClickMenu = new ContextMenu();
            var menuItemGreedyColoring = new MenuItem { Header = "StartUiUpdate greedy coloring" };
            //menuItemGreedyColoring.Click += MenuItemGreedyColoring_Click;
            vertexRighClickMenu.Items.Add(menuItemGreedyColoring);
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
                ContextMenu = vertexRighClickMenu
            };
            Extensions.SetVertexState(vertex, (int)VertexState.Normal);
            vertex.Click += Vertex_Click;
            return vertex;
        }
        //private void MenuItemGreedyColoring_Click(object sender, RoutedEventArgs e)
        //{
        //    var btnStartNode = ((sender as MenuItem).Parent as ContextMenu)
        //        .PlacementTarget as Button;
        //    _graph.GreedyColoring(_graph.Vertices.Single(v => v.Name == btnStartNode.Name), GraidGraph);
        //}
        private void Vertex_Click(object sender, RoutedEventArgs e)
        {
            var currentButton = (Button)sender;
            if (IsOnGreedyColoringState)
            {
                if (Extensions.GetVertexState(currentButton) != (int)VertexState.ReadyForGreedyColoring) return;
                var normalEffect = new DropShadowBitmapEffect
                {
                    Direction = 0,
                    ShadowDepth = 0,
                    Softness = 0,
                    Opacity = 0
                };
                currentButton.BitmapEffect = null;
                Extensions.SetVertexState(currentButton, (int)VertexState.Normal);
                GraidGraph.Children.OfType<Label>()
                        .Single(l => l.Name == currentButton.Name).Content =
                    _queuebButtons.Count + 1;
                _queuebButtons.Enqueue(_graph.Vertices.Single(v => v.Name == currentButton.Name));
                if (GraidGraph.Children.OfType<Button>()
                    .All(b => Extensions.GetVertexState(b) !=
                              (int)VertexState.ReadyForGreedyColoring))
                {
                    IsOnGreedyColoringState = false;
                    _graph.GreedyColoring(_queuebButtons, GraidGraph);
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
                        currentButton.Background = Brushes.Black;
                        return;
                    }
                    //draw a line in visual graph
                    {
                        var l = new Line
                        {
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 2.0,
                            Name = "line",
                            X1 = PrevVertex.X,
                            X2 = currentPoint.X + currentButton.ActualWidth / 2,
                            Y1 = PrevVertex.Y,
                            Y2 = currentPoint.Y + currentButton.ActualHeight / 2
                        };
                        Panel.SetZIndex(l, -10);
                        GraidGraph.Children.Add(l);
                    }
                    //Add edge to graph
                    {
                        var firstVertex = _graph.Vertices
                            .Single(g => g.Name == PrevButton.Name);
                        var secendVertex = _graph.Vertices
                         .Single(g => g.Name == currentButton.Name);
                        _graph.AddPair(firstVertex, secendVertex);
                    }
                    PrevButton.Background = Brushes.Black;
                    IsVertexSelected = false;
                }
                else
                {
                    currentButton.Background = Brushes.Aqua;
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
            _graph.OptimalColoring(GraidGraph);
        }
        private void BtnStartGreedyColoring_OnClick(object sender, RoutedEventArgs e)
        {
            ResetColoringAndLabels();
            GlowVertices(!IsOnGreedyColoringState);
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
                foreach (var btnVertex in GraidGraph.Children.OfType<Button>())
                {
                    btnVertex.BitmapEffect = readyToSelectEffect;
                    Extensions.SetVertexState(btnVertex, (int)VertexState.ReadyForGreedyColoring);
                }
                IsOnGreedyColoringState = true;
            }
            else
            {
                foreach (var btnVertex in GraidGraph.Children.OfType<Button>())
                {
                    btnVertex.BitmapEffect = null;
                    Extensions.SetVertexState(btnVertex, (int)VertexState.Normal);
                }
                IsOnGreedyColoringState = false;
            }
        }
        //private void GlowVertices(object sender, EventArgs e)
        //{
        //    var readyToSelectEffect = new DropShadowBitmapEffect
        //    {
        //        Color = Colors.Blue,
        //        Direction = 320,
        //        ShadowDepth = 0,
        //        Softness = 1,
        //        Opacity = 1
        //    };
        //    foreach (var btnVertex in GraidGraph.Children.OfType<Button>())
        //    {
        //        btnVertex.BitmapEffect = readyToSelectEffect;
        //        Extensions.SetVertexState(btnVertex, (int)VertexState.ReadyForGreedyColoring);
        //    }
        //    IsOnGreedyColoringState = true;
        //}
        private void ResetColoringAndLabels()
        {
            foreach (var buttonVertex in GraidGraph.Children.OfType<Button>())
            {
                buttonVertex.Background = new SolidColorBrush(Colors.Black);
            }
            foreach (var labelVertex in GraidGraph.Children.OfType<Label>())
            {
                labelVertex.Content = string.Empty;
            }
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
    }
}
