using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFIrcRelay
{
    public interface IService : IDisposable
    {
        void OnStart(string[] args);
        void OnStop();
        void OnPause();
        void OnContinue();
        void OnCustomCommand(int command);
        void OnShutdown();
    }
}
