using ModelsEx.Models;
using System.Net;


namespace ModelsEx.Abstraction
{
    public interface IMessageSource
    {
        void Send(MessageUdp message, IPEndPoint ep);
        MessageUdp Receive(ref IPEndPoint ep);
    }
}
