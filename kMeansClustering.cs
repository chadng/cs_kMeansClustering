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
    public class kMeansClustering {
        private class PointData {
            public PointF Location;
            public int Cluster;
        }

        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        private List<PointData> _points = new List<PointData>( );
        private List<PointF> _lastCentroids;
        private Random _random = new Random( );

        public kMeansClustering( ) { }

        /// <summary>
        /// Sets a sequence of points as the input data
        /// </summary>
        /// <param name="points">The sequence of points; it will be fully enumerated</param>
        public void SetData( IEnumerable<PointF> points ) {
            this._points = points.Select( p => new PointData {
                Location = p,
                Cluster = 0
            } ).ToList( );
        }

        /// <summary>
        /// Splits the data into clusters using the K Means Algorithm
        /// </summary>
        /// <param name="k">Number of clusters</param>
        /// <param name="iterations">Runs of kmc; the best clustering found is returned</param>
        /// <returns>The centroids of the k clusters found</returns>
        public List<PointF> FindCentroids( int k, int iterations = 100 ) {
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

                        onProgressUpdate( iterDone, iterations, string.Format( "{0}/{1} iterations done...",
                            iterDone, iterations + 1 ) );
                    }
                }
            } );

            onProgressUpdate( 100, 100, string.Format( "Done. Clustering cost: {0:F}", allCosts.Min( ) ) );

            PointF[ ] centroids = allCentroids[ allCosts.Min( c => c ).Item1 ];
            return _lastCentroids = centroids.ToList( );
        }

        /// <summary>
        /// Once correctly located the centroids this method groups all the points in
        /// their respective cluster
        /// </summary>
        /// <returns>A sequence of cluster, each containing the centroid and the points assigned to it</returns>
        public IEnumerable<Cluster> GetClusteredData( ) {
            if ( _lastCentroids != null ) {
                return _points.GroupBy( p => p.Cluster )
                    .Select( c => new Cluster {
                        Centroid = _lastCentroids[ c.Key ],
                        Points = c.Select( p => p.Location )
                    } );
            }
            else return null;
        }

        /// <summary>
        /// Tries to cluster the points into k clusters
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

                // LINQ??
                for ( int i = 0; i < k; i++ )
                    centroids[ i ] = mean( _points.Where( p => p.Cluster == i ).Select( p => p.Location ) );

                cost = clusterCost( centroids );
            } while ( ( prevClusteringCost - cost ) / prevClusteringCost > 0.01 );
            // stop when less than 1% variation

            return centroids;
        }

        /// <summary>
        /// Assign each point to the nearest centroid
        /// </summary>
        /// <param name="centroids">A list of centroids.</param>
        private void assignClusters( PointF[ ] centroids ) {
            Parallel.ForEach( _points, p => {
                p.Cluster = centroids.Min( c => c.Distance( p.Location ) ).Item1;
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

        /// <summary>
        /// Cost of the curent clustering; what kmc actually does is to minimize this function with
        /// respect to the centroids position. As a naive countermeasure against local optima kmc
        /// is run multiple times using random starting centroids and the clustering which yielded
        /// the minimum cost is taken.
        /// </summary>
        /// <param name="centroids">Centroids of the clusters</param>
        /// <returns>The cost</returns>
        private double clusterCost( PointF[ ] centroids ) {
            return ( 1.0 / _points.Count ) * _points.Sum( p => p.Location.Distance( centroids[ p.Cluster ] ) );
        }

        private void onProgressUpdate( int val, int max, string text ) {
            if ( ProgressUpdated != null ) {
                ProgressUpdated( this, new ProgressUpdatedEventArgs {
                    Text = text, Value = val, Maximum = max
                } );
            }
        }
    }
}
