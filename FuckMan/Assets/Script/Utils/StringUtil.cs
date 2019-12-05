using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using UnityEngine;

public class StringUtil {
    internal static byte[] toUtf8Array(string str) {
        if (str == null) {
            str = "";
        }
        return Encoding.UTF8.GetBytes(str);
    }
    internal static string fromUtf8Array(byte[] str) {
        if (str == null) {
            str = new byte[0];
        }
        return Encoding.UTF8.GetString(str);
    }


}
