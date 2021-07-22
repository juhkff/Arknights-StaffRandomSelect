using StaffRandomSelect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace StaffRandomSelect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ObservableCollection<Staff> staffLists;

        private static string projectPath = Environment.CurrentDirectory.ToString();
        private static string fileName = "StaffList.xml";
        private static string path;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadList();
        }

        //打开程序时加载文件数据
        private void LoadList()
        {
            path = System.IO.Path.Combine(projectPath, fileName);
            ListRead();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //退出时写入文件
            ListWrite();
            base.OnExit(e);
        }



        //处理数据
        private void ListRead(/*List<Staff> StaffLists*/)
        {
            staffLists = new ObservableCollection<Staff>();
            //if (staffLists != null)
            //{
            //    return;
            //}
            //StaffLists = new List<Staff>();
            XDocument xDocument = XDocument.Load(path);
            IEnumerable<XElement> staffList = xDocument.Elements("staffList");
            foreach(XElement item in staffList)
            {
                IEnumerable<XElement> careerList = staffList.Elements("career");
                foreach(XElement career in careerList)
                {
                    IEnumerable<XElement> staffs = career.Elements("staff");
                    foreach(XElement each in staffs)
                    {
                        Staff staff = new Staff();
                        staff.Name = each.Element("name").Value;
                        staff.Star = int.Parse(each.Element("star").Value);
                        //staff.Career = career.Attribute("type");
                        staff.Career = (Career)System.Enum.Parse(typeof(Career), career.Attribute("type").Value);
                        staff.IsSelected = Convert.ToBoolean(int.Parse(each.Element("selected").Value));
                        staffLists.Add(staff);
                    }
                }
            }
        }

        private void ListWrite()
        {
            XDocument xDocument = XDocument.Load(path);
            //xDocument.RemoveNodes();
            
            //重新写入节点
            // Code
            xDocument.Save(new FileStream(path, FileMode.Create));
        }

        //生成名字集合
        public static HashSet<string> GetNameSet()
        {
            return staffLists.Select(staff => staff.Name).ToHashSet();   //获得名字集合
        }
    }
}
