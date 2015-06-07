using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;

namespace Project_F8.SupportWindows.Handlers
{
    public class DialogHandler
    {
        private Regex messageRegex = new Regex(@"[0-9a-zA-Zа-яёА-ЯЁ \@\#\$\%\^\&\*\(\)\/\`\~\;\.\,\!\?\-_\:]+");
        private String userName; //With how we have a dialog
        private String userME; //current
        public Boolean IsWaitingForAnswer { get; set; }
        private List<Message> messageMemory = new List<Message>();
        private Message message;

        private struct Message
        {
            public String text;
            public String date;
            public String sender;
        }

        public DialogHandler(String userName, String userME) 
        {
            this.userME = userME;
            this.userName = userName;
            IsWaitingForAnswer = false;
        }

        public void SendStartMessage()
        {
            Connection.GetInstance().SendMessage("privateHistory|" + userName + "|20|");
            IsWaitingForAnswer = true;
        }

        public void SaveMessageHistory(String serverAnswer)
        {
            var match = messageRegex.Matches(serverAnswer);
            for (Int32 iteration = 2; iteration<match.Count; iteration+=3)
            {
                message.text = match[iteration].ToString();
                message.date = match[iteration+1].ToString();
                message.sender = match[iteration+2].ToString();
                messageMemory.Add(message);
            }
        }

        public List<String> GetMessagesToPost()
        {
            StringBuilder item = new StringBuilder();
            List<String> result = new List<String>();
            foreach (Message messageItem in messageMemory)
            {
                item.Append(messageItem.date + "  " + messageItem.sender + " wrote:\n" + messageItem.text);
                result.Add(item.ToString());
                item.Clear();
            }
            return result;
        }

        public void SendUsersMessage(String text)
        {
            message.text = text;
            message.date = DateTime.Now.ToString();
            message.sender = userME;
            messageMemory.Add(message);
            Connection.GetInstance().SendMessage("private|"+userName+"|"+text+"|");
        }

        public Boolean IsRightDialog(String serverAnswer)
        {
            var match = messageRegex.Matches(serverAnswer);
            if (userName == match[1].ToString())
                return true;
            else return false;
        }

        public Boolean IsRightName(String name)
        {
            return name == userName;
        }

        public void ReceiveMessage(String serverAnswer)
        {
            var match = messageRegex.Matches(serverAnswer);
            message.sender = match[1].ToString();
            message.text = match[2].ToString();
            message.date = match[3].ToString();
            messageMemory.Add(message);
        }

        public String GetName()
        {
            return userName;
        }

    }
}
