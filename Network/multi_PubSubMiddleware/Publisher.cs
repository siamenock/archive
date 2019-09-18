using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Publisher
{
    public static string ip = "xxx.xxx.xxx.xxx";
    public static int portListen = 15230;
    static List<Megazine> megazines = new List<Megazine>();
    static Topic    topic = null;

    public static void _Main(string[] args)
    {
        string cmd = string.Empty;
        Console.WriteLine("make your topic!");
        Console.Write("name\t:");                               string name = Console.ReadLine();
        Console.Write("period\t:");                             int period  = int.Parse(Console.ReadLine());
        Console.Write("Send past log to new users? (y/n):");    bool pastLog= (Console.ReadLine()[0] == 'y');
        Publisher.InitIP();
        portListen = GetAvailablePort(portListen);

        topic = new Topic(name, ip, portListen, period, pastLog);
        Middleware.InitIP();


        // (1) 소켓 객체 생성 (TCP 소켓)
        Socket sockMiddle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // (2) 서버에 연결
        
        var ep = new IPEndPoint(IPAddress.Parse(Middleware.IP), Middleware.PORT_LISTEN);
        sockMiddle.Connect(ep);
        Console.WriteLine("sending new topic to middleware");

        byte[] buff = Protocol.Write.RegisterTopic(topic);

        Socket sockListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint epClient = new IPEndPoint(IPAddress.Any, portListen);
        sockListen.Bind(epClient);
        sockListen.Listen(10);
        Thread cilentAcceptThread = new Thread(new ThreadStart(() => ClientAcceptThread(sockListen)));
        cilentAcceptThread.Start();

        // (3) 서버에 데이타 전송
        sockMiddle.Send(buff, SocketFlags.None);


        while (true) {
            Console.WriteLine("write down any data you want to send to clients");
            string data = Console.ReadLine();
            Megazine megazine = new Megazine(topic.name, topic.ipHost, topic.portNum, data);
            megazines.Add(megazine);
            Console.WriteLine("\nsend megazine!\n\n");
        }

        sockListen.Close();
        // (5) 소켓 닫기
        sockMiddle.Close();
    }


    static void ClientAcceptThread(Socket sockListen) {
        while (true)
        {
            Socket sockClient = sockListen.Accept();
            Thread threadClient = new Thread(new ThreadStart(() => ClientHandleThread(sockClient)));
            threadClient.Start();
            Console.WriteLine("Here comes a new subscriber!");
        }
    }
    static void ClientHandleThread(Socket sockClient) {
        int curIndex = topic.readPast? 0 : megazines.Count;
        while (true) {
            if (curIndex >= megazines.Count) {
                Thread.Sleep(1000);
                continue;
            }
            Console.WriteLine("new megazine detected");
            byte[] buff = Protocol.Write.SendMegazine(megazines[curIndex ++]);
            sockClient.Send(buff, SocketFlags.None);
        }
    }

    public static void InitIP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Console.WriteLine(ip.ToString());
                Publisher.ip = ip.ToString();
            }
        }
    }

    public static int GetAvailablePort(int startingPort)
    {
        var portArray = new List<int>();

        var properties = IPGlobalProperties.GetIPGlobalProperties();

        // Ignore active connections
        var connections = properties.GetActiveTcpConnections();
        portArray.AddRange(from n in connections
                           where n.LocalEndPoint.Port >= startingPort
                           select n.LocalEndPoint.Port);

        // Ignore active tcp listners
        var endPoints = properties.GetActiveTcpListeners();
        portArray.AddRange(from n in endPoints
                           where n.Port >= startingPort
                           select n.Port);

        // Ignore active UDP listeners
        endPoints = properties.GetActiveUdpListeners();
        portArray.AddRange(from n in endPoints
                           where n.Port >= startingPort
                           select n.Port);

        portArray.Sort();

        for (var i = startingPort; i < UInt16.MaxValue; i++)
            if (!portArray.Contains(i))
                return i;

        return 0;
    }



}
