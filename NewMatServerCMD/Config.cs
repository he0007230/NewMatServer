using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using LogHelper;
using System.Configuration;
using NewMatServerCMD.Model;
using System.Xml;
using NewMatServerCMD.DB;
using NewMatServerCMD.tool;

namespace NewMatServerCMD
{
    public class Config
    {


        private static Config _instance;
        public SysInfo sysInfo;
        public Dictionary<int, RedirectModel> redirectDict;
        public delegate void actionByMain(CompactFormatter.TransDTO transDTO, RedirectModel redirecgtModel, out object message);
        public Dictionary<string, actionByMain> mainList;
        public string outStr;
        public string[] appName;
        public string[] appVersion;
        public string[] appPath;
        public int timeoutCount;


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
                //using (FileStream fs = new FileStream("D:\\MatServer\\CONFIG.XML", FileMode.Open))
                //{
                    //XmlSerializer formatter = new XmlSerializer(typeof(XmlData));
                    //xmlData = (XmlData)formatter.Deserialize(fs);
                //}
                timeoutCount = 0;
                XmlDocument xmlDoc1 = new XmlDocument();
                xmlDoc1.Load("D:\\MatServer\\CONFIG.XML");
                sysInfo = new SysInfo();
                sysInfo.name = xmlDoc1.SelectSingleNode("Root/System/name").InnerText;
                sysInfo.version = xmlDoc1.SelectSingleNode("Root/System/version").InnerText;
                sysInfo.wince_path = xmlDoc1.SelectSingleNode("Root/System/wince_path").InnerText;
                sysInfo.pc_path = xmlDoc1.SelectSingleNode("Root/System/pc_path").InnerText;
                sysInfo.stock_name = xmlDoc1.SelectSingleNode("Root/System/stock_name").InnerText;
                sysInfo.stock_no = xmlDoc1.SelectSingleNode("Root/System/stock_no").InnerText;
                sysInfo.server_ip = xmlDoc1.SelectSingleNode("Root/System/server_ip").InnerText;
                sysInfo.server_port = int.Parse(xmlDoc1.SelectSingleNode("Root/System/server_port").InnerText);
                sysInfo.maxSessionTimeout = int.Parse(xmlDoc1.SelectSingleNode("Root/System/maxSessionTimeout").InnerText);
                sysInfo.remoteServer = xmlDoc1.SelectSingleNode("Root/System/remoteServer").InnerText;
                sysInfo.maxListener = int.Parse(xmlDoc1.SelectSingleNode("Root/System/maxListener").InnerText);
                sysInfo.printName = xmlDoc1.SelectSingleNode("Root/System/printName").InnerText;
                sysInfo.ttspaServer = xmlDoc1.SelectSingleNode("Root/System/ttspaServer").InnerText;
                if (xmlDoc1.SelectSingleNode("Root/System/recordCodeStr").InnerText == "Y")
                {
                    sysInfo.recordCodeStr = true;
                }
                else
                {
                    sysInfo.recordCodeStr = false;
                }
                if (xmlDoc1.SelectSingleNode("Root/System/recordOutStr").InnerText == "Y")
                {
                    sysInfo.recordOutStr = true;
                }
                else
                {
                    sysInfo.recordOutStr = false;
                }
                if (xmlDoc1.SelectSingleNode("Root/System/recordTestStr").InnerText == "Y")
                {
                    sysInfo.recordTestStr = true;
                }
                else
                {
                    sysInfo.recordTestStr = false;
                }
                XmlNode xn1 = xmlDoc1.SelectSingleNode("Root/Applications");
                XmlNodeList xnl1 = xn1.ChildNodes;
                appName = new String[xnl1.Count];
                appPath = new String[xnl1.Count];
                appVersion = new String[xnl1.Count];
                int i = 0;
                foreach (XmlNode cn1 in xnl1)
                {
                    appName[i] = cn1.SelectSingleNode("name").InnerText;
                    appPath[i] = cn1.SelectSingleNode("pc_path").InnerText;
                    appVersion[i] = cn1.SelectSingleNode("version").InnerText;
                    i++;
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
                        LoggerHelper.Info("重复加载处理模块 ID:" + redirectModel.id + " Name:" + redirectModel.redirectName, false, false);
                    }
                    else
                    {
                        redirectDict.Add(redirectModel.id, redirectModel);
                        LoggerHelper.Info("载入处理模块 ID:" + redirectModel.id + " Name:" + redirectModel.redirectName, false, false);
                    }
                }

                mainList = new Dictionary<string, actionByMain>();
                mainList.Add("nlscan_pkg", nlscan_pkg);
                mainList.Add("UpdateClientConfig", UpdateClientConfig);
                mainList.Add("CheckVersion", CheckVersion);
                mainList.Add("UpdateApplication",UpdateApplication);
                mainList.Add("Test", Test);

