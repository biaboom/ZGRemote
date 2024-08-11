using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGRemote.Server.Core;

namespace ZGRemote.Server.UI.ViewModels
{
    public abstract partial class RemoteViewModelBase : ViewModelBase, IDisposable
    {
        private bool disposedValue;

        public User? User { get; private set; }

        public RemoteViewModelBase(User? user)
        {
            User = user;
        }

        public virtual void Dispose()
        {
            if (disposedValue) return;
            User = null;
            disposedValue = true;
            GC.SuppressFinalize(this);
        }
    }
}
