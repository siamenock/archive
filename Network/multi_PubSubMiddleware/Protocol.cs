using System;
using System.Collections.Generic;
using System.Text;

static class Protocol
{
    // [OPCODE         ] [additional data]
    // int               ???
    //###### 
    // [REGISTER_TOPIC ] [topic]    // pub -> midleware
    // [REQUEST_TOPIC_LIST]         // sub -> mid
    // [REQUEST_SUBSCRIBE ]         // sub -> pub

    // [REPLY_TOPIC_LIST  ] [topic count] [topic] [topic] [topic] ... // mid -> sub
    // [REPLY PUBLISH     ]         // mid -> pub

    // [SEND_MEGAZINE] [megazine]   // pub -> sub
    public enum OPCODE {
        REGISTER_TOPIC,
        REQUEST_TOPIC_LIST,
        REQUEST_SUBSCRIBE,
        REPLY_TOPIC_LIST,
        REPLY_PUBLISH,
        SEND_MEGAZINE
    }
    static readonly int SUCCESS = 1;

    enum TARGET{ MIDDLEWARE, PUBLISHER, SUBSCRIBER }

    // -------------------------
    // 메세지를 만드는 파트
    // -------------------------
    // 아... 이거 buff가 아니라 string타입으로 한 다음 마지막에 buff로 변환시켰어야 됐는데 생각이 짧았다.
    // 이 구조로는 동적인 길이 할당이 안됨. 데이터길이가 가변적인 이 상황에서는 치명적
    public static class Write
    {
        public static byte[] RegisterTopic(Topic topic)
        {
            int i = 0;  // index
            byte[] buff = new byte[8192];   // 모지라면 더 붙여야되는데.. 음... 일단 requst Publish에서는 충분할듯
            Buff.Write(buff, ref i, (int)OPCODE.REGISTER_TOPIC);
            Buff.Write(buff, ref i, topic);
            return Buff.Cut(buff, i);
        }
        public static byte[] RequestTopicList()
        {
            int i = 0; // index
            byte[] buff = new byte[8192];   // 모지라면 더 붙여야되는데..
            Buff.Write(buff, ref i, (int)OPCODE.REQUEST_TOPIC_LIST);
            return Buff.Cut(buff, i);
        }
        public static byte[] RequestSubscribe(Topic topic)
        {
            int i = 0; // index
            byte[] buff = new byte[8192];   // 모지라면 더 붙여야되는데...
            Buff.Write(buff, ref i, (int)OPCODE.REQUEST_SUBSCRIBE);
            Buff.Write(buff, ref i, topic);
            return Buff.Cut(buff, i);
        }
        public static byte[] ReplyTopicList(List<Topic> topicArr)
        {
            int i = 0; // index
            byte[] buff = new byte[8192];   // 모지라면 더 붙여야되는데...
            Buff.Write(buff, ref i, (int)OPCODE.REPLY_TOPIC_LIST);

            Buff.Write(buff, ref i, topicArr.Count);
            foreach (Topic topic in topicArr)
            {
                Buff.Write(buff, ref i, topic);
            }

            return Buff.Cut(buff, i);
        }
        public static byte[] SendMegazine(Megazine megazine)
        {
            int i = 0; // index
            byte[] buff = new byte[8192];   // 모지라면 더 붙여야되는데...
            Buff.Write(buff, ref i, (int)OPCODE.REQUEST_SUBSCRIBE);
            Buff.Write(buff, ref i, megazine);
            return Buff.Cut(buff, i);
        }
    }
    public static class Read {
        public static OPCODE Opcode(byte[] buff) {
            int i = 0;
            return (OPCODE) Buff.ReadInt(buff, ref i);
        }

        public static Topic RegisterTopic(byte[] buff)
        {
            int i = 0;
            int opcode = Buff.ReadInt(buff, ref i);
            if ((OPCODE)opcode != OPCODE.REGISTER_TOPIC)
            {
                Console.WriteLine("invalid opcode");
                return null;
            }
            return Buff.ReadTopic(buff, ref i);
        }
        public static Topic RequestSubscribe(byte[] buff) {
            int i = 0;
            int opcode = Buff.ReadInt(buff, ref i);
            if ((OPCODE)opcode != OPCODE.REQUEST_SUBSCRIBE)
            {
                Console.WriteLine("invalid opcode");
                return null;
            }
            return Buff.ReadTopic(buff, ref i);
        }
        public static Topic[] ReplyTopicList(byte[] buff) {
            int i = 0;
            int opcode = Buff.ReadInt(buff, ref i);
            if ((OPCODE)opcode != OPCODE.REPLY_TOPIC_LIST)
            {
                Console.WriteLine("invalid opcode");
                return null;
            }
            int count = Buff.ReadInt(buff, ref i);
            Topic[] ret = new Topic[count];
            for (int j = 0; j < count; j++) {
                ret[j] = Buff.ReadTopic(buff, ref i);
            }
            return ret;
        }
        public static Megazine SendMegazine(byte[] buff) {
            int i = 0;
            int opcode = Buff.ReadInt(buff, ref i);
            if ((OPCODE)opcode != OPCODE.REQUEST_SUBSCRIBE)
            {
                Console.WriteLine("invalid opcode");
                return null;
            }
            return Buff.ReadMegazine(buff, ref i);
        }
    }
    
}
