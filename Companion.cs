using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using SimpleTCP;
using System.Net;

namespace Vision_Automix
{
    class Companion
    {
        public void sendPush(string ip, int port, int page, int bank)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                string message = ("BANK-PRESS " + page.ToString() + " " + bank.ToString());

                byte[] packetData = System.Text.ASCIIEncoding.ASCII.GetBytes(message);

                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);


                client.SendTo(packetData, ep);

                //Console.WriteLine("Pressing button " + page + "-" + bank);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending TCP packet");
                MessageBox.Show("Error sending TCP packet to Companion. The TCP client has probably not been initialized with a IP and Port yet.        " + e);
            }


        }

        public string getIPstringFromCon(int[] con)
        {
            try
            {
                string returnString = (con[0] + "." + con[1] + "." + con[2] + "." + con[3]);
                return returnString;
            } catch
            {
                Console.WriteLine("Error creating string for CompanionIP. Returning 127.0.0.1");
                return "127.0.0.1";
            }
        }
    }
}
