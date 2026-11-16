using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using UnityEngine;

public class NetworkManager
{
    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;

    public event Action<string> OnMessageReceived;
    
    public NetworkManager(string ip, int port){
        client = new TcpClient();
        client.Connect(ip, port);
        stream = client.GetStream();
        reader = new StreamReader(stream);
    }

    public IEnumerator Connect()
    {     
        while (true)
        {
            if (stream != null && stream.DataAvailable)
            {
                string line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    OnMessageReceived?.Invoke(line);
                }
            }
            yield return null;
        }
    }

    public void Send(string msg)
    {
        if (stream != null && stream.CanWrite)
        {
            byte[] data = Encoding.ASCII.GetBytes(msg);
            stream.Write(data, 0, data.Length);
        }
    }

    public void Close()
    {
        reader?.Close();
        stream?.Close();
        client?.Close();
    }
}
