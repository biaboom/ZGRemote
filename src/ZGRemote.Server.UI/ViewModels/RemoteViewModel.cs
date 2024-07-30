using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZGRemote.Server.Core;

namespace ZGRemote.Server.UI.ViewModels
{
    partial class RemoteViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<User> users;
        public RemoteViewModel()
        {
            users = new ObservableCollection<User>();
            App.Current.Server.Connect += Connect;
            App.Current.Server.DisConnect += DisConnect;
        }

        private void Connect(User user)
        {
            App.Current.Dispatcher.BeginInvoke(() => { Users.Add(user); });
        }

        private void DisConnect(User user)
        {
            App.Current.Dispatcher.BeginInvoke(() => { Users.Remove(user); });
        }
    }
}
