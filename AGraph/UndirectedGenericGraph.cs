using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace AGraph
{
    class UndirectedGenericGraph<T>
    {
        private readonly Action<IEnumerable<Vertex<T>>> _visitVertices = vertices =>
        {
            foreach (var vertex in vertices)
            {
                vertex.IsVisited = true;
            }
        };
        public UndirectedGenericGraph(params Vertex<T>[] initialNodes)
            : this((IEnumerable<Vertex<T>>)initialNodes) { }
        public UndirectedGenericGraph(IEnumerable<Vertex<T>> initialNodes = null)
        {
            Vertices = initialNodes?.ToList() ?? new List<Vertex<T>>();
        }
        public List<Vertex<T>> Vertices { get; }
        public int Size => Vertices.Count;

        #region Public Methods
        public bool AddPair(Vertex<T> first, Vertex<T> second)
        {
            AddToList(first);
            AddToList(second);
            return AddNeighbors(first, second);
        }
        public bool IsEulerianGraph()
        {
            if (!IsConnectedGraph())
                return false;
            return HaveHasAllVertexEvenDegree();
        }
        public bool IsConnectedGraph()
        {
            UnvisitAll();
            DepthFirstSearchImplementation(Vertices.First());
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
        public bool IsGraph2Colorable()
        {
            UnvisitAll();
            return HasGraphOddCircle();
            //return Can2ColoringGraph();
        }

        public int[] GetVerticesCountOfEachLevel(Vertex<T> vertexRoot)
        {
            var CountVerticesOfEachLevel = new List<int>();
            var CalculateCountVerticesOfEachLevel = new Func<IEnumerable<Vertex<T>>, bool>(vertices =>
            {
                _visitVertices(vertices);
                CountVerticesOfEachLevel.Add(vertices.Count());
                return true;
            });
            UnvisitAll();
            BreathFirstSearchImplementation(CalculateCountVerticesOfEachLevel,vertexRoot);
            return CountVerticesOfEachLevel.ToArray();
        }

        #endregion
        #region Private Methods
        private bool Can2ColoringGraph()
        {
            while (true)
            {
                if (Vertices.All(v => v.Color != null)) break;
                var rootVertex = Vertices.FirstOrDefault(v => v.Color == null);
                if (rootVertex == null) break;
                rootVertex.Color = Colors.Red;
                foreach (var neighborVertex in rootVertex.Neighbors.Where(n => n.Color == null))
                {
                    neighborVertex.Color = Colors.Blue;
                }
            }
            foreach (var vertex in Vertices)
            {
                if (vertex.Neighbors.Any(n => n.Color == vertex.Color))
                    return false;
            }
            return true;

        }
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
        private void DepthFirstSearchImplementation(Vertex<T> rootVertex)
        {
            if (!rootVertex.IsVisited)
            {
                rootVertex.IsVisited = true;

                foreach (var neighbor in rootVertex.Neighbors)
                {
                    DepthFirstSearchImplementation(neighbor);
                }
            }
        }
        private bool HasGraphOddCircle()
        {
            var isContainOddCircleFun = new Func<IEnumerable<Vertex<T>>, bool>(vertices =>
            {
                _visitVertices(vertices);
                return vertices.SelectMany(vertex => vertex.Neighbors).All(vertexNeighbor => !vertices.Contains(vertexNeighbor));
            });
            return BreathFirstSearchImplementation(isContainOddCircleFun);
        }
        private bool BreathFirstSearchImplementation(Func<IEnumerable<Vertex<T>>, bool> doInBfs, Vertex<T> vertexRoot = null)
        {
            var res = true;
            var queueVertex = new Queue<Vertex<T>>();
            if (Vertices.Count == 0)
                return false;
            queueVertex.Enqueue(vertexRoot ?? Vertices.FirstOrDefault());
            while (queueVertex.Count > 0)
            {
                var currentVertices = new Vertex<T>[queueVertex.Count];
                queueVertex.CopyTo(currentVertices, 0);
                queueVertex.Clear();
                foreach (var vertex in currentVertices)
                {
                    foreach (var neighbor in vertex.Neighbors.Where(n => n.IsVisited == false))
                    {
                        queueVertex.Enqueue(neighbor);
                    }
                }
                if (doInBfs(currentVertices)) continue;
                res = false;
                break;
            }
            return res;
        }
        private void AddToList(Vertex<T> vertex)
        {
            if (!Vertices.Contains(vertex))
            {
                Vertices.Add(vertex);
            }
        }
        private bool AddNeighbors(Vertex<T> first, Vertex<T> second)
        {
            return AddNeighbor(first, second) && AddNeighbor(second, first);
        }
        private bool AddNeighbor(Vertex<T> first, Vertex<T> second)
        {
            if (first.Neighbors.Contains(second)) return false;
            first.AddEdge(second);
            return true;
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
    }
}