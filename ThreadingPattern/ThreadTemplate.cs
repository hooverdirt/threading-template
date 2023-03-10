using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThreadingPattern {

    public class ThreadTemplate {
        private ManualResetEvent stopThread;
        private ManualResetEvent finishedThread;
        private Thread thread;
        private Form form;

        public delegate void StringInvoker(string text);

        public ThreadTemplate(ManualResetEvent hasstopped, ManualResetEvent hasfinished) {
            this.stopThread = hasstopped;
            this.finishedThread = hasfinished;
        }

        // START: any public properties/methods to accept
        // settings from outside of the class
        public Form TargetForm {
            get { return this.form; }
            set { this.form = value; }
        }


        // END: any public properties/methods to accept
        // settings from outside of the class

        /// <summary>
        /// 
        /// </summary>
        public Thread CurrentThread {
            get { return thread; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        private int doSomething(int i) {
            return i * 10;
        }


        /// <summary>
        /// 
        /// </summary>
        private void runThread() {
            // THIS IS WHERE YOUR LONG RUNNING CODE GOES
            // so below is just a piece of sample code...
            try {
                for (int i = 0; i < 200000; i++) {
                    // update something
                    this.updateThreadIsRunning("We're running");


                    // the user pressed the stop button....
                    if (this.stopThread.WaitOne(0, true)) {
                        this.updateThreadIsRunning("We've been stopped");
                        break;
                    }
                }
            }
            catch (Exception e) {
                // do  something
            }
            finally {
                // end of the road stop here and set the finished thread.
                this.finishedThread.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Execute() {
            this.thread = new Thread(runThread);
            this.thread.Name = "GiveItWhatevername_" + Guid.NewGuid().ToString();
            this.thread.Start();
        }


        // START
        // Any methods to update the main thread.

        // this is an example where the label on that form is being updated..
        private void updateThreadIsRunning(String isRunningText) {
            if (this.TargetForm != null) {
                if (this.TargetForm.InvokeRequired) {
                    this.TargetForm.Invoke(new StringInvoker(updateThreadIsRunning),
                        new object[] { isRunningText });
                }
                else {
                    (this.TargetForm as mainForm).StatusLabel.Text = isRunningText;
                }
            }
        }

        // END

    }

}