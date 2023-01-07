using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using WebAPI;

internal class Program
{

    private async static Task Foo()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:5001/Grpc");
        var client = new Books.BooksClient(channel);
        var collection = new List<Book>();
        using (var stream = client.GetAllBooks(new BooksGetRequest() { Limit = 10 }))
        {
            while (await stream.ResponseStream.MoveNext())
            {
                collection.Add(stream.ResponseStream.Current);
            }
        }
    }

    private static void Main(string[] args)
    {
        Foo().Wait();
    }
}