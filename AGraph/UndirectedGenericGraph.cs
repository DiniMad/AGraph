using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace AGraph
{
    class UndirectedGenericGraph<T>
    {
        public UndirectedGenericGraph(params Vertex<T>[] initialNodes)
            : this((IEnumerable<Vertex<T>>)initialNodes) { }
        public UndirectedGenericGraph(IEnumerable<Vertex<T>> initialNodes = null)
        {
            Vertices = initialNodes?.ToList() ?? new List<Vertex<T>>();
        }
        public List<Vertex<T>> Vertices { get; }
        public int Size => Vertices.Count;

        #region Public Methods
        public void AddPair(Vertex<T> first, Vertex<T> second)
        {
            AddToList(first);
            AddToList(second);
            AddNeighbors(first, second);
        }
        public void DepthFirstSearch(Vertex<T> root)
        {
            UnvisitAll();
            DepthFirstSearchImplementation(root);
        }
        public bool IsEulerianGraph()
        {
            if (!IsConnectedGraph())
                return false;
            return HaveHasAllVertexEvenDegree();
        }
        public bool IsConnectedGraph()
        {
            DepthFirstSearch(Vertices.First());
            return HaveAllVerticesVisited();
        }
        public void OptimalColoring(Grid graphGrid)
        {
            //Remove color of all vertices
            foreach (var vertex in Vertices)
            {
                vertex.Color = null;
            }
            //Execute coloring algorithm
            OptimalColoringImplementation();
            //OptimalColoring UI element base on their pair vertex
            foreach (var buttonVertex in graphGrid.Children.OfType<Button>())
            {
                var color = Vertices.Single(v => v.Name == buttonVertex.Name).Color;
                if (color != null)
                    buttonVertex.Background =
                        new SolidColorBrush((Color)color);
            }
        }
        public void GreedyColoring(Queue<Vertex<T>> queueVertices, Grid graphGrid)
        {
            //Remove color of all vertices
            foreach (var vertex in Vertices)
            {
                vertex.Color = null;
            }
            //Execute greedy coloring algorithm
            GreedyColoringImplementation(queueVertices);
            //OptimalColoring UI element base on their pair vertex
            foreach (var buttonVertex in graphGrid.Children.OfType<Button>())
            {
                var color = Vertices.Single(v => v.Name == buttonVertex.Name).Color;
                if (color != null)
                    buttonVertex.Background =
                        new SolidColorBrush((Color)color);
            }
        }
        #endregion
        #region Private Methods
        private void GreedyColoringImplementation(Queue<Vertex<T>> queueVertices)
        {
            //Sequential coloring of selected vertices. 
            while (queueVertices.Count > 0)
            {
                var vertex = queueVertices.Dequeue();
                var colorsCollection = new ColorsCollection();
                var colors = colorsCollection.customColors;
                foreach (var nighber in vertex.Neighbors.FindAll(v => v.Color != null))
                {
                    if (nighber.Color != null) colors.Remove((Color)nighber.Color);
                }
                vertex.Color = colors.First();
            }
            //OptimalColoring all remaining vertices
            foreach (var vertex in Vertices.FindAll(v => v.Color == null))
            {
                var colorsCollection = new ColorsCollection();
                var colors = colorsCollection.customColors;
                foreach (var nighber in vertex.Neighbors.FindAll(v => v.Color != null))
                {
                    if (nighber.Color != null) colors.Remove((Color)nighber.Color);
                }
                vertex.Color = colors.First();
                //graphGrid.Children.OfType<Button>().Single(b => b.Name == vertex.Name).Background = new SolidColorBrush(colors.First());
            }
        }
        private void OptimalColoringImplementation()
        {
            foreach (var vertex in Vertices.FindAll(v => v.Color == null))
            {
                var colorsCollection = new ColorsCollection();
                var colors = colorsCollection.customColors;
                foreach (var nighber in vertex.Neighbors.FindAll(v => v.Color != null))
                {
                    if (nighber.Color != null) colors.Remove((Color)nighber.Color);
                }
                vertex.Color = colors.First();
            }
        }
        //private void GreedyColoringImplementation(Vertex<T> startNode, Grid graphGrid)
        //{
        //    var btnStartNode = graphGrid.Children.OfType<Button>().Single(b => b.Name == startNode.Name);
        //    btnStartNode.Background = Brushes.Red;
        //    ColoringButtons(startNode, graphGrid);
        //}

        //private void ColoringButtons(Vertex<T> startNode, Grid graphGrid)
        //{
        //    foreach (var vertex in startNode.Neighbors.FindAll(v => v.Color == null))
        //    {
        //        var colorsCollection = new ColorsCollection();
        //        var colors = colorsCollection.customColors;
        //        foreach (var nighber in vertex.Neighbors.FindAll(v => v.Color == null))
        //        {
        //            colors.Remove(((SolidColorBrush)graphGrid.Children.OfType<Button>()
        //                .Single(b => b.Name == nighber.Name).Background).Color);
        //        }
        //        graphGrid.Children.OfType<Button>().Single(b => b.Name == vertex.Name).Background = new SolidColorBrush(colors.First());
        //        ColoringButtons(vertex, graphGrid);
        //    }
        //}
        private void DepthFirstSearchImplementation(Vertex<T> root)
        {
            if (!root.IsVisited)
            {
                root.IsVisited = true;

                foreach (Vertex<T> neighbor in root.Neighbors)
                {
                    DepthFirstSearchImplementation(neighbor);
                }
            }
        }
        private void AddToList(Vertex<T> vertex)
        {
            if (!Vertices.Contains(vertex))
            {
                Vertices.Add(vertex);
            }
        }
        private void AddNeighbors(Vertex<T> first, Vertex<T> second)
        {
            AddNeighbor(first, second);
            AddNeighbor(second, first);
        }
        private void AddNeighbor(Vertex<T> first, Vertex<T> second)
        {
            if (!first.Neighbors.Contains(second))
            {
                first.AddEdge(second);
            }
        }
        private void UnvisitAll()
        {
            foreach (var vertex in Vertices)
            {
                vertex.IsVisited = false;
            }
        }
        private bool HaveHasAllVertexEvenDegree()
        {
            foreach (var vertex in Vertices)
            {
                if (vertex.NeighborCount % 2 != 0)
                    return false;
            }
            return true;
        }
        private bool HaveAllVerticesVisited()
        {
            foreach (var vertex in Vertices)
            {
                if (vertex.IsVisited)
                    continue;
                return false;
            }
            return true;
        }
        #endregion

        private void tst()
        {

        }
    }
}