using ModelsEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModelsEx.Abstraction;

namespace ModelsEx.Services
{
    public class UdpMessageSource : IMessageSource
    {
        private UdpClient udpClient;

        public UdpMessageSource()
        {
            udpClient = new UdpClient(12345);
        }

        public MessageUdp Receive(ref IPEndPoint ep)
        {
            byte[] receiveBytes = udpClient.Receive(ref ep);
            string receivedData = Encoding.ASCII.GetString(receiveBytes);

            return MessageUdp.MessageFromJson(receivedData);
        }

        public void Send(MessageUdp message, IPEndPoint ep)
        {
            byte[] forwardBytes = Encoding.ASCII.GetBytes(message.MessageToJson());

            udpClient.Send(forwardBytes, forwardBytes.Length, ep);
        }
    }

}
