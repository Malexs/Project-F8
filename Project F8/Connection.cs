using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Project_F8
{
    public class Connection
    {
        byte[] buff = new byte[51200]; //50kb
        Socket clientSocket;
        String ipAddress = "127.0.0.1";
        Int32 serverPort = 3128;
        static Connection instance = null;

        public Connection()
        {
            ConnectionStart();
        }

        public static Connection GetInstance()
        {
            if (instance == null)
                return instance = new Connection();
            else return instance;    
        }

        public static Boolean IsExist()
        {
            return (instance != null);
        }

        public Int32 ConnectionStart()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(ipAddress, serverPort);
                return 0;
            }
            catch (SocketException ex)
            {
                instance = null;
                return -1;
            }
        }

        public Int32 SendMessage(String text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);
            try
            {
                lock (clientSocket)
                {
                    clientSocket.Send(message);
                }
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public String Receive()
        {
            if (clientSocket.Available > 0)
                return Encoding.UTF8.GetString(buff, 0, clientSocket.Receive(buff));
            else return null;
        }

        public Int32 Dispose(String login)
        {
            byte[] message = Encoding.UTF8.GetBytes("disconnect|"+login+"|");
            try
            {
                clientSocket.Send(message);
                clientSocket.Dispose();
                instance = null;
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
