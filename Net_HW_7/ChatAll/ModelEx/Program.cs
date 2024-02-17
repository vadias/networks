using Microsoft.EntityFrameworkCore;
using ModelsEx.Models;

namespace ConsoleApp2
{
    public class Program
    {

        // 1. Итак, мы собираемся добавить поддержку работы с базой данных в наше приложение чата.
        //Давайте разработаем для нее модель. Создадим новый проект - это будет серверное приложение. 
        //В проекте мы будем использовать CodeFirst подход.Начнем с двух таблиц - Messages и Users.
        //В Messages должны храниться сообщения, тогда как в users список пользователей. 
        //Разработайте модель таким образом чтобы учесть что в сообщениях есть не только автор но и адресат и статус получениям им сообщения.
        static void Main(string[] args)
        {
          /*  var optionsBuilder = new DbContextOptionsBuilder<MessageContext>()
                                    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Chat;Trusted_Connection=True")
                                    .UseLazyLoadingProxies();


            using (var ctx = new MessageContext(optionsBuilder.Options))
            {
                
                
            }*/
        }
    }
}