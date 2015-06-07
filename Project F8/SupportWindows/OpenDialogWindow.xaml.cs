using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Project_F8.SupportWindows
{
    /// <summary>
    /// Логика взаимодействия для OpenDialogWindo.xaml
    /// </summary>
    public partial class OpenDialogWindow : Window
    {
        private static OpenDialogWindow instance = null;
        private String user = "";
        private Regex messageRegex = new Regex(@"[а-яёА-ЯЁ\|]+");

        public OpenDialogWindow()
        {
            InitializeComponent();
        }

        public static OpenDialogWindow GetInstance()
        {
            if (instance == null) instance = new OpenDialogWindow();
            return instance;
        }

        private void openDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text.Length >= 4 && nameBox.Text.Length <= 16)
            {
                user = nameBox.Text;
                instance = null;
                this.Close();
            }
        }

        public String GetUser()
        {
            return user;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            nameBox.Focus();
        }

        private void loginBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = messageRegex.IsMatch(e.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            instance = null;
        }
    }
}
