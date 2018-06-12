using System;
using System.ComponentModel;

namespace DotaInstaller.src.Utilities
{
    public class SyncThread
    {
        private BackgroundWorker w;

        public SyncThread()
        {
            w = new BackgroundWorker();
        }

        public SyncThread(Func<object> pFunc) : this()
        {
            Register(pFunc);
            RunAsync();
        }

        public SyncThread(Func<object> pFunc, Func<object> pComp) : this()
        {
            Register(pFunc);
            RunAfter(pComp);
            RunAsync();
        }

        public void Register(Func<object> pFunc)
        {
            w.DoWork += (o, e) => pFunc.Invoke();
        }

        public void RunAfter(Func<object> pFunc)
        {
            w.RunWorkerCompleted += (o, e) => pFunc.Invoke();
        }

        public void RunAsync()
        {
            w.RunWorkerAsync();
        }
    }
}