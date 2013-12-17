using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kmc {
    public class PointData {
        public static float Radius = 3;

        public int Cluster;
        public PointF Location;
        public Brush Brush {
            get { return ClusterBrushes[ Cluster % ClusterBrushes.Length ]; }
        }

        public static readonly Brush[ ] ClusterBrushes = new Brush[ ] { 
                Brushes.LightGreen, Brushes.LightBlue, Brushes.Yellow, Brushes.Red, Brushes.Blue,
                Brushes.Orange, Brushes.White, Brushes.Black
            };
    }
}
