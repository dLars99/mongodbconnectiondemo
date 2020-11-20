using System;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDbConnectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var un = "dLars99";
            // Paste in password before use
            var pw = "";
            var connDb = "sample_training";

            MongoClient dbClient = new MongoClient($"mongodb+srv://{un}{pw}@cluster0.yivaw.mongodb.net/{connDb}?retryWrites=true&w=majority");

            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of the databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }

            // Connect to the appropriate database
            var database = dbClient.GetDatabase("sample_training");
            // Select the desired collection
            var collection = database.GetCollection<BsonDocument>("grades");

            // CREATE
            var document = new BsonDocument
            {
                { "student_id", 10000 },
                { "scores", new BsonArray
                    {
                        new BsonDocument{ { "type", "exam"}, { "score", 88.12334193287023 } },
                        new BsonDocument{ { "type", "quiz"}, { "score", 74.92381029342834 } },
                        new BsonDocument{ { "type", "homework"}, { "score", 89.97929384290324 } },
                        new BsonDocument{ { "type", "homework"}, { "score", 82.12931030513218 } },
                    }
                },
                { "class_id", 480 }
            };

            collection.InsertOne(document);

            // READ
            // This returns the first document in the collection
            var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();
            Console.WriteLine(firstDocument.ToString());

            // Reading with a filter: create a filter that matches a student_id
            var filter = Builders<BsonDocument>.Filter.Eq("student_id", 10000);
            // Pass that filter into a Find command
            var studentDocument = collection.Find(filter).FirstOrDefault();
            Console.WriteLine(studentDocument.ToString());

            // Read all/multiple documents
            var documents = collection.Find(new BsonDocument()).ToList();
            // With filters - returns documents with a score within the scores array >= 95
            var highExamScoreFilter = Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
                "scores", new BsonDocument { {  "type", "exam" },
                    { "score", new BsonDocument { {  "$gte", 95 } } }
            });
            // Iterate through results
            var cursor = collection.Find(highExamScoreFilter).ToCursor();
            foreach (var matchDocument in cursor.ToEnumerable())
            {
                Console.WriteLine(matchDocument);
            }

            // Sorting results
            var sort = Builders<BsonDocument>.Sort.Descending("student_id");
            var highestScores = collection.Find(highExamScoreFilter).Sort(sort);
            // Append .First() to get the top result
            var highestScore = collection.Find(highExamScoreFilter).Sort(sort).First();

            // UPDATE
            // Find the record to update
            filter = Builders<BsonDocument>.Filter.Eq("student_id", 10000);
            // Set the updated field
            var update = Builders<BsonDocument>.Update.Set("class_id", 483);
            // Make the change to the collection
            collection.UpdateOne(filter, update);

            // Match multiple records as an array
            var arrayFilter = Builders<BsonDocument>.Filter.Eq("student_id", 10000) & Builders<BsonDocument>.Filter.Eq("scores.type", "quiz");
            // Set the specific item using the positional $ operator
            var arrayUpdate = Builders<BsonDocument>.Update.Set("scores.$.score", 84.92381029342834);
            // Complete the update
            collection.UpdateOne(arrayFilter, arrayUpdate);

            // DELETE
            // Filter the record to delete
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("student_id", 10000);
            // Then, delete it
            collection.DeleteOne(deleteFilter);

            // Multiple deletes - drop all students with exams containing a score which is less than 60
            var deleteLowExamFilter = Builders<BsonDocument>.Filter.ElemMatch<BsonValue>("scores",
                new BsonDocument { { "type", "exam" }, { "score", new BsonDocument { { "$lt", 60 } } }
            });

            collection.DeleteMany(deleteLowExamFilter);
        }
    }
}
