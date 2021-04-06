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
        // Simpul dalam graph
        public List<Vertex> Vertices;

        // Default constructor
        public Graph()
        {
            Vertices = new List<Vertex>();
        }

        // Constructor menerima nama file
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

        // Menambah simpul
        public void AddVertex(string v, double x, double y)
        {
            Vertices.Add(new Vertex(v, x, y));
        }

        // Menambah sisi (Asumsi nama simpul valid)
        public void AddEdge(string v1Name, string v2Name, double w)
        {
            Edge e1 = new Edge(v2Name, w);
            Edge e2 = new Edge(v1Name, w);
            Vertex v1 = Vertices.Find(v => v.Name == v1Name);
            Vertex v2 = Vertices.Find(v => v.Name == v2Name);

           // Jika sisi sudah ada tidak ditambahkan
           foreach (Edge e in v1.Edges)
            {
                if (e.ToVertex == v2Name)
                {
                    return;
                }
            }

            v1.Edges.Add(e1);
            v2.Edges.Add(e2);
        }

        // Fungsi A Star mencari jalan dari simpul source ke simpul dest
        public Tuple<List<string>, string> AStar(string source, string dest)
        {
            // Inisiasi
            Dictionary<string, double> cost = new Dictionary<string, double>();
            Dictionary<string, string> prev = new Dictionary<string, string>();
            List<Tuple<string, double>> pq = new List<Tuple<string, double>>();

            foreach (Vertex v in Vertices)
            {
                prev[v.Name] = null;
            }

            // Cost node asal = 0
            cost[source] = 0;

            // Tambah node asal ke dalam list simpul yang akan diekspan
            pq.Add(new Tuple<string, double>(source, 0));

            // Selama masih ada simpul yang dapat diekspan
            while (pq.Count > 0)
            {
                // Ambil simpul yang memiliki estimasi cost terendah
                double min = pq.Min(i => i.Item2);
                Tuple<string, double> current = pq.FirstOrDefault(i => i.Item2 == min);

                // Keluarkan simpul terendah
                pq.Remove(current);

                Vertex curVertex = Vertices.Find(v => v.Name == current.Item1);

                // Jika simpul saat ini sudah sama dengan tujuan, pencarian selesai
                if (curVertex.Name == dest)
                {
                    break;
                }

                // Untuk setiap sisi pada simpul saat ini
                foreach (Edge e in curVertex.Edges)
                {
                    // Hitung cost baru untuk simpul tujuan sisi
                    double newCost = cost[curVertex.Name] + e.Weight;

                    // Jika simpul belum ada pada dictionary cost atau cost baru lebih kecil dari yang lama
                    if (!cost.ContainsKey(e.ToVertex) || newCost < cost[e.ToVertex])
                    {
                        // Update cost simpul
                        cost[e.ToVertex] = newCost;

                        // Hitung estimasi cost menuju goal
                        double estCost = newCost + CalcHeuristic(e.ToVertex, dest);

                        // Tambahkan dalam list simpul ekspan
                        pq.Add(new Tuple<string, double>(e.ToVertex, estCost));

                        // Update nilai prev untuk simpul
                        prev[e.ToVertex] = curVertex.Name;
                    }
                }
            }

            // Ambil jalur yang ditemukan
            List<string> path = GetPath(source, dest, prev);

            // String yang akan ditampilkan pada GUI
            string resultString = "";

            // Jika tidak ada jalur
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
            }

            return new Tuple<List<string>, string>(path, resultString);
        }

        // Fungsi untuk menghasilkan jalur dari source ke dest sesuai dengan prev
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

        // Fungsi mengubah derajat ke radian
        public double degreeToRadian (double degree) {
            return degree*Math.PI/180;
        }

        // Fungsi haversine untuk menghitung jarak antara dua koordinat
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

        // Fungsi heuristic dua simpul
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
