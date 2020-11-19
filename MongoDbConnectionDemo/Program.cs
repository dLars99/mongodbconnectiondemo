using System;
using MongoDB.Driver;

namespace MongoDbConnectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://dLars99:rL4ypGkObW75x0nO@cluster0.yivaw.mongodb.net/sample_airbnb?retryWrites=true&w=majority");

            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of the databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
        }
    }
}
