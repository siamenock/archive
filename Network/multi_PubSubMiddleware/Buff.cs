using System;
using System.Collections.Generic;
using System.Text;

// 주로 string데이터를 다룬다는게 문제임.
// buff길이를 미리 알기 어려운 경우를 대비해서 list로 처리했어야 함. 이제와서 고치긴 늦었네.
static class Buff {
    // 뒤에 남는 buff 제거한 subArray 반환
    public static byte[] Cut(byte[] buff, int len)
    {
        byte[] ret = new byte[len];
        Array.Copy(buff, 0, ret, 0, len);
        return ret;
    }

    // 걍 int 처리하자
    public static void Write(byte[] buff, ref int index, bool   val) {
        Write(buff, ref index, val? 1:0);
    }
    public static void Write(byte[] buff, ref int index, int    val) {
        Array.Copy(BitConverter.GetBytes(val), 0, buff, index, sizeof(int));
        index += sizeof(int);
    }
    public static void Write(byte[] buff, ref int index, string val)
    {
        Write(buff, ref index, val.Length);

        byte[] StrByte = Encoding.UTF8.GetBytes(val);
        Array.Copy(StrByte, 0, buff, index, sizeof(byte) * StrByte.Length);
        index += StrByte.Length;
    }
    public static void Write(byte[] buff, ref int index, Topic  val)
    {
        Write(buff, ref index, val.name);
        Write(buff, ref index, val.ipHost);
        Write(buff, ref index, val.portNum);
        Write(buff, ref index, val.period);
        Write(buff, ref index, val.readPast);
    }
    public static void Write(byte[] buff, ref int index, Megazine val)
    {
        Write(buff, ref index, val.name);
        Write(buff, ref index, val.ipHost);
        Write(buff, ref index, val.portNum);
        Write(buff, ref index, val.timeLog);
        Write(buff, ref index, val.data);
    }

    public static bool      ReadBool    (byte[] buff, ref int index){
        return ReadInt(buff, ref index) == 0? false: true;
    }
    public static int       ReadInt     (byte[] buff, ref int index) {
        int ret = BitConverter.ToInt32(buff, index);
        index += sizeof(int);

        return ret;
    }
    public static string    ReadString  (byte[] buff, ref int index)
    {
        int len = ReadInt(buff, ref index);
        string str = Encoding.Default.GetString(buff, index, len);
        index += len;
        return str;
    }
    public static Topic     ReadTopic   (byte[] buff, ref int index)
    {
        String name = ReadString(buff, ref index);
        String ipHost = ReadString(buff, ref index);
        int portNum = ReadInt(buff, ref index);
        int duration = ReadInt(buff, ref index);
        bool readPast = ReadBool(buff, ref index);
        return new Topic(name, ipHost, portNum, duration, readPast);
    }
    public static Megazine  ReadMegazine(byte[] buff, ref int index)
    {
        String name     = ReadString(buff, ref index);
        String ipHost   = ReadString(buff, ref index);
        int    portNum  = ReadInt   (buff, ref index);
        string timeLog  = ReadString(buff, ref index);
        string data     = ReadString(buff, ref index);
        return new Megazine(name, ipHost, portNum, data, timeLog);
    }
}

