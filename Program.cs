using System.Net.Sockets;
using System.Text;

namespace P.Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("192.168.0.105", 8021);
            NetworkStream networkStream = tcpClient.GetStream();
            byte[] login = Encoding.ASCII.GetBytes("auth ClueCon");
            networkStream.Write(login, 0, login.Length);
            byte[] command = Encoding.ASCII.GetBytes("api show calls ");
            networkStream.Write(command, 0, command.Length);
            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            int bytesRead = networkStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));




         
        }
    }
}