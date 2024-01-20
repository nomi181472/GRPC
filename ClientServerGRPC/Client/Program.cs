using Grpc.Core;
using GuessNumber;
using HelloWord;
using ServerStreaming;


internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please enter your name.");
        }
        else
        {


            int port = 5555;
            string url = $"localhost:{port}";
            Random rand = new Random();

            // ChannelCredentials.Insecure: it disables transport security (TLS/SSL) and sends data in plaintext. 
            Channel channel = new Channel(url, ChannelCredentials.Insecure)
            {
            };

            try
            {
                await channel.ConnectAsync();
                string name = args[0];

                Console.WriteLine($"Client: {name} Connected to server");

                var client = new GuessGameService.GuessGameServiceClient(channel);
                var duplex = client.Play();
                var _ = Task.Run(async () =>
                {
                    while (await duplex.ResponseStream.MoveNext())
                    {
                        Thread.Sleep(1000);
                        if (!String.IsNullOrEmpty(duplex.ResponseStream.Current.Report))
                        {
                            Console.WriteLine(duplex.ResponseStream.Current.Report);
                        }

                        if (duplex.ResponseStream.Current.Exit)
                        {
                            await duplex.RequestStream.CompleteAsync();
                            break;
                        }
                        else if (duplex.ResponseStream.Current.Start)
                        {
                            int number = rand.Next(duplex.ResponseStream.Current.Min, duplex.ResponseStream.Current.Max);
                            Console.WriteLine($"{name} guessed {number}");
                            await duplex.RequestStream.WriteAsync(new PlayGame()
                            {
                                Name = name,
                                Number = number,
                                IsJoinedRequest = false
                            });
                        }






                    }
                });

                await duplex.RequestStream.WriteAsync(new PlayGame()
                {
                    Name = name,
                    Number = 0,
                    IsJoinedRequest = true
                });


                Console.ReadLine();

            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error has been thrown: {ex.Message}");
            }
            finally
            {
                if (channel is not null)
                {
                    Console.WriteLine("Cient is shutting down");
                    await channel.ShutdownAsync();
                }
            }
        }
    }
    static async Task ServerSideStreaming()
    {
        int port = 5555;
        string url = $"localhost:{port}";
        // ChannelCredentials.Insecure: it disables transport security (TLS/SSL) and sends data in plaintext. 
        Channel channel = new Channel(url, ChannelCredentials.Insecure)
        {
        };

        try
        {
            await channel.ConnectAsync();
            Console.WriteLine("Connected to server");
            var client = new ServerStreamingService.ServerStreamingServiceClient(channel);
            var response = client.GetParagraphs(new ShortParagraphRequest()
            {
                Sentence = "write a paragraph on grpc and also compare  rest vs grpc?"
            });

            while (await response.ResponseStream.MoveNext())
            {

                Console.Write(response.ResponseStream.Current.Line);

            }
            Console.ReadLine();
        }
        catch (Exception ex)
        {

            Console.WriteLine($"An error has been thrown: {ex.Message}");
        }
        finally
        {
            if (channel is not null)
            {
                Console.WriteLine("Cient is shutting down");
                await channel.ShutdownAsync();
            }
        }
    }
    static async Task HelloWorldTest()
    {
        int port = 5555;
        string url = $"localhost:{port}";
        // ChannelCredentials.Insecure: it disables transport security (TLS/SSL) and sends data in plaintext. 
        Channel channel = new Channel(url, ChannelCredentials.Insecure)
        {
        };

        try
        {
            await channel.ConnectAsync();
            Console.WriteLine("Connected to server");
            var client = new HelloService.HelloServiceClient(channel);
            var response = await client.SayHelloAsync(new HelloRequest() { Name = "Noman" });
            Console.WriteLine(response);
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error has been thrown: {ex.Message}");
        }
        finally
        {
            if (channel is not null)
            {
                Console.WriteLine("Cient is shutting down");
                await channel.ShutdownAsync();
            }
        }
    }
}