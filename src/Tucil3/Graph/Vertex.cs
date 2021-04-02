using System;
using System.Collections.Generic;
using System.Text;

namespace Tucil3.Graph
{
    class Vertex
    {
        // Nama dari simpul
        public string Name
        {
            get;
            set;
        }

        // Simpul yang bersisian
        public List<Edge> Edges
        {
            get;
            set;
        }

        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        public double Test
        {
            get;
            set;
        }

        // Constructor
        public Vertex(string name, double x, double y)
        {
            this.Name = name;
            this.Edges = new List<Edge>();
            this.X = x;
            this.Y = y;
        }
        public Vertex(string name, double a)
        {
            this.Name = name;
            this.Edges = new List<Edge>();
            this.Test = a;
        }
    }
}
