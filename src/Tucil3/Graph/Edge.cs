using System;
using System.Collections.Generic;
using System.Text;

namespace Tucil3.Graph
{
    class Edge
    {
        public string FromVertex
        {
            get;
            set;
        }

        public string ToVertex
        {
            get;
            set;
        }

        public double Weight
        {
            get;
            set;
        }

        public Edge(string v, double w)
        {
            ToVertex = v;
            Weight = w;
        }
    }
}
