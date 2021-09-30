using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace ClientProject
{
    public class Client
    {
        public int ID;
        public string ipAddr;
        public int port;
        public IPEndPoint iPEndPoint;
        public Socket socket;
        public Client()
        {
            this.ID++;
            this.ipAddr = "127.0.0.1";
            this.port = 8000;
            this.iPEndPoint = new IPEndPoint(IPAddress.Parse(ipAddr), port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public Client(Socket socket)
        {
          
            this.socket = socket;
            
        }

        public void Connect()
        {
            int exit = 0;
            bool trycon = false;
            do
            {
                try
                {
                    socket.Connect(iPEndPoint);
                    trycon = true;
                    Console.WriteLine("Connect success!");
                }
                catch (Exception)
                {

                    Console.WriteLine("Try to connect...");
                    exit++;
                    Thread.Sleep(700);
                   
                }
                if (exit == 5)
                {
                    Console.WriteLine("Try to Connect Failed");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            } while (!trycon);
            Console.Clear();
        }
        public void SendMsg(string sms)
        {
            byte[] data = new byte[256];
            data = Encoding.Unicode.GetBytes(sms);
            socket.Send(data);
        }
        public void SendMsg(string[] sms)
        {
            string outstr = String.Empty;
            byte[] data = new byte[256];
            foreach (var item in sms)
            {
                data = Encoding.Unicode.GetBytes(outstr+=$"{Path.GetFileName(item)}\n");
            }
         
          
            socket.Send(data);
        }
        public StringBuilder GetMsg()
        {
            int bytes = 0;
            byte[] data = new byte[256];
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                bytes = socket.Receive(data);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (socket.Available > 0);

            if(stringBuilder.ToString().ToLower() == "exit")
            {
                Environment.Exit(0);
            }
            return stringBuilder;
        }
        public void GetServerCommand(StringBuilder command)
        {
            string[] arr_command = command.ToString().Split("\n");
            string[] tmp = { };
            if (arr_command[0] == "search")
            {
                try
                {
                    tmp = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop", "*", SearchOption.AllDirectories);
                    SendMsg(tmp);
                }
                catch (Exception ex)
                {

                    SendMsg(ex.Message);
                }
            }
            if (arr_command[0] == "start")
            {
                string[] arg = { };
                try
                {
                    tmp = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop", "*", SearchOption.AllDirectories);

                    foreach (var item in tmp)
                    {
                        if (item.ToLower().EndsWith(arr_command[1].ToLower()))
                        {
                            arg = item.Split(".");
                            Process.Start(new ProcessStartInfo(item, arg[1]) { UseShellExecute = true });
                            SendMsg("Success start!");
                            break;
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    SendMsg(ex.Message);

                }
            }
            else
            {
                if (Directory.Exists(command.ToString()))
                {
                    try
                    {
                        SendMsg(Directory.GetFiles(command.ToString()));
                    }
                    catch (Exception ex)
                    {

                        SendMsg(ex.Message);
                    }
                      
                }
                else
                {
                    SendMsg("Not found Directory");
                }
            }
           
        }
        public void Search()
        {
        try
            {
                SendMsg(Directory.GetFiles(@$"C:\Users\" + $"{Environment.UserName}" + @"\Desktop", "*", SearchOption.AllDirectories));
            }
            catch (Exception ex)
            {
                SendMsg(ex.Message);
            }
        }

    }
}
