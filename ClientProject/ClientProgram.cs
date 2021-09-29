using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientProject
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Connect();
            while (true)
            {

                client.GetServerCommand(client.GetMsg());
                
                
            }
          
        }
    }
}
