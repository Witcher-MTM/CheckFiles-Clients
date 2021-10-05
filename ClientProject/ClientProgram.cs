using Microsoft.Win32;
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
            RegistryKey key = Registry.CurrentUser;
            Client client = new Client();
            client.ReadServerConfig(key);
            client.Connect();
           
            if (!client.ExistRegisteryDir(key))
            {
               
                RegistryKey newKey = key.CreateSubKey("ConsoleSize");
                newKey.SetValue("HEIGHT", "500");
                newKey.SetValue("WIDTH", "800");
               
            }
            else
            {
                RegistryKey newKey = key.OpenSubKey("ConsoleSize");

                Console.BufferHeight = int.Parse(newKey.GetValue("HEIGHT").ToString());
                Console.BufferWidth = int.Parse(newKey.GetValue("WIDTH").ToString());
            }




            while (true)
            {
               
                client.GetServerCommand(client.GetMsg());
               
            }


        }
    }
}
