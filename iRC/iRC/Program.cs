using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;
using System.Threading;

namespace iRC
{
    class Program
    {
        static string server = "irc.freenode.net";
        static string username = "CLI";
        static string nickname = "CLI";
        static string channel = "#testirctest";
        static IrcClient client = new IrcClient(server, new IrcUser(nickname, username));

        static void Main(string[] args)
        {
            Print("iRC by ion 0.1a", ConsoleColor.White, true);
            Print("\nServer: ", ConsoleColor.White);
            server = Console.ReadLine();
            Print("Channel: ", ConsoleColor.White);
            channel = Console.ReadLine();
            Print("Username: ", ConsoleColor.White);
            username = Console.ReadLine();
            Print("Nickname: ", ConsoleColor.White);
            nickname = Console.ReadLine();
            Connect();
            IrcChannel chan = client.Channels[0];
            client.ChannelMessageRecieved += (s, e) => //get messages
            {
                /*
                    Printing text and changing color
                    Fix userinput location to stay on bottom
                */
                int currentTopCursor = Console.CursorTop;
                int currentLeftCursor = Console.CursorLeft;

                Console.MoveBufferArea(0, currentTopCursor, Console.WindowWidth, 1, 0, currentTopCursor + 1);

                Console.CursorTop = currentTopCursor;

                Console.CursorLeft = 0;

                Print(e.PrivateMessage.User.Nick + ">", ConsoleColor.Green);
                Print(e.PrivateMessage.Message, ConsoleColor.White, true);

                Console.CursorTop = currentTopCursor + 1;
                Console.CursorLeft = currentLeftCursor;
            };

            Thread t = new Thread(Read); //start background reader
            t.Start();
        }

        private static void Connect()
        {
            Print("\n\nConnecting...\n\n", ConsoleColor.Green);
            client.ConnectionComplete += (s, e) => client.JoinChannel(channel);
            
            client.ConnectAsync();

            Thread.Sleep(20000);

            WriteInfoHead();
        }

        private static void Read()
        {
            IrcChannel chan = client.Channels[0];
            Console.Write("Input>>");
            chan.SendMessage(Console.ReadLine().Replace("Input>>", ""));
            Read();
        }

        private static void Print(string text, ConsoleColor font, bool newLine = false)
        {
            Console.ForegroundColor = font;
            if(newLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void PrintMulti(string text1, string text2, ConsoleColor font1, ConsoleColor font2, bool newLine = false)
        {
            Console.ForegroundColor = font1;
            Console.Write(text1);
            Console.ForegroundColor = font2;
            Console.Write(text2);
            if(newLine)
            {
                Console.WriteLine("");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void WriteInfoHead()
        {
            Console.Clear();
            IrcChannel chan = client.Channels[0];
            string head = "Server: " + server + "\nChannel: " + channel + "\nTopic: " + chan.Topic + "\n\n";
            Console.Write(head);
        }
    }
}
