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

    // Мы собираемся добавить поддержку работы с базой данных в наше
    // приложение чата.
    // Давайте разработаем для нее модель.
    // Создадим новый проект - это будет серверное приложение.
    // В проекте мы будем использовать CodeFirst подход.
    // Начнем с двух таблиц - Messages и Users.
    // В Messages должны храниться сообщения, тогда как в users список
    // пользователей.
    // Разработайте модель таким образом чтобы учесть что в сообщениях есть не только автор но и
    // адресат и статус получениям им сообщения.
    public class Server
    {
        private UdpClient udpClient;
        private IPEndPoint ip;
        private Dictionary<String, IPEndPoint> clients = new Dictionary<String, IPEndPoint>();

        IMessageSource messageSource;

        bool work = true;

        //Токен для остановки сервера
        static private CancellationTokenSource cts = new CancellationTokenSource();

        static private CancellationToken ct = cts.Token;


        public Server(IMessageSource source)
        {
            messageSource = source;
        }

        public void Work()
        {
            udpClient = new UdpClient(12345);
            ip = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Клиент ожидает сообщение.");


            while (work)
            {

                var message = messageSource.Receive(ref ip);
                ProcessMessage(message);

            }


           
        }

        public void Stop() 
        {
            work = false; 
        }


        private void ProcessMessage(MessageUdp messageUdp)
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
                    Register(messageUdp);
                    break;
                case Command.Confirmation:
                    ConfirmMessageReceived(messageUdp.Id);
                    break;
            }

        }

        private void RelyMessage(MessageUdp messageUdp)
        {
            if (clients.TryGetValue(messageUdp.ToName, out IPEndPoint ep))
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


        private void Register(MessageUdp messageUdp)
        {
            Console.WriteLine($"Message register, Name = {messageUdp.FromName}");
            clients.Add(messageUdp.FromName, ip);

            using (MessageContext context = new MessageContext())
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
