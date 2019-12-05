using Matchvs;
using System.Collections.Generic;
using UnityEngine;

namespace Matchvs{



    public class MsRoomInfo{
        public JoinRoomType joinType;
        public uint userID;
        public ulong roomID;
        public uint gameID;
        public uint maxPlayer;
        public int mode;
        public int canWatch;
        public Dictionary<string,string> tags;
        public string userProfile;
        public int visibility;
        public string roomProperty;
        public WatchSetting watchSetting;
        public string roomName ;

        public MsRoomInfo(JoinRoomType joinType,uint userID,ulong roomID,uint gameID,uint maxPlayer = 2,int mode = 0,
            int canWatch = 1,string userProfile = "",Dictionary<string,string> tags = null,int visibility = 1,
            string roomProperty = "matchvs",WatchSetting ws = null,string roomName = "roomName"){
            this.joinType = joinType;
            this.userID = userID;
            this.roomID = roomID;
            this.gameID = gameID;
            this.maxPlayer = maxPlayer;
            this.mode = mode;
            this.canWatch = canWatch;
            this.tags = tags;//k-v map as json object  ex:[{dd:'SB',AA:'dd',re1:123},{cc:'dd',lk:'1qw'}];
            this.userProfile = userProfile;
            this.visibility = visibility;//新加2018-10-31
            this.roomProperty = roomProperty;//新加2018-10-31
            this.roomName = roomName;

            if(ws==null){
                ws = new WatchSetting();
                ws.MaxWatch = 10;
            }

            this.watchSetting = ws;
            Log.i(" MsRoomJoin:{0}",ToString());
        }

        public string ToString(){ return "roomID:"+roomID
            +"userID:"+userID
            +"joinType:"+joinType
            +"visibility:"+visibility
            ; }
    }
}