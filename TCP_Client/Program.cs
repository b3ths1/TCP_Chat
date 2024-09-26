using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888;
    private const string ServerIp = "127.0.0.1";

    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port); //Инициализира клас TcpClient като се свързва към зададения сървър и неговия порт. Може да изпраща и получава данни.
        Console.WriteLine("Connected to server. Start chatting!"); //Принтира съобщение, за да се потвърди на клиента, че е свързан.

        NetworkStream stream = client.GetStream(); //Връща NetworkStream, използван за изпращане и получаване на данни.

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start(stream);

        while (true)
        {
            string message = Console.ReadLine();
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    static void ReceiveMessages(object obj)
    {
        NetworkStream stream = (NetworkStream)obj;
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
                Console.WriteLine(message);
            }
            catch (Exception)
            {
                break;
            }
        }
    }
}
