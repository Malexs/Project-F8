using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Project_F8.Exeptions;
using Project_F8.Logic.Forum;
using Project_F8.Logic.SignUp;

namespace Project_F8.Logic.Login
{
    public class LoginWindowHandler
    {
        private static LoginWindowHandler instance = null;
        private LoginManager loginManager = null;
        private Int32 status = 0;
        public static MainWindow Parent { set; get;}
        public static Connection Client { set; get;}


        public LoginWindowHandler() { }
        

        public static LoginWindowHandler GetInstance()
        {
            if (instance == null)
                return instance = new LoginWindowHandler();
            else return instance;
        }

        public static void Show(Object sender)
        {
            if (sender is ForumWindowHandler)
            {
                Parent.forumToLogin.Begin();
                Parent.entryGrid.IsEnabled = true;
                Parent.forumGrid.IsEnabled = false;
                Parent.loginBox.Focus();

                if (Client!=null)
                {
                    Client.Dispose(LoginInfo.Login);
                }  
            } 
            else if (sender is SignUpWindowHandler)
            {
                Parent.regToEntryMove.Begin();
                Parent.entryGrid.IsEnabled = true;
                Parent.regGrid.IsEnabled = false;
                Parent.loginBox.Focus();
            }
          
        }

        public void HandleLoginButtonClick()
        {
            loginManager = LoginManager.getInstance();
            try
            {
                GetStatus();
            }
            catch (System.Net.Sockets.SocketException)
            {
                Parent.logBox.Text = "No connection to server.";
                Parent.loginBox.Clear();
                Parent.passBox.Clear();
                Parent.loginBox.Focus();
            }
            catch (dataException ex)
            {
                Parent.logBox.Text = ex.ToString();
                Parent.loginBox.Clear();
                Parent.passBox.Clear();
                Parent.loginBox.Focus();
            }
            catch (inputException ex)
            {
                Parent.loginBox.Clear();
                Parent.passBox.Clear();
                Parent.loginBox.Focus();
                Parent.logBox.Text = ex.ToString();
            }
            catch (Exception)
            {
                Parent.loginBox.Clear();
                Parent.passBox.Clear();
                Parent.loginBox.Focus();
                Parent.logBox.Text = "Something wrong";
            }
        }


        public void HandleSignUpButtonClick()
        {
            Parent.logBox.Clear();
            Parent.loginBox.Clear();
            Parent.passBox.Clear();
            SignUpWindowHandler.Parent = Parent;
            SignUpWindowHandler.Show();
        }


        private void GetStatus()
        {
            try
            {
                loginManager.Entry(Parent.loginBox.Text, Parent.passBox.Password);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw ex;
            }
        }
        
        public void Open(String message)
        {
            status = Int32.Parse(message);
            switch (message)
            {
                case "1":
                    ToForum("User");
                    break;
                case "2":
                    ToForum("Moder");
                    Parent.changeItem.IsEnabled = true;
                    break;
                case "3":
                    Parent.addItem.IsEnabled = true;
                    Parent.deleteItem.IsEnabled = true;
                    Parent.changeItem.IsEnabled = true;
                    ToForum("Admin");
                    break;
                default:
                    Parent.loginBox.Clear();
                    Parent.passBox.Clear();
                    Parent.loginBox.Focus();
                    Parent.logBox.Text = "Incorrect log/pass or user already online.";
                    break;
            }
        }
        
        public void ToForum(String stat)
        {
            LoginInfo.Login = Parent.loginBox.Text;
            Parent.loginLabel.Content = Parent.loginBox.Text;
            LoginInfo.Status = status;
            Parent.statusLabel.Content = stat;
            Parent.logBox.Clear();
            Parent.loginBox.Clear();
            Parent.passBox.Clear();
            ForumWindowHandler.Parent = Parent;
            ForumWindowHandler.Show();
            ForumWindowHandler.GetInstance().HandleRoomBoxDoubleClick("root");
            Parent.IsSuccessfullyLoged(LoginInfo.Login);
        }
        
    }
}
