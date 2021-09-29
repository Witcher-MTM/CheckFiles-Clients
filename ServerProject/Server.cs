using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using ClientProject;
using System.IO;
using System.Linq;
using System.Threading;

namespace ServerProject
{
    class Server
    {
        public int Client_ID;
        private string ipAddr;
        private int port;
        private IPEndPoint ipPoint;
        public Socket socket;
        public Socket socketclient;
        public List<Client> clients;


        public Server()
        {
            this.Client_ID = -1;
            this.ipAddr = "127.0.0.1";
            this.port = 8000;
            this.ipPoint = new IPEndPoint(IPAddress.Parse(ipAddr), port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.clients = new List<Client>();

        }
        public void StartServer()
        {
            try
            {

                this.socket.Bind(ipPoint);
                this.socket.Listen(10);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
          

        }
        public void ConnectOne()
        {
            bool check = true;
            while (check)
            {
                this.socketclient = this.socket.Accept();
                clients.Add(new Client(socketclient));

                if (clients.Count > 0)
                {
                    check = false;
                }
            }

        }
        public void Connects()
        {
            while (true)
            {

                this.socketclient = this.socket.Accept();
                clients.Add(new Client(socketclient));
                this.Client_ID++;
                clients[clients.Count - 1].ID = this.Client_ID;

            }

        }
        public StringBuilder GetMsg()
        {

            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[256];
            foreach (var item in clients)
            {
                do
                {

                    bytes = item.socket.Receive(data);

                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (item.socket.Available > 0);
            }

            return builder;
        }
        public void SendMsg(string message)
        {
            byte[] data = new byte[256];
            foreach (var item in clients)
            {
                if (File.Exists(message) && Path.GetFileName(message).Contains(".txt") || Path.GetFileName(message).Contains(".rtf"))
                {
                    item.socket.Send(Encoding.Unicode.GetBytes(File.ReadAllText(message)));
                }
                else
                {
                    item.socket.Send(Encoding.Unicode.GetBytes(message));
                }
            }

        }
        public void SendMsg(string message, int user)
        {
            byte[] data = new byte[256];
            if (File.Exists(message) && Path.GetFileName(message).Contains(".txt") || Path.GetFileName(message).Contains(".rtf"))
            {
                clients[user].socket.Send(Encoding.Unicode.GetBytes(File.ReadAllText(message)));
            }
            else
            {
                clients[user].socket.Send(Encoding.Unicode.GetBytes(message));
            }

        }
        public void SendCommand(int choice)
        {
            Console.Clear();
           
            int ID_choice = 0;
            bool check = false;
          
            Exception exception = new Exception();
            if (clients.Count <= 0)
            {
                Console.Clear();
                Console.WriteLine("Wait for connection!");
                Thread.Sleep(1000);
            }
            else
            {
                do
                {
                    ShowAllClients();
                    Console.WriteLine("Choice ID");
                    try
                    {
                        ID_choice = int.Parse(Console.ReadLine());

                        check = true;
                    }
                    catch (Exception)
                    {

                        check = false;
                        Console.Clear();
                    }
                } while (!check);
                Console.Clear();
                switch (choice)
                {
                    
                    case 1:
                        {
                            StartBrowser(ID_choice);
                            break;
                        }
                    case 2:
                        {

                            ThrowDisconnect(ID_choice);

                            break;
                        }
                    case 3:
                        {
                            ChooseDirectory(ID_choice);


                            break;
                        }
                    default:
                        Console.WriteLine("There isnt this action!");
                        Thread.Sleep(1000);
                        Console.Clear();
                        break;
                }
            }
            try
            {
                Console.WriteLine(GetMsg().ToString());
                Console.ReadLine();
            }
            catch (Exception)
            {
            }
        }
        public void SendBrowser(int choice)
        {
            switch (choice)
            {
                case 1:
                    {
                        SendMsg("Start Opera");
                        break;
                    }
                case 2:
                    {
                        SendMsg("Start Chrome");
                        break;
                    }
                case 3:
                    {
                        SendMsg("Start Mozilla");
                        break;
                    }
                case 4:
                    {
                        SendMsg("Start Edge");
                        break;
                    }
                default:
                    break;
            }
        }
        public void SendBrowser(int choice, int user)
        {
            switch (choice)
            {
                case 1:
                    {
                        SendMsg("Start Opera", user);
                        break;
                    }
                case 2:
                    {
                        SendMsg("Start Chrome", user);
                        break;
                    }
                case 3:
                    {
                        SendMsg("Start Mozilla", user);
                        break;
                    }
                case 4:
                    {
                        SendMsg("Start Edge", user);
                        break;
                    }
                default:
                    break;
            }
        }
        public void ShowAllClients()
        {
            lock (clients)
            {
                foreach (var item in clients)
                {
                    Console.WriteLine($"<ID: {item.ID}> " + $"Connected: {item.socket.Connected}");
                }
            }
        }
        public void ChooseDirectory(int ID_choice)
        {
            int choiceDisk = 0;
            string directory = String.Empty;
           
            lock (clients)
            {

                try
                {
                    Console.WriteLine("Which Disk you want to check C: - [1] D: - [2]");
                    choiceDisk = int.Parse(Console.ReadLine());
                    Console.Clear();
                    Console.WriteLine("Enter a directory name");
                    directory = Console.ReadLine();
                    Console.Clear();
                    switch (choiceDisk)
                    {
                        case 1:
                            {
                                for (int i = 0; i < clients.Count; i++)
                                {
                                    if (clients[i].ID == ID_choice)
                                    {
                                        SendMsg(@"C:\" + directory, i);

                                    }
                                }
                                break;
                            }

                        case 2:
                            {
                                for (int i = 0; i < clients.Count; i++)
                                {
                                    if (clients[i].ID == ID_choice)
                                    {
                                        SendMsg(@"D:\" + directory, i);
                                    }
                                }
                                break;
                            }
                        default:
                            break;
                            Console.Clear();
                    }

                    Console.WriteLine("All files which you can start\n" + GetMsg().ToString());
                    Console.ReadLine();
                }
                catch (Exception)
                {

                    Console.WriteLine("You can choose only numbs!");
                }

            }
        }
        public void ThrowDisconnect(int ID_choice)
        {
            lock (clients)
            {
                for (int i = 0; i < clients.Count; i++)
                {

                    if (clients[i].ID == ID_choice)
                    {
                        SendMsg("Exit", i);
                        clients[i].socket.Disconnect(false);

                        clients.RemoveAt(i);
                    }

                }

            }
        }
        public void StartBrowser(int ID_choice)
        {
            int server_choice = 0;
            lock (clients)
            {

                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].ID == ID_choice)
                    {
                        Console.WriteLine("Choice a Browser\nOpera: 1\nChrome: 2\nMozilla FireFox: 3\n Edge: 4");
                        try
                        {
                            server_choice = int.Parse(Console.ReadLine());
                            if (server_choice > 4)
                            {
                                throw new Exception();
                            }
                            SendBrowser(server_choice, i);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("You can entry only numbs");
                            i = 0;
                        }

                    }

                }
            }
        }
        public void CheckClient()
        {
            foreach (var item in clients)
            {
                if(item.socket.Connected == false)
                {
                    clients.Remove(item);
                }
            }
        }

    }
}
