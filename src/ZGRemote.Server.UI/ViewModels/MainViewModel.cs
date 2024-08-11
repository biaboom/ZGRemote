using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGRemote.Server.UI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase homeViewModel;

        [ObservableProperty]
        private ViewModelBase remoteViewModel;

        [ObservableProperty]
        private ViewModelBase builderViewModel;

        [ObservableProperty]
        private ViewModelBase settingsViewModel;

        [ObservableProperty]
        private ViewModelBase currentViewModel;

        public ObservableCollection<RemoteViewModelBase> RemoteViewModelList;

        public MainViewModel()
        {
            homeViewModel = new HomeViewModel();
            remoteViewModel = new RemoteViewModel();
            builderViewModel = new BuilderViewModel();
            settingsViewModel = new SettingsViewModel();
            NavigateTo(RemoteViewModel);
            RemoteViewModelList = new ObservableCollection<RemoteViewModelBase>();
        }

        [RelayCommand]
        private void Navigate(object viewModel)
        {
            if(viewModel is ViewModelBase vm)
            {
                NavigateTo(vm);
            }
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            if (CurrentViewModel != viewModel) 
            { 
                CurrentViewModel = viewModel; 
                viewModel.IsSelected = true;
            }
        }

        [RelayCommand]
        private void RemoveRemoteViewModel(RemoteViewModelBase viewModel)
        {
            RemoteViewModelList.Remove(viewModel);
            viewModel.Dispose();
        }
    }
}
