using ChatCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatNetWork
{
    public interface IMessageSource_Old
    {
        void Send(MessageUdp message, IPEndPoint ep);
        MessageUdp Receive(ref IPEndPoint ep);
    }
}
