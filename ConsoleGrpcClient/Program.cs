using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using System.Diagnostics;
using WebAPI;

internal class Program
{

    private async static Task Foo()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:5001/Grpc");
        var client = new Books.BooksClient(channel);
        var collection = new List<Book>();
        var stopWatch = new Stopwatch();
        using (var stream = client.GetAllBooks(new BooksGetRequest() { Limit = 1 }))
        {
            stopWatch.Start();
            while (await stream.ResponseStream.MoveNext())
            {
                collection.Add(stream.ResponseStream.Current);
            }
            Console.WriteLine(stopWatch.ElapsedMilliseconds + " " + collection.Count);
            collection.Clear();
            Console.ReadLine();
        }
    }

    private static void Main(string[] args)
    {
        Foo().Wait();
    }
}