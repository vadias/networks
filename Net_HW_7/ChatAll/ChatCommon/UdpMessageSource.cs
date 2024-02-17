using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatCommon
{
    public class UdpMessageSource : IMessageSource<IPEndPoint>
    {
        private UdpClient udpClient;


        public UdpMessageSource(int port = 12345)
        {
            udpClient = new UdpClient(port);
        }

        public IPEndPoint CopyT(IPEndPoint t)
        {
            return new IPEndPoint(t.Address, t.Port);
        }

        public IPEndPoint CreateNewT()
        {
            return new IPEndPoint(IPAddress.Any, 0);
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

            udpClient.Send(forwardBytes, forwardBytes.Length, toAddr);
        }
    }
}
