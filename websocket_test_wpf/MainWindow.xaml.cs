using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using SuperSocket.ClientEngine;
using WebSocket4Net;
using System.Windows.Threading;

namespace websocket_test_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // config this to use websocket client
        private string serverUri = "";

        private WebSocket websocket;
        private bool is_connected = false;

        public MainWindow()
        {
            InitializeComponent();

            ServicePointManager.ServerCertificateValidationCallback = delegate (
                object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors SslPolicyErrors) {
                return true;
            };

            if(serverUri == "")
            {
                sendText.Text = "Error";
                commandText.Text = "Error \n";
                commandText.Text += "Please input URL to use this websocket.";
            }
            else
            {
                websocket = new WebSocket(serverUri);
                websocket.Closed += new EventHandler(websocket_Closed);
                websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                websocket.Opened += new EventHandler(websocket_Opened);
                commandText.Text += "[init]\n";
                sendText.Focus();
                websocket.Open();

            }
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                commandText.Text += "\nRX: " + e.Message + "\n";
                commandText.ScrollToEnd();
            }));
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            is_connected = true;
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                button.IsEnabled = true;
                commandText.Text += "[Open socket connection]\n";
            }));
        }

        private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                commandText.Text += "[Error] " + e.Exception.Message + "\n";
                button.IsEnabled = false;
            }));
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                commandText.Text += "[Closed]\n";
                button.IsEnabled = false;
            }));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        private void sendText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                sendMessage();
            }
        }

        private void sendMessage()
        {
            if (is_connected)
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {
                    websocket.Send(sendText.Text);
                    commandText.Text += "TX: " + sendText.Text + "\"\n";
                    sendText.Text = "";
                }));
            }
        }
    }
}
