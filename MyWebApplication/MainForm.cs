using System;
using System.Threading;
using System.Windows.Forms;

namespace MyWebApplication
{
    public partial class MainForm : Form
    {
        private static SynchronizationContext synchronizationContext;

        public MainForm()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;

            webBrowser.Navigated += WebBrowser_Navigated;
            webBrowser.NavigateError += WebBrowser_NavigateError;
            webBrowser.Navigate("http://localhost:5000");
        }

        private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.Text = e.Url.ToString();
        }

        private void WebBrowser_NavigateError(object sender, WebBrowserNavigateErrorEventArgs e)
        {
            MessageBox.Show($"An error occurred (status code {e.StatusCode} while loading {e.Url})");
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            webBrowser.Refresh();
        }

        public void AlertUser(string message)
        {
            synchronizationContext.Post(AlertUserInner, message);
        }

        private void AlertUserInner(object obj)
        {
            string message = (string) obj;
            MessageBox.Show(message);
        }
    }
}
