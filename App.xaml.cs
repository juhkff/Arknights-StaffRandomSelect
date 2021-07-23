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
using System.Xml;
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
            if (!File.Exists(path))
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlDeclaration xmldec = xmldoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                xmldoc.AppendChild(xmldec);

                //添加根节点
                XmlElement rootElement = xmldoc.CreateElement("staffList");
                rootElement.InnerText = "";
                xmldoc.AppendChild(rootElement);
                xmldoc.Save(path);
            }
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
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            XmlNode root = xmlDocument.SelectSingleNode("staffList");
            root.RemoveAll();
            //删除除staffList外的所有节点后得到职业分组
            IEnumerable<StaffGroupByCareer> GroupList = staffLists.GroupBy(x => x.Career).Select(x => new StaffGroupByCareer { Career = x.Key, StaffList = x.ToList() });
            foreach (StaffGroupByCareer staffsWithCareer in GroupList)
            {
                XmlElement careerElement = xmlDocument.CreateElement("career");
                careerElement.SetAttribute("type", staffsWithCareer.Career.ToString());
                foreach (Staff staff in staffsWithCareer.StaffList)
                {
                    AddStaff(xmlDocument, careerElement, staff);
                }
                root.AppendChild(careerElement);
            }
            xmlDocument.Save(path);
        }

        private void AddStaff(XmlDocument file,XmlElement parentNode,Staff staff)
        {
            /*<staff>
                <name>风笛</name>
                <star>6</star>
                <selected>1</selected>
            </staff>*/
            XmlElement staffElement = file.CreateElement("staff");
            XmlElement nameElement = file.CreateElement("name");
            XmlElement starElement = file.CreateElement("star");
            XmlElement selectedElement = file.CreateElement("selected");
            nameElement.InnerText = staff.Name.ToString();
            starElement.InnerText = staff.Star.ToString();
            selectedElement.InnerText = Convert.ToInt32(staff.IsSelected).ToString();
            staffElement.AppendChild(nameElement);
            staffElement.AppendChild(starElement);
            staffElement.AppendChild(selectedElement);
            parentNode.AppendChild(staffElement);
        }

        //生成名字集合
        public static HashSet<string> GetNameSet()
        {
            return staffLists.Select(staff => staff.Name).ToHashSet();   //获得名字集合
        }
    }
}
