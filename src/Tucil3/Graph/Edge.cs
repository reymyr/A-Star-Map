using System;
using System.Collections.Generic;
using System.Text;

namespace Tucil3.Graph
{
    class Edge
    {
        // Nama simpul tujaun
        public string ToVertex
        {
            get;
            set;
        }

        // Weight sisi
        public double Weight
        {
            get;
            set;
        }

        // Constructor
        public Edge(string v, double w)
        {
            ToVertex = v;
            Weight = w;
        }
    }
}
