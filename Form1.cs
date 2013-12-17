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
        public Form1( ) {
            InitializeComponent( );
            CheckForIllegalCrossThreadCalls = false;    // temp fix
        }

        private void btnFindClusters_Click( object sender, EventArgs e ) {
            IEnumerable<Cluster> clusters; // not that i really care about these...
            clusters = kMeansClustering1.ClusterData( ( int ) udClusters.Value );
        }

        private void kMeansClustering1_ProgressUpdated( object sender, kmcControl.ProgressUpdatedEventArgs e ) {
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
    }
}
