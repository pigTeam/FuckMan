using Matchvs;

namespace Matchvs{
    public class MsFrameData{
        public uint frameIndex;
        public FrameDataNotify[] frameItems;

        public MsFrameData(uint lastIdx,FrameDataNotify[] frameData){
            this.frameIndex = lastIdx;
            this.frameItems = frameData;
        }
    }
}