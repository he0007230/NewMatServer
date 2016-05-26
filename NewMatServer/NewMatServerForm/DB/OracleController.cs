using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Data;
using System.Net.NetworkInformation;
using System.Configuration;

namespace NewMatServerForm.DB
{
    class OracleController
    {
        public static string data_cache(string pkg_name, int pflag, string codeStr, string stockNo, string applicationName)
        {
            OracleCommand cmd;
            string outstr;
            string codestrTmp;
            OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ttmatLocalConnStr"].ToString());
            try
            {
                conn.Open();
            }
            catch(Exception er)
            {
                outstr = "数据库连接失败(data_cache):"+er.Message;
                //base.SendDatagram(outstr);
                return  outstr;
            }
            //数据缓存处理
            //连接到本地ttmat用户进行处理
            cmd = new OracleCommand(pkg_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            OracleParameter p1 = new OracleParameter("pflag", OracleType.Int16, 2);
            p1.Direction = ParameterDirection.Input;
            OracleParameter p2 = new OracleParameter("outstr", OracleType.VarChar, 200);
            p2.Direction = ParameterDirection.Output;
            OracleParameter p3 = new OracleParameter("codestr", OracleType.VarChar, 1000);
            p3.Direction = ParameterDirection.Input;
            OracleParameter p4 = new OracleParameter("return_value", OracleType.Int16, 2);
            p4.Direction = ParameterDirection.ReturnValue;

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            try
            {
                codestrTmp = pflag + "|" + codeStr.Replace('|', '$') + "|" + stockNo + "|" + applicationName + "|";
                cmd.Parameters["pflag"].Value = 9;
                cmd.Parameters["codestr"].Value = codestrTmp;
                cmd.ExecuteNonQuery();
                //outstr = m_command.Parameters["outstr"].Value.ToString();
                outstr = cmd.Parameters["outstr"].Value.ToString();

            }
            catch (Exception err)
            {
                outstr = "数据库打开失败(data_cache):"+err.Message;

            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            //MessageBox.Show(outstr);
            //base.SendDatagram(outstr);
            return outstr;
        }

        public static string NlscanPkg(string connStr, string pkg_name, int pflag, string codestr, string clientName, string appliacationName, string stockNo,string isCache)
        {

            //private string _ttmatConnStr = "Data Source=ttdata;user id=ttmat;password=ttmat789";
            //private string _ttshopConnStr = "Data Source=orttshop;user id=ttshop;password=ttadmin456";
            //private string _ttmatPkgName = "TTMAT_PKG.ttmat_nlscan";
            //private string _ttshopNLScanPkg = "TTSHOP_NLSACN_PKG.check_production_date";
            OracleCommand cmd;
            string outstr;
            //string connStr = _ttmatConnStr;
            if ((!PingIpOrDomainName(Config.Instance.xmlData.System.remoteServer)) && (isCache == "Y"))
            {
                data_cache(pkg_name, pflag, codestr, stockNo, appliacationName);
                return "服务器连接失败...";
            }
            OracleConnection conn = new OracleConnection(connStr);
            try
            {
                conn.Open();
            }
            catch(Exception er)
            {
                conn.Dispose();
                if (stockNo != "01" && pkg_name == ConfigurationManager.AppSettings["ttmatPkgName"])
                {
                    data_cache(pkg_name, pflag, codestr, stockNo, appliacationName);
                }
                else
                {
                    outstr = "数据库连接失败(nlscan_pkg)！";
                    //base.SendDatagram(outstr);
                    return outstr;
                }
                return "连接不上远程服务器:"+er.Message;
            }
            cmd = new OracleCommand(pkg_name, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            OracleParameter p1 = new OracleParameter("pflag", OracleType.Int16, 2);
            p1.Direction = ParameterDirection.Input;
            OracleParameter p2 = new OracleParameter("outstr", OracleType.VarChar, 200);
            p2.Direction = ParameterDirection.Output;
            OracleParameter p3 = new OracleParameter("codestr", OracleType.VarChar, 1000);
            p3.Direction = ParameterDirection.Input;
            OracleParameter p4 = new OracleParameter("return_value", OracleType.Int16, 2);
            p4.Direction = ParameterDirection.ReturnValue;

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            try
            {
                cmd.Parameters["pflag"].Value = pflag;
                cmd.Parameters["codestr"].Value = codestr;
                cmd.ExecuteNonQuery();
                //outstr = m_command.Parameters["outstr"].Value.ToString();
                outstr = cmd.Parameters["outstr"].Value.ToString();

            }
            catch (Exception err)
            {
                outstr = "数据库打开失败:"+err.Message;

            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            //MessageBox.Show(outstr);
            //base.SendDatagram(outstr);
            return outstr;
        }
        /// <summary>
        /// 用于检查IP地址或域名是否可以使用TCP/IP协议访问(使用Ping命令),true表示Ping成功,false表示Ping失败 
        /// </summary>
        /// <param name="strIpOrDName">输入参数,表示IP地址或域名</param>
        /// <returns></returns>
        public static bool PingIpOrDomainName(string strIpOrDName)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 500;
                PingReply objPinReply = objPingSender.Send(strIpOrDName, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
