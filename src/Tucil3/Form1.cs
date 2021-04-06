using GMap.NET.WindowsForms;
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
        private List<GMapMarker> addEdgeMarkers = new List<GMapMarker>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) // Jika pengguna memilih file, asumsi format file masukan benar
            {
                gmap.Overlays[0].Clear();
                gmap.Overlays[2].Clear();
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

                double lat = 0;
                double longi = 0;
                int counter = 0;        
                
                // Isi pilihan akun dengan semua simpul graf dan tambahkan marker pada map
                startComboBox.Items.Clear();
                goalComboBox.Items.Clear();
                foreach (Vertex v in graph.Vertices)
                {
                    startComboBox.Items.Add(v.Name);
                    goalComboBox.Items.Add(v.Name);
                    lat += v.Latitude;
                    longi += v.Longitude;
                    counter++;

                    GMapMarker marker = 
                        new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                            new GMap.NET.PointLatLng(v.Latitude, v.Longitude),
                            GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue);
                    marker.ToolTipText = v.Name;
                    gmap.Overlays[2].Markers.Add(marker);

                    List<GMap.NET.PointLatLng> points = new List<GMap.NET.PointLatLng>();
                    GMap.NET.PointLatLng curPoint = new GMap.NET.PointLatLng(v.Latitude, v.Longitude);
                    foreach (Edge edge in v.Edges)
                    {
                        if (v.Name.CompareTo(edge.ToVertex) < 0)
                        {
                            points.Clear();
                            points.Add(curPoint);
                            points.Add(new GMap.NET.PointLatLng(
                                graph.Vertices.Find(v => v.Name == edge.ToVertex).Latitude,
                                graph.Vertices.Find(v => v.Name == edge.ToVertex).Longitude));
                            GMapPolygon line = new GMapPolygon(points, v.Name + edge.ToVertex);
                            line.Stroke = new Pen(Color.Blue, 1);
                            gmap.Overlays[0].Polygons.Add(line);
                        }
                    }
                }

                // Set maps di tengah-tengah node
                gmap.Position = new GMap.NET.PointLatLng(lat / counter, longi / counter);
            }
        }

        private void gmap_Load(object sender, EventArgs e)
        {
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.ShowCenter = false;
            gmap.Position = new GMap.NET.PointLatLng(-6.892616, 107.610423);
            GMapOverlay lines = new GMapOverlay("lines");
            gmap.Overlays.Add(lines);

            GMapOverlay routeLines = new GMapOverlay("routeLines");
            gmap.Overlays.Add(routeLines);

            GMapOverlay markers = new GMapOverlay("markers");
            gmap.Overlays.Add(markers);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (startComboBox.Text != "" && goalComboBox.Text != "")
            {
                gmap.Overlays[1].Clear();
                Tuple<List<string>, string> result = graph.AStar(startComboBox.Text, goalComboBox.Text);

                // Isi result dengan hasil explore
                resultBox.Text = result.Item2;

                Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();

                List<string> path = result.Item1;

                // Warnai jalur explore pada graf jika ditemukan jalur
                Microsoft.Msagl.Drawing.Graph msaglgraph = graph.getMSAGLGraph(openFileDialog1.SafeFileName);
                if (path != null)
                {
                    List<GMap.NET.PointLatLng> points = new List<GMap.NET.PointLatLng>();
                    gmap.Overlays[1].Clear();
                    int i;
                    for (i = 0; i < path.Count() - 1; i++)
                    {
                        string first = path[i].CompareTo(path[i + 1]) < 0 ? path[i] : path[i + 1];
                        string second = path[i].CompareTo(path[i + 1]) < 0 ? path[i + 1] : path[i];

                        msaglgraph.FindNode(path[i]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Orange;

                        msaglgraph.EdgeById(first + second).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;

                        points.Clear();
                        points.Add(new GMap.NET.PointLatLng(
                            graph.Vertices.Find(v => v.Name == first).Latitude,
                            graph.Vertices.Find(v => v.Name == first).Longitude));
                        points.Add(new GMap.NET.PointLatLng(
                            graph.Vertices.Find(v => v.Name == second).Latitude,
                            graph.Vertices.Find(v => v.Name == second).Longitude));
                        GMapPolygon line = new GMapPolygon(points, "route" + i.ToString());
                        line.Stroke = new Pen(Color.Orange, 4);
                        gmap.Overlays[1].Polygons.Add(line);
                    }
                    msaglgraph.FindNode(path[i]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Orange;
                }

                viewer.Graph = msaglgraph;

                viewer.Dock = DockStyle.Fill;
                graphVisualizer.Controls.Clear();
                graphVisualizer.Controls.Add(viewer);

            } else
            {
                resultBox.Text = "Please select the nodes";
            }
        }

        private void gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                resultBox.Text = "";
                double latitude = gmap.FromLocalToLatLng(e.X, e.Y).Lat;
                double longitude = gmap.FromLocalToLatLng(e.X, e.Y).Lng;


                GMapMarker marker =
                    new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                        new GMap.NET.PointLatLng(latitude, longitude),
                        GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue);
                if (graph == null) graph = new Graph.Graph();
                string name = "Node " + graph.Vertices.Count.ToString();
                marker.ToolTipText = name;
                gmap.Overlays[2].Markers.Add(marker);
                graph.AddVertex(name, latitude, longitude);

                startComboBox.Items.Add(name);
                goalComboBox.Items.Add(name);

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
            }
        }

        private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (addEdgeMarkers.Count < 1)
                {
                    addEdgeMarkers.Add(item);
                }
                else
                {
                    resultBox.Text = "";
                    List<GMap.NET.PointLatLng> points = new List<GMap.NET.PointLatLng>();
                    points.Add(new GMap.NET.PointLatLng(addEdgeMarkers[0].Position.Lat, addEdgeMarkers[0].Position.Lng));
                    points.Add(new GMap.NET.PointLatLng(item.Position.Lat, item.Position.Lng));
                    GMapPolygon line = new GMapPolygon(points, addEdgeMarkers[0].ToolTipText + item.ToolTipText);
                    line.Stroke = new Pen(Color.Blue, 1);
                    gmap.Overlays[0].Polygons.Add(line);

                    double distance = Math.Round(graph.haversine(
                        addEdgeMarkers[0].Position.Lat,
                        item.Position.Lat,
                        addEdgeMarkers[0].Position.Lng,
                        item.Position.Lng), 2);
                    graph.AddEdge(addEdgeMarkers[0].ToolTipText, item.ToolTipText, distance);

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

                    addEdgeMarkers.Clear();
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            gmap.Overlays[0].Clear();
            gmap.Overlays[1].Clear();
            gmap.Overlays[2].Clear();
            resultBox.Text = "";
            labelFilename.Text = "";
            graph = new Graph.Graph();
            graphVisualizer.Controls.Clear();
            startComboBox.Items.Clear();
            goalComboBox.Items.Clear();
        }
    }
}
