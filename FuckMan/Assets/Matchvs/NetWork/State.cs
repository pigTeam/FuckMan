using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matchvs{
    public class State{
        public static int state = 0;
        public static int INIT = 1<<1;
        public static int LOGIN = 1<<2;
        public static int ROOM = 1<<3;
        public static int TEAM = 1<<4;
        public static int WATCH = 1<<5;
        public static int RECONNECT = 1<<6;


        public bool isHaveState(int _state){ return (state&_state)==_state ? true:false; }

        public void clearState(int _state){ state = state&~_state; }
        public void setState(int _state){ state = state|_state; }

        internal void SetIniting(){ state |= INIT; }

        internal bool HaveLogin(){ return isHaveState(LOGIN); }

        internal void SetLogin(){ state |= LOGIN; }


        internal void SetJoinRooming(){ state |= ROOM; }
        internal void SetWatch(){ setState(WATCH); }

        internal bool HaveInRoom(){ return isHaveState(ROOM); }
        internal bool HaveInWatch(){ return isHaveState(WATCH); }
    }
}