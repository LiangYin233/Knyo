using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Net;

namespace KnyoMSL
{
    /// <summary>
    /// runServer.xaml 的交互逻辑
    /// </summary>
    public partial class runServer : Window
    {
        public startServer ss = new startServer();
        public bool knyoM = true;
        public Process p = new Process();
        DispatcherTimer timer = new DispatcherTimer();

        public runServer()
        {
            InitializeComponent();
            notice.Text = httpGet("https://cdn.imly.top/KnyoNotice");
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);   //设置刷新的间隔时间
        }

        static string httpGet(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Action methodDelegate = delegate ()
            {
                this.server_RAM.Text = "当前设备内存总占用: " + ((int)((ss.GetPhisicalMemory() - ss.GetAvailablePhysicalMemory()) / 1024 / 1024)).ToString() + " MB";

            };
            this.Dispatcher.BeginInvoke(methodDelegate);
        }


        private void start_server_click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (File.Exists("eula.txt"))
                    File.WriteAllText("eula.txt", "#By changing the setting below to TRUE you are indicating your agreement to our EULA (https://account.mojang.com/documents/minecraft_eula).\neula = true");
                else
                {
                    File.Create("eula.txt").Close();
                    File.WriteAllText("eula.txt", "#By changing the setting below to TRUE you are indicating your agreement to our EULA (https://account.mojang.com/documents/minecraft_eula).\neula = true");
                    
                }
            }
            catch
            {
                MessageBox.Show("EULA同意失败，您可能需要通过手动更改来启动服务器。", "Knyo - 错误");
            }

            this.Title = "Knyo - " + ss.serverName;
            
            if (knyoM)
            {
                ss.generateMknyo();
                ss.creatStartBat();
            }
            else
            {
                ss.creatStartBat();
            }

            p = new Process();
            startserver();
        }

        private void startserver()
        {
            start_server.Visibility = Visibility.Hidden;
            stop_server.Visibility = Visibility.Visible;
            var path = Environment.SystemDirectory;
            const string cmd = "Start.bat";
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = cmd;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            p.BeginOutputReadLine();
            p.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
            timer.Start();
        }

        private void ProcessOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            try
            {
                Action methodDelegate = delegate ()
                {
                    if (!String.IsNullOrEmpty(outLine.Data))
                    {
                        this.console_box.Text += outLine.Data + Environment.NewLine;
                        this.console_box.ScrollToEnd();
                    }
                  
                };
                this.Dispatcher.BeginInvoke(methodDelegate);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Knyo - 错误");
            }
        }
        private void ExecuteTclCommand(string tclCommand)
        {
            p.StandardInput.WriteLine(tclCommand);
            p.StandardInput.AutoFlush = true;
        }

        private void console_box_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (console_box_input.Text != null)
                {
                    if (console_box_input.Text == "stop")
                    {
                        start_server.Visibility = Visibility.Visible;
                        stop_server.Visibility = Visibility.Hidden;
                        timer.Stop();
                    }
                    ExecuteTclCommand(console_box_input.Text);
                    console_box_input.Text = "";
                }
            }
        }

        private void window_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (File.Exists("Start.bat"))
                File.Delete("Start.bat");
        }

        private void stop_server_click(object sender, RoutedEventArgs e)
        {
            start_server.Visibility = Visibility.Visible;
            stop_server.Visibility = Visibility.Hidden;
            timer.Stop();
            ExecuteTclCommand("stop");
        }

        private void properties_edit_click(object sender, RoutedEventArgs e)
        {
            serverEdit se = new serverEdit();
            se.Show();
        }
    }
}
