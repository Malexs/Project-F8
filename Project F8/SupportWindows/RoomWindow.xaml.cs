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
using System.Threading;
using System.Text.RegularExpressions;

using Project_F8.SupportWindows.Handlers;

namespace Project_F8.SupportWindows
{
    /// <summary>
    /// Логика взаимодействия для RoomWindow.xaml
    /// </summary>
    public partial class RoomWindow : Window
    {
        private List<RoomWindowHandler> roomHandlers = new List<RoomWindowHandler>();
        private RoomWindowHandler roomHandler;//
        private RichTextBox postBox;//
        private TextBox textBox;
        private RoomWindowHandler currHandler = null;
        private static RoomWindow instance = null;
        private Regex messageRegex = new Regex(@"[а-яёА-ЯЁ\|]+");

        public static RoomWindow GetInstance()
        {
            if (instance == null) instance = new RoomWindow();
            return instance;
        }

        public RoomWindow()
        {
            InitializeComponent();
        //    roomHandler = new RoomWindowHandler();
        //    isWaitingForPosts = false;
        //    isWaitingForRoomCount = false;
        //    isWaitingForUpdate = false;
        }
        
        public void AddRoom(Int32 id, String name)
        {
            String currName = name.Substring(0, name.IndexOf("  ("));
            textBox = SetRoomBoxProperties(currName);
            roomHandler = new RoomWindowHandler(id, currName);
            roomHandlers.Add(roomHandler);
            roomList.Items.Add(textBox);
            if (this.IsActive == false)
            {
                this.Show();
            }
            //if (roomHandlers.Count == 1)
            {
                roomHandler.SendStartMessages(id);
                currHandler = roomHandler;
            }
        }

        public TextBox SetRoomBoxProperties(String name)
        {
            textBox = new TextBox();
            textBox.Text = name;
            textBox.Width = 160;
            textBox.IsReadOnly = true;
            textBox.BorderThickness = template.BorderThickness;
            return textBox;
        }

        private RoomWindowHandler GetHandlerByName(String name)
        {
            foreach (RoomWindowHandler handler in roomHandlers)
            {
                if (handler.GetName() == name)
                    return handler;
            }
            return null;
        }

        public void HandleWindowOpening(String serverAnswer)
        {
            foreach (RoomWindowHandler handler in roomHandlers)
            {
                if (handler.isWaitingForPosts)
                {
                    postsBox.Items.Clear();
                    handler.ReceivePostFromServer(serverAnswer);
                    Print(handler.fabricatePosts());
                    handler.isWaitingForPosts = false;
                    header.Content = handler.GetName();
                    currPageLabel.Content = handler.CurrentPage;
                }
            }
        }

        public void Print(List<String> posts)
        {
            foreach (String post in posts)
            {
                postBox = SetBoxProperties(post);
                postsBox.Items.Add(postBox);
                postsBox.Items.Add(new Separator());
            }
        }

        private RichTextBox SetBoxProperties(String text)
        {
            RichTextBox target = new RichTextBox();
            target.AppendText(text);
            target.Width = 450;
            target.Margin = template.Margin;
            target.BorderThickness = template.BorderThickness;
            target.Background = postsBox.Background;
            target.IsManipulationEnabled = false;
            target.IsReadOnly = true;

            return target;
        }


        public void SendStartMessages(String answer)
        {
            if (currHandler.isSuccessfullyAdded(answer))
            {
                messageBox.Clear();
                currHandler.GetPosts();
                Thread.Sleep(50);
                currHandler.GetRoomCount();
                currHandler.isWaitingForUpdate = false;
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            currHandler.GetPosts();
        }



        public void ReceiveRoomCount(String answer)
        {
            foreach (RoomWindowHandler handler in roomHandlers)
            {
                if (handler.isWaitingForRoomCount)
                {
                    maxPageLabel.Content = handler.SavePagesCount(answer);
                    handler.isWaitingForRoomCount = false;
                }
            }
        }

        private void nextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!currHandler.isMaxPage())
            {
                currHandler.CurrentPage++;
                currHandler.GetPosts();
            }
        }

        private void prevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currHandler.CurrentPage > 1)
            {
                currHandler.CurrentPage--;
                currHandler.GetPosts();
            }
        }

        private void firstPageButton_Click(object sender, RoutedEventArgs e)
        {
            currHandler.CurrentPage = 1;
            currHandler.GetPosts();
        }

        private void lastPageButton_Click(object sender, RoutedEventArgs e)
        {
            currHandler.SetMaxPage();
            currHandler.GetPosts();
        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (messageBox.Text.Length != 0)
            {
                currHandler.AddMessage(messageBox.Text);
                currHandler.isWaitingForUpdate = true;
            }
        }

        private void roomList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (roomList.SelectedItem != null)
            {
                textBox = (TextBox)roomList.SelectedItem;
                Console.WriteLine("Вошел в if");
                foreach (RoomWindowHandler handler in roomHandlers)
                {
                    if (handler.isCurrentName(textBox.Text))
                    {
                        Console.WriteLine("{0} {1} click handled",handler.GetID(),handler.GetName());
                        currHandler = handler;
                        currHandler.SendStartMessages(currHandler.GetID());
                        break;
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            instance = null;
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (roomList.SelectedItem != null)
            {
                textBox = (TextBox)roomList.SelectedItem;
                List<RoomWindowHandler> procedureHandler = roomHandlers;
                foreach (RoomWindowHandler handler in procedureHandler)
                {
                    if (handler.isCurrentName(textBox.Text))
                    {
                        Console.WriteLine("{0} {1} closed", handler.GetID(), handler.GetName());
                        procedureHandler.Remove(handler);
                        break;
                    }
                }
                roomHandlers = procedureHandler;
            }
            roomList.Items.Clear();
            foreach (RoomWindowHandler handler in roomHandlers)
            {
                roomList.Items.Add(SetRoomBoxProperties(handler.GetName()));
            }
        }

        private void messageBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = messageRegex.IsMatch(e.Text);
        }

    }

}
