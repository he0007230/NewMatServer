using System;
using System.Collections.Generic;
using System.Text;
using FastReport;
using System.Data;
using System.Drawing.Printing;
using System.Data.OracleClient;

namespace NewMatServerCMD.tool
{
    public class PrintHelper
    {
        private static PrintHelper _instance;
        public const string defaultPrinter = "default";
        private static PrintDocument printDocument;
        private PrintHelper() 
        {
            printDocument = new PrintDocument();
        }
        public static PrintHelper GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PrintHelper();
            }
            return _instance;
        }

        /// <summary>
        /// 打印报表函数
        /// </summary>
        /// <param name="connStr">连接语句</param>
        /// <param name="sqlStr">查询语句</param>
        /// <param name="tableName">表名</param>
        /// <param name="frxPath">frx路径</param>
        /// <param name="printName">打印机名称 默认打印机设置为default</printName>
        public void PrintReport(string connStr,string sqlStr,string tableName, string frxPath, string printName)
        {
            OracleConnection conn = new OracleConnection(connStr);
            try
            {
                conn.Open();
            }
            catch
            {
                return;
            }
            OracleDataAdapter da = new OracleDataAdapter(sqlStr, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ds.Tables[0].TableName = tableName;
            da.Dispose();
            conn.Close();
            conn.Dispose();
            PrintReport(ds,frxPath,printName);
            ds.Dispose();

        }

        /// <summary>
        /// 打印报表函数
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="frxPath">frx路径</param>
        /// <param name="param">传递到frx的参数</param>
        /// <param name="printName">打印机名称 默认打印机设置为default</printName>
        public void PrintReport(DataSet ds, string frxPath,Dictionary<string,string> param,string printName)
        {
            Report report = new Report();
            try
            {
                report.Load(frxPath);
                report.RegisterData(ds);
                if (param != null && param.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kvp in param)
                    {
                        report.SetParameterValue(kvp.Key, kvp.Value);
                    }
                }
                if (printName == defaultPrinter)
                {
                    report.PrintSettings.Printer = printDocument.PrinterSettings.PrinterName;
                }
                else
                {
                    foreach (string pName in PrinterSettings.InstalledPrinters)
                    {
                        if (pName.ToUpper().IndexOf(printName.ToUpper()) >= 0)
                        {
                            report.PrintSettings.Printer = pName;
                        }
                    }
                }
                report.PrintSettings.ShowDialog = false;
                report.Print();
            }
            finally
            {
                report.Dispose();
            }
        }

        /// <summary>
        /// 打印报表函数
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="frxPath">frx路径</param>
        /// <param name="printName">打印机名称 默认打印机设置为default</printName>
        public void PrintReport(DataSet ds, string frxPath, string printName)
        {
            Dictionary<string,string> param = new Dictionary<string,string>();
            PrintReport(ds,frxPath,param,printName);
        }
        
    }
}
