using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888; //задава постоянен номер за порта на сървъра
    private const string ServerIp = "127.0.0.1"; //задава IP-то на сървъра

    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port); //Инициализира клас TcpClient като се свързва към зададения сървър и неговия порт. Може да изпраща и получава данни.
        Console.WriteLine("Connected to server. Start chatting!"); //Принтира съобщение, за да се потвърди на клиента, че е свързан.

        NetworkStream stream = client.GetStream(); //Връща NetworkStream, използван за изпращане и получаване на данни.

        //Thread се използва за обработка на съобщения от сървъра.
        Thread receiveThread = new Thread(ReceiveMessages); 
        receiveThread.Start(stream);

        while (true) //безкраен цикъл (докато не се даде знак да спре)
        {
            string message = Console.ReadLine(); //чете съобщението, написано от клиента
            byte[] buffer = Encoding.ASCII.GetBytes(message); //превръща съобщението в масив от ASCII символи/с ASCII encoding
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    static void ReceiveMessages(object obj) //този метод се използва при Thread-овете горе ↑
    {
        NetworkStream stream = (NetworkStream)obj;
        byte[] buffer = new byte[1024]; //инициализира се буфер, който запазва данните на съобщението
        int bytesRead; //променлива, която следи за прочетените байтове

        while (true)
        {
            try //try-catch, за да се предпази програмата от грешки или нещо такова :')
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length); //чете данни в буфера и връща броя на прочетените байтове
                if (bytesRead == 0) //ако няма прочетени байтове/съобщение, да се прекъсне цикъла
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); //обратно - превръща масива с байтове в string
                Console.WriteLine(message); //съобщението излиза
            }
            catch (Exception) //ако има грешки, да се прекъсне цикъла
            {
                break; 
            }
        }
    }
}
