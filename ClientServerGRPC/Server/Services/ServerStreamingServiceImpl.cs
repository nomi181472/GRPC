using Grpc.Core;
using ServerStreaming;

namespace Server.Services
{
    public class ServerStreamingServiceImpl:ServerStreamingService.ServerStreamingServiceBase
    {
        public override async Task GetParagraphs(ShortParagraphRequest request, IServerStreamWriter<ShortParagraphResponse> responseStream, ServerCallContext context)
        {
            string[] paragraph=GetParagph();
            
            foreach (var line in paragraph)
            {
                foreach (var cha in line)
                {
                    await responseStream.WriteAsync(new ShortParagraphResponse()
                    {
                        Line = cha.ToString(),
                    });
                    Thread.Sleep(10);
                }
                await responseStream.WriteAsync(new ShortParagraphResponse()
                {
                    Line = "\n"
                });

            }

            return;
        }

        private string[] GetParagph()
        {
           
             string[] paragraph =
                        {
                "gRPC, or Remote Procedure Call, is an open-source framework developed by Google that facilitates efficient communication between distributed systems.",
                "It employs the Protocol Buffers serialization format," ,
                " providing a binary serialization that is more compact and faster than the traditional JSON format used by REST.",
                "One of the key strengths of gRPC is its support for bi-directional streaming," ,
                " allowing for simultaneous communication between client and server." ,
                "Automated code generation simplifies the development process by creating client and server code,",
                " reducing the likelihood of errors.",
                "When comparing gRPC with REST, ",
                "it's evident that gRPC offers superior performance due to its efficient binary serialization,",
                "multiplexing, and support for streaming,",
                "making it well-suited for scenarios where low latency and high performance are crucial." ,
                "REST, while widely adopted and simpler to understand,",
                " can be less efficient in terms of data size and lacks built-in support for bidirectional streaming." ,
                "The choice between gRPC and REST depends on the specific requirements of a project,",
                " with gRPC excelling in performance-critical and complex distributed systems.",
            };
            return paragraph;
        }
    }
}
