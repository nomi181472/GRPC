using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Singleton
{
    public class RandomN
    {
        public  int guessedNumber = 0;
        public bool isSomeOneGuess=false;
    }
    public class RandomNumberInMemory
    {
        private static RandomNumberInMemory _instance;
        static RandomN random = new RandomN();
        public event Action<string> NumberGuess;

        private RandomNumberInMemory()
        {
            
        }
        public   int GetGuessNumber()
        {
            return random.guessedNumber;
        }
        public static RandomNumberInMemory GetInstance()
        {
            return _instance;
        }
        public static void SetInstance()
        {
            if (_instance == null)
            {
                _instance = new RandomNumberInMemory();
            }
        }
        public void GuessNumber(int min,int max)
        {
            lock (random)
            {
                if (random.guessedNumber==0)
                {
                    Random rand = new Random();
                    random.guessedNumber = rand.Next(min, max);
                    Console.WriteLine($"Server has guessed :{random.guessedNumber}");
                }
               

            }
        }
        public bool IsNumberMatch(string name,int number)
        {
            lock(random)
            {
                if(random.isSomeOneGuess==false && random.guessedNumber == number)
                {
                    random.isSomeOneGuess = true;
                    OnNumberGuessed(name);
                    Console.WriteLine($"{name} is winner");
                    return true;

                }
                return false;

            }
        }


        private void OnNumberGuessed(string name)
        {
            NumberGuess?.Invoke(name);
        }
    }
}
