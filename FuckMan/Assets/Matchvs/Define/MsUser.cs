namespace Matchvs{
    public class MsUser{
        public uint userid = 111;
        public string token = "0";
        public string avatar = "";

        public string deviceID = "";
        public int gender = 0;
        public string mac = "";
        public string nickname = "";
        public string regTime = "";

        public override string ToString(){ return $"id:{userid},token:{token},name:{nickname}"; }
    }
}