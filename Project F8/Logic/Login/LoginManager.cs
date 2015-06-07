using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Project_F8.Exeptions;

namespace Project_F8.Logic.Login
{
    public class LoginManager
    {
        Regex loginRegex = new Regex(@"\W");
        Connection client = null;
        static LoginManager instance = null;
        public String stat = "";

        public LoginManager() { }

        public static LoginManager getInstance()
        {
            if (instance == null)
            {
                instance = new LoginManager();
            }
            return instance; 
        }

        public void Entry(String login, String password)
        {
            if (Check(login, password) == 0)
            {
                try
                {
                    client = Connection.GetInstance(); 
                    client.SendMessage("credentials|" + login + "|" + password + "|");
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                   throw ex;
                }
            }
        }

        private Int32 Check(String login, String password)
        {
            if (login.Length < 4 || password.Length < 4 || login.Length > 16 || password.Length > 16)
            {
                throw new dataException("Incorrect login/password");
            }
            if (!(IsCorrect(login)&&IsCorrect(password)))
            {
                throw new inputException("Incorrect entry data");
            }
            return 0;
        }

        private Boolean IsCorrect(String text)      //String mustn't contain system chars
        {
            return !loginRegex.IsMatch(text);
        }
        
    }
}
