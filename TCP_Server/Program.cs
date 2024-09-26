using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static readonly List<TcpClient> clients = new List<TcpClient>(); //list to keep track of all clients
    private const int Port = 8888; //задава се постоянен номер на порта за сървъра

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port); //създава TCP listener, който следи за входящи връзки на всеки локален IP адрес и конкретен порт
        server.Start(); //включва TCP listener-а
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient(); //приема нова връзка с клиент
            clients.Add(client); //добавя клиента към списъка от горе ↑
            Thread clientThread = new Thread(HandleClient); //създаване на thread за обработка на комуникацията със свързания клиент
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj) //метод, който се полза за клиента горе ↑
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream stream = tcpClient.GetStream(); //Network stream - за четене и запис на данни между сървъра и клиента

        byte[] buffer = new byte[1024];
        int bytesRead;

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                BroadcastMessage(tcpClient, message);
            }
            catch (Exception)
            {
                break;
            }
        }

        clients.Remove(tcpClient);
        tcpClient.Close();
    }

    static void BroadcastMessage(TcpClient sender, string message)
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);

        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
    }
}
