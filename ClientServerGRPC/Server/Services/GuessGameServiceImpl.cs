
using Grpc.Core;
using GuessNumber;
using Server.Singleton;


namespace Server.Services
{
    public class GuessGameServiceImpl: GuessGameService.GuessGameServiceBase
        
    {
        
        int min = 10;
        int max = 20;
        int trials = 5;
        int totalPlayers = 3;
        public GuessGameServiceImpl()
        {
            UserInMemoryDB.SetInstance();
            RandomNumberInMemory.SetInstance();
        }
        public override async Task Play(IAsyncStreamReader<PlayGame> requestStream, IServerStreamWriter<GameSync> responseStream, ServerCallContext context)
        {

           

           
            UserInMemoryDB.GetInstance().SetTotalUsers(totalPlayers);
            // Subscribe to the event // obeservable pattern can also be use here.
            SubcribeOnUserListUpdated(requestStream, responseStream, min, max, totalPlayers); //notify to all clients if some members join the game
            SubscribeOnAllUsersJoined(responseStream, min, max); //notify to clients if all players joined
            SubscribeOnUserGuessTheNumber(requestStream, responseStream, min, max);//notify to all clients if someone win the game.

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
                    Max = max,
                    Min = min,
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
                        Max = max,
                        Min = min,
                        Report = $"{key} has joined the game. Members:{UserInMemoryDB.GetInstance().GetLength()}/{totalPlayers}",
                       
                    });
                }


            };
        }

    }
}
