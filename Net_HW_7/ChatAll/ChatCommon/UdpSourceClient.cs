using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatCommon
{
    public class UdpSourceClient : IMessageSourceClient<IPEndPoint>
    {
        private UdpClient udpClient;

        private IPEndPoint ipEndPoint;

        public UdpSourceClient(string address = "127.0.0.1", int port = 12345)
        {
            udpClient = new UdpClient(address, port);

        }


        public IPEndPoint CreateNewT()
        {
            return new IPEndPoint(IPAddress.Any, 0);
        }

        public IPEndPoint CreateNewT(string address, int port)
        {
            return new IPEndPoint(IPAddress.Parse(address), port);
        }

        public IPEndPoint GetServer()
        {
            return ipEndPoint;
        }

        public MessageUdp Receive(ref IPEndPoint fromAddr)
        {
            byte[] receiveBytes = udpClient.Receive(ref fromAddr);
            string receivedData = Encoding.ASCII.GetString(receiveBytes);

            return MessageUdp.MessageFromJson(receivedData);
        }

        public void Send(MessageUdp message, IPEndPoint toAddr)
        {
            byte[] forwardBytes = Encoding.ASCII.GetBytes(message.MessageToJson());

            udpClient.Send(forwardBytes, forwardBytes.Length, ipEndPoint);
        }
    }
}
