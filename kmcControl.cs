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
        /// <summary>
        /// Controls how the resulting clustering is displayed
        /// </summary>
        private enum eDisplayStyle {
            /// <summary>
            /// Draw the region assigned to each cluster
            /// </summary>
            Gradient,

            /// <summary>
            /// Draw the points in different colors according to the assigned cluster
            /// </summary>
            Points
        }

        public IEnumerable<PointF> Points { get { return _points; } }

        private List<PointF> _points = new List<PointF>( );
        private eDisplayStyle _displayStyle = eDisplayStyle.Points;
        private List<PointF> _centroids;
        private List<Cluster> _clusters;

        private PointF _mousePosition = new PointF( );
        private float _brushRadius = 25;
        private float _pointRadius = 3;
        private MouseButtons _mouseBtnDown;
        private Random _random = new Random( );
        private Bitmap _bmpGradient;

        private static readonly Brush[ ] _clusterBrushes = new Brush[ ] { 
                Brushes.LightGreen, Brushes.LightBlue, Brushes.Yellow, Brushes.Red, Brushes.Blue,
                Brushes.Orange, Brushes.White, Brushes.Black
            };

        public kmcControl( ) {
            InitializeComponent( );
        }

        public void DisplayGradient( IEnumerable<PointF> centroids ) {
            _displayStyle = eDisplayStyle.Gradient;
            _centroids = centroids.ToList( );
            drawClusterGradient( );

            Invalidate( );
        }

        public void DisplayColoredPoints( IEnumerable<Cluster> clusters ) {
            _displayStyle = eDisplayStyle.Points;
            _clusters = clusters.ToList( );

            Invalidate( );
        }

        private void kMeansClustering_Paint( object sender, PaintEventArgs e ) {
            // always draw our points, the old ones will be eventually overwritten
            foreach ( PointF p in _points )
                drawPoint( e.Graphics, p, Brushes.Red );

            if ( _displayStyle == eDisplayStyle.Points && _clusters != null ) {
                foreach ( Tuple<int, Cluster> cluster in _clusters.Enumerate( ) ) {
                    Brush color = _clusterBrushes[ cluster.Item1 % _clusterBrushes.Length ];

                    foreach ( PointF p in cluster.Item2.Points )
                        drawPoint( e.Graphics, p, color );
                }
            }
            else if ( _displayStyle == eDisplayStyle.Gradient && _bmpGradient != null ) {
                e.Graphics.DrawImage( _bmpGradient, Point.Empty );

                foreach ( PointF p in _points )
                    drawPoint( e.Graphics, p, Brushes.White );
            }

            e.Graphics.DrawEllipse( Pens.Black, _mousePosition.X - _brushRadius,
                _mousePosition.Y - _brushRadius, 2 * _brushRadius, 2 * _brushRadius );
        }

        private void drawPoint( Graphics g, PointF position, Brush color ) {
            g.FillEllipse( color, position.X - _pointRadius, position.Y - _pointRadius,
                2 * _pointRadius, 2 * _pointRadius );

            g.DrawEllipse( Pens.Black, position.X - _pointRadius, position.Y - _pointRadius,
                2 * _pointRadius, 2 * _pointRadius );
        }

        private void kMeansClustering_MouseMove( object sender, MouseEventArgs e ) {
            _mousePosition.X = e.X;
            _mousePosition.Y = e.Y;

            Invalidate( );
        }

        private void drawClusterGradient( ) {
            Bitmap bmp = new Bitmap( Width, Height );

            object lockBmp = new object( );
            float maxDist = Math.Max( Width, Height );

            Parallel.For( 1, Width, x => {
                Parallel.For( 1, Height, y => {
                    Tuple<int, float> nearestCentroid = _centroids.Min( c => c.Distance( new PointF( x, y ) ) );

                    float shade = nearestCentroid.Item2 / maxDist;
                    Color target = ( _clusterBrushes[ nearestCentroid.Item1 ] as SolidBrush ).Color;

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
                _points.Add( new PointF(
                        _mousePosition.X + 2 * _brushRadius * ( ( float ) _random.NextDouble( ) - 0.5f ),
                        _mousePosition.Y + 2 * _brushRadius * ( ( float ) _random.NextDouble( ) - 0.5f ) ) );
            }
            else if ( _mouseBtnDown == MouseButtons.Right ) {
                _points.RemoveAll( p => p.Distance( _mousePosition ) < _brushRadius );
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
    }
}
