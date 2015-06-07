using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;

using Project_F8.Logic;
using Project_F8.Logic.Login;
using Project_F8.Logic.Forum;
using Project_F8.Logic.SignUp;
using Project_F8.Logic.Memory;
using Project_F8.Exeptions;

using Project_F8.SupportWindows;

namespace Project_F8
{
    /*
     * TODO (total)
     * - remake your code in MVC model
     * - make local logging
     * - make "last rooms" and "favorite rooms";
     * - 
     */

    public partial class MainWindow : Window
    {
        private Connection client;
        private Observer observer = null;
        private RoomWindow roomWindow;
        private DialogWindow dialogWindow;
        private List<RoomWindow> roomWindows = new List<RoomWindow>();
        private Regex messageRegex = new Regex(@"[а-яёА-ЯЁ\|]+");
        private Boolean isFriendsShown = false;
        private FriendsMemory friendsMemory = null;

        public MainWindow()
        {
            InitializeComponent();
            loginBox.Focus(); //TODO: add this to login Manager!
            observer = Observer.GetInstance();
            observer.ActivateThread(this);        
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            LoginWindowHandler.Client = Connection.GetInstance();
            LoginWindowHandler.Parent = this;
            LoginWindowHandler.GetInstance().HandleLoginButtonClick();
        }

        public void IsSuccessfullyLoged(String login)
        {
            friendsMemory = new FriendsMemory(login);
            friendsMemory.GetMemoryFromDisk();
        }

        private void signUpButton_Click(object sender, RoutedEventArgs e) //Moving to registration part
        {
            e.Handled = true;
            LoginWindowHandler.Parent = this;
            SignUpWindowHandler.Client = Connection.GetInstance();
            LoginWindowHandler.GetInstance().HandleSignUpButtonClick();
        }


        /***********************************************************************************************************************/
        /***********************************************************************************************************************/
        //Here "Registraion Form" starts
        private void signUpEndButton_Click(object sender, RoutedEventArgs e) //Sign up
        {
            e.Handled = true;
            SignUpWindowHandler.GetInstance().HandleSignUpButtonClick();

        }

        private void repassBox_LostFocus(object sender, RoutedEventArgs e) //Correct password repetition check
        {
            SignUpWindowHandler.GetInstance().HandlePasswordCorrection("repass");
        }

        private void newPassBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SignUpWindowHandler.GetInstance().HandlePasswordCorrection("newPass");
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)  //Press to move to entry form
        {
            SignUpWindowHandler.GetInstance().HandleReturnButtonClick();
        }

        /***********************************************************************************************************************/
        /***********************************************************************************************************************/
        //Here "Registraion Form" ends

        //And "Forum Form" begins
        /***********************************************************************************************************************/
        /***********************************************************************************************************************/


        private void logOutButton_Click(object sender, RoutedEventArgs e) //Log out 
        {
            e.Handled = true; 
            if (dialogWindow != null)
            {
                dialogWindow.IsNeed = false;
                dialogWindow.Close();
            }
            if (roomWindow != null)
            {
                roomWindow.Close();
            }
            if (isFriendsShown)
            {
                roomSelectGrid.Visibility = Visibility.Visible;
                hideFriends.Begin();
                isFriendsShown = !isFriendsShown;
                friendGrid.Visibility = Visibility.Hidden;
            }
            friendsMemory.SaveMemoryToDisk();
            ForumWindowHandler.GetInstance().SignOut();
        }

