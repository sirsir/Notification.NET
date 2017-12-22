using System.Windows.Forms;

namespace AmivoiceWatcher
{
    partial class FormWaiting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWaiting));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.progressBar2 = new CircularProgressBar.CircularProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // progressBar2
            // 
            this.progressBar2.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.progressBar2.AnimationSpeed = 500;
            this.progressBar2.BackColor = System.Drawing.Color.Transparent;
            this.progressBar2.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold);
            this.progressBar2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.progressBar2.InnerColor = System.Drawing.Color.Transparent;
            this.progressBar2.InnerMargin = 0;
            this.progressBar2.InnerWidth = -1;
            this.progressBar2.Location = new System.Drawing.Point(1, 1);
            this.progressBar2.Margin = new System.Windows.Forms.Padding(0);
            this.progressBar2.MarqueeAnimationSpeed = 2000;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.OuterColor = System.Drawing.Color.Silver;
            this.progressBar2.OuterMargin = -25;
            this.progressBar2.OuterWidth = 26;
            this.progressBar2.ProgressColor = System.Drawing.Color.DeepSkyBlue;
            this.progressBar2.ProgressWidth = 3;
            this.progressBar2.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.progressBar2.Size = new System.Drawing.Size(89, 93);
            this.progressBar2.StartAngle = 270;
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar2.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.progressBar2.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.progressBar2.SubscriptText = ".23";
            this.progressBar2.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.progressBar2.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.progressBar2.SuperscriptText = "°C";
            this.progressBar2.TabIndex = 5;
            this.progressBar2.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.progressBar2.Value = 68;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::AmivoiceWatcher.Properties.Resources.logo;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Location = new System.Drawing.Point(20, 21);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(50, 50);
            this.panel1.TabIndex = 6;
            // 
            // FormWaiting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(95, 96);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar2);
            this.DoubleBuffered = true;
            this.Enabled = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormWaiting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormWaiting_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Timer timer1;
        //private ProgressBar progressBar2;
        private CircularProgressBar.CircularProgressBar progressBar2;
        private Panel panel1;
    }
}