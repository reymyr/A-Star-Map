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
        public Form1()
        {
            InitializeComponent();
            Graph.Graph a = new Graph.Graph();

            ReadFile.readFile("input.txt", ref a);
            //a.AddVertex("A", 366);
            //a.AddVertex("B", 0);
            //a.AddVertex("C", 160);
            //a.AddVertex("D", 242);
            //a.AddVertex("E", 161);
            //a.AddVertex("F", 176);
            //a.AddVertex("G", 77);
            //a.AddVertex("H", 151);
            //a.AddVertex("I", 226);
            //a.AddVertex("L", 244);
            //a.AddVertex("M", 241);
            //a.AddVertex("N", 234);
            //a.AddVertex("O", 380);
            //a.AddVertex("P", 100);
            //a.AddVertex("R", 193);
            //a.AddVertex("S", 253);
            //a.AddVertex("T", 329);
            //a.AddVertex("U", 80);
            //a.AddVertex("V", 199);
            //a.AddVertex("Z", 374);
            //a.AddEdge("A", "Z", 75);
            //a.AddEdge("A", "S", 140);
            //a.AddEdge("A", "T", 118);
            //a.AddEdge("B", "F", 211);
            //a.AddEdge("B", "P", 101);
            //a.AddEdge("B", "G", 90);
            //a.AddEdge("B", "U", 85);
            //a.AddEdge("C", "D", 120);
            //a.AddEdge("C", "P", 138);
            //a.AddEdge("C", "R", 146);
            //a.AddEdge("D", "M", 75);
            //a.AddEdge("E", "H", 86);
            //a.AddEdge("F", "S", 99);
            //a.AddEdge("H", "U", 98);
            //a.AddEdge("I", "N", 87);
            //a.AddEdge("I", "V", 92);
            //a.AddEdge("L", "M", 70);
            //a.AddEdge("L", "T", 111);
            //a.AddEdge("O", "Z", 71);
            //a.AddEdge("O", "S", 151);
            //a.AddEdge("P", "R", 97);
            //a.AddEdge("R", "S", 80);
            //a.AddEdge("U", "V", 142);

            richTextBox1.Text = a.AStar("Gerbang_Belakang_ITB", "RS_Borromeus");
        }

    }
}
