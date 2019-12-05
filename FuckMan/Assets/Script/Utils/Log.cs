using System;
using System.Threading;

public class Log {
    static bool isUnity = true;
    public static bool isDebug = false;
    public delegate void LOGGER(string format, params object[] args);
    public static void info(String msg, params object[] args) {
        string smsg = msg;
        foreach (object s in args) {
            smsg += s;
        }

        Console.Out.WriteLine("[INFO][{0}]:   {1} ", Thread.CurrentThread.ManagedThreadId, smsg);
    }
    public static void debug(String msg, params object[] args) {

        string smsg = msg;
        foreach (object s in args) {
            smsg += s;
        }
        Console.Out.WriteLine("[DEBUG][{0}]:   {1} ", Thread.CurrentThread.ManagedThreadId, smsg);
    }
    public static void warn(String msg, params object[] args) {
        string smsg = msg;
        foreach (object s in args) {
            smsg += s;
        }
        Console.Out.WriteLine("[WARN][{0}]:        {1} ", Thread.CurrentThread.ManagedThreadId, smsg);
    }
    public static void error(String msg, params object[] args) {
        string smsg = msg;
        foreach (object s in args) {
            smsg += s;
        }
        Console.Error.WriteLine("[ERROR][{0}]:       {1} ", Thread.CurrentThread.ManagedThreadId, smsg);
    }

    public static LOGGER d = isDebug ? (isUnity ? (LOGGER)UnityEngine.Debug.LogFormat : debug):(string msg, object[] args)=>{ };
    public static LOGGER e = isUnity ? (LOGGER)UnityEngine.Debug.LogErrorFormat : error;
    public static LOGGER i = isUnity ? (LOGGER)UnityEngine.Debug.LogFormat : info;
    public static LOGGER w = isUnity ? (LOGGER)UnityEngine.Debug.LogWarningFormat : debug;
}
