using System;
using System.Collections.Generic;
using System.Text;

namespace SYNCCC
{
    public class SocketModel
    {
        private int _type;

        public int type
        {
            get { return _type; }
            set { _type = value; }
        }
        private int _area;

        public int area
        {
            get { return _area; }
            set { _area = value; }
        }
        private int _command;

        public int command
        {
            get { return _command; }
            set { _command = value; }
        }
        private object _message;

        public object message
        {
            get { return _message; }
            set { _message = value; }
        }

        public SocketModel() { }

        public SocketModel(int type, int area, int command, object message)
        {
            this.type = type;
            this.area = area;
            this.command = command;
            this.message = message;
        }

        public T getMessage<T>()
        {
            return (T)message;
        }
    }
}

