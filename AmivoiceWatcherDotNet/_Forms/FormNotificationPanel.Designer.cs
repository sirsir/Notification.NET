namespace AmivoiceWatcher
{
    partial class FormNotificationPanel
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormNotificationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(484, 329);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(200, 50);
            this.Name = "FormNotificationPanel";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Sizable Tool";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNotificationPanel_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormNotificationPanel_FormClosed);
            this.Load += new System.EventHandler(this.FormNotificationPanel_Load);
            this.ResizeEnd += new System.EventHandler(this.FormLongNotification_ResizeEnd);
            this.VisibleChanged += new System.EventHandler(this.FormLongNotification_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormLongNotification_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormLongNotification_MouseDown);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Timer timer1;

        #endregion

        //public System.Windows.Forms.WebBrowser webBrowser1;
    }
}