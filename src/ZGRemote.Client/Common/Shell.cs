using System;
using System.Diagnostics;
using System.IO;

namespace ZGRemote.Client.Common
{
    public class Shell : IDisposable
    {
        private Process process;

        private StreamWriter sw;

        private bool disposedValue;

        public event Action<string> OutputError;

        public event Action<string> OutputText;

        public Shell()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
            };

            process = new Process();
            process.StartInfo = processStartInfo;

            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                // Prepend line numbers to each line of the output.
                if (!String.IsNullOrEmpty(e.Data))
                {
                    OutputText?.Invoke(e.Data);
                }
            });

            process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                // Prepend line numbers to each line of the output.
                if (!String.IsNullOrEmpty(e.Data))
                {
                    OutputError?.Invoke(e.Data);
                }
            });
        }

        public void Start()
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            sw = process.StandardInput;
            sw.AutoFlush = true;
            sw.WriteLine();
        }

        public void RunCmd(string cmd)
        {
            sw.WriteLine(cmd);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OutputError = null;
                    OutputText = null;
                }
                if (process == null) return;
                if(!process.HasExited) process.Kill();
                sw.Dispose();
                process?.Close();
                process?.Dispose();
                process = null;
                disposedValue = true;
            }
        }

        ~Shell()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
