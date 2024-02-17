using ChatCommon;
using ChatNetWork;
using System.Net;

namespace ChatApp;

class Program
{

   
    
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Server<IPEndPoint> server = new Server<IPEndPoint>(new UdpMessageSource());
            server.Work();
        }

        else
        {
            //UdpSourceClient source = new UdpSourceClient(args[0], Int32.Parse(args[1]));
            UdpSourceClient source = new UdpSourceClient();
            Client<IPEndPoint> client = new Client<IPEndPoint>(source, args[0]);
            client.Start();
        }
 
    }
}
