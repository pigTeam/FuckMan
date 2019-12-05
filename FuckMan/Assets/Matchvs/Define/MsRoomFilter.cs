namespace Matchvs{
    public class MsRoomFilter{
        public uint maxPlayer;
        public int mode;
        public int canWatch;
        public string roomProperty;
        public int full;
        public Matchvs.RoomState state;
        public Matchvs.RoomListSort sort;
        public Matchvs.SortOrder order;
        public int pageNo;
        public int pageSize;
        public int getSystemRoom;

        /**
        *
        * @param maxPlayer {number} 房间最大人数 (0-全部)
        * @param mode {number} 模式（0-全部）*创建房间时，mode最好不要填0
        * @param canWatch {number} 是否可以观战（0-全部 1-可以 2-不可以）
        * @param roomProperty {string}
        * @param full {number} 0-全部 1-满 2-未满
        * @param state {number} 0-StateNil 1-StateOpen 2-StateClosed
        * @param sort  {number} 0-RoomSortNil 1-RoomSortCreateTime 2-SortPlayerNum 3-SortState
        * @param order {number} 0-SortAsc 1-SortDesc
        * @param pageNo {number}
        * @param pageSize {number}
        * @param getSystemRoom {number} 0玩家创建,1系统创建,2玩家+系统创建
        * @constructor
        */
        public MsRoomFilter(uint maxPlayer = 0,int mode = 0,int canWatch = 0,string roomProperty = "",int full = 0,
            Matchvs.RoomState state = Matchvs.RoomState.Nil,
            Matchvs.RoomListSort sort = Matchvs.RoomListSort.RoomSortNil,
            Matchvs.SortOrder order = Matchvs.SortOrder.SortAsc,int pageNo = 0,int pageSize = 10,int getSystemRoom = 2){
            this.maxPlayer = maxPlayer;
            this.mode = mode;
            this.canWatch = canWatch;
            this.roomProperty = roomProperty;
            this.full = full;
            this.state = state;
            this.sort = sort;
            this.order = order;
            this.pageNo = pageNo;
            this.pageSize = pageSize;
            this.getSystemRoom = getSystemRoom;
            Log.i(this+" MsRoomFilterEx:"+this);
        }
    }
}