                LoggerHelper.Info("读取配置成功!");
            }
            catch(Exception e)
            {
                LoggerHelper.Info("读取配置失败!  "+e.Message);
            }
        }

        private void nlscan_pkg(CompactFormatter.TransDTO transDTO, RedirectModel redirectModel, out object message)
        {
            message = OracleController.NlscanPkg(redirectModel.connStr, redirectModel.pkgName, redirectModel.pFlag
                , transDTO.CodeStr, transDTO.IP, transDTO.AppName, transDTO.StockNo, redirectModel.isCache);
            if (redirectModel.command % 2 == 0)
            {
                //专柜数据
                message = OracleController.GetSpaData(message.ToString(), ConfigurationManager.ConnectionStrings["ttspaConnStr"].ToString());
            }
            else if (redirectModel.command % 3 == 0)
            {
                if (message.ToString().Contains("回执单号"))
                {
                    int i = message.ToString().IndexOf("回执单号")+4;
                    //message.ToString().Substring(i,13)
                    LoggerHelper.Info("打印司机送货回执:" + message.ToString().Substring(i, 13));
                    string sqlStr = "select receipt_no,trip_no,stock_name,worker_name from ttmat_trip_receipt where receipt_no='";
                    sqlStr += message.ToString().Substring(i, 13) + "'";
                    PrintHelper.GetInstance().PrintReport(ConfigurationManager.ConnectionStrings["ttmatLocalConnStr"].ToString()
                        , sqlStr, "query1", "D://MatServer//ttmat_receipt.frx", Config.Instance.sysInfo.printName);
                }
                //打印司机送货回执
                //PrintHelper.GetInstance().PrintReport(
            }
            if (sysInfo.recordCodeStr)
            {
                LoggerHelper.Info("[" + transDTO.IP + "]["+redirectModel.redirectName+"][" + transDTO.CodeStr + "][" + transDTO.Remark + "]", false, false);
            }
            if (sysInfo.recordOutStr)
            {
                LoggerHelper.Info("[" + transDTO.IP + "]["+redirectModel.redirectName+"结果][" + message.ToString() + "]", false, false);
            }

        }
        private void UpdateClientConfig(CompactFormatter.TransDTO transDTO, RedirectModel redirectMode, out object message)
        {
            LoggerHelper.Info("[" + transDTO.IP + "][更新配置文件]");
            FileInfo sendFile = new FileInfo(sysInfo.pc_path);
            FileStream sendStream = sendFile.OpenRead();
            byte[] byte_data = new byte[sendStream.Length];
            sendStream.Read(byte_data, 0, byte_data.Length);
            message = byte_data;
        }
        private void CheckVersion(CompactFormatter.TransDTO transDTO, RedirectModel redirectModel, out object message)
        {
            //LoggerHelper.Info(transDTO.IP + "  --CheckVersion");

            string[] data = transDTO.CodeStr.Split('#');
            string tmpstr="";
            int id = 0;
            if (data.Length == 2)
            {
                if (int.TryParse(data[0], out id))
                {
                    if (appVersion[id] == data[1])
                    {
                        message = "NEW";
                    }
                    else
                    {
                        message = "UPDATE#"+appVersion[id];
                    }
                    tmpstr = appName[id] + "->" + message.ToString();
                }
                else
                {
                    message = "ERROR";
                    tmpstr = "PARSE->" + message.ToString();
                }
                
            }
            else
            {
                message = "ERROR";
                tmpstr = "NULL->" + message.ToString();
            }
            LoggerHelper.Info("[" + transDTO.IP + "][检查程序版本]" + tmpstr);
        }
        private void UpdateApplication(CompactFormatter.TransDTO transDTO, RedirectModel redirectModel, out object message)
        {

            int id = 0;
            if (int.TryParse(transDTO.CodeStr, out id))
            {
                FileInfo sendFile = new FileInfo(appPath[id]);
                FileStream sendStream = sendFile.OpenRead();
                byte[] byte_data = new byte[sendStream.Length];
                sendStream.Read(byte_data, 0, byte_data.Length);
                message = byte_data;
                LoggerHelper.Info("[" + transDTO.IP + "][更新程序->" + appName[id] + "][" + byte_data.Length + "]");
            }
            else
            {
                LoggerHelper.Info("[" + transDTO.IP + "][更新程序失败]");
                message = null;
            }
        }
        private void Test(CompactFormatter.TransDTO transDTO, RedirectModel redirectModel, out object message)
        {
            if (sysInfo.recordTestStr)
            {
                LoggerHelper.Info("[" + transDTO.IP + "][测试数据][" + transDTO.CodeStr + "]", false, false);
            }
            message = "test";
        }

    }
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
        public int maxSessionTimeout { get; set; }
        public string remoteServer { get; set; }
        public int maxListener { get; set; }
        public string printName { get; set; }
        public string ttspaServer { get; set; }
        public bool recordCodeStr { get; set; }
        public bool recordOutStr { get; set; }
        public bool recordTestStr { get; set; }
    }
}
