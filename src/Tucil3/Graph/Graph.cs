using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tucil3.Graph
{
    class Graph
    {
        private List<Vertex> Vertices;

        //private Dictionary<string, Dictionary<string, double>> adjMatrix;

        public Graph()
        {
            Vertices = new List<Vertex>();
            //adjMatrix = new Dictionary<string, Dictionary<string, double>>();
        }

        public void AddVertex(string v, double x, double y)
        {
            Vertices.Add(new Vertex(v, x, y));
        }

        public void AddVertex(string v, double a)
        {
            Vertices.Add(new Vertex(v, a));
        }

        public void RemoveVertex(string v)
        {
            Vertices.RemoveAll(vertex => vertex.Name == v);
        }

        public void AddEdge(string v1, string v2, double w)
        {
            Edge e1 = new Edge(v2, w);
            Edge e2 = new Edge(v1, w);
            Vertices.Find(v => v.Name == v1).Edges.Add(e1);
            Vertices.Find(v => v.Name == v2).Edges.Add(e2);
        }

        public void RemoveEdge(string v1, string v2)
        {
            Vertices.Find(v => v.Name == v1).Edges.RemoveAll(edge => edge.ToVertex == v2);
            Vertices.Find(v => v.Name == v2).Edges.RemoveAll(edge => edge.ToVertex == v1);
        }

        public string AStar(string source, string dest)
        {
            Dictionary<string, double> cost = new Dictionary<string, double>();
            Dictionary<string, string> prev = new Dictionary<string, string>();
            List<Tuple<string, double>> pq = new List<Tuple<string, double>>();

            foreach (Vertex v in Vertices)
            {
                prev[v.Name] = null;
            }

            cost[source] = 0;
            pq.Add(new Tuple<string, double>(source, 0));

            while (pq.Count > 0)
            {
                double min = pq.Min(i => i.Item2);
                Tuple<string, double> current = pq.FirstOrDefault(i => i.Item2 == min);
                pq.Remove(current);

                Vertex curVertex = Vertices.Find(v => v.Name == current.Item1);

                if (curVertex.Name == dest)
                {
                    break;
                }

                foreach (Edge e in curVertex.Edges)
                {
                    double newCost = cost[curVertex.Name] + e.Weight;
                    if (!cost.ContainsKey(e.ToVertex) || newCost < cost[e.ToVertex])
                    {
                        cost[e.ToVertex] = newCost;
                        double estCost = newCost + CalcHeuristic(e.ToVertex, dest);
                        pq.Add(new Tuple<string, double>(e.ToVertex, estCost));
                        prev[e.ToVertex] = curVertex.Name;
                    }
                }
            }

            List<string> path = GetPath(source, dest, prev);

            string resultString = "";

            if (path == null)
            {
                resultString += "Tidak ada jalur";
            }
            else
            {
                foreach (string node in path)
                {
                    resultString += node;
                    if (node != path[path.Count - 1])
                    {
                        resultString += " -> ";
                    }
                }
            }

            resultString += cost[dest].ToString();

            return resultString;
        }

        public List<string> GetPath(string source, string dest, Dictionary<string, string> prev)
        {
            List<string> path = new List<string>();
            string now = dest;
            while (now != null)
            {
                path.Add(now);
                now = prev[now];
            }

            path.Reverse();

            if (path[0] == source)
            {
                return path;
            }
            else
            {
                return null;
            }
        }

        private double CalcHeuristic(string v1, string goal)
        {
            Vertex a = Vertices.Find(v => v.Name == v1);
            Vertex b = Vertices.Find(v => v.Name == goal);

            //return Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2));
            return a.Test;
        }
    }
}
