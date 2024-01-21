
//RandomNumberInMemory
namespace Server.Singleton
{
    public class GuessedNumberDetail
    {
        public  int guessedNumber = 0;
        public bool isSomeOneGuess=false;
    }
    public class RandomNumberInMemory
    {
        private static RandomNumberInMemory _instance;
        static GuessedNumberDetail numDetail = new GuessedNumberDetail();
        public event Action<string> NumberGuess;

        private RandomNumberInMemory()
        {
            
        }
        public   int GetGuessNumber()
        {
            return numDetail.guessedNumber;
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
            lock (numDetail)
            {
                if (numDetail.guessedNumber==0)
                {
                    Random rand = new Random();
                    numDetail.guessedNumber = rand.Next(min, max);
                    Console.WriteLine($"Server has guessed :{numDetail.guessedNumber}");
                }
               

            }
        }
        public bool IsNumberMatch(string name,int number)
        {
            lock(numDetail)
            {
                if(numDetail.isSomeOneGuess==false && numDetail.guessedNumber == number)
                {
                    numDetail.isSomeOneGuess = true;
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
