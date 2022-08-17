using System;
using System.Threading;
using LesegaisParserTestJob.DataBase;
using LesegaisParserTestJob.Http;

namespace LesegaisParserTestJob;

internal static class Program
{
    private static readonly string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
                                                   $"AttachDbFilename={Environment.CurrentDirectory}\\DataBase\\Deals.mdf;" +
                                                   "Integrated Security=True;" +
                                                   "MultipleActiveResultSets=True";

    // Максимальное количество строк данных в запросе
    private const int RequestSize = 20000;

    // Время ожидания между парсингом (10 минут)
    private const int TimePause = 600000;

    private static void Main(string[] args)
    {
        var sql = new MSSQL(ConnectionString);
        var client = new LesegaisHttpClient();
        Console.WriteLine("Начинаю парсинг...");

        while (true)
        {
            Console.WriteLine("Для завершения работы нажмите любую кнопку");

            try
            {
                sql.Connection();
                var allSize = client.PostDealsCount(RequestSize, 0);
                double total = allSize.Total;
                var iterations = Convert.ToInt32(Math.Ceiling(total / RequestSize));

                Console.WriteLine($"Всего записей: {total}");

                for (var i = 0; i <= iterations; i++)
                {
                    sql.Bulk(client.PostDeal(RequestSize, i));

                    Console.WriteLine($"Страница {i}/{iterations} загружена");
                }

                Console.WriteLine($"Записей в базе {sql.GetDealsCount()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
                Console.ReadKey();
                break;
            }
            finally
            {
                sql.Disconnect();
            }

            Console.WriteLine($"Парсинг завершен, следущий парсинг через {TimePause / 60000} минут...");
            Thread.Sleep(TimePause);
        }
    }
}