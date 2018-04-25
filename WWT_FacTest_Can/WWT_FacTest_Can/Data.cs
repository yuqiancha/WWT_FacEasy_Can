using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace WWT_FacTest
{
    class Data
    {
        public static SqLiteHelper sql;

        public static String UniqueCode = null;
        public static String ReturnStr = null;

        public static bool AlarmStatus = true;//蜂鸣器
        public static int UltrasonicStatus = 3;//超声测试，挡板到位 0:正常 1:超声探头1异常 2:超声探头2异常 3:探头12都异常
        public static bool MagneticStatus = true;//地磁测试，车子到位
        public static void SaveConfig(string Path,string key ,string value)
        {
            XDocument xDoc = XDocument.Load(Path);
            XmlReader reader = xDoc.CreateReader();

            bool Matched = false;//是否已在XML中

            foreach (var p in xDoc.Root.Elements("add"))
            {
                if (p.Attribute("key").Value == key)
                {
                    p.Attribute("value").Value = value;
                    Matched = true;
                }
            }
            if(Matched == false)
            {
                XElement element = new XElement("add", new XAttribute("key", key), new XAttribute("value", value));
                xDoc.Root.Add(element);
            }
            xDoc.Save(Path);
        }

        public static string GetConfig(string Path,string key)
        {
            XDocument xDoc = XDocument.Load(Path);
            XmlReader reader = xDoc.CreateReader();
            string value = "Error";

            var query = from p in xDoc.Root.Elements("add")
                        where p.Attribute("key").Value == key
                        select p.Attribute("value").Value;

            foreach(string s in query)
            {
                value = s;
            }

            //foreach (var p in xDoc.Root.Elements("add"))
            //{
            //    if (p.Attribute("key").Value == key)
            //    {
            //        value = p.Attribute("value").Value;
            //    }
            //}
            return value;

        }

        public static string GetConfigStr(string Path, string key, string name)
        {
            XDocument xDoc = XDocument.Load(Path);
            XmlReader reader = xDoc.CreateReader();
            string value = "Error";
            var query = from p in xDoc.Root.Elements("add")
                        where p.Attribute("key").Value == key
                        select p.Attribute(name).Value;

            foreach (string s in query)
            {
                value = s;
            }

            return value;
        }

        public static void SaveConfigStr(string Path, string key, string name ,string value)
        {
            XDocument xDoc = XDocument.Load(Path);
            XmlReader reader = xDoc.CreateReader();

            bool Matched = false;//是否已在XML中

            foreach (var p in xDoc.Root.Elements("add"))
            {
                if (p.Attribute("key").Value == key)
                {
                    p.Attribute(name).Value = value;
                    Matched = true;
                }
            }
            if (Matched == false)
            {
                XElement element = new XElement("add", new XAttribute("key", key), new XAttribute("name", value));
                xDoc.Root.Add(element);
            }

            xDoc.Save(Path);
        }
    }
}
