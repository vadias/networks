using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCommon
{
    public interface IMessageSourceClient<T>
    {
        public void Send(MessageUdp message, T toAddr);
        public MessageUdp Receive(ref T fromAddr);
        public T CreateNewT();
        public T CreateNewT(string addr, int port);
        public T GetServer();

    }

   
}
