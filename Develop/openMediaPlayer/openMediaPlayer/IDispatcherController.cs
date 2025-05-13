using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface IDispatcherController
    {
        void Invoke(Action action);
        Task InvokeAsync(Action action);
    }
}
