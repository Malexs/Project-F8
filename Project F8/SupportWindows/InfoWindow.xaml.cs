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

namespace Project_F8
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        public void OpenInfoWindow(String[] info)
        {
            Console.WriteLine(info[0] + " " + info[1] + " " + info[2]);
            nameTextBox.Text = info[0];
            idTextBox.Text = info[1];
            discriptionTextBox.Text = info[2];
            typeTextBox.Text = GetStringByType(info[3]);
            this.ShowDialog();
        }

        private String GetStringByType(String type)
        {
            switch (type)
            {
                case "1":
                    return "Folder";
                case "2":
                    return "Room";
            }
            return null;
        }

        public void CloseInfoWindow()
        {
            nameTextBox.Clear();
            idTextBox.Clear();
            discriptionTextBox.Clear();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            CloseInfoWindow();
            this.Close();
        }

        private void infoWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseInfoWindow();
        }
    }
}
