using BestHTTP.WebSocket;
using System;
using System.Collections.Generic;
namespace Matchvs{
public class NW {

    public interface NWCB {
        void onConnect(string host);
        /**
         *
         * @param buf DataView
         */
        void onMsg(byte[] buf);
        /**
         *
         * @param errCode int
         * @param errMsg String
         */
        void onErr(string host, int errCode, string errMsg);
    }


    private string host;
    private NWCB engine;
    private WebSocket webSocket;

    private Queue<byte[]> bufQueue = new Queue<byte[]>();
    public NW(string hOST_GATWAY_ADDR, NWCB engine) {
        this.host = hOST_GATWAY_ADDR;
        this.engine = engine;
        initWebSocket();
    }
    private void initWebSocket() {
        webSocket = new WebSocket(new Uri(host));
        webSocket.OnOpen += (WebSocket ws1) => {
            while (bufQueue.Count > 0) {
                send(bufQueue.Dequeue());
            }
            engine.onConnect(host);

        };
        webSocket.OnBinary += (WebSocket ws2, byte[] message2) => {
            engine.onMsg(message2);
        };
        webSocket.OnClosed += (WebSocket ws3, UInt16 code2, string message3) => {
            webSocket = null;
            Log.i("[WebSocket.OnClosed] host:{0},code:{1},Msg:{2}",code2,host,message3);
            engine.onErr(host, code2, message3);
        };
        webSocket.OnError += (WebSocket ws4, Exception ex4) => {
            string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
            if (ws4.InternalRequest.Response != null) {
                errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws4.InternalRequest.Response.StatusCode, ws4.InternalRequest.Response.Message);
            }
#endif
            webSocket = null;
            Log.w("[WebSocket.OnError] host:{0},errMsg:{1}",host, errorMsg);
            engine.onErr(host, 1006, errorMsg);
        };

        webSocket.Open();

    }
    internal void close() {
        if (webSocket == null) { return; }
        webSocket.Close();
    }

    internal void send(byte[] buf) {
        if (webSocket == null || !webSocket.IsOpen) {
            bufQueue.Enqueue(buf);
            return;
        }
        webSocket.Send(buf);
    }


}
}