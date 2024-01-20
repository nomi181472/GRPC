using Google.Protobuf;
using Grpc.Core;
using GuessNumber;
using Server.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class GuessGameServiceImpl: GuessGameService.GuessGameServiceBase
        
    {
        public GuessGameServiceImpl()
        {
            UserInMemoryDB.SetInstance();
            RandomNumberInMemory.SetInstance();
        }
        public override async Task Play(IAsyncStreamReader<PlayGame> requestStream, IServerStreamWriter<GameSync> responseStream, ServerCallContext context)
        {

            Random rand = new Random();
            int min = 10;
            int max = 20;
            int trials = 5;
            int totalPlayers = 4;

           
            UserInMemoryDB.GetInstance().SetTotalUsers(totalPlayers);
            // Subscribe to the event // obervable pattern can also be use here.
            SubcribeOnUserListUpdated(requestStream, responseStream, min, max, totalPlayers);
            SubscribeOnAllUsersJoined(responseStream, min, max);
            SubscribeOnUserGuessTheNumber(requestStream, responseStream, min, max);

            while (await requestStream.MoveNext())
            {
                if (requestStream.Current.IsJoinedRequest)
                {
                    await responseStream.WriteAsync(new GameSync()
                    {
                        Start = false,
                        Exit = false,
                        Min = min,
                        Max = max,
                        Report = "Request Accepted",

                    });
                    UserInMemoryDB.GetInstance().AddName(requestStream.Current.Name, trials);
                    Console.WriteLine($"{requestStream.Current.Name} has joined the game, Members:{UserInMemoryDB.GetInstance().GetLength()}/{totalPlayers}");



                }
                else
                {
                    if (!RandomNumberInMemory.GetInstance().IsNumberMatch(requestStream.Current.Name, requestStream.Current.Number))
                    {
                        var user = UserInMemoryDB.GetInstance().GetUser(requestStream.Current.Name);


                        if (user?.TrialRemaining == 0)
                        {
                            await responseStream.WriteAsync(new GameSync()
                            {
                                Start = true,
                                Exit = true,
                                Min = min,
                                Max = max,
                                Report = $"Trial Remaining is 0, Game Over.",

                            });
                        }
                        else
                        {
                            user.TrialRemaining--;
                            UserInMemoryDB.GetInstance().Update(user);
                            await responseStream.WriteAsync(new GameSync()
                            {
                                Start = true,
                                Exit = false,
                                Min = min,
                                Max = max,
                                Report = $"Trials Remaining: {user.TrialRemaining}.",

                            });
                        }

                    }

                }
            }

            return;
        }

        private static void SubscribeOnUserGuessTheNumber(IAsyncStreamReader<PlayGame> requestStream, IServerStreamWriter<GameSync> responseStream, int min, int max)
        {
            RandomNumberInMemory.GetInstance().NumberGuess += async (name) =>
            {
                if (requestStream.Current.Name == name)
                {
                    await responseStream.WriteAsync(new GameSync()
                    {
                        Start = true,
                        Exit = true,
                        Min = min,
                        Max = max,
                        Report = "You Win the Game.",
                     

                    });
                }
                else
                {
                    await responseStream.WriteAsync(new GameSync()
                    {
                        Start = true,
                        Exit = true,
                        Min = min,
                        Max = max,
                        Report = $"You loss the game, Winner is {name}.",
                       

                    });
                }
            };
        }

        private static void SubscribeOnAllUsersJoined(IServerStreamWriter<GameSync> responseStream, int min, int max)
        {
            UserInMemoryDB.GetInstance().AllUsersJoined += async () =>
            {
                await responseStream.WriteAsync(new GameSync()
                {
                    Start = true,
                    Exit = false,
                    Max = 1,
                    Min = 0,
                    Report = "Game has been started",
                    
                });
                RandomNumberInMemory.GetInstance().GuessNumber(min, max);
            };
        }

        private static void SubcribeOnUserListUpdated(IAsyncStreamReader<PlayGame> requestStream, IServerStreamWriter<GameSync> responseStream, int min, int max, int totalPlayers)
        {
            UserInMemoryDB.GetInstance().ItemAdded += async (key) =>
            {
                if (!requestStream.Current.Name.Equals(key))
                {

                    await responseStream.WriteAsync(new GameSync()
                    {
                        Start = false,
                        Exit = false,
                        Max = 1,
                        Min = 0,
                        Report = $"{key} has joined the game. Members:{UserInMemoryDB.GetInstance().GetLength()}/{totalPlayers}",
                       
                    });
                }


            };
        }

    }
}
