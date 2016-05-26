using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using LogHelper;
using System.Configuration;
using NewMatServerForm.Model;
using System.Xml;
using NewMatServerForm.DB;
using transprot.dto;

namespace NewMatServerForm
{
    public class Config
    {


        private static Config _instance;
        public XmlData xmlData;
        public Dictionary<int, RedirectModel> redirectDict;
        public delegate void actionByMain(TransDTO transDTO, RedirectModel redirecgtModel);
        public Dictionary<string, actionByMain> mainList;
        public string outStr;

        private Config() 
        {
            LoadProfile();
        }
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config();
                }
                return _instance;
            }
 
        }
        private void LoadProfile()
        {
            try
            {
                using (FileStream fs = new FileStream("D:\\MatServer\\CONFIG.XML", FileMode.Open))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(XmlData));
                    xmlData = (XmlData)formatter.Deserialize(fs);
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("D:\\MatServer\\REDIRECT.XML");
                XmlNode xn = xmlDoc.SelectSingleNode("Root");
                XmlNodeList xnl = xn.ChildNodes;
                redirectDict = new Dictionary<int, RedirectModel>();
                foreach (XmlNode cn in xnl)
                {
                    RedirectModel redirectModel = new RedirectModel();
                    XmlElement xe = (XmlElement)cn;
                    redirectModel.id = int.Parse(xe.GetAttribute("ID").ToString());
                    redirectModel.redirectName = xe.GetAttribute("Name").ToString();
                    redirectModel.funcName = xe.SelectSingleNode("funcName").InnerText;
                    redirectModel.connStr = ConfigurationManager.ConnectionStrings[xe.SelectSingleNode("connStr").InnerText].ToString();
                    redirectModel.pkgName = ConfigurationManager.AppSettings[xe.SelectSingleNode("pkgName").InnerText];
                    redirectModel.pFlag = int.Parse(xe.SelectSingleNode("pflag").InnerText);
                    redirectModel.isCache = xe.SelectSingleNode("isCache").InnerText;
                    redirectModel.area = int.Parse(xe.SelectSingleNode("area").InnerText);
                    redirectModel.type = int.Parse(xe.SelectSingleNode("type").InnerText);
                    redirectModel.command = int.Parse(xe.SelectSingleNode("command").InnerText);
                    if (redirectDict.ContainsKey(redirectModel.id))
                    {
                        LoggerHelper.Info("重复加载处理模块 ID:"+redirectModel.id+" Name:"+redirectModel.redirectName);
                    }
                    else
                    {
                        redirectDict.Add(redirectModel.id, redirectModel);
                        LoggerHelper.Info("载入处理模块 ID:" + redirectModel.id + " Name:" + redirectModel.redirectName);
                    }
                }

                mainList = new Dictionary<string, actionByMain>();
                mainList.Add("nlscan_pkg", nlscan_pkg);
                mainList.Add("UpdateClientConfig", UpdateClientConfig);
                mainList.Add("CheckVersion", CheckVersion);


                LoggerHelper.Info("读取配置成功!");
            }
            catch(Exception e)
            {
                LoggerHelper.Info("读取配置失败!  "+e.Message);
            }
        }

        private void nlscan_pkg(TransDTO transDTO, RedirectModel redirectModel)
        {
            outStr = OracleController.NlscanPkg(redirectModel.connStr, redirectModel.pkgName, redirectModel.pFlag
                , transDTO.codestr, transDTO.ip, transDTO.appName, transDTO.stockNo, redirectModel.isCache);
            LoggerHelper.Info(transDTO.ip+"  --nlscan_pkg:"+outStr);
        }
        private void UpdateClientConfig(TransDTO transDTO, RedirectModel redirectModel)
        {
            outStr = "UpdateClientConfig";
            LoggerHelper.Info(transDTO.ip+"  --UpdateClientConfig");
        }
        private void CheckVersion(TransDTO transDTO, RedirectModel redirectModel)
        {
            outStr = "CheckVersion";
            LoggerHelper.Info(transDTO.ip+"  --CheckVersion");
        }

    }

    [Serializable]
    public class AppInfo
    {
        public string name { get; set; }
        public string version { get; set; }
        public string wince_path { get; set; }
        public string pc_path { get; set; }
    }
    [Serializable]
    public class MatClientInfo : AppInfo 
    {
        public string reload_interval { get; set; }
    }

    [Serializable]
    public class GoodsHandleInfo : AppInfo { }

    [Serializable]
    public class TurnoverClientInfo : AppInfo { }

    [Serializable]
    public class AppsInfo
    {
        public MatClientInfo MatClient { get; set; }
        public GoodsHandleInfo GoodsHandle { get; set; }
        public TurnoverClientInfo TurnoverClient { get; set; }
    }

    [Serializable]
    public class SysInfo
    {
        public string name { get; set; }
        public string version { get; set; }
        public string wince_path { get; set; }
        public string pc_path { get; set; }
        public string stock_name { get; set; }
        public string stock_no { get; set; }
        public string server_ip { get; set; }
        public int server_port { get; set; }
        public string maxSessionTimeout { get; set; }
        public string remoteServer { get; set; }
        public int maxListener { get; set; }
    }


    [Serializable]
    [XmlRoot("Root")]
    public class XmlData
    {
        public AppsInfo Applications { get; set; }
        public SysInfo System { get; set; }
    }
}
