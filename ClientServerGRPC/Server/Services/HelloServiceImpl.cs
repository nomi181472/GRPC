//HelloServiceImpl.cs
using Grpc.Core;
using HelloWord;


namespace Server.Services
{
    public class HelloServiceImpl : HelloService.HelloServiceBase
    {
        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse() { Description = $" Hello {request.Name} " });
        }
    }
}