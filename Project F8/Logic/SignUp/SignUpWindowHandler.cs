using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_F8.Exeptions;
using Project_F8.Logic.Login;

namespace Project_F8.Logic.SignUp
{
    public class SignUpWindowHandler
    {
        public static SignUpWindowHandler instance = null;
        public static MainWindow Parent { get; set; }
        public static Connection Client { get; set; }
        private SignUpManager signUpManager = null;

        public SignUpWindowHandler() { }

        public static SignUpWindowHandler GetInstance()
        {
            if (instance == null)
                return instance = new SignUpWindowHandler();
            else return instance;
        }

        /*  
         *      Entry and send part
         */

        public static void Show()
        {
            Parent.regGrid.IsEnabled = true;
            Parent.entryGrid.IsEnabled = false;
            Parent.entryToRegMove.Begin();
            Parent.newLoginBox.Focus();
        }

        public void HandleSignUpButtonClick()
        {
            Parent.regLogLabel.Clear();
            
            signUpManager = SignUpManager.getInstance();
            if (Parent.newPassBox.Password.Equals(Parent.repassBox.Password))
            {
                try
                {
                    signUpManager.SignUp(Parent.newLoginBox.Text, Parent.newPassBox.Password, Parent.mailBox.Text,
                        Parent.countryBox.Text, Parent.cityBox.Text, Parent.URLBox.Text);
                }
                catch (inputException ex)
                {
                    Parent.newLoginBox.Focus();
                    Parent.regLogLabel.Text = ex.ToString();
                }
                catch (dataException ex)
                {
                    Parent.newLoginBox.Focus();
                    Parent.regLogLabel.Text = ex.ToString();
                    clearRegistrationForm();
                }
                catch
                {
                    Parent.newLoginBox.Focus();
                    clearRegistrationForm();
                }
            }
        }





        /*
         *      Receive and exit part
         */

        public void ToEntry(String answer)
        {
            try
            {
                signUpManager.GetServerAnswer(answer);
                LoginWindowHandler.Show(this);
            }
            catch { }
        }

        public void HandleReturnButtonClick()
        {
            clearRegistrationForm();
            LoginWindowHandler.Show(this);
        }

        private void clearRegistrationForm() //Must be clear for new registration try
        {
            Parent.newLoginBox.Clear();
            Parent.newPassBox.Clear();
            Parent.repassBox.Clear();
            Parent.mailBox.Clear();
            Parent.countryBox.Clear();
            Parent.cityBox.Clear();
            Parent.URLBox.Clear();
            Parent.regLogLabel.Clear();
        }

        /*
         *      Other maintenance
         */
        public void HandlePasswordCorrection(String boxType)
        {
            switch (boxType)
            {
                case "repass":
                    if (Parent.newPassBox.Password.Length != 0)
                    {
                        checkPasswords();
                    }
                    break;
                case "newPass":
                    if (Parent.repassBox.Password.Length != 0)
                    {
                        checkPasswords();
                    }
                    break;
            }
        }

        private void checkPasswords()
        {
            if (!Parent.repassBox.Password.Equals(Parent.newPassBox.Password))
            {
                Parent.regLogLabel.Text = "Repeat your password correctly";
            }
            else
            {
                Parent.regLogLabel.Text = "";
            }
        }

    }
}
