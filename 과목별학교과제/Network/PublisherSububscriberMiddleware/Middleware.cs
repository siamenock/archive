using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


class Middleware
{
    public static string IP= "xxx.xxx.xxx.xxx";
    public static readonly int PORT_LISTEN = 7500;
    static List<Topic> topics = new List<Topic>();
    
    public static void _Main(string[] args)
    {
        // (1) 소켓 객체 생성 (TCP 소켓)
        Socket sockListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // (2) 포트에 바인드
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, PORT_LISTEN);
        sockListen.Bind(ep);
        // (3) 포트 Listening 시작
        sockListen.Listen(10);

        while (true)
        {
            Socket sockClient = sockListen.Accept();
            Thread threadClient = new Thread(new ThreadStart(()=>ClientThread(sockClient)));
            threadClient.Start();
        }
        sockListen.Close();
    }

    // publisher, subscriber 다 이거로 받음
    public static void ClientThread(Socket sockClient) {
        byte[] buff = new byte[8192];
        byte[] buffReply;
        while (true)
        {
            // (5) 소켓 수신
            int n = sockClient.Receive(buff);

            string data = Encoding.UTF8.GetString(buff, 0, n);
            Console.WriteLine(data);

            switch (Protocol.Read.Opcode(buff)) {
                case Protocol.OPCODE.REGISTER_TOPIC:
                    Topic topic = Protocol.Read.RegisterTopic(buff);
                    // register it
                    Console.WriteLine("토픽 등록 요청 받음");
                    Console.WriteLine("토픽명\t: " + topic.name);
                    Console.WriteLine("IP\t: " + topic.ipHost);
                    Console.WriteLine("port\t: " + topic.portNum);
                    Console.WriteLine("period\t: " + topic.period);
                    if (Register(topic))
                        Console.WriteLine("토픽목록으로 등록함");
                    else
                        Console.WriteLine("등록할 수 없는 토픽");
                    Console.Write("\n");

                    break;

                case Protocol.OPCODE.REQUEST_TOPIC_LIST:
                    buffReply = Protocol.Write.ReplyTopicList(topics);      // 일단 대충
                    sockClient.Send(buffReply, 0, buffReply.Length, SocketFlags.None);
                    Console.WriteLine("토픽 목록 요청 받음.");
                    break;

                default:
                    Console.WriteLine("나한테 보낼 필요 없는 프로토콜을 받음.");
                    break;

            }
        }
        // (7) 소켓 닫기
        sockClient.Close();
    }


    public static void InitIP() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Console.WriteLine(ip.ToString());
                Middleware.IP = ip.ToString();
            }
        }
    }

    static bool Register(Topic topic) {
        if (Find(topic) == null) {
            topics.Add(topic);
            return true;
        }
        return false;
    }
    static Topic Find(Topic topic) {
        foreach (Topic t in topics) {
            if ((t.ipHost == topic.ipHost) && (t.portNum == topic.portNum) && (t.name == topic.name)){
                return t;
            }
        }
        return null;
    }
}
