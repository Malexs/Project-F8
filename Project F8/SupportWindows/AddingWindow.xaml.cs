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

namespace Project_F8
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class AddindWindow : Window
    {

        private static String[] info = new String[3];
        private Boolean isCanceled = false;
        private Int32 currentFolderId = 1;
        private Regex messageRegex = new Regex(@"[а-яёА-ЯЁ\|]+");

        public AddindWindow()
        {
            InitializeComponent();
        }

        public void OpenAddingWindow(int currentFolderID)
        {
            this.currentFolderId = currentFolderID;
            if (currentFolderId == 1)
            {
                folderRadioButton.IsEnabled = true;
                folderRoomsRadioButton.IsEnabled = true;
                folderRadioButton.IsChecked = true;
                roomRadioButton.IsEnabled = false;
            }
            else
            {
                roomRadioButton.IsEnabled = true;
                folderRadioButton.IsEnabled = false;
                folderRoomsRadioButton.IsEnabled = false;
                roomRadioButton.IsChecked = true;
            }
            this.ShowDialog();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text.Length >= 4 && nameTextBox.Text.Length <= 16)
            {
                info[0] = nameTextBox.Text;
                if (discriptionTextBox.Text != "") info[1] = discriptionTextBox.Text;
                else info[1] = " ";
                if (folderRadioButton.IsChecked == true) info[2] = "1";
                else if (folderRoomsRadioButton.IsChecked == true) info[2] = "2";
                else info[2] = "3";
                this.Close();
            }
        }

        public String[] GetAddInfo()
        {
            if (!isCanceled)
                return info;
            return null;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            isCanceled = true;
            this.Close();
        }

        private void addWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void loginBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = messageRegex.IsMatch(e.Text);
        }

    }
}
