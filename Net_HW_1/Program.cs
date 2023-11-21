// Попробуйте переработать приложение, добавив подтверждение об отправке сообщений как в сервер, так и в клиент.

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Net_HW_1;
class Program
{

    static void Server(string name)
    {
        try
        {
            UdpClient udpClient;
            IPEndPoint remoteEndPoint;

            udpClient = new UdpClient(12345);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("UDP Клиент ожидает сообщений...");

            while (true)
            {
                byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedData = Encoding.ASCII.GetString(receiveBytes);

                var message = Message.FromJson(receivedData);

                Console.WriteLine($"Получено сообщение от {message.FromName} ({message.Date}):");
                Console.WriteLine(message.Text);

                Console.Write("Введите ответ и нажмите Enter: ");
                string replyMessage = Console.ReadLine();

                var replyMessageJson = new Message() { Date = DateTime.Now, FromName = name, Text = replyMessage }.ToJson();

                byte[] replyBytes = Encoding.ASCII.GetBytes(replyMessageJson);

                udpClient.Send(replyBytes, replyBytes.Length, remoteEndPoint);
                Console.WriteLine("Ответ отправлен.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
        }
    }


    static void Client(string name, string ip)
    {
        UdpClient udpClient;
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);

        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);

        while (true)
        {
            try
            {
                Console.WriteLine("UDP Клиент ожидает ввода сообщения");

                Console.Write("Введите сообщение и нажмите Enter: ");
                string message = Console.ReadLine();

                var messageJson = new Message() { Date = DateTime.Now, FromName = name, Text = message }.ToJson();

                byte[] replyBytes = Encoding.ASCII.GetBytes(messageJson);

                udpClient.Send(replyBytes, replyBytes.Length, remoteEndPoint);
                Console.WriteLine("Сообщение отправлено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
            }

            byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
            string receivedData = Encoding.ASCII.GetString(receiveBytes);
            var messageReceived = Message.FromJson(receivedData);

            Console.WriteLine($"Получено сообщение от {messageReceived.FromName} ({messageReceived.Date}):");
            Console.WriteLine(messageReceived.Text);
        }
    }


    static void Main(string[] args)
    {
        if (args.Length == 1)
        {
            Console.WriteLine(string.Join(" ", args));
            Server(args[0]);
        }
        else
        if (args.Length == 2)
        {
            Client(args[0], args[1]);
        }
        else
        {
            Console.WriteLine("Для запуска сервера введите ник-нейм как параметр запуска приложения");
            Console.WriteLine("Для запуска клиента введите ник-нейм и IP сервера как параметры запуска приложения");
        }dotnet run
    }
}


public class Message
{
    public DateTime Date { get; set; }
    public string Text { get; set; }

    public required string FromName { get; set; }
    // Метод для сериализации в JSON
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    // Статический метод для десериализации JSON в объект MyMessage
    public static Message FromJson(string json)
    {
        return JsonSerializer.Deserialize<Message>(json);
    }
}
