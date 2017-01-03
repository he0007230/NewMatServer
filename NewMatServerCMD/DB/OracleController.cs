using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Data;
using System.Net.NetworkInformation;
using System.Configuration;
using System.Threading;

namespace NewMatServerCMD.DB
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
            return outstr;
        }

        public static string NlscanPkg(string connStr, string pkg_name, int pflag, string codestr, string clientName, string appliacationName, string stockNo,string isCache)
        {

            OracleCommand cmd;
            string outstr;
            if ((!PingIpOrDomainName(Config.Instance.sysInfo.remoteServer)) && (isCache == "Y"))
            {
                return data_cache(pkg_name, pflag, codestr, stockNo, appliacationName);
            }
            OracleConnection conn = new OracleConnection(connStr);
            try
            {
                conn.Open();
            }
            catch(Exception er)
            {
                conn.Dispose();
                return "连接不上数据库:"+er.Message;
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
            return outstr;
        }

        public static string GetSpaData(string outStr,string connStr)
        {
            string[] data = outStr.Split('#');
            string tmp = outStr;
            if ((data.Length < 12) || (data[0].Length < 6))
            {
                return tmp;
            }
            OracleConnection conn = new OracleConnection(connStr);

            try
            {
                conn.Open();
            }
            catch
            {
                return tmp;
            }

            string sqlStr = "select nvl(a.stk_amount,0)-nvl(b.amount,0),d.type_name from ttshop_goods_stk a";
            sqlStr += ",ttshop_else_goods b ,ttshop_goods c left join ttshop_goods_type d on ";
            sqlStr += "c.goods_type=d.goods_type where c.goods_no=a.goods_no and a.goods_no=b.goods_no ";
            sqlStr += "and a.stock_no=b.stock_no and a.goods_no=" + data[0] + " and a.stock_no='" + data[8] + "'";
            OracleCommand cmd = new OracleCommand(sqlStr, conn);
            try
            {
                OracleDataReader odr = cmd.ExecuteReader();

                if (odr.HasRows)
                {

                    if (odr.Read())
                    {
                        data[2] = odr[0].ToString();
                        data[7] = odr[1].ToString();
                    }
                }
            }
            finally
            {
                cmd.Dispose();
            }
            sqlStr = "select nvl(sum(in_amount),0) from ttshop_goods_match a,ttshop_goods_match_det b";
            sqlStr += " where a.check_flag not in('08','09') and a.send_stock_no='" + data[8];
            sqlStr += "' and b.goods_no=" + data[0] + " and a.in_date=to_char(sysdate,'yyyymmdd') and a.match_no=b.match_no";
            OracleCommand cmd2 = new OracleCommand(sqlStr, conn);
            try
            {
                OracleDataReader odr = cmd2.ExecuteReader();

                if (odr.HasRows)
                {

                    if (odr.Read())
                    {
                        data[9] = odr[0].ToString();
                    }
                }
            }
            finally
            {
                cmd2.Dispose();
            }


            sqlStr = "select nvl(sum(in_amount),0) from ttshop_goods_match a,ttshop_goods_match_det b";
            sqlStr += " where a.check_flag not in('08','09') and a.send_stock_no='" + data[8];
            sqlStr += "' and b.goods_no=" + data[0] + " and a.in_date=to_char(sysdate-1,'yyyymmdd') and a.match_no=b.match_no";
            OracleCommand cmd3 = new OracleCommand(sqlStr, conn);
            try
            {
                OracleDataReader odr = cmd3.ExecuteReader();

                if (odr.HasRows)
                {

                    if (odr.Read())
                    {
                        data[10] = odr[0].ToString();
                    }
                }
            }
            finally
            {
                cmd3.Dispose();
            }
            sqlStr = "select remark from ttshop_discount_set where start_date<=to_char(sysdate,'yyyymmdd')";
            sqlStr += " and end_date>=to_char(sysdate,'yyyymmdd') and stock_no='" + data[8] + "' and goods_no=" + data[0];
            OracleCommand cmd4 = new OracleCommand(sqlStr, conn);
            try
            {
                OracleDataReader odr = cmd4.ExecuteReader();

                if (odr.HasRows)
                {

                    if (odr.Read())
                    {
                        data[11] = odr[0].ToString();                        
                    }
                }
            }
            finally
            {
                cmd4.Dispose();
            }
            tmp = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                tmp += "#" + data[i];
            }
            return tmp;
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
