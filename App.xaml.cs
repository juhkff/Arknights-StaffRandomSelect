using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StuffRandomSelect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private List<string> staffLists { set; get; }
        private string projectPath = Environment.CurrentDirectory.ToString();
        private string fileName = "StaffList.txt";
        private string path;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadList();
        }

        //打开程序时加载文件数据
        private void LoadList()
        {
            path = System.IO.Path.Combine(projectPath, fileName);
            if (staffLists != null)
            {
                return;
            }
            staffLists = new List<string>();
            //读取文件
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length <= 0)
                    {
                        continue;
                    }
                    
                }
            }
        }

        //处理数据
        //每行结构: 名称\t职业\t星级
        private void staffHandle()
        {

        }
    }
}
