using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GrpcInterface;

var channel = GrpcChannel.ForAddress("https://localhost:7190");
var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "나는 천재" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();