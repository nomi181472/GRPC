
using GuessNumber;
using HelloWord;
using Server.Services;
using ServerStreaming;
using G = Grpc.Core;

await HelloWorldTest();

static async Task HelloWorldTest()
{
    int port = 5555;
    string host = "localhost";
    G.Server server = new G.Server()
    {
        Ports = { new G.ServerPort(host, port, G.ServerCredentials.Insecure
    ) },
        Services = 
        { 
            HelloService.BindService(new HelloServiceImpl()), 
            ServerStreamingService.BindService(new ServerStreamingServiceImpl()),
            GuessGameService.BindService(new GuessGameServiceImpl())
        },

    };
    try
    {
        server.Start();
        Console.WriteLine($"Server is listening to port:{port}");
        Console.ReadLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error has been thrown: {ex.Message}");
    }
    finally
    {
        if (server is not null)
        {
            Console.WriteLine("shutting down");
            await server.ShutdownAsync();
        }
    }
}