using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Project_F8.Exeptions;

namespace Project_F8.Logic.SignUp
{
    public class SignUpManager
    {

        static SignUpManager instance = null;
        Connection client;
        String[] signUpInfo = { "null", "null", "null", "null", "null", "null" };
        Regex systemRegex = new Regex(@"\W");
        Regex mailRegex = new Regex("[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}");
        Regex URLRegex = new Regex(@"|");

        public SignUpManager() { }
        
        public static SignUpManager getInstance()
        {
            if (instance == null)
                instance = new SignUpManager();
            return instance;    
        }

        public void SignUp(String login, String password, String mail, String country, String city, String URL)
        {
            this.client = SignUpWindowHandler.Client;
            switch (FormCheck(login, password, mail, country, city, URL))
            {
                case 0:
                    client.SendMessage("registration|" + signUpInfo[0] + "|" + signUpInfo[1] + "|"
                        + signUpInfo[2] + "|" + signUpInfo[3] + "|" + signUpInfo[4] + "|" + signUpInfo[5] + "|");
                    //GetServerAnswer();
                    break;
                case 1:
                    throw new inputException("Incorrect login");
                case 2:
                    throw new inputException("Incorrect password");
                case 3:
                    throw new inputException("Incorrect mail");
                case 4:
                    throw new inputException("Incorrect country");
                case 5:
                    throw new inputException("Incorrect city");
                case 6:
                    throw new inputException("Incorrect URL");
            }
        }

        private Int32 FormCheck(String login, String password, String mail, String country, String city, String URL)
        {
            if (login.Length < 4 || login.Length > 16 || !IsCorrect("Login", login)) return 1;
            else if (password.Length < 4 || password.Length > 16 || !IsCorrect("Pass", password)) return 2;
            else if (mail.Length == 0 || !IsCorrect("mail",mail)) return 3;
            else if (country.Length > 16 || !IsCorrect("country", country)) return 4;
            else if (city.Length > 16 || !IsCorrect("city", city)) return 5;
            else if (!IsCorrect("URL", URL) && URL.Length != 0) return 6;
            else
            {
                signUpInfo[0] = login;
                signUpInfo[1] = password;
                signUpInfo[2] = mail;
                if (country.Length != 0) signUpInfo[3] = country;
                if (city.Length != 0) signUpInfo[4] = city;
                if (URL.Length != 0) signUpInfo[5] = URL;
            }
            return 0;
        }

        public void GetServerAnswer(String answer)
        {
            switch (answer)
            {
                case "0":
                    break;
                case "1":
                    throw new dataException("Login already in use");
                case "2":
                    throw new dataException("E-mail already in use");
                default:
                    throw new dataException("Programm error, try again");
            }  
        }

        private Boolean IsCorrect(String source,String text)      //String mustn't contain system chars
        {
            switch(source)
            {
                case "mail":
                    {
                        return mailRegex.IsMatch(text);
                    }
                case "URL":
                    {
                        return !URLRegex.IsMatch(text);
                    }
                default:
                    {
                        return !systemRegex.IsMatch(text);
                    }
            }
        }

    }
}
