using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


/*
 * 미들웨어 켜 놓은 상태에서 퍼블리셔 접속. 퍼블리시 신청.
 * 섭스크라이버는 퍼블리시 된 데이터를 request 신청가능. request목록은 미들웨어에 저장되고, 퍼블리셔한테도 전달됨
 * 새 섭스크라이버를 확인한 퍼블리셔는 섭스크라이버에게 향후 데이터를 전달함. 옵션에 따라서 옜날 정보도 한번에 보낼수도 있음.
     */

class MainClass
{
    static void Main(string[] args)
    {
        Console.WriteLine(Megazine.CurDateTime());

        while (true)
        {
            int choose = -1;
            Console.WriteLine("Choose mode");
            Console.WriteLine("1: Middleware");
            Console.WriteLine("2: Publisher");
            Console.WriteLine("3: Subscriber");
            try
            {
                choose = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("invalid input!");
                continue;
            }

            switch (choose)
            {
                case 1: Middleware._Main(args); return;
                case 2: Publisher ._Main(args); return;
                case 3: Subscirber._Main(args); return;
            }

        }
    }
}

