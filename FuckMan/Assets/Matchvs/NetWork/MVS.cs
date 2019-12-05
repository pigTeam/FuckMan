using Matchvs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matchvs{
    public class MVS{

        public static uint GameID = 0;
        public static uint NodeID = 0;
        public static ulong RoomID = 0L;
        public static ulong WatchRoomID = 0L;
        public static Statistics mtaReport = null;
        internal static string Channel = "Matchvs";
        internal static string Platform = "alpha";
        internal static int Version;
        internal static string Appkey;
        internal static int threshold;
        internal static uint UserID;
        internal static string Token;
        internal static string DeviceID = "0";
        internal static ServerAddress Host = new ServerAddress();
        internal static ServerAddress APIPATH = Host;
        internal static LocalConfig Config = new LocalConfig();
        public static ulong TeamID = 0l;


        public class LocalConfig{
            internal int MAXPLAYER_LIMIT = 1000;
            internal int MINPLAYER_LIMIT = 0;
        }

        public class ServerAddress{
            public string MAIN_URL = "https://sdk.matchvs.com";
            public int HOST_GATWAY_PORT = 7001;
            public string HOST_GATWAY_ADDR = "ws://122.224.7.6:7001";
            public string HOST_HOTEL_ADDR = "";
            public string HOST_WATCH_ADDR = "";
            public string CMSNS_URL = "";
            public string VS_USER_URL = "";
            public string VS_OPEN_URL = "https://vsopen.matchvs.com";
            public string VS_PAY_URL = "";
            public string VS_PRODUCT_URL = "";
            public string HOSTLIST = "/v1/gateway/query";
            public string REGISTERUSER = "/wc3/regit.do";
            public string NODELIST = "/v3/gateway/listSets?";
            public string SCHEDULE = "/v3/gateway/schedule?";
        }


        internal class AppKeyCheck{
            public AppKeyCheck(){ }

            internal bool isInvailed(object appkey){
                Log.w("[warn] not check the appkey");
                return true;
            }
        }

        public static string PassWord = "";

        public static string getHotelUrl(BookInfo bookInfo){
            return isNeedWSS() ? ("wss://"+bookInfo.WssProxy+"/proxy?hotel="+bookInfo.Addr):("ws://"+bookInfo.Addr);
        }

        public static bool isNeedWSS(){ return false; }
    }
}