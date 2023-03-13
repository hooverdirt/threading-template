# DISCLAIMER

This is my own code but the idea for it was used as a model for some of the products.

# History

In 2008 I was hired to work as a programmer at a company creating software for the insurance industry. Initially, I was working on a new product - but eventually I got exposed to one of its core products, "The Product". The Product was initially maintained in .Net 1 and there had been already preparations made to make it work in .Net 2. I think a few months in, to my best recollection, the release of the .Net 2 version had been rolled out to production. However, there were threading issues - related to the fact that .Net 2.0's threading model had changed.

I was comfortable in multi-threading programming - mostly because I did a lot of this in an open-source project written in Delphi (RoundAbout). In the Summer of 2008, someone assigned me a ticket to fix up the threading problems (and errors) in "The Product", particularly around the pausing and the part where a user could stop a process from the main thread. I wrote up a small test program first (the usual POC) using two manual reset events: one reset for allowing the user to stop the thread and one to signal the main thread that the secondary thread had ended. The next part was implementing this into "The Product".

I did this in a few days - and since it was exactly the day before I was supposed to go on vacation I did my commit to my own branch and told the lead dev and the CTO that my work had been completed, generally explaining how it worked  - and off I went.

A few weeks later, the lead dev mentioned and confirmed that my changes had gone to production: no issues were found and the code worked as intended. We high-fived, and I continued tracking the lead dev's "foul" f-word count.

(At a later stage - maybe a year later, I was asked to implement the same threading model in a side product that we called "The Debugger").

# A pattern

The threading pattern mentioned above is actually a very safe way of running threads and interrupting them - and since I ended up using it in other products, I had made an example/demo for this for the earlier mentioned company. The code here is brand new rewritten code, based on some of the work I did in other open source projects (Convendro for example).

As mentioned - this threading technique is using two manual reset events, instantiated outside of the actual threaded code. The main manual reset event (the one that signals that a user wants to stop processing) is being monitored from the secondary thread. If that event is triggered, the threading code can then break off and should set the second reset event to signal the main thread that everything was completed.

# A closer look at the code

The main threading code is built into the ThreadTemplate class - its private member runThread().

```
    for (int i = 0; i < 200000; i++) {
        // update something
        this.updateThreadIsRunning("We're running");


        // the user pressed the stop button from the main thread
        if (this.stopThread.WaitOne(0, true)) {
            this.updateThreadIsRunning("We've been stopped");
            break;
        }
    }

    [...]
    // signal the main thread that we're done...
    this.finishedThread.Set()
```

The rest of the code should speak for itself - but pay attention to how we invoke the main thread to update elements on the main form.

```
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
```

To gracefully stop the secondary thread, in the main thread (or the main form):

```
    if (this.thread.CurrentThread != null && this.thread.CurrentThread.IsAlive) {
        // tell the thread to shut itself down...
        this.stopThreadEvent.Set();

        // let's wait until the thread is dead...
        while (this.thread.CurrentThread.IsAlive) {
            if (WaitHandle.WaitAll(new ManualResetEvent[] { threadHasStoppedEvent }, 100, true)) {
                break;
            }
            // give the main thread some breathing room - depending on how we coded the threaded code,
            // this may take a bit..
            Application.DoEvents();
        }
    }
```

And that's it!

# Conclusion

This is a full ready to go template. Use where ever you see fit (or use the fancy TPL): pure Windows style threading is more fun tho. You can do a lot more crazy things with reset events - if you are crazy enough and willing to debug it.
