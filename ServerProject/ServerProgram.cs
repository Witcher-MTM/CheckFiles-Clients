using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClientProject;
using System.Threading;

namespace ServerProject
{
    class ServerProgram
    {

        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartServer();
            
            try
            {
                Task.Factory.StartNew(() => server.Connects());
               
                while (true)
                {
                    Console.WriteLine("Start Browser: 1\n-Disconnect from server: 2\nCheck download apps: 3");
                    try
                    {
                        server.SendCommand(int.Parse(Console.ReadLine()));
                        Console.Clear();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("You can enter only numbs![1][2][3]");
                        Thread.Sleep(1000);
                        Console.Clear();
                    }
                 
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
    }
}
