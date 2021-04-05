using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tucil3.Graph;

namespace Tucil3
{
    public partial class Form1 : Form
    {
        private Graph.Graph graph;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) // Jika pengguna memilih file, asumsi format file masukan benar
            {
                gmap.Overlays.Clear();
                resultBox.Text = "";
                labelFilename.Text = openFileDialog1.SafeFileName;

                // Bentuk graf dari file
                graph = new Graph.Graph(openFileDialog1.FileName);

                // Membuat MSAGL viewer
                Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();

                // Mengambil bentuk graf MSAGL dari graf masukan 
                Microsoft.Msagl.Drawing.Graph msaglgraph = graph.getMSAGLGraph(openFileDialog1.SafeFileName);

                // Bind the graph to the viewer 
                viewer.Graph = msaglgraph;

                // Add the graph to visualizer panel
                viewer.Dock = DockStyle.Fill;
                graphVisualizer.Controls.Clear();
                graphVisualizer.Controls.Add(viewer);

                //set maps di tengah-tengah node
                double lat = 0;
                double longi = 0;
                int counter = 0;
                foreach (Vertex v in graph.Vertices)
                {
                    lat += v.Latitude;
                    longi += v.Longitude;
                    counter++;
                }

                gmap.Position = new GMap.NET.PointLatLng(lat/counter, longi/counter);

                GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
                gmap.Overlays.Add(markers);

                // Isi pilihan akun dengan semua simpul graf dan tambahkan marker pada map
                startComboBox.Items.Clear();
                goalComboBox.Items.Clear();
                foreach (Vertex v in graph.Vertices)
                {
                    startComboBox.Items.Add(v.Name);
                    goalComboBox.Items.Add(v.Name);
                    GMap.NET.WindowsForms.GMapMarker marker = 
                        new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                            new GMap.NET.PointLatLng(v.Latitude, v.Longitude),
                            GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue);
                    marker.ToolTipText = v.Name;
                    markers.Markers.Add(marker);
                }
            }
        }

        private void gmap_Load(object sender, EventArgs e)
        {
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gmap.ShowCenter = false;
            gmap.Position = new GMap.NET.PointLatLng(-6.892616, 107.610423);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            resultBox.Text = graph.AStar(startComboBox.Text, goalComboBox.Text);
        }
    }
}
