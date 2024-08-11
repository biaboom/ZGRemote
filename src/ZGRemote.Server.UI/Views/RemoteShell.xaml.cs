using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZGRemote.Server.UI.Views
{
    /// <summary>
    /// RemoteShell.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteShell : UserControl
    {
        StreamWriter? sw;
        public RemoteShell()
        {
            InitializeComponent();
            // 使用ProcessStartInfo对象来配置进程
            DataContextChanged += DataContextChanged_;
            // 启动进程
            Task.Run(() =>
            {
                Process p = new Process();



                ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                info.UseShellExecute = false;

                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    // Prepend line numbers to each line of the output.
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        ConsoleOutputText(e.Data);
                    }
                });

                p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    // Prepend line numbers to each line of the output.
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        ConsoleOutputError(e.Data);
                    }
                });

                p.StartInfo = info;
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                sw = p.StandardInput;
                sw.AutoFlush = true;
                sw.WriteLine();



                
                
            });
        }

        private void DataContextChanged_(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void ConsoleOutputText(string text)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Run run = new Run()
                {
                    Text = $"{text}\n"
                };
                ConsoleBox.Inlines.Add(run);
                RichTextBox_.ScrollToEnd();
            });
            
        }

        private void ConsoleOutputError(string text)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Run run = new Run()
                {
                    Text = $"{text}\n",
                    Foreground = new SolidColorBrush(Colors.Red)
                   
                };
                ConsoleBox.Inlines.Add(run);
                RichTextBox_.ScrollToEnd();
            });
           
        }

        private void CommandBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sw?.WriteLine(CommandBox.Text);
                CommandBox.Clear();
            }
        }
    }
}
