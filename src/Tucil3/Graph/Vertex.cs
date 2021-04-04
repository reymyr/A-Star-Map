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

        public double Latitude
        {
            get;
            set;
        }

        public double Longitude
        {
            get;
            set;
        }

        // Constructor
        public Vertex(string name, double lat, double lon)
        {
            this.Name = name;
            this.Edges = new List<Edge>();
            this.Latitude = lat;
            this.Longitude = lon;
        }
    }
}
