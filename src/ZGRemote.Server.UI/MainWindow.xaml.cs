using HandyControl.Interactivity;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MemuView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine();
        }

        private void MemuView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }

    public class MemuSelectChangeEventArgsToViewModelConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
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