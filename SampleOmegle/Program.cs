using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using dotOmegle;

namespace SampleOmegle
{
    class Program
    {
        public static Omegle OmegleObj = new Omegle();
        public static bool recon = true;
        public static void Main(string[] args)
        {
            OmegleObj.Connected += new EventHandler(omegle_Connected);
            //OmegleObj.CaptchaRefused += new EventHandler(omegle_CaptchaRefused);
            //OmegleObj.CaptchaRequired += new CaptchaRequiredEvent(omegle_CaptchaRequired);
            OmegleObj.Interests.Add("memes");
            OmegleObj.MessageReceived += new MessageReceivedEvent(omegle_MessageReceived);
            OmegleObj.StrangerDisconnected += new EventHandler(omegle_StrangerDisconnected);
            OmegleObj.StrangerTyping += new EventHandler(omegle_StrangerTyping);
            OmegleObj.StrangerStoppedTyping += new EventHandler(omegle_StrangerStoppedTyping);
            OmegleObj.WaitingForPartner += new EventHandler(omegle_WaitingForPartner);

            var t = new System.Threading.Thread(meanwhile);
            t.Start();

            OmegleObj.Connect();
        }

        private static void meanwhile()
        {
            var sb = new StringBuilder();
            while (true)
            {
                var k = Console.ReadKey(true);
                if (!recon)
                {
                    recon = true;
                    continue;
                }
                var c = k.Key.ToString();
                if ((k.Modifiers & ConsoleModifiers.Shift) != ConsoleModifiers.Shift)
                    c = c.ToLower();
                if (c.Length == 1)
                    sb.Append(c);
                if (c == "backspace" && sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                if (c == "spacebar")
                    sb.Append(" ");
                if (c == "enter")
                {
                    OmegleObj.SendMessage(sb.ToString());
                    Console.WriteLine("You: " + sb.ToString());
                    sb.Clear();
                }
                Console.Title = sb.ToString() + " [" + c + "]";
            }
        }

        private static void omegle_WaitingForPartner(object sender, EventArgs e)
        {
            Console.WriteLine("SEARCHING.");
        }

        private static void omegle_StrangerDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("DISCONNECTED. Press any key to reconnect.");
            recon = false;
            do
                System.Threading.Thread.Sleep(100);
            while (!recon);
            Console.WriteLine("RECONNECTING.");
            OmegleObj.Reconnect();
        }

        private static void omegle_StrangerTyping(object sender, EventArgs e)
        {
            Console.WriteLine("Stranger is typing...");
        }

        private static void omegle_StrangerStoppedTyping(object sender, EventArgs e)
        {
            Console.WriteLine("Stranger stopped typing...");
        }

        private static void omegle_MessageReceived(object sender, MessageReceivedArgs e)
        {
            Console.WriteLine("Stranger: " + e.message);
        }

        private static void omegle_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("CONNECTED.");
        }

        ~Program()
        {
            OmegleObj.SendDisconnect();
        }
    }
}
