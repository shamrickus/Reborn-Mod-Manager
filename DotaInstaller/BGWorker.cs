using System;
using System.ComponentModel;

namespace DotaInstaller
{
    public class BGWorker
    {
        private BackgroundWorker w;

        public BGWorker()
        {
            w = new BackgroundWorker();
        }

        public BGWorker(Func<object> pFunc) : this()
        {
            Register(pFunc);
            RunAsync();
        }

        public BGWorker(Func<object> pFunc, Func<object> pComp) : this()
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