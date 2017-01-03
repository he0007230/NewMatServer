using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DotNetRemoting
{
    public class ComplexObjectCreator
    {
        public static ComplexObject Create()
        {
            ComplexObject co = new ComplexObject();
            co._SomeArrayList = new System.Collections.ArrayList();
            for (int i = 0; i < 20; i++)
                co._SomeArrayList.Add(i.ToString() + "," + "#string");
            co._SomeInt = 12345;
            co._SomeString = "some string";
            co._SomeDateTime = DateTime.Now;
            return co;
        }

        public static ICollection GetItemsAsStringCollection(ComplexObject co)
        {
            ArrayList al = new ArrayList();

            al.Add("******Complex Object*******");
            al.Add("_SomeArrayList items");
            foreach (string s in co._SomeArrayList)
            {
                al.Add(s);
            }
            al.Add("_SomeInt=" + co._SomeInt.ToString());
            al.Add("_SomeString=" + co._SomeString);
            al.Add("_SomeDateTime=" + co._SomeDateTime.ToString());

            return al;
        }

        public static Dictionary<string, int> CreateDictionary()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < 20; i++)
                dict.Add(i.ToString(), i * 20);
            return dict;
        }

        public static ICollection GetDictItemsAsStringCollection(Dictionary<string, int> dict)
        {
            ArrayList al = new ArrayList();

            al.Add("******Generics Dictionary Object*******");
             
            foreach (string Key in dict.Keys)
            {
                al.Add(Key + "," + dict[Key].ToString());
            }

            return al;
        }

    }

    public class DataCreator
    {
        public static DataSet CreateDataSet(int Rows)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(CreateDataTable(Rows));
            return ds;
        }

        public static DataTable CreateDataTable(int Rows)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("LastName", typeof(string)));
            dt.Columns.Add(new DataColumn("Date", typeof(DateTime)));
           
            for (int i = 0; i < Rows; i++)
            {
                DataRow dr = dt.NewRow();
                dr.ItemArray = new object[] { i, "Tom", "Fox", DateTime.Now };
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static string GetDataTableAsString(DataTable dt)
        {
            return "DataTable, Rows number = " + dt.Rows.Count.ToString();
        }

        public static string GetDataSetAsString(DataSet ds)
        {
           return "DataSet, Rows number = " + ds.Tables[0].Rows.Count.ToString();
        }
    }
}
