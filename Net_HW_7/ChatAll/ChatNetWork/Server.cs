using ChatCommon;
using ChatDb;
using ChatModel;
using System.Net;
using System.Net.Sockets;

namespace ChatNetWork
{
    public class Server<T>
    {
        //private UdpClient udpClient;
        //private IPEndPoint ip;
        private Dictionary<String, T> clients = new Dictionary<String, T>();

        IMessageSource<T> messageSource;

        bool work = true;

        //Токен для остановки сервера
        static private CancellationTokenSource cts = new CancellationTokenSource();

        static private CancellationToken ct = cts.Token;


        public Server(IMessageSource<T> source)
        {
            messageSource = source;
        }

       

        public void Work()
        {
            
            //udpClient = new UdpClient(12345);
            T ip = messageSource.CreateNewT();
            //ip = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Клиент ожидает сообщение.");


            while (work)
            {

                var message = messageSource.Receive(ref ip);
                ProcessMessage(message, ip);

            }



        }

        public void Stop()
        {
            work = false;
        }


        private void ProcessMessage(MessageUdp messageUdp, T ep)
        {
            Console.WriteLine($"Получено сообщение от {messageUdp?.FromName} " +
                                $"для {messageUdp?.ToName} с командой {messageUdp?.Command}:");
            Console.WriteLine(messageUdp?.Text);

            switch (messageUdp?.Command)
            {
                case Command.Message:
                    RelyMessage(messageUdp);
                    break;
                case Command.Register:
                    Register(messageUdp, ep);
                    break;
                case Command.Confirmation:
                    ConfirmMessageReceived(messageUdp.Id);
                    break;
            }

        }

        private void RelyMessage(MessageUdp messageUdp)
        {
            if (clients.TryGetValue(messageUdp.ToName, out T ep))
            {
                int id;

                using (MessageContext context = new MessageContext())
                {
                    var fromUser = context.Users.FirstOrDefault((u) => u.Name == messageUdp.FromName);
                    var toUser = context.Users.FirstOrDefault((u) => u.Name == messageUdp.ToName);
                    var messageDd = new Message()
                    {
                        Text = messageUdp.Text,
                        DateMessage = DateTime.Now,
                        IsReceived = false,
                        ToUser = toUser,
                        FromUser = fromUser,
                    };
                    context.Messages.Add(messageDd);
                    context.SaveChanges();
                    id = messageDd.Id;
                }

                var forwardMessageJson = new MessageUdp()
                {
                    Id = id,
                    Command = Command.Message,
                    ToName = messageUdp.ToName,
                    FromName = messageUdp.FromName,
                    Text = messageUdp.Text
                };

                messageSource.Send(forwardMessageJson, ep);

                Console.WriteLine($"Message Relied, from = {messageUdp.FromName}" +
                                  $" to = {messageUdp.ToName}");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }

        }

        void ConfirmMessageReceived(int? id)
        {
            Console.WriteLine("Message confirmation id=" + id);

            using (var ctx = new MessageContext())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.Id == id);

                if (msg != null)
                {
                    msg.IsReceived = true;
                    ctx.SaveChanges();
                }
            }
        }


        private void Register(MessageUdp messageUdp, T ip)
        {
            Console.WriteLine($"Message register, Name = {messageUdp.FromName}");
            clients.Add(messageUdp.FromName, ip);

            using (ChatDb.MessageContext context = new MessageContext())
            {
                if (context.Users.Any((u) => u.Name == messageUdp.FromName))
                {
                    return;
                }
                context.Add(new User() { Name = messageUdp.FromName });
                context.SaveChanges();
            }
        }


    }
}
