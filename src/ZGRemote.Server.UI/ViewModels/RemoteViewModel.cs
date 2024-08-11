using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.DataContracts;
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

        [ObservableProperty]
        private User? selectUser;

        [ObservableProperty]
        private bool isExpanded;

        public RemoteViewModel()
        {
            users = new ObservableCollection<User>();
            App.Current.Server.Connect += Connect;
            App.Current.Server.DisConnect += DisConnect;
        }

        [RelayCommand]
        private void OpenRemoteShell()
        {
            if (SelectUser == null) return;
            var vm = App.Current.MainViewModel.RemoteViewModelList.
                Where(vm => vm.GetType() == typeof(RemoteShellViewModel) && vm.User == SelectUser).FirstOrDefault();
            if (vm == null)
            {
                vm = new RemoteShellViewModel(SelectUser);
                vm.Header = $"RemoteShell@{SelectUser.Name}";
                App.Current.MainViewModel.RemoteViewModelList.Add(vm);
            }
            App.Current.MainViewModel.NavigateTo(vm);
            IsExpanded = true;
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
