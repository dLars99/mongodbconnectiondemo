using System;
using MongoDB.Driver;

namespace MongoDbConnectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var un = "dLars99";
            var x = ":rL4ypGkObW75x0nO@cluster0";
            var db = "sample_airbnb";

            MongoClient dbClient = new MongoClient($"mongodb+srv://{un}{x}.yivaw.mongodb.net/{db}?retryWrites=true&w=majority");

            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of the databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
        }
    }
}
