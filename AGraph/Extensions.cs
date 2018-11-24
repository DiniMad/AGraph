using System.Windows;
namespace AGraph
{
    enum VertexState
    {
        Normal,
        ReadyForGreedyColoring
    }
    public class Extensions
    {
        public static readonly DependencyProperty VertexStateProperty =
            DependencyProperty.RegisterAttached("VertexState", typeof(int), typeof(Extensions), new PropertyMetadata(default(int)));

        public static void SetVertexState(UIElement element, int value)
        {
            element.SetValue(VertexStateProperty, value);
        }

        public static int GetVertexState(UIElement element)
        {
            return (int)element.GetValue(VertexStateProperty);
        }
    }
}