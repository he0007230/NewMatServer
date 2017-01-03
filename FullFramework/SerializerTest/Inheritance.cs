using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SerTest
{
    [Serializable]
    public class Inheritance
    {
        private string _StringProp;

        public string StringProp
        {
            get { return _StringProp; }
            set { _StringProp = value; }
        }

        public ArrayList SomeArrayList;

        public float f;

        private Hashtable _ht;

        public Hashtable Ht
        {
            get { return _ht; }
            set { _ht = value; }
        }

        // generics objects
        public List<string> stringlist;
        public Dictionary<string, string> stringdict;
    }

    [Serializable]
    public class Inheritance1 : Inheritance
    {
        private string _StringProp1;

        public string StringProp1
        {
            get { return _StringProp1; }
            set { _StringProp1 = value; }
        }
    }

    [Serializable]
    public class Inheritance2 : Inheritance1
    {
        private ArrayList _Items;

        public ArrayList Items
        {
            get { return _Items; }
            set { _Items = value; }
        }
    }

    /// <summary>
    /// some class to serialize
    /// </summary>
    [Serializable] // that is not required. only for the compatibility with a standard serialization
    public class MyObject
    {
        private string _StringProp;

        public string StringProp
        {
            get { return _StringProp; }
            set { _StringProp = value; }
        }

        public ArrayList SomeArrayList;

        public float f;

        private Hashtable _ht;

        public Hashtable Ht
        {
            get { return _ht; }
            set { _ht = value; }
        }

        // generics objects
        public List<string> stringlist;
        public Dictionary<string, string> stringdict;
    }

}
