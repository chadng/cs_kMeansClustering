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
    public partial class kmcControl : UserControl {
        public class ProgressUpdatedEventArgs : EventArgs {
            public int Value { get; set; }
            public int Maximum { get; set; }
            public string Text { get; set; }
        }
        
        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        private List<PointData> _points = new List<PointData>( );
        private PointF _mousePosition = new PointF( );

        private float _drawRadius = 25;
        private MouseButtons _mouseBtnDown;
        private Random _random = new Random( );
        private Bitmap _bmpGradient;
        private List<PointF> _lastCentroids;

        public kmcControl( ) {
            InitializeComponent( );
        }

        #region Graphical Management
        private void kMeansClustering_Paint( object sender, PaintEventArgs e ) {
            if ( _bmpGradient != null )
                e.Graphics.DrawImage( _bmpGradient, Point.Empty );

            foreach ( PointData p in _points ) {
                Brush color = _bmpGradient == null ? p.Brush : Brushes.White;

                e.Graphics.FillEllipse( color, p.Location.X - PointData.Radius,
                    p.Location.Y - PointData.Radius, 2 * PointData.Radius, 2 * PointData.Radius );

                e.Graphics.DrawEllipse( Pens.Black, p.Location.X - PointData.Radius,
                    p.Location.Y - PointData.Radius, 2 * PointData.Radius, 2 * PointData.Radius );
            }

            e.Graphics.DrawEllipse( Pens.Black, _mousePosition.X - _drawRadius,
                _mousePosition.Y - _drawRadius, 2 * _drawRadius, 2 * _drawRadius );
        }

        private static float distance( float x1, float y1, float x2, float y2 ) {
            return ( float ) Math.Sqrt( Math.Pow( x1 - x2, 2 ) + Math.Pow( y1 - y2, 2 ) );
        }

        private void kMeansClustering_MouseMove( object sender, MouseEventArgs e ) {
            _mousePosition.X = e.X;
            _mousePosition.Y = e.Y;

            Invalidate( );
        }

        private void onProgressUpdate( int val, int max, string text ) {
            if ( ProgressUpdated != null ) {
                ProgressUpdated( this, new ProgressUpdatedEventArgs {
                    Text = text, Value = val, Maximum = max
                } );
            }
        }

        private void drawClusterGradient( ) {
            Bitmap bmp = new Bitmap( Width, Height );
            this.DrawToBitmap( bmp, new Rectangle( Point.Empty, this.Size ) );

            object lockBmp = new object( );
            float maxDist = Math.Max( Width, Height );

            Parallel.For( 1, Width, x => {
                Parallel.For( 1, Height, y => {
                    float minDist = float.MaxValue;
                    int minIndex = 0;
                    for ( int i = 0; i < _lastCentroids.Count; i++ ) {
                        float dist = distance( x, y, _lastCentroids[ i ].X, _lastCentroids[ i ].Y );
                        if ( dist < minDist ) {
                            minDist = dist;
                            minIndex = i;
                        }
                    }
                    float shade = minDist / maxDist;
                    Color target = ( PointData.ClusterBrushes[ minIndex ] as SolidBrush ).Color;

                    /*
                    // linear interpolation between target (near) and white (far)
                    Color col = Color.FromArgb(
                        Math.Min( 255, target.R + ( int ) ( ( 255 - target.R ) * shade ) ),
                        Math.Min( 255, target.G + ( int ) ( ( 255 - target.G ) * shade ) ),
                        Math.Min( 255, target.B + ( int ) ( ( 255 - target.B ) * shade ) ) );
                    */

                    // linear interpolation between target (near) and black (far)
                    Color col = Color.FromArgb(
                        Math.Max( 0, ( int ) ( target.R * ( 1 - shade ) ) ),
                        Math.Max( 0, ( int ) ( target.G * ( 1 - shade ) ) ),
                        Math.Max( 0, ( int ) ( target.B * ( 1 - shade ) ) ) );


                    lock ( lockBmp )
                        bmp.SetPixel( x, y, col );
                } );
            } );

            _bmpGradient = bmp;
        }

        private void timer_Tick( object sender, EventArgs e ) {
            if ( _mouseBtnDown == MouseButtons.Left ) {
                    _points.Add( new PointData {
                        Location = new PointF(
                            _mousePosition.X + 2 * _drawRadius * ( ( float ) _random.NextDouble( ) - 0.5f ),
                            _mousePosition.Y + 2 * _drawRadius * ( ( float ) _random.NextDouble( ) - 0.5f ) )
                    } );
            }
            else if ( _mouseBtnDown == MouseButtons.Right ) {
                _points.RemoveAll( p => distance( p.Location.X, p.Location.Y, _mousePosition.X, _mousePosition.Y ) < _drawRadius );
            }

            Invalidate( );
        }

        private void kMeansClustering_MouseDown( object sender, MouseEventArgs e ) {
            _mouseBtnDown = e.Button;
            timer.Start( );
        }

        private void kMeansClustering_MouseUp( object sender, MouseEventArgs e ) {
            timer.Stop( );
        }
        #endregion

        #region K Means Clustering Algorithm
        /// <summary>
        /// Find k proper clusters for the current list of points.
        /// </summary>
        /// <param name="k">Number of desired clusters</param>
        /// <returns>The k clusters found.</returns>
        public IEnumerable<Cluster> ClusterData( int k ) {
            const int iterations = 50;
            int iterDone = 0;
            object lockIter = new object( );

            List<double> allCosts = new List<double>( );
            List<PointF[ ]> allCentroids = new List<PointF[ ]>( );

            if ( _points.Count == 0 )
                return null;

            Parallel.For( 0, iterations, i => {
                double cost;
                PointF[ ] centr = clusterData( k, out cost );

                if ( !double.IsNaN( cost ) ) {
                    lock ( lockIter ) {
                        allCentroids.Add( centr );
                        allCosts.Add( cost );
                        iterDone += 1;

                        onProgressUpdate( iterDone, iterations, string.Format( "{0}/{1} iterations done...", iterDone, iterations ) );
                    }
                }
            } );

            PointF[ ] centroids = allCentroids[ allCosts.IndexOf( allCosts.Min( ) ) ];
            _lastCentroids = centroids.ToList( );

            onProgressUpdate( 100, 100, "Drawing gradient..." );
            drawClusterGradient( );
            Invalidate( );
            onProgressUpdate( 100, 100, string.Format( "Done. Clustering cost: {0:F}", allCosts.Min( ) ) );

            return _points.GroupBy( p => p.Cluster )
                .Select( c => new Cluster {
                    Centroid = centroids[ c.Key ],
                    Points = c as IEnumerable<PointF>
                } );
        }

        /// <summary>
        /// Tries to cluster the points into k clusters.
        /// </summary>
        /// <param name="k">Number of clusters</param>
        /// <param name="cost">The cost found</param>
        /// <returns>A list of k clusters</returns>
        private PointF[ ] clusterData( int k, out double cost ) {
            PointF[ ] centroids = new PointF[ k ];
            double prevClusteringCost;

            // random initialization
            cost = double.MaxValue;
            for ( int i = 0; i < k; i++ )
                centroids[ i ] = _points[ _random.Next( _points.Count ) ].Location;

            do {
                prevClusteringCost = cost;

                assignClusters( centroids );
                for ( int i = 0; i < k; i++ )
                    centroids[ i ] = mean( _points.Where( p => p.Cluster == i ).Select( p => p.Location ) );

                cost = clusterCost( centroids );
            } while ( prevClusteringCost - cost > 1 );

            return centroids;
        }

        /// <summary>
        /// Assign each point to the nearest cluster.
        /// </summary>
        /// <param name="centroids">A list of centroids.</param>
        private void assignClusters( PointF[ ] centroids ) {
            Parallel.ForEach( _points, p => {
                float minDistance = float.MaxValue;
                int minIndex = 0;

                for ( int i = 0; i < centroids.Length; i++ ) {
                    float dist = distance( p.Location.X, p.Location.Y, centroids[ i ].X, centroids[ i ].Y );
                    if ( dist < minDistance ) {
                        minDistance = dist;
                        minIndex = i;
                    }
                }

                p.Cluster = minIndex;
            } );
        }

        /// <summary>
        /// Find the mean point of the given point cloud
        /// </summary>
        /// <param name="points">A set of points</param>
        /// <returns>The centroid</returns>
        private static PointF mean( IEnumerable<PointF> points ) {
            int count = points.Count( );
            float sumX = points.Sum( p => p.X ),
                sumY = points.Sum( p => p.Y );

            return new PointF( sumX / count, sumY / count );
        }

        private double clusterCost( PointF[ ] centroids ) {
            return ( 1.0 / _points.Count ) * _points.Sum( p => distance( p.Location.X, p.Location.Y,
                centroids[ p.Cluster ].X, centroids[ p.Cluster ].Y ) );
        }
        #endregion
    }
}
