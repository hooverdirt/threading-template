namespace ThreadingPattern {
    public partial class mainForm : Form {

        private ManualResetEvent stopThreadEvent = new ManualResetEvent(false);
        private ManualResetEvent threadHasStoppedEvent = new ManualResetEvent(false);
        private ThreadTemplate thread;
        public mainForm() {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public Label StatusLabel {
            get { return this.label1; }
            set { this.label1 = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            this.threadHasStoppedEvent.Reset();

            this.stopThreadEvent.Reset();

            // deactivate button!
            this.button1.Enabled = false;
            this.button2.Enabled = true;

            // activate some stuff and then start thread class
            thread = new ThreadTemplate(this.stopThreadEvent,
                this.threadHasStoppedEvent);
            thread.TargetForm = this;
            thread.Execute();

        }

        private void button2_Click(object sender, EventArgs e) {
            if (this.thread != null) {
                if (this.thread.CurrentThread != null && this.thread.CurrentThread.IsAlive) {
                    // tell the thread to shut itself down...
                    this.stopThreadEvent.Set();

                    // let's wait until the thread is dead...
                    while (this.thread.CurrentThread.IsAlive) {
                        if (WaitHandle.WaitAll(new ManualResetEvent[] { threadHasStoppedEvent }, 100, true)) {
                            break;
                        }
                        Application.DoEvents();
                    }
                }
            }

            this.button1.Enabled = true;
            this.button2.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = false;

            if (this.thread != null) {
                if (this.thread.CurrentThread.IsAlive) {
                    // maybe show a dialog and if yes...
                    this.stopThreadEvent.Set();

                    // let's wait until the thread is dead...
                    while (this.thread.CurrentThread.IsAlive) {
                        if (WaitHandle.WaitAll(new ManualResetEvent[] { threadHasStoppedEvent }, 100, true)) {
                            break;
                        }
                        Application.DoEvents();
                    }
                }
            }
        }
    }


}