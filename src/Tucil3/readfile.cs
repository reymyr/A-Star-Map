using System; 
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Tucil3
{
     class ReadFile
     {
          public static void readFile(string filename, ref Graph.Graph graf) {
               string[] filePerLine = File.ReadAllLines(filename); //baca file per line
               //hitung jumlah vertex
               int nVertex = int.Parse(filePerLine[0]);
               //container isi file per line
               string[] pairVertex;
               string[] pairEdge;
               Dictionary<int, string> kamusVertex = new Dictionary<int, string>(); //buat nyari nama vertex pas masukin edge adj matrix

               int i = 1;
               for (; i <= nVertex; i++) {
                    pairVertex = filePerLine[i].Split(' '); //baca vertex
                    graf.AddVertex(pairVertex[2], double.Parse(pairVertex[1]), double.Parse(pairVertex[0])); //karena urutan di file : lintang bujur nama
                    kamusVertex.Add(i-1, pairVertex[2]); //nambahin nama vertex ke kamus
               }
               int iterAdjMatrix = 0; //biar baca adj matrixnya ga repetitive
               for (int j = i+1; j < filePerLine.Length; j++) {
                    pairEdge = filePerLine[i].Split(' '); //baca adj matrix
                    for (int k = 0; k < iterAdjMatrix; k++) {
                         if (double.Parse(pairEdge[k]) > 0) {
                              graf.AddEdge(kamusVertex[iterAdjMatrix], kamusVertex[k], double.Parse(pairEdge[k])); //intinya ini masukin edge berasal dari kamus nama    
                         }
                    }
                    iterAdjMatrix++;
               }
          }
     }
}