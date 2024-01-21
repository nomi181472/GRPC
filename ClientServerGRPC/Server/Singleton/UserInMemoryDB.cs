//UsersInMemoryDB.cs
using System.Collections.Concurrent;


namespace Server.Singleton
{
    public class UserDetails
    {
        public  string   Name { get; set; }=String.Empty;
        public int TrialRemaining { get; set; }
        public bool IsWin=false;
    }
    public class UserInMemoryDB
    {
        private  static  UserInMemoryDB instance;

        ConcurrentDictionary<string, UserDetails> userNames;
     
        public event Action<string>? ItemAdded;
        public event Action? AllUsersJoined;

        int totalUsers = 0;
        private UserInMemoryDB()
        {
           userNames = new ConcurrentDictionary<string, UserDetails>();
        }
        public static UserInMemoryDB GetInstance() => instance;
        public void SetTotalUsers(int pTotalUsers)
        {
            totalUsers = pTotalUsers;
        }

        public static void SetInstance()
        {
            if (instance == null)
            {
                instance= new UserInMemoryDB();
            }
        }
        public UserDetails? GetUser(string name)
        {
             if (userNames.TryGetValue(name,out var user)){
                return user;
            }
             return null;
        }
        public bool Update(UserDetails details)
        {
            
            return userNames.TryUpdate(details.Name, details,GetUser(details.Name)??new UserDetails() { });
        }
       
        public bool AddName(string name,int trials)
        {
            
            var result= userNames.TryAdd(name, new UserDetails() { IsWin = false, Name = name, TrialRemaining = trials });
            if (result)
            {
                OnItemAdded(name, name);
                if (userNames.Count == totalUsers)
                {
                    OnAllUsersJoined();
                }
            }
            return result;

        }
        public int GetLength()
        {
            return userNames.Count;
        }
        private void OnItemAdded(string key, string value)
        {
            ItemAdded?.Invoke(key);
        }
        private void OnAllUsersJoined()
        {
            AllUsersJoined?.Invoke();
        }


    }
}
