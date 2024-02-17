using ModelsEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModelsEx.Abstraction;

//Сегодня мы продолжим работу над ним, а также напишем немного тестов.
//Начнем с разработки клиентского приложения.
//Возьмем нашего старого клиента и модифицируем его код так чтобы оно стало поддерживать наш новый протокол,
//после чего адаптируем его и сервер под тесты и наконец реализуем



namespace ModelsEx.Services
{

    /*
     1. Сегодня мы продолжим работу над ним, а также напишем немного тестов. 
     Начнем с разработки клиентского приложения. 
    Возьмем нашего старого клиента и модифицируем его код так чтобы оно стало 
    поддерживать наш  новый протокол, после чего адаптируем его и сервер под 
    тесты и наконец  реализуем '


     2. Подумаем как бы мы могли протестировать сервер. 
    Очевидно что источником данных для сервера является UDP 
    соединение и заменив источник данных мы смогли бы точно
    выяснить пересылает ли сервер сообщения.  
    Давайте перепишем код сервера таким образом чтобы он 
    получал сообщения от интерфейса IMessageSource и его же использовал для отправки
    */

    public class Client
    {
        IPEndPoint ipServer;
        UdpClient udpClient;

        private readonly string _adress;
        private readonly int _port;
        private readonly string _name;

        IMessageSource messageSource;

        bool work = true;

        public Client(IMessageSource source, string adress, int port, string name)
        {
            _adress = adress;
            _port = port;
            _name = name;
            messageSource = source;
            udpClient = new UdpClient(_port);
            ipServer = new IPEndPoint(IPAddress.Parse(_adress), _port);

        }

        public void Start()
        {
            new Thread(() => { Listener(); }).Start();
            Sender();
        }

        public void Stop()
        {
            work = false;
        }

        public void Listener()
        {
            while (work)
            {
                try
                {
                    var message = messageSource.Receive(ref ipServer);
                    Console.WriteLine("Получено сообщение от: " + message.FromName);
                    Console.WriteLine(message.Text);

                    Confirmation();

                }
                catch (Exception e)
                {

                    Console.WriteLine("Возникло исключение: " + e);
                }

            }
        }

        public void Sender()
        {


            while (work)
            {
                try
                {
                    Console.WriteLine("Ожидается ввод сообщения:");
                    var text = Console.ReadLine();
                    Console.WriteLine("Введите имя получателя:");
                    var toName = Console.ReadLine();

                    MessageUdp message = new MessageUdp()
                    {
                        Command = Command.Message,
                        ToName = toName,
                        FromName = _name,
                        Text = text
                    };

                    messageSource.Send(message, ipServer);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка при обработке сообщения : " + e);
                }

            }


        }
        public void Register()
        {
            MessageUdp message = new MessageUdp()
            {
                Command = Command.Register,
                ToName = null,
                FromName = _name,
                Text = null
            };
            messageSource.Send(message, ipServer);
        }

        public void Confirmation()
        {
            MessageUdp message = new MessageUdp()
            {
                Command = Command.Confirmation,
                ToName = null,
                FromName = _name,
                Text = null
            };
            messageSource.Send(message, ipServer);
        }
    }
}