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

        byte[] buffer = new byte[1024]; //инициализира се буфер, който запазва данните на съобщението
        int bytesRead; //променлива, която следи за прочетените байтове

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length); //чете данни в буфера и връща броя на прочетените байтове
                if (bytesRead == 0) //ако няма прочетени байтове/съобщение, да се прекъсне цикъла
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); //обратно - превръща масива с байтове в string
                Console.WriteLine($"Received: {message}"); //показва полученото съобщение

                BroadcastMessage(tcpClient, message); //показва съобщението на всички клиенти
            }
            catch (Exception) //ако има грешки/изключения, да се прекъсне цикъла
            {
                break;
            }
        }

        clients.Remove(tcpClient); //маха клиентите от списъка при прекъсване на връзката/други грешки/изключения
        tcpClient.Close(); //затваряне на връзката
    }

    static void BroadcastMessage(TcpClient sender, string message) //метода за broadcast=ване съобщение на всички клиенти, използван горе ↑
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message); //превръща съобщението в масив от ASCII символи/с ASCII encoding

        foreach (TcpClient client in clients) //за всеки клиент в списъка с клиенти
        {
            if (client != sender) //идеята е съобщението да се broadcast-не на всички клиенти без този, който го е написал/пратил
            {
                NetworkStream stream = client.GetStream();
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
    }
}
