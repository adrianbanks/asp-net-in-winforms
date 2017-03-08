using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyWebApplication
{
    static class Program
    {
        private static MainForm form;

        [STAThread]
        static void Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var webServerTask = Task.Run(() => StartWebServer(cancellationTokenSource.Token), cancellationTokenSource.Token);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new MainForm();
            Application.Run(form);

            cancellationTokenSource.Cancel();
            webServerTask.Wait(5000);
        }

        private static void StartWebServer(CancellationToken cancellationToken)
        {
            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:5000")
                .UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), "..", ".."))
                .UseStartup<Startup>();

            using (var host = webHostBuilder.Build())
            {
                host.Run(cancellationToken);
            }
        }

        public static void MessageBox(string message)
        {
            form.AlertUser(message);
        }
    }
}
