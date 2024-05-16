using PracticProject3.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace PracticProject3.Cores
{
    static public class XMLCode
    {
        static public void CreateXMLDataPack(List<DataPack> data_obj)
        {
            XDocument xdoc = new XDocument();
            XElement main = new XElement("Datapacks");
            for (int i = 0; i < data_obj.Count; i++)
            {
                XElement data_pack = new XElement("Datapack");
                XAttribute data_pack_name = new XAttribute("name", data_obj[i].DataPackName);
                data_pack.Add(data_pack_name);
                for (int j = 1; j < data_obj[i].Data.Count; j++)
                {
                    XElement data_line = new XElement("Dataline");
                    for (int k = 0; k < data_obj[i].Data[j].line.Count; k++)
                    {
                        try
                        {
                            XElement data_cell = new XElement(data_obj[i].Data[0].line[k], data_obj[i].Data[j].line[k]);
                            data_line.Add(data_cell);
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка с ячейкой");
                        }
                    }
                    data_pack.Add(data_line);
                }
                main.Add(data_pack);
            }
            xdoc.Add(main);
            xdoc.Save("datapack.xml");
        }
        
        static public void AddConfig(string ServerName)
        {
            XDocument xdoc;
            if (File.Exists("config.xml"))
            {
                xdoc = XDocument.Load("config.xml");
                XElement main = xdoc.Root;
                XElement config_ServerList = main.Element("ServerList");
                foreach (XElement obj in config_ServerList.Elements()) 
                {
                    if (obj.Value == ServerName) 
                    {
                        return;
                    }
                }
                XElement config_server = new XElement("Server", ServerName);
                config_ServerList.Add(config_server);
                xdoc.Save("config.xml");
            }
            else 
            { 
                xdoc = new XDocument();
                XElement main = new XElement("Config");
                XElement config_ServerList = new XElement("ServerList");
                XElement config_server = new XElement("Server", ServerName);
                XElement config_profile = new XElement("Profile");
                config_profile.Add(new XElement("User", "Admin"));
                config_profile.Add(new XElement("Password", "1553"));
                config_ServerList.Add(config_server);
                main.Add(new XElement("DatabaseName", "DecryptosBase"));
                main.Add(config_ServerList);
                main.Add(config_profile);
                xdoc.Add(main);
                xdoc.Save("config.xml");
            }
        }
        static public List<string> ReadConfigServers()
        {
            List<string> list = new List<string>();
            if (File.Exists("config.xml"))
            {
                XDocument xdoc = XDocument.Load("config.xml");
                XElement main = xdoc.Root;
                XElement config_ServerList = main.Element("ServerList");
                foreach (XElement obj in config_ServerList.Elements())
                {
                    list.Add(obj.Value);
                }
            }
            return list;
        }
        static public string ReadConfigDbName()
        {
            if (File.Exists("config.xml"))
            {
                XDocument xdoc = XDocument.Load("config.xml");
                XElement main = xdoc.Root;
                return main.Element("DatabaseName").Value;
            }
            return "DecryptosBase";
        }
        static public string[] ReadConfigProfile() 
        {
            string[] list = {"" ,""};
            if (File.Exists("config.xml")) 
            {
                XDocument xdoc = XDocument.Load("config.xml");
                XElement main = xdoc.Root;
                XElement Profile = main.Element("Profile");
                list[0] = Profile.Element("User").Value;
                list[1] = Profile.Element("Password").Value;
            }
            return list;
        }

        static public string GetSettingsConfig()
        {
            XDocument xdoc;
            try
            {
                xdoc = XDocument.Load("config.xml");
                XElement main = xdoc.Root;
                XElement config_Server = main.Element("Server");
                XElement config_Db = main.Element("DataBase");
                XElement config_etc = main.Element("Add");
                return $"Server={config_Server.Value};Database={config_Db.Value};" + config_etc.Value;
            }
            catch
            {
                xdoc = new XDocument();
                XElement main = new XElement("Config");
                XElement config_Server = new XElement("Server", "TestServer");
                XElement config_Db = new XElement("DataBase", "TestDb");
                XElement config_etc = new XElement("Add", "Etc;");
                main.Add(config_Server);
                main.Add(config_Db);
                main.Add(config_etc);
                xdoc.Add(main);
                xdoc.Save("config.xml");
                return "Server=TestServer;Database=TestDb;Trusted_Connection=True;Encrypt=false;";
            }
        }
    }
}
