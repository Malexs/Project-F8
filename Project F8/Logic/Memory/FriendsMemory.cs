using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Project_F8.Logic.Memory
{
    [Serializable]
    public class FriendsMemory
    {
        private List<String> friends = new List<String>();
        private String userME;
        String path;

        public FriendsMemory(String userME)
        {
            this.userME = userME;
            path = userME + "FriendMemory.bin";
        }

        public void SaveMemoryToDisk()
        {
            if (friends.Count > 0)
            {
                FileStream stream = File.Create(path);
                //Serialization
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, friends);
                stream.Close();
            }
        }

        public void GetMemoryFromDisk()
        {
            if (File.Exists(path))
            {
                try
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryFormatter bf = new BinaryFormatter();
                    friends = (List<String>)bf.Deserialize(fs);
                    fs.Close();
                }
                catch
                {
                    friends = new List<String>();
                }
            }
        }

        public void PutUserToFriends(String name)
        {
            Boolean isExist = false;
            foreach (String friend in friends)
            {
                if (friend == name)
                {
                    isExist = true;
                }
            }
            if (!isExist)
                friends.Add(name);
        }

        public void DeleteUserFromFriends(String name)
        {
            if (friends.Count > 0)
            {
                List<String> procList = friends;
                foreach (String friend in procList)
                {
                    if (friend == name) procList.Remove(friend);
                    break;
                }
                friends = procList;
            }
        }

        public List<String> GetListOfFriends()
        {
            return friends;
        }
    }
}
