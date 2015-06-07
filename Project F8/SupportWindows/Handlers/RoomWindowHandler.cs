using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;

namespace Project_F8.SupportWindows.Handlers
{
    public class RoomWindowHandler
    {
        private Int32 id;
        private String name;

        private struct Post
        {
            public String user;
            public String date;
            public String content;
        }

        private List<Post> posts = new List<Post>();
        private Post post = new Post();
        private Regex postRegex = new Regex(@"[0-9a-zA-Zа-яёА-ЯЁ \.\,\!\?\-_\:]+");
        private Regex etcRegex = new Regex(@"[0-9a-zA-Z]+");
        public Int32 CurrentPage { get; set; }
        private Int32 pagesCount = 1;


        private Int32 myID;
        public Boolean isWaitingForPosts { get; set; }
        public Boolean isWaitingForRoomCount { get; set; }
        public Boolean isWaitingForUpdate { get; set; }


        public void SendStartMessages(Int32 myID)
        {
            this.myID = myID;
            GetPosts();
            Thread.Sleep(25);
            GetRoomCount();
            //this.Show();
        }

        public String GetName()
        {
            return name;
        }

        public Int32 GetID()
        {
            return myID;
        }

        public void GetPosts()
        {
            this.SendMessageToServer();
            isWaitingForPosts = true;
        }

        public void GetRoomCount()
        {
            this.SendMessageToServerForRoomCount();
            isWaitingForRoomCount = true;
        }


        public Boolean isCurrentName(String outerName)
        {
            return (outerName == name);
        }


        public RoomWindowHandler(Int32 id, String name)
        {
            this.myID = id;
            this.name = name;
            CurrentPage = 1;
            Console.WriteLine("{0}  {1} created", id, name);
        }

        private void RememberPosts(MatchCollection postsInfo)
        {
            posts.Clear();
            Int32 iteration = 2;

            while (iteration < postsInfo.Count)
            {
                post.content = postsInfo[iteration].Value;
                post.date = postsInfo[iteration + 1].Value;
                post.user = postsInfo[iteration + 2].Value;
                iteration += 3;
                posts.Add(post);
            }
        }

        public void ReceivePostFromServer(String serverAnswer)
        {
            var match = postRegex.Matches(serverAnswer);
            RememberPosts(match);
        }

        public List<String> fabricatePosts()
        {
            StringBuilder builder = new StringBuilder();
            List<String> retVal = new List<String>();
            foreach (Post mes in posts)
            {
                builder.Append(mes.date + " " + mes.user + " wrote:\n" + mes.content);
                retVal.Add(builder.ToString());
                builder.Clear();
            }
            return retVal;
        }

        public void SendMessageToServer()
        {
            Connection.GetInstance().SendMessage("room|" + myID + "|" + CurrentPage + "|");
        }

        public void SendMessageToServerForRoomCount()
        {
            Connection.GetInstance().SendMessage("pagesAmount|" + myID + "|");
        }

        public Int32 SavePagesCount(String answer)
        {
            var matches = etcRegex.Matches(answer);
            pagesCount = Int32.Parse(matches[1].ToString());
            return pagesCount;
        }

        public Boolean isMaxPage()
        {
            return (CurrentPage == pagesCount);
        }

        public void SetMaxPage()
        {
            CurrentPage = pagesCount;
        }

        public void AddMessage(String message)
        {
            Connection.GetInstance().SendMessage("roomPostAdd|" + myID + "|" + message + "|");
        }

        public Boolean isSuccessfullyAdded(String answer)
        {
            var match = postRegex.Matches(answer);
            //var match = etcRegex.Matches(answer);
            if (match[1].ToString() == "1") return true;
            return false;
        }
    }
}