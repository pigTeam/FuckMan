using UnityEngine;

namespace Matchvs{
    public class UnityContext:MonoBehaviour{
        public delegate void HeartBeat();

        public HeartBeat heartBeat;

        // Use this for initialization
        void Start(){ }

        // Update is called once per frame
        void Update(){ }

        private void heartBeatServer(){ heartBeat?.Invoke(); }
    }
}