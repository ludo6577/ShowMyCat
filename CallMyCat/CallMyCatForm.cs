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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CallMyCat
{
    public partial class CallMyCatForm : Form
    {
        private static string CatIP = "127.0.0.1";
        private static int CatPort = 9876;

        public CallMyCatForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            DialogResult dr = fd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string base64String;
                using (Image image = Image.FromFile(fd.FileName))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }


                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAdd = System.Net.IPAddress.Parse(CatIP);
                IPEndPoint remoteEP = new IPEndPoint(ipAdd, CatPort);
                soc.Connect(remoteEP);

                byte[] byData = System.Text.Encoding.ASCII.GetBytes(base64String);
                soc.Send(byData);

                soc.Close();
            }
        }
    }
}
