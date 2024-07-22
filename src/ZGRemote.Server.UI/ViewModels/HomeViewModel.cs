using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGRemote.Server.UI.ViewModels
{
    partial class HomeViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string header = "Home";
    }
}
