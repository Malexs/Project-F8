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
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public MainWindow Parent { get; set; }
        private static DialogWindow instance = null;
        private List<DialogHandler> handlers = new List<DialogHandler>();
        private DialogHandler currentHandler = null;
        private String userME;
        private Regex messageRegex = new Regex(@"[а-яёА-ЯЁ\|]+");
        public Boolean IsNeed { get; set;}
        

        public DialogWindow()
        {
            IsNeed = true;
            Parent = null;
            InitializeComponent();
        }

        public static DialogWindow GetInstance()
        {
            if (instance == null) instance = new DialogWindow();
            return instance;
        }

        public void SetMyUser(String userME)
        {
            this.userME = userME;
        }

        public void OpenDialog(String userName)
        {
            if (!IsUserHandled(userName))
            {
                DialogHandler handler = new DialogHandler(userName, userME);
                handlers.Add(handler);
                AddUserToBox(userName);
                currentHandler = handler;
                //if (handlers.Count == 1) 
                {
                    SendStartDialogMessage();
                }
            }
            else
            {
                foreach (DialogHandler handler in handlers)
                {
                    if (handler.IsRightName(userName))
                    {
                        currentHandler = handler;
                        foreach (ListBoxItem box in dialogBox.Items)
                        {
                            if (box.Content.ToString()==userName)
                            {
                                dialogBox.SelectedValue = box;
                                box.Background = Brushes.White;
                            }
                        }
                        messageBoxUpdate();
                    }
                }
            }
        
        }
        
        private void AddUserToBox(String name)
        {
            ListBoxItem lb = new ListBoxItem();
            lb.Content = name;
            dialogBox.Items.Add(lb);
        }

        private Boolean IsUserHandled(String name)
        {
            if (handlers.Count>0)
            {
                foreach (DialogHandler handler in handlers)
                {
                    if (handler.IsRightName(name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void OpenDialogMessages(String serverAnswer)
        {
            foreach (DialogHandler handler in handlers)
            {
                if (handler.IsWaitingForAnswer)
                {
                    handler.SaveMessageHistory(serverAnswer);
                    handler.IsWaitingForAnswer = false;
                    if (handlers.Count==1)
                    messageBoxUpdate();
                }
            }
        }

        private RichTextBox SetMessagesBoxProperties(String text)
        {
            RichTextBox target = new RichTextBox();
            target.AppendText(text);
            target.Width = 382;
            target.Margin = template.Margin;
            target.BorderThickness = template.BorderThickness;
            target.Background = messageBox.Background;
            target.IsManipulationEnabled = false;
            target.IsReadOnly = true;
            return target;
        }

        private void SendStartDialogMessage()
        {
            currentHandler.SendStartMessage();
        }

        private void messageBoxUpdate()
        {
            messageBox.Items.Clear();
            foreach (String message in currentHandler.GetMessagesToPost())
            {
                RichTextBox richTextBox = SetMessagesBoxProperties(message);
                messageBox.Items.Add(richTextBox);
                messageBox.Items.Add(new Separator());
            }
        }

        private void dialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsNeed)
            {
                e.Cancel = true;
                this.Hide();
            } else
            instance = null;
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text!="")
            {
                currentHandler.SendUsersMessage(textBox.Text);
                messageBoxUpdate();
            }
            textBox.Clear();
        }

        public void PrivateMessageReceive(String serverAnswer)
        {
            Boolean received = false;
            DialogHandler receivedDialog=null;

            foreach (DialogHandler handler in handlers)
            {
                if (handler.IsRightDialog(serverAnswer))
                {
                    handler.ReceiveMessage(serverAnswer);
                    receivedDialog = handler;
                    received = true;
                }
            }
            if (!received)
            {
                Thread.Sleep(10);
                receivedDialog = GetNewClient(serverAnswer);
                if (currentHandler == null)
                    currentHandler = receivedDialog;
            }

            messageBoxUpdate();

            //if (receivedDialog!=currentHandler)
            {
                foreach (ListBoxItem box in dialogBox.Items)
                {
                    if (receivedDialog.IsRightName(box.Content.ToString()))
                    {
                        box.Background = Brushes.Yellow;
                    }
                }
            }
        }

        private DialogHandler GetNewClient(String answer)
        {
            String tempString = answer;
            tempString = tempString.Substring(tempString.IndexOf("|")+1);
            tempString = tempString.Substring(0, tempString.IndexOf("|"));
            DialogHandler handler = new DialogHandler(tempString, userME);
            handlers.Add(handler);
            handler.SendStartMessage();
            AddUserToBox(tempString);
            return handler;
        }

        private void dialogBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dialogBox.SelectedItem != null)
            {
                ListBoxItem tmpBox = (ListBoxItem)dialogBox.SelectedItem;
                if (tmpBox != null)
                {
                    //dialogBox.Items.Clear();
                    foreach (DialogHandler handler in handlers)
                    {
                        if (handler.IsRightName(tmpBox.Content.ToString()))
                        {
                            currentHandler = handler;
                            tmpBox.Background = Brushes.White;
                            messageBoxUpdate();
                        }
                    }
                }
            }
        }
        
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = messageRegex.IsMatch(e.Text);
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dialogBox.SelectedItem != null)
            {
                ListBoxItem lb = (ListBoxItem)dialogBox.SelectedItem;
                List<DialogHandler> procedureHandler = handlers;
                foreach (DialogHandler handler in procedureHandler)
                {
                    if (handler.IsRightName(lb.Content.ToString()))
                    {
                        procedureHandler.Remove(handler);
                        break;
                    }
                }
                handlers = procedureHandler;
            }
            messageBox.Items.Clear();
            dialogBox.Items.Clear();
            foreach (DialogHandler handler in handlers)
            {
                ListBoxItem lb = new ListBoxItem();
                lb.Content = handler.GetName();
                dialogBox.Items.Add(lb);
                //dialogBox.Items.Add(SetUserBoxProperties(handler.GetName()));
            }
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Parent.AddDialog();
        }

        private void AddFriendMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dialogBox.SelectedItem != null)
            {
                ListBoxItem tmpBox = (ListBoxItem)dialogBox.SelectedItem;
                if (tmpBox != null)
                {
                    //dialogBox.Items.Clear();
                    foreach (DialogHandler handler in handlers)
                    {
                        if (handler.IsRightName(tmpBox.Content.ToString()))
                        {
                            Parent.AddFriendByName(tmpBox.Content.ToString());
                        }
                    }
                }
            }
        }

        private void dialogWindow_Activated(object sender, EventArgs e)
        {
            textBox.Focus();
        }

    }
}
