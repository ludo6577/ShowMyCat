using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowMyCat
{
    public partial class ShowMyCatForm : Form
    {
        private static int CatPort = 9876;

        public ShowMyCatForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var th = new Thread(Worker);
            th.Start();
        }

        private void Worker()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ipAddress, CatPort);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                var childSocketThread = new Thread(() =>
                {
                    string msg;
                    using (NetworkStream ns = client.GetStream())
                    using (StreamReader sr = new StreamReader(ns))
                    {
                        msg = sr.ReadToEnd();
                    }

                    byte[] binaryData = Convert.FromBase64String(msg);
                    var image = Image.FromStream(new MemoryStream(binaryData));

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            this.BackgroundImage = image;
                            this.Visible = true;
                            this.TopMost = true;
                            this.FormBorderStyle = FormBorderStyle.None;
                            this.WindowState = FormWindowState.Maximized;
                        }));
                    }
                    
                    client.Close();
                });
                childSocketThread.Start();
            }
        }


        delegate IntPtr GetWindowHandleDelegate();

        private IntPtr GetWindowHandle()
        {

            if (this.InvokeRequired == true)
            {

                return (IntPtr)this.Invoke((GetWindowHandleDelegate)delegate()
                {

                    return GetWindowHandle();

                });
            }

            return this.Handle;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

    }
}
