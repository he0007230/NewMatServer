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
        /// ��ӡ������
        /// </summary>
        /// <param name="connStr">�������</param>
        /// <param name="sqlStr">��ѯ���</param>
        /// <param name="tableName">����</param>
        /// <param name="frxPath">frx·��</param>
        /// <param name="printName">��ӡ������ Ĭ�ϴ�ӡ������Ϊdefault</printName>
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
        /// ��ӡ������
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="frxPath">frx·��</param>
        /// <param name="param">���ݵ�frx�Ĳ���</param>
        /// <param name="printName">��ӡ������ Ĭ�ϴ�ӡ������Ϊdefault</printName>
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
        /// ��ӡ������
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="frxPath">frx·��</param>
        /// <param name="printName">��ӡ������ Ĭ�ϴ�ӡ������Ϊdefault</printName>
        public void PrintReport(DataSet ds, string frxPath, string printName)
        {
            Dictionary<string,string> param = new Dictionary<string,string>();
            PrintReport(ds,frxPath,param,printName);
        }
        
    }
}
