using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Project_F8.Logic.Login;

namespace Project_F8.Logic.Forum
{
    public class ForumWindowHandler
    {

        public static MainWindow Parent { get; set; }
        public static Int32 CurrentFolderType { get; set; }
        private Regex regex = new Regex(@"[0-9a-zA-Zа-яёА-ЯЁ \.\,\!\?\-_]+");
        private Int32 rootID = 1;
        private Int32 currentFolderID = 1;
        private String currentFolderPath = "./";
        private Stack<Int32> idStack = new Stack<Int32>();
        private Stack<String> pathStack = new Stack<String>();

        private static ForumWindowHandler instance = null;

        public ForumWindowHandler() { }

        public static ForumWindowHandler GetInstance()
        {
            if (instance == null)
                return instance = new ForumWindowHandler();
            else return instance;
        }

        public static void Show()
        {
            instance = ForumWindowHandler.GetInstance();
            Parent.entryToForumMove.Begin();
            Parent.forumGrid.IsEnabled = true;
            Parent.entryGrid.IsEnabled = false;
        }

        public void HandleRoomBoxDoubleClick(String name)
        {
            if (CurrentFolderType != 2)
            {
                Int32 newFolderID = 1;
                if (name == "root")
                {
                    newFolderID = rootID;
                    currentFolderPath = "./";
                }
                else
                {
                    newFolderID = ForumManager.getInstance().SearchIDbyName(name);
                    Parent.pathBox.AppendText(name.Substring(0, name.IndexOf("  (")) + '/');
                    currentFolderPath = Parent.pathBox.Text;
                }
                HandleDoubleClick(newFolderID,currentFolderPath);
            }
            else
            {
                Parent.GoToRoom(ForumManager.getInstance().SearchIDbyName(name),name);
            }
        }

        private void HandleDoubleClick(Int32 id,String path)
        {
            currentFolderID = id;
            idStack.Push(id);
            pathStack.Push(path);
            Parent.roomBox.Items.Clear();
            Connection.GetInstance().SendMessage("catalog|" + id + "|");
        }

        public void GetServerAnswer(String message)
        {
            String type = "";
            var matches = regex.Matches(message);
            switch (CurrentFolderType = Int32.Parse(matches[2].ToString()))
            {
                case 1:
                    type = "  (folder)";
                    break;
                case 2:
                    type = "  (room)";
                    break;
            }
            if (matches.Count > 0)
            {
                if (matches[2].Value == "1" || matches[2].Value == "2")
                {
                    ForumManager.getInstance().RememberFolders(matches);
                    foreach (String name in ForumManager.getInstance().GetNames())
                    {
                        Parent.roomBox.Items.Add(name+type);
                    }
                }
            }
        }

        public void HandleBackButtonClick()
        {
            if (idStack.Count>1)
            {
                currentFolderID = idStack.Pop();
                currentFolderID = idStack.Pop();
                currentFolderPath = pathStack.Pop();
                currentFolderPath = pathStack.Pop();
                HandleDoubleClick(currentFolderID,currentFolderPath);
                Parent.pathBox.Text = currentFolderPath;
            }
            else if (idStack.Count == 1)
            {
                currentFolderID = 1;
                currentFolderPath = "./";
                HandleDoubleClick(currentFolderID,currentFolderPath);
                //currentFolderPath = pathStack.Pop();
                Parent.pathBox.Text = currentFolderPath;
            }
        }

        public String[] HandleInformationMenuClick(String name)
        {
            return ForumManager.getInstance().GetFolderInfo(name);
        }

        public void HandleAddMenuClick(String[] info)
        {
            StringBuilder sendInfo = new StringBuilder();
            if (info != null)
            {
                switch (info[2])
                {
                    case "3": //adding room                        
                        sendInfo.Append("addRoom|");
                        sendInfo.Append(info[0] + '|' + info[1] + '|' + currentFolderID + '|');
                        break;
                    default: //adding folder
                        sendInfo.Append("addCatalog|");
                        sendInfo.Append(info[0] + '|' + info[1] + '|'+ info[2]+ '|' + currentFolderID + '|');
                        break;
                }
                Connection.GetInstance().SendMessage(sendInfo.ToString());
                currentFolderID = idStack.Pop();
                HandleDoubleClick(currentFolderID,currentFolderPath);
            }
        }

        public void SignOut()
        {
            currentFolderID = 1;
            Parent.addItem.IsEnabled = false;
            Parent.deleteItem.IsEnabled = false;
            Parent.changeItem.IsEnabled = false;
            LoginWindowHandler.Show(this);
        }

    }
}
