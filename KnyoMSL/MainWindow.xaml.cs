using System;
using System.Windows;
using System.Management;
using Microsoft.Win32;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace KnyoMSL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists("KnyoConfig.json"))
            {
                runServer rs = new runServer();
                StreamReader reader = File.OpenText("KnyoConfig.json");
                JsonTextReader jsonTextReader = new JsonTextReader(reader);
                JObject jsonObject = (JObject)JToken.ReadFrom(jsonTextReader);

                try
                {
                    rs.ss.minM = int.Parse(jsonObject["minM"].ToString());
                    rs.ss.maxM = int.Parse(jsonObject["maxM"].ToString());
                    rs.ss.path = jsonObject["serverPath"].ToString();
                    rs.ss.serverName = jsonObject["name"].ToString();
                    rs.ss.javaPath = jsonObject["javaPath"].ToString();
                    rs.knyoM = bool.Parse(jsonObject["knyoM"].ToString());
                    rs.ss.otherArgs = jsonObject["otherArgs"].ToString(); 
                }
                catch
                {
                    MessageBox.Show("无法读取来自 Knyo 的配置文件", "Knyo - 错误");
                    Environment.Exit(0);
                }

                reader.Close();
                jsonTextReader.Close();
                rs.Show();
                this.Close();
            }

            diy_max.Maximum = int.Parse((GetPhisicalMemory() / 1024 / 1024).ToString());
            diy_min.Maximum = int.Parse((GetPhisicalMemory() / 1024 / 1024).ToString());
            diy_max.Value = int.Parse((GetPhisicalMemory() / 1024 / 1024).ToString());
            diy_min.Value = 0;
            diy_max.Minimum = 0;
            diy_min.Minimum = 0;
            m_knyo.IsChecked = true;

            hitokoto.Text = httpGet("https://v1.hitokoto.cn/?c=j&encode=text");

            choose_java.Items.Add("下载 AdoptOpenJDK 8 x64 Windows");
            choose_java.Items.Add("下载 AdoptOpenJDK 16 x64 Windows");
            choose_java.SelectedIndex = 0;

            java_system.IsChecked = true;
            choose_java.IsEnabled = false;
            java_diy_path.IsEnabled = false;
            java_download.IsEnabled = false;
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

        // P.S. 下列一个函数来自于 CSDN-郝伟博士

        private static long GetPhisicalMemory()
        {
            long capacity = 0;
            try
            {
                foreach (ManagementObject mo1 in new ManagementClass("Win32_PhysicalMemory").GetInstances())
                    capacity += long.Parse(mo1.Properties["Capacity"].Value.ToString());
            }
            catch (Exception ex)
            {
                capacity = -1;
                Console.WriteLine(ex.Message);
            }
            return capacity;
        }

        private void jar_choose_button_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Jar File (*.jar)|*.jar";
            if (fileDialog.ShowDialog() == true)
            {
                jar_path.Text = fileDialog.FileName;
            }
        }

        private void m_diy_checked(object sender, RoutedEventArgs e)
        {
            diy_min.IsEnabled = true;
            diy_max.IsEnabled = true;
        }

        private void m_knyo_checked(object sender, RoutedEventArgs e)
        {
            diy_min.IsEnabled = false;
            diy_max.IsEnabled = false;
        }

        private void start_server_panel_click(object sender, RoutedEventArgs e)
        {
            if (jar_path.Text != "" && server_name.Text != "")
            {
                runServer rs = new runServer();
                rs.ss.minM = int.Parse(diy_min.Value.ToString());
                rs.ss.maxM = int.Parse(diy_max.Value.ToString());
                rs.ss.otherArgs = other_args.Text;
                rs.ss.path = jar_path.Text;
                rs.ss.serverName = server_name.Text;
                if ((bool)java_diy.IsChecked)
                    rs.ss.javaPath = java_diy_path.Text;
                else if ((bool)java_system.IsChecked)
                    rs.ss.javaPath = "java";
                else
                    rs.ss.javaPath = "java";
                rs.knyoM = (bool)m_knyo.IsChecked;

                // JSON Write
                StringWriter sw = new StringWriter();
                JsonWriter writer = new JsonTextWriter(sw);
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                writer.WriteValue(server_name.Text);
                writer.WritePropertyName("serverPath");
                writer.WriteValue(jar_path.Text);
                writer.WritePropertyName("knyoM");
                writer.WriteValue(rs.knyoM);
                writer.WritePropertyName("minM");
                writer.WriteValue(rs.ss.minM);
                writer.WritePropertyName("maxM");
                writer.WriteValue(rs.ss.maxM);
                writer.WritePropertyName("javaPath");
                writer.WriteValue(rs.ss.javaPath);
                writer.WritePropertyName("otherArgs");
                writer.WriteValue(rs.ss.otherArgs);
                writer.WriteEndObject();
                writer.Flush();
                File.WriteAllText("KnyoConfig.json", sw.GetStringBuilder().ToString());
                writer.Close();
                sw.Close();

                rs.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("一个或多个值为空","Knyo - 警告");
            }
        }














        // Get Java
        private string getJavaName(string v)
        {
            JArray openApiResult = JArray.Parse(httpGet("https://api.adoptopenjdk.net/v2/info/releases/openjdk" + v));
            foreach (var item in openApiResult[0]["binaries"])
            {
                if (item["os"].ToString() == "windows" && item["architecture"].ToString() == "x64" && item["architecture"].ToString() == "x64" && item["binary_type"].ToString() == "jdk" && item["openjdk_impl"].ToString() == "hotspot")
                    return item["binary_name"].ToString();
            }
            return "none";
        }

 

        private void java_system_checked(object sender, RoutedEventArgs e)
        {
            java_download.IsEnabled = false;
            choose_java.IsEnabled = false;
            java_diy_path.IsEnabled = false;
        }

        private void java_diy_checked(object sender, RoutedEventArgs e)
        {
            java_download.IsEnabled = false;
            choose_java.IsEnabled = false;
            java_diy_path.IsEnabled = true;
        }

        private void java_choose_checked(object sender, RoutedEventArgs e)
        {
            java_download.IsEnabled = true;
            choose_java.IsEnabled = true;
            java_diy_path.IsEnabled = false;
        }

        private async void java_download_Click(object sender, RoutedEventArgs e)
        {
            int version = 16;
            if (choose_java.SelectedIndex == 0)
                version = 8;
            string fileName = getJavaName(version.ToString());
            var url = "https://mirrors.tuna.tsinghua.edu.cn/AdoptOpenJDK/" + version + "/jdk/x64/windows/" + fileName;
            using (var web = new WebClient())
            {
                await web.DownloadFileTaskAsync(url, fileName);
            }
            new FastZip().ExtractZip(System.Environment.CurrentDirectory + @"\" + fileName, System.Environment.CurrentDirectory + @"\Java", "");
            java_diy_path.Text =  Directory.GetDirectories(System.Environment.CurrentDirectory + "\\Java\\")[0] + "\\bin\\java.exe";
            
            java_download.IsEnabled = false;
            choose_java.IsEnabled = false;
            java_diy_path.IsEnabled = true;
            java_diy.IsChecked = true;
        }
    }
}
