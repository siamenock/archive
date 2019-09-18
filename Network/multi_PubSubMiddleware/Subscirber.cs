using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Subscirber
{
    public static void _Main(string[] args)
    {
        Middleware.InitIP();

        // (1) 소켓 객체 생성 (TCP 소켓)
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // (2) 서버에 연결
        var ep = new IPEndPoint(IPAddress.Parse(Middleware.IP), Middleware.PORT_LISTEN);
        sock.Connect(ep);

        int cmd = -1;
        byte[] buff;
        byte[] buffReceive = new byte[8192];

        Topic[] topics = new Topic[0];

        // Q 를 누를 때까지 계속 Echo 실행
        while (true)
        {
           // 일단 처음에 한번 떼오는건 기본으로 함.
            switch (cmd) {
                case -1:
                    buff = Protocol.Write.RequestTopicList();
                    sock.Send(buff, SocketFlags.None);
                    sock.Receive(buffReceive);
                    Protocol.OPCODE opcode = Protocol.Read.Opcode(buffReceive);
                    if (opcode == Protocol.OPCODE.REPLY_TOPIC_LIST){
                        Console.WriteLine("get topic list");
                        topics = Protocol.Read.ReplyTopicList(buffReceive);
                    }
                    break;
                default:
                    Topic topic = topics[cmd];
                    Thread threadSubscribe = new Thread(new ThreadStart(()=>SubscribeThread(topic))) ;
                    threadSubscribe.Start();
                    break;
            }

            Console.WriteLine("enter input for your action.");
            Console.WriteLine("\t-1: refresh topic list. get data from middleware");
            for (int i = 0; i < topics.Length; i++)
            {
                Console.WriteLine("\t" + i + ": request subscribe on " + topics[i].ipHost + ":" + topics[i].portNum);
                Console.WriteLine("\t\t" + "name\t:" + topics[i].name);
                Console.WriteLine("\t\t" + "period\t:" + topics[i].period);
                Console.WriteLine("\t\t" + "readPast:" + topics[i].readPast);
                Console.WriteLine();
            }

            cmd = int.Parse(Console.ReadLine());
        }

        
    }
    static void SubscribeThread(Topic topic) {
        
        Socket sockPublisher = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint epPublisher = new IPEndPoint(IPAddress.Parse(topic.ipHost), topic.portNum);
        sockPublisher.Connect(epPublisher);

        byte[] buffReceive  = new byte[8192];
        sockPublisher.Send(Protocol.Write.RequestSubscribe(topic));
        //Console.WriteLine("Send subscribe request to publisher!");


        while (true) {
            sockPublisher.Receive(buffReceive);
            Megazine megazine = Protocol.Read.SendMegazine(buffReceive);
            Console.WriteLine(megazine.timeLog + "\t" + topic.ipHost + ":" + topic.portNum + " :: " + topic.name);
            Console.WriteLine(megazine.data);
        }
    }
    
    
    
}
