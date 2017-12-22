using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AmivoiceWatcher
{
    public partial class FormWaiting : Form
    {
        #region hide from task list
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        #endregion

        //public FormWaiting()
        //{
        //    InitializeComponent();
        //}

        #region waiting windows
        delegate void CloseDelegateCallback();
        public static void CloseDelegate()
        {
            try
            {
                if (splashForm.InvokeRequired)
                {
                    CloseDelegateCallback d = new CloseDelegateCallback(CloseDelegate);
                    splashForm.Invoke(d, new object[] { });
                }
                else
                {
                    if (splashForm != null)
                    {
                        splashForm.Close();
                    }                    
                }
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }
        }

        //The type of form to be displayed as the splash screen.
        private static FormWaiting splashForm;

        static public void ShowSplashScreen()
        {
            // Make sure it is only launched once.
            if (splashForm != null)
            {
                return;
            }else
            {
                splashForm = new FormWaiting();
                //PictureBox pictureBox1 = new PictureBox();
                //pictureBox1.Location = new Point(0,0);          
                //splashForm.Controls.Add(pictureBox1);
                splashForm.InitializeComponent();
                
                splashForm.Show();
            }
                
            //Thread thread = new Thread(new ThreadStart(FormWaiting.ShowForm));
            //thread.IsBackground = true;
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
        }

        delegate void ShowFormCallback();
        public static void ShowForm()
        {

            if (splashForm.InvokeRequired)
            {
                ShowFormCallback d = new ShowFormCallback(ShowForm);
                splashForm.Invoke(d, new object[] { });
            }
            else
            {
                //splashForm = new FormWaiting();
                splashForm.Show();
                //Application.Run(splashForm);
            }
        }

        static public void CloseForm()
        {
            //splashForm.Invoke(new CloseDelegate(FormWaiting.CloseFormInternal));
            CloseFormInternal();

        }

        static private void CloseFormInternal()
        {
            splashForm.Close();
        }
        #endregion

        private void FormWaiting_Load(object sender, EventArgs e)
        {
            timer1.Interval = 30000;
            timer1.Start();

            backgroundWorker1.RunWorkerAsync();
           
            this.Width = this.Height;

            progressBar2.Height = (int)(0.8*this.Height);
            progressBar2.Width = progressBar2.Height;

            progressBar2.Top = (int)(0.1 * this.Height);
            progressBar2.Left = progressBar2.Top;

            panel1.Height = (int)(this.Height * 0.4);
            panel1.Width = panel1.Height;

            panel1.Left = (int)((this.Width - panel1.Width) / 2);
            panel1.Top = panel1.Left;

            this.Location = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width), (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height));
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                Thread.Sleep(200);
                backgroundWorker1.ReportProgress(i);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
            this.Text = e.ProgressPercentage.ToString();
        }

        private void timer1_Tick(object sender, EventArgs ev)
        {
            try
            {
                Globals.log.Debug("FormWaitings:> timer1_tick will close waiting form.");
                FormWaiting.CloseDelegate();

                AmivoiceWatcher.NotificationPanel.ShowPanel();
            }
            catch(Exception e)
            {
                Globals.log.Debug(e.ToString());
            }            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
