using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Net_HW_1_1
{
    // 1.  Мы пишем простое чат-приложение способное передавать сообщения с компьютера на компьютер.
    //Начнем с разработки модели сообщений для нашего чата.
    //Договоримся что у каждого пользователя может быть свой ник-нейм - уникальное имя.
    //Ему можно будет передать сообщение, состоящее из даты, никнейма отправителя  и текста сообщения.
    //Как мог бы выглядеть класс представляющий сообщение в этом случае.

// 2. Убедившись что у нас есть все для отправки и получения сообщения напишем прообраз нашего чата.
//    Это будет утилита которая умеет работать как сервер или же как клиент в зависимости от
//    параметров командной строки.Сервер будет уметь отправлять сообщения тогда как клиент принимать.

// 3. Добавляем многопоточность в чат позволяя серверной части получать сообщения сразу от нескольких
//    респондентов.
//    Временно удалим из сервера возможность ввода сообщений.
//    Сделаем так чтобы чат всегда отвечал “Сообщение получено”.
//    Протестируем наш чат запустив сразу 10 клиентов.
//    Для удобства сделаем так чтобы клиент также ничего не запускал но просто слал привет.

/*
 Добавьте возможность ввести слово Exit в чате клиента, чтобы можно было завершить его работу. 
В коде сервера добавьте ожидание нажатия клавиши, чтобы также прекратить его работу.
*/
internal class Program
{
    public static void Server()
    {

        UdpClient udpServer = new UdpClient(12345);
        IPEndPoint localRemouteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Console.WriteLine("Ожидаем сообщение от пользователя:");
        Console.WriteLine("Чтобы завершить работу нажмите Enter");

        while (Console.ReadLine() == null)
        {
            try
            {
                byte[] buffer = udpServer.Receive(ref localRemouteEndPoint);
                string data = Encoding.ASCII.GetString(buffer);
                var message = Message.MessageFromJson(data);
                Console.WriteLine($"Получено сообщение от {message.NickName}," +
                $" время получения {message.DateMessage}, ");
                Console.WriteLine(message.TextMessage);
                string answer = "Сообщение получено";

                var answerMessage = new Message()
                {
                    DateMessage = DateTime.Now,
                    NickName = message.NickName,
                    TextMessage = answer
                };

                var answerData = answerMessage.MessageToJson();
                byte[] bytes = Encoding.ASCII.GetBytes(answerData);
                udpServer.Send(bytes, bytes.Length, localRemouteEndPoint);
                Console.WriteLine("Сообщение отпавлено (клиенту)!");

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
                
            }
        }

    public static void Client(string name, string ip)
    {
        UdpClient udpClient = new UdpClient();
        IPEndPoint localRemouteEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
        string message = "Привет!";
        var mess = new Message()
        {
            DateMessage = DateTime.Now,
            NickName = name,
            TextMessage = message
        };

        try
        {
            var data = mess.MessageToJson();
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            udpClient.Send(bytes, bytes.Length, localRemouteEndPoint);

            Console.WriteLine("Сообщение отпавлено!");

            byte[] buffer = udpClient.Receive(ref localRemouteEndPoint);
            data = Encoding.ASCII.GetString(buffer);
            var messageReception = Message.MessageFromJson(data);
            Console.WriteLine($"Получено сообщение от {messageReception.NickName}," +
            $" время получения {messageReception.DateMessage}, ");
            Console.WriteLine(messageReception.TextMessage);         
              
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
               
        }

        }

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Server();
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                new Thread( () => Client(args[0] + i, args[1]) ).Start();               
            }
             
                Console.WriteLine("Для завершения нажмите exit");
                while ( Console.ReadLine() != "exit" )
                {
                    
                }
                Environment.Exit(0);
         }
        //var message = new Message() { DateMessage = DateTime.Now,
        //                            NickName = "Nick",
        //                            TextMessage = "Hello"};
        //string mess = message.MessageToJson();

        //Console.WriteLine(mess);

        //var des = Message.MessageFromJson(mess);
    }
}
}