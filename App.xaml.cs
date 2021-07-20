using StaffRandomSelect;
using System;
using System.Collections.Generic;
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
        public static List<Staff> staffLists;

        private string projectPath = Environment.CurrentDirectory.ToString();
        private string fileName = "StaffList.xml";
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
            //读取文件
            ListRead();
        }

        //处理数据
        //每行结构: 名称\t职业\t星级
        private void ListRead()
        {
            //if (staffLists != null)
            //{
            //    return;
            //}
            staffLists = new List<Staff>();
            XElement xElement = XElement.Load(path);
            IEnumerable<XElement> careerList = from elements in xElement.Elements("staffList")
                                               select elements;
            foreach(XElement career in careerList)
            {
                IEnumerable<XElement> staffList = from element in career.Elements("career")
                                                  select element;
                foreach(XElement each in staffList)
                {
                    //目前只考虑罗列人员列表
                    //name  star
                    Staff staff = new Staff();
                    staff.Name = each.Element("name").Value;
                    staff.Star = int.Parse(each.Element("star").Value);
                    //staff.Career = career.Attribute("type");
                    staff.Career = (Career)System.Enum.Parse(typeof(Career), career.Attribute("type").Value);
                    staffLists.Add(staff);
                }
            }
        }
    }
}
