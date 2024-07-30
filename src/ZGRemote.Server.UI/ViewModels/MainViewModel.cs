﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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

        public MainViewModel()
        {
            homeViewModel = new HomeViewModel();
            remoteViewModel = new RemoteViewModel();
            builderViewModel = new BuilderViewModel();
            settingsViewModel = new SettingsViewModel();
            currentViewModel = RemoteViewModel;
        }

        [RelayCommand]
        public void Navigate(object viewModel)
        {
            if(viewModel is ViewModelBase vm)
            {
                CurrentViewModel = vm;
            }
        }

        [RelayCommand]
        public void Test()
        {
            SettingsViewModel.IsSelected = true;
        }
    }
}
