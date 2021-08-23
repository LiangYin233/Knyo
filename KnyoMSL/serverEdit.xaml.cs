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

namespace KnyoMSL
{
    /// <summary>
    /// serverEdit.xaml 的交互逻辑
    /// </summary>
    public partial class serverEdit : Window
    {
        public serverEdit()
        {
            InitializeComponent();
            if(File.Exists("server.properties"))
            {
                string config = File.ReadAllText("server.properties");
                config = config.Replace("true","开启");
                config = config.Replace("false", "关闭");
                config = config.Replace("max-build-height", "最高建筑高度");
                config = config.Replace("level-seed", "地图种子");
                config = config.Replace("allow-nether", "地狱传送门是否传送玩家");
                config = config.Replace("enable-command-block", "命令方块启用性");
                config = config.Replace("server-port", "服务器端口号");
                config = config.Replace("gamemode", "游戏默认模式");
                config = config.Replace("op-permission-level", "OP权限等级");
                config = config.Replace("enable-query", "允许使用GameSpy4协议的服务器监听器");
                config = config.Replace("level-name", "世界名称");
                config = config.Replace("force-gamemode", "强制玩家加入模式为默认游戏模式");
                config = config.Replace("white-list", "启用白名单");
                config = config.Replace("pvp", "PVP启用性");
                config = config.Replace("spawn-npcs", "是否生成村民");
                config = config.Replace("spawn-animals", "是否生成动物");
                config = config.Replace("level-name", "世界名称");
                config = config.Replace("difficulty", "难度");
                config = config.Replace("level-type", "世界生成类型");
                config = config.Replace("spawn-monsters", "是否生成怪物");
                config = config.Replace("max-players", "最大玩家数量");
                config = config.Replace("online-mode", "正版验证启用性");
                config = config.Replace("allow-flight", "启用飞行");
                config = config.Replace("max-world-size", "世界大小");
                edit.Text = config;
            }
            else
            {
                MessageBox.Show("服务器配置文件还未创建！", "Knyo - 警告");
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string config = edit.Text;
            config = config.Replace("开启","true");
            config = config.Replace("关闭","false");
            config = config.Replace("最高建筑高度","max-build-height");
            config = config.Replace("地图种子","level-seed");
            config = config.Replace("地狱传送门是否传送玩家","allow-nether");
            config = config.Replace("命令方块启用性","enable-command-block");
            config = config.Replace("服务器端口号","server-port");
            config = config.Replace("游戏默认模式","gamemode");
            config = config.Replace("OP权限等级","op-permission-level");
            config = config.Replace("允许使用GameSpy4协议的服务器监听器","enable-query");
            config = config.Replace("世界名称","level-name");
            config = config.Replace("强制玩家加入模式为默认游戏模式","force-gamemode");
            config = config.Replace("启用白名单","white-list");
            config = config.Replace("PVP启用性","pvp");
            config = config.Replace("是否生成村民","spawn-npcs");
            config = config.Replace("是否生成动物","spawn-animals");
            config = config.Replace("世界名称","level-name");
            config = config.Replace("难度","difficulty");
            config = config.Replace("世界生成类型","level-type");
            config = config.Replace("是否生成怪物","spawn-monsters");
            config = config.Replace("最大玩家数量","max-players");
            config = config.Replace("正版验证启用性","online-mode") ;
            config = config.Replace("启用飞行","allow-flight");
            config = config.Replace("世界大小","max-world-size");
            
            File.WriteAllText(System.Environment.CurrentDirectory + "\\server.properties", config);
            
            MessageBox.Show("写入完成！可能需要重启服务器。", "Knyo - 提示");
        }
    }
}
