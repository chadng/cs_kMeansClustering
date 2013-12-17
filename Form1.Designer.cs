namespace kmc {
    partial class Form1 {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent( ) {
            this.kMeansClustering1 = new kmc.kmcControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.btnFindClusters = new System.Windows.Forms.Button();
            this.udClusters = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udClusters)).BeginInit();
            this.SuspendLayout();
            // 
            // kMeansClustering1
            // 
            this.kMeansClustering1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.kMeansClustering1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.kMeansClustering1.Location = new System.Drawing.Point(12, 12);
            this.kMeansClustering1.Name = "kMeansClustering1";
            this.kMeansClustering1.Size = new System.Drawing.Size(588, 422);
            this.kMeansClustering1.TabIndex = 0;
            this.kMeansClustering1.ProgressUpdated += new System.EventHandler<kmc.kmcControl.ProgressUpdatedEventArgs>(this.kMeansClustering1_ProgressUpdated);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 437);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(721, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(238, 17);
            this.lblStatus.Text = "Left click to add points, right click to erase...";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // btnFindClusters
            // 
            this.btnFindClusters.Location = new System.Drawing.Point(606, 38);
            this.btnFindClusters.Name = "btnFindClusters";
            this.btnFindClusters.Size = new System.Drawing.Size(103, 20);
            this.btnFindClusters.TabIndex = 11;
            this.btnFindClusters.Text = "Find Clusters!";
            this.btnFindClusters.UseVisualStyleBackColor = true;
            this.btnFindClusters.Click += new System.EventHandler(this.btnFindClusters_Click);
            // 
            // udClusters
            // 
            this.udClusters.Location = new System.Drawing.Point(659, 12);
            this.udClusters.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udClusters.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.udClusters.Name = "udClusters";
            this.udClusters.Size = new System.Drawing.Size(50, 20);
            this.udClusters.TabIndex = 10;
            this.udClusters.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udClusters.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(606, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Clusters:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 459);
            this.Controls.Add(this.btnFindClusters);
            this.Controls.Add(this.udClusters);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.kMeansClustering1);
            this.Name = "Form1";
            this.Text = "K-Means Clustering";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udClusters)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private kmcControl kMeansClustering1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Button btnFindClusters;
        private System.Windows.Forms.NumericUpDown udClusters;
        private System.Windows.Forms.Label label1;
    }
}