        private void roomBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (roomBox.SelectedItem != null)
            {
                ForumWindowHandler.GetInstance().HandleRoomBoxDoubleClick(roomBox.SelectedItem.ToString());
            }
        }

        private void InfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (roomBox.SelectedItem != null)
            {
                InfoWindow infoWindow = new InfoWindow();
                String[] info = ForumWindowHandler.GetInstance().HandleInformationMenuClick(roomBox.SelectedItem.ToString());
                infoWindow.OpenInfoWindow(info);
            }
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddindWindow addWindow = new AddindWindow();
            addWindow.OpenAddingWindow(ForumWindowHandler.CurrentFolderType);
            String[] info = addWindow.GetAddInfo();
            ForumWindowHandler.GetInstance().HandleAddMenuClick(info);
        }


        private void backPathButton_Click(object sender, RoutedEventArgs e)
        {
            ForumWindowHandler.GetInstance().HandleBackButtonClick();
        }

        public void GoToRoom(Int32 id,String name)
        {
            roomWindow = RoomWindow.GetInstance();
            roomWindow.AddRoom(id, name);
        }

        public void OpenCurrentRoom(String answer)
        {
            Thread.Sleep(10);
            roomWindow.HandleWindowOpening(answer);
        }

        public void SetRoomCount(String answer)
        {
            roomWindow.ReceiveRoomCount(answer);
        }

        public void UpdatingRoom(String answer)
        {
            Thread.Sleep(10);
            roomWindow.SendStartMessages(answer);
        }
        /***********************************************************************************************************************/
        /***********************************************************************************************************************/
        //Here "Forum Form" ends
        /***********************************************************************************************************************/
        /***********************************************************************************************************************/

        /*
         *  Dialog part
         */
        private void addDialogButton_Click(object sender, RoutedEventArgs e)
        {
            AddDialog();
        }

        public void AddDialog()
        {
            OpenDialogWindow openDialogWindow = OpenDialogWindow.GetInstance();
            openDialogWindow.ShowDialog();
            String name = openDialogWindow.GetUser();
            if (name != LoginInfo.Login && name != "")
            {
                SetDialogWindowProperties();
                dialogWindow.OpenDialog(name);
                dialogWindow.Show();
            }
        }

        public void PrivateHistoryReceive(String serverAnswer)
        {
            SetDialogWindowProperties();
            dialogWindow.OpenDialogMessages(serverAnswer);
        }

        public void PrivateMessageReceive(String serverAnswer)
        {
            SetDialogWindowProperties();
            dialogWindow.PrivateMessageReceive(serverAnswer);
        }

        private void loginBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = messageRegex.IsMatch(e.Text);
        }

        private void openialogButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogWindowProperties();
            dialogWindow.Show();
        }

        private void SetDialogWindowProperties()
        {
            dialogWindow = DialogWindow.GetInstance();
            dialogWindow.Parent = this;
            dialogWindow.SetMyUser(LoginInfo.Login);
        }

        private void friendsShowButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            friendlistRefresh();
            if (!isFriendsShown)
            {
                friendGrid.Visibility = Visibility.Visible;
                friendlistRefresh();
                showFriends.Begin();
                friendGridOP.Completed += RoomToFriendsHandler;
            }
            else
            {
                roomSelectGrid.Visibility = Visibility.Visible;
                hideFriends.Begin();
                roomGridOp.Completed += FriendToRoomHandler;
            }
            isFriendsShown = !isFriendsShown;
        }

        private void RoomToFriendsHandler(object sender, EventArgs e)
        {
            roomSelectGrid.Visibility = Visibility.Hidden;
        }

        private void FriendToRoomHandler(object sender, EventArgs e)
        {
            friendGrid.Visibility = Visibility.Hidden;
        }

        private void friendlistRefresh()
        {
            friendListView.Items.Clear();
            foreach (String friend in friendsMemory.GetListOfFriends())
            {
                friendListView.Items.Add(friend);
            }
        }

        private void addFriend_Click(object sender, RoutedEventArgs e)
        {
            
            AddFriend();
        }

        private void AddFriend()
        {
            OpenDialogWindow openDialogWindow = OpenDialogWindow.GetInstance();
            openDialogWindow.ShowDialog();
            String name = openDialogWindow.GetUser();
            if (name != LoginInfo.Login && name != "")
            {
                friendsMemory.PutUserToFriends(name);
            }
            friendlistRefresh();
        }

        public void AddFriendByName(String name)
        {
            friendsMemory.PutUserToFriends(name);
            friendlistRefresh();
        }

        private void friendListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (friendListView.SelectedItem!=null)
            {
                SetDialogWindowProperties();
                dialogWindow.OpenDialog(friendListView.SelectedItem.ToString());
                dialogWindow.Show();
            }
        }


        private void EntryWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client = Connection.GetInstance();
            if (friendsMemory != null)
            {
                friendsMemory.SaveMemoryToDisk();
            }
            if (dialogWindow != null)
            {
                dialogWindow.IsNeed = false;
                dialogWindow.Close();
            }
            if (roomWindow != null)
            {
                roomWindow.Close();
            }
            if (observer != null)
            {
                observer.AbortThread();
            }
            if (client != null)
            {
                Int32 result = client.Dispose(LoginInfo.Login);
                LoginInfo.Login = null;
            }
        }

        private void deleteFriend_Click(object sender, RoutedEventArgs e)
        {
            if (friendListView.SelectedItem!=null)
            {
                friendsMemory.DeleteUserFromFriends(friendListView.SelectedItem.ToString());
            }
            friendlistRefresh();
        }

    }
}
