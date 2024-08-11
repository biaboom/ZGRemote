using HandyControl.Interactivity;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZGRemote.Server.UI.Controls;
using ZGRemote.Server.UI.ViewModels;

namespace ZGRemote.Server.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        private MainViewModel mainViewModel;
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            mainViewModel = viewModel;
            DataContext = viewModel;
            mainViewModel.RemoteViewModelList.CollectionChanged += RemoteViewModels_CollectionChanged;
        }

        private void RemoteViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add) 
            { 
                foreach(ViewModelBase item in e.NewItems! )
                {
                    MemuViewItem memuItem = new MemuViewItem();
                    memuItem.HasClose = true;
                    memuItem.Header = item.Header;
                    memuItem.DataContext = item;

                    var binding = new Binding("IsSelected");
                    binding.Source = item;
                    memuItem.SetBinding(MemuViewItem.IsSelectedProperty, binding);

                    RemoteItem.Items.Add(memuItem);
                }
            }else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ViewModelBase viewModel in e.OldItems!)
                {
                    MemuViewItem? memuViewItem = null;
                    foreach (MemuViewItem item in RemoteItem.Items)
                    {
                        if (item.DataContext == viewModel)
                        {
                            memuViewItem = item;
                            break;
                        }
                    }
                    if (memuViewItem != null) RemoteItem.Items.Remove(memuViewItem);
                }
            }
        }

        private void RemoteItem_CloseButtonClick(object sender, RoutedEventArgs e)
        {

            if (((MemuViewItem)e.Source).DataContext is ViewModelBase viewModel)
            {
                mainViewModel.RemoveRemoteViewModelCommand.Execute(viewModel);
            }
        }

    }

    public class MemuSelectChangeEventArgsToViewModelConverter : IEventArgsConverter
    {
        public object? Convert(object value, object parameter)
        {
            if (value is RoutedPropertyChangedEventArgs<object> e)
            {
                if (e.NewValue is MemuViewItem item)
                {
                    return item.DataContext;
                }
            }
            return null;
        }
    }
}