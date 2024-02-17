using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        //Реализуйте тип сообщений List, 
        //при котором клиент будет получать все непрочитанные сообщения с сервера.
        public virtual List<Message>? ToMessage { get; set; }
        public virtual List<Message>? FromMessages { get; set; }
    }
}
