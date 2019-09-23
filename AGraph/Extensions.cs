using System;
using System.Linq.Expressions;
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
        public static void VertexState(UIElement element, int value)
        {
            element.SetValue(VertexStateProperty, value);
        }
        public static int VertexState(UIElement element)
        {
            return (int)element.GetValue(VertexStateProperty);
        }

        //public static readonly DependencyProperty RegisterNameProperty =
        //    DependencyProperty.RegisterAttached("RegisterName", typeof(bool), typeof(Extensions), new PropertyMetadata(default(bool)));
        //public static void RegisterName(UIElement element, bool value)
        //{
        //    element.SetValue(RegisterNameProperty, value);
        //}
        //public static bool RegisterName(UIElement element)
        //{
        //    return (bool)element.GetValue(RegisterNameProperty);
        //}
        public static readonly DependencyProperty IsPassedProperty =
           DependencyProperty.RegisterAttached("IsPassed", typeof(bool), typeof(Extensions), new PropertyMetadata(default(bool)));
        public static void IsPassed(UIElement element, bool value)
        {
            element.SetValue(IsPassedProperty, value);
        }
        public static bool IsPassed(UIElement element)
        {
            return (bool)element.GetValue(IsPassedProperty);
        }
    }
}