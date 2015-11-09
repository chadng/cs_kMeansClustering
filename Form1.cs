using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kmc {
    public partial class Form1 : Form {
        private kMeansClustering kmc = new kMeansClustering( );
        private bool _displayGradient = false;

        private List<PointF> _lastCentroids = null;
        private IEnumerable<Cluster> _lastClusters = null;
        
        public Form1( ) {
            InitializeComponent( );

            kmc.ProgressUpdated += kmc_ProgressUpdated;
            CheckForIllegalCrossThreadCalls = false;    // temp fix
            udClusters.Maximum = kmcControl.ClusterBrushes.Length;
        }

        private void btnFindClusters_Click( object sender, EventArgs e ) {
            kmc.SetData( kmcControl1.Points );

            _lastCentroids = kmc.FindCentroids( ( int ) udClusters.Value );
            _lastClusters = kmc.GetClusteredData( );            // will be enumerated only if needed

            if ( _displayGradient )
                kmcControl1.DisplayGradient( _lastCentroids );  // i.e. in this case
            else
                kmcControl1.DisplayColoredPoints( _lastClusters );

        }

        private void kmc_ProgressUpdated( object sender, ProgressUpdatedEventArgs e ) {
            /* freezes here, starvation elsewhere
            if ( InvokeRequired ) {
                Invoke( new Action( ( ) => {
                    lblStatus.Text = e.Text;
                    progressBar.Maximum = e.Maximum;
                    progressBar.Value = e.Value;
                } ) );
            }
            */

            lblStatus.Text = e.Text;
            progressBar.Maximum = e.Maximum;
            progressBar.Value = e.Value;
        }

        private void coloredPointsToolStripMenuItem_Click( object sender, EventArgs e ) {
            _displayGradient = false;
            if ( _lastClusters != null )
                kmcControl1.DisplayColoredPoints( _lastClusters );
        }

        private void displayGradientToolStripMenuItem_Click( object sender, EventArgs e ) {
            _displayGradient = true;
            if ( _lastCentroids != null )
                kmcControl1.DisplayGradient( _lastCentroids );
        }
    }
}
