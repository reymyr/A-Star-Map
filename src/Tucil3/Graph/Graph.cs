using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Globalization;

namespace Tucil3.Graph
{
    class Graph
    {
        public List<Vertex> Vertices;

        //private Dictionary<string, Dictionary<string, double>> adjMatrix;

        public Graph()
        {
            Vertices = new List<Vertex>();
            //adjMatrix = new Dictionary<string, Dictionary<string, double>>();
        }

        public Graph(string filename)
        {
            Vertices = new List<Vertex>();
            string[] filePerLine = File.ReadAllLines(filename); //baca file per line
                                                                //hitung jumlah vertex
            int nVertex = int.Parse(filePerLine[0]);
            //container isi file per line
            string[] pairVertex;
            string[] pairEdge;
            Dictionary<int, string> kamusVertex = new Dictionary<int, string>(); //buat nyari nama vertex pas masukin edge adj matrix

            int i = 1;
            for (; i <= nVertex; i++)
            {
                pairVertex = filePerLine[i].Split(' '); //baca vertex
                this.AddVertex(pairVertex[2], double.Parse(pairVertex[0], CultureInfo.InvariantCulture), double.Parse(pairVertex[1], CultureInfo.InvariantCulture)); //karena urutan di file : lintang bujur nama
                kamusVertex.Add(i - 1, pairVertex[2]); //nambahin nama vertex ke kamus
            }

            for (int j = i; j < filePerLine.Length; j++)
            {
                pairEdge = filePerLine[j].Split(' '); //baca adj matrix
                for (int k = 0; k <= j-i; k++)
                {
                    if (double.Parse(pairEdge[k]) > 0)
                    {
                        this.AddEdge(kamusVertex[j-i], kamusVertex[k], double.Parse(pairEdge[k], CultureInfo.InvariantCulture)); //intinya ini masukin edge berasal dari kamus nama    
                    }
                }

            }
        }

        public void printVertex()
        {
            foreach (Vertex simpul in Vertices)
                Console.WriteLine(simpul.Name);
        }

        public void AddVertex(string v, double x, double y)
        {
            Vertices.Add(new Vertex(v, x, y));
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

        public Tuple<List<string>, string> AStar(string source, string dest)
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
            resultString += "\n";
            resultString += "Jarak yang ditempuh: ";
            if (cost[dest] >= 1000)
            {
                double km = cost[dest] / 1000;
                resultString += km.ToString() + " kilometer";
            }
            else
            {
                resultString += cost[dest].ToString() + " meter";
            }

            return new Tuple<List<string>, string>(path, resultString);
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

        public double degreeToRadian (double degree) {
            return degree*Math.PI/180;
        }

        public double haversine(double curLintang, double goalLintang, double curBujur, double goalBujur)
        { //semua parameter belum diubah ke radian
            curBujur = degreeToRadian(curBujur);
            goalBujur = degreeToRadian(goalBujur);
            curLintang = degreeToRadian(curLintang);
            goalLintang = degreeToRadian(goalLintang);

            double diffBujur = goalBujur - curBujur;
            double diffLintang = goalLintang - curLintang;
            double haversineFormula = Math.Pow(Math.Sin(diffLintang/2),2) + Math.Cos(curLintang) * Math.Cos(goalLintang) * Math.Pow(Math.Sin(diffBujur/2),2);
            double distancePerKilometer = 2 * Math.Asin(Math.Sqrt(haversineFormula));
            return distancePerKilometer * 6378137; //6378137 itu jari jari bumi (satuan meter)
        }

        private double CalcHeuristic(string v1, string goal)
        {
            Vertex curVertex = Vertices.Find(v => v.Name == v1);
            Vertex goalVertex = Vertices.Find(v => v.Name == goal);

            return haversine(curVertex.Latitude, goalVertex.Latitude, curVertex.Longitude, goalVertex.Longitude);
        }

        // Mengembalikan graf dalam bentuk graph MSAGL untuk divisualisasikan
        public Microsoft.Msagl.Drawing.Graph getMSAGLGraph(string name)
        {
            Microsoft.Msagl.Drawing.Graph g = new Microsoft.Msagl.Drawing.Graph(name);
            g.Directed = false;
            Vertices.ForEach(v => g.AddNode(v.Name).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle);

            foreach (Vertex v in Vertices)
            {
                foreach (Edge e in v.Edges)
                {
                    int a = v.Name.CompareTo(e.ToVertex);
                    if (v.Name.CompareTo(e.ToVertex) < 0)
                    {
                        Microsoft.Msagl.Drawing.Edge edge = g.AddEdge(v.Name, e.Weight.ToString(), e.ToVertex);
                        edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                        edge.Attr.Id = v.Name + e.ToVertex;
                    }
                }
            }
            return g;
        }
    }
}
