using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using pb = global::Google.Protobuf;
public class DataUtil
{
    public static  byte[] Serialize(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream st = new MemoryStream();

        bf.Serialize(st, obj);
        byte[] bt = new byte[st.Length];
        st.Position = 0;
        st.Read(bt, 0, (int)(st.Length));
        st.Close();
        st.Dispose();
        return bt;
    }

    public static T Deserialize<T>(pb::ByteString data)
    {
        if (data.IsEmpty)
        {
            return default(T);
        }

        MemoryStream st = new MemoryStream();

        BinaryFormatter bf = new BinaryFormatter();

        byte[] bt = data.ToByteArray();


        st.Position = 0;
        st.Write(bt, 0, bt.Length);
        st.Position = 0;

        object obj = bf.Deserialize(st);
        T frame = (T)(obj);
        st.Close();
        st.Dispose();
        return frame;
    }
}
