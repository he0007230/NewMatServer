// This is a part of DotNetRemoting Library
// Copyright (C) 2002-2007 Amplefile
// All rights reserved.
//
// This source code can be used, distributed or modified
// only under terms and conditions 
// of the accompanying license agreement.

using System;
using System.Collections;
using System.IO;

namespace DotNetRemoting
{
    /// <summary>
    /// status
    /// </summary>
    public enum status
    {
        none,
        connected,
        disconnected,
        error,
        info,
        serializerset,
        serializer_instance_created,
        socketconnector_stat_connected,
        ready,
        timeout,
        connect_request,
        reconnect_request,
        disconnect_request,
        socket_dispose_request,
        reconnection_failed,
        client_handshake_complete,
        server_handshake_complete_for_client,
        server_client_joined
    };
    /// <summary>
    /// rpc commands
    /// </summary>
    public enum commands 
    { 
        none, 
        rpc_no_response_required, 
        rpc_response, 
        rpc_response_required 
    };
    /// <summary>
    /// for rpc mstly
    /// </summary>
    public enum stat_type 
    { 
        none, 
        error, 
        warning, 
        info 
    };

    /// <summary>
    /// Interface for connector (default or the specific)
    /// </summary>
    public interface IConnector
    {
        object Connect();
        bool Bind { get;set;}
        status Status { get;}
        string StatusMessage { get; }
        int ConnectTimeOut { get;set;}
    }

    public interface IEncryptor
    {
        string Name { get;}
        string Password { get;set;}
        byte[] Encrypt(byte[] barr );
        byte[] Decrypt(byte[] barr);
    }

    public interface IFastSerializer : IDNR_Formatter
    {
        Type[] GetTypes();
        IDNR_Formatter GenericFormatter { get;set; }
    }

    public interface IDNR_Formatter
    {
        void Serialize(Stream s, object obj);
        object Deserialize(Stream s);
        string Name { get;}
    }

    [Serializable]
    public class ComplexObject
    {
        public ArrayList _SomeArrayList;
        public string _SomeString;
        public DateTime _SomeDateTime;
        public int _SomeInt;
    }

    [Serializable]
    public class DrawingObject
    {
        public bool Start;
        public byte Color;
        public byte X;
        public byte Y;

        public override string ToString()
        {
            return Start.ToString() + "," + X.ToString() + "," + Y.ToString();
        }
    }

    [Serializable]
    public class SerializerSetObject
    {
        public string FormatterName;
        public string[] EncryptorsNames;
        public byte[] EncryptorsIndexes;
        public int KeepConnectionAlivePeriod;
    }

    [Serializable]
    public class SyncCommsObject
    {
        public bool WithExecution;
        public commands Command;
        public int ClientID;
        public string CodeMessage;
        public string Message = "not set";
        public object InObject;
        public object OutObject;
        public object Tag;// reserved
    }

    [Serializable]
    public class RpcObject
    {
        public int CommandID;// reserved
        public string MethodName;
        public string ClassName;
        public object[] Args;
        public object OutObject;
    }

    [Serializable]
    public class ConnectionAliveObject
    {
        public string CodeMessage;
        public object Tag;
    }

    [Serializable]
    public class GenericObject
    {
        public string Mess;
        public object Obj;
        public int ID;
        public int Port;
    }
}
