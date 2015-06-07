using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

using Project_F8.Logic.SignUp;
using Project_F8.Logic.Login;
using Project_F8.Logic.Forum;
using Project_F8.SupportWindows;

namespace Project_F8.Logic
{
    public class Observer
    {

        private static Observer instance = null;
        private MainWindow window = null;
        private Thread thr = null;
        private String answer,tmpAnswer;
        delegate void processHandler(Object stateInfo);
        private processHandler handler;

        public Observer() 
        {
        }

        public static Observer GetInstance()
        {
            if (instance == null)
                return instance = new Observer();
            else return instance;
        }

        public void ActivateThread(MainWindow window)
        {
            this.window = window;

            if (thr == null)
            {
                thr = new Thread(StartSocketReader);
                thr.Start();
            }
        }

        private void StartSocketReader()
        {
            while (true)
            {
                lock (Connection.GetInstance())
                {
                    tmpAnswer = Connection.GetInstance().Receive();
                }
                if (tmpAnswer != "" && tmpAnswer != null)
                {
                    answer = tmpAnswer;
                    switch (getMessageType(answer))
                    {
                        case 1:
                            handler = new processHandler(registrationMessageCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break;
                        case 2:
                            handler = new processHandler(loginMessageCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break;
                        case 3:
                            handler = new processHandler(forumMessageCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break;
                        case 4:
                            handler = new processHandler(roomPostsCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break;
                        case 5:
                            handler = new processHandler(roomPageCountCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break;
                        case 6:
                            handler = new processHandler(messageAddedResponseCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break;
                        case 7:
                            handler = new processHandler(privateHistoryCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break; 
                        case 8:
                            handler = new processHandler(privateMessageCatch);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(handler));
                            break;
                        case 0:
                            System.Console.WriteLine(answer);
                            break;
                    }
                }

                Thread.Sleep(5);
            }
        }

        private Int32 getMessageType(String message)
        {
            if (message!=null)
            {
                if (message.Contains("registrationRespone") && message.IndexOf("Respone")<message.IndexOf("|"))
                    return 1;
                if (message.Contains("credentialsRespone") && message.IndexOf("Respone") < message.IndexOf("|"))
                    return 2;
                if (message.Contains("catalogRespone") && message.IndexOf("Respone") < message.IndexOf("|"))
                    return 3;
                if (message.Contains("roomRespone") && message.IndexOf("Respone") < message.IndexOf("|"))
                    return 4;
                if (message.Contains("pageAmountRespone") && message.IndexOf("Respone") < message.IndexOf("|"))
                    return 5;
                if (message.Contains("roomPostAddRespone") && message.IndexOf("Respone") < message.IndexOf("|"))
                    return 6;
                if (message.Contains("privateHistory") && message.IndexOf("History") < message.IndexOf("|"))
                    return 7;
                if (message.Contains("privateMessage") && message.IndexOf("Message") < message.IndexOf("|"))
                    return 8;
                else return 0;
            }
            return -1;
        }

        private void registrationMessageCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                new System.Windows.Threading.DispatcherOperationCallback(delegate
                {
                    SignUpWindowHandler.GetInstance().ToEntry(getMessageAnswer(answer));
                    return 0; 
                }), null);
        }

        private void loginMessageCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                new System.Windows.Threading.DispatcherOperationCallback(delegate
                {
                    LoginWindowHandler.GetInstance().Open(getMessageAnswer(answer));
                    return null;
                }), null); 
        }

        private void forumMessageCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                new System.Windows.Threading.DispatcherOperationCallback(delegate
                {
                    ForumWindowHandler.GetInstance().GetServerAnswer(answer);
                    return null;
                }), null); 
        }

        private void roomPostsCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                new System.Windows.Threading.DispatcherOperationCallback(delegate
                {
                    window.OpenCurrentRoom(answer);
                    return 0;
                }), null);
        }

        private void roomPageCountCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                new System.Windows.Threading.DispatcherOperationCallback(delegate
                {
                    window.SetRoomCount(answer);
                    return 0;
                }), null);
        }

        private void messageAddedResponseCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                new System.Windows.Threading.DispatcherOperationCallback(delegate
                {
                    window.UpdatingRoom(answer);
                    return 0;
                }), null);
        }

        private void privateHistoryCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
             new System.Windows.Threading.DispatcherOperationCallback(delegate
             {
                 window.PrivateHistoryReceive(answer);
                 return 0;
             }), null);
        }

        private void privateMessageCatch(Object stateInfo)
        {
            window.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
             new System.Windows.Threading.DispatcherOperationCallback(delegate
             {
                 window.PrivateMessageReceive(answer);
                 return 0;
             }), null);
        }

        private String getMessageAnswer(String message) //For registration and login!!!
        {
            Int32 result = message.IndexOf("|") + 1;
            return message[result].ToString();
        }

        public void AbortThread()
        {
            if (thr != null)
            {
                thr.Abort();
            }
        }
    }
}
