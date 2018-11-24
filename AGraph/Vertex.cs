using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AGraph
{
    class Vertex<T>
    {
        public Vertex(string name, params Vertex<T>[] parameters)
            : this(name, (IEnumerable<Vertex<T>>)parameters) { }

        public Vertex(string name, IEnumerable<Vertex<T>> neighbors = null)
        {
            Name = name;
            Neighbors = neighbors?.ToList() ?? new List<Vertex<T>>();
            IsVisited = false;  // can be omitted, default is false but some
            // people like to have everything explicitly
            // initialized
        }

        public string Name { get; }   // can be made writable

        public List<Vertex<T>> Neighbors { get; }

        public bool IsVisited { get; set; }
        public Color? Color { get; set; } = null;
        public int NeighborCount => Neighbors.Count;

        public void AddEdge(Vertex<T> vertex)
        {
            Neighbors.Add(vertex);
        }

        public void AddEdges(params Vertex<T>[] newNeighbors)
        {
            Neighbors.AddRange(newNeighbors);
        }

        public void AddEdges(IEnumerable<Vertex<T>> newNeighbors)
        {
            Neighbors.AddRange(newNeighbors);
        }

        public void RemoveEdge(Vertex<T> vertex)
        {
            Neighbors.Remove(vertex);
        }

        //public override string ToString()
        //{
        //    return Neighbors.Aggregate(new StringBuilder($"{Value}: "), (sb, n) => sb.Append($"{n.Value} ")).ToString();
        //    //return $"{Value}: {(string.Join(" ", Neighbors.Select(n => n.Value)))}";
        //}

    }
}
