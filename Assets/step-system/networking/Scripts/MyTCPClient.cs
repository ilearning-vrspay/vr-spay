using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MyTCPClient 
{
	private int HEADER = 64;
	private string SERVER = "127.0.0.1";
	private int PORT = 5050;
	private string DISCONNECT_MESSAGE = "!DISCONNECT";
	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;

	public bool isConnected;

	public MyTCPClient(int header, string server, int port)
    {
		this.HEADER = header;
		this.SERVER = server;
		this.PORT = port;
    }

	public MyTCPClient()
	{
		
	}

	public void ConnectToTcpServer()
	{
		try
		{
			isConnected = true;
			socketConnection = new TcpClient(SERVER, PORT);
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}

	public void ConnectToTcpServer(Action action)
	{
		try
		{
			isConnected = true;
			socketConnection = new TcpClient(SERVER, PORT);
			clientReceiveThread = new Thread(new ThreadStart(() => ListenForData(action)));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}

	public void DisconnectFromTcpServer()
    {
		SendMessage(DISCONNECT_MESSAGE);
		isConnected = false;
    }
	
	private void ListenForData()
	{
		try
		{
			Byte[] bytes = new Byte[1024];
			while (isConnected)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	private void ListenForData(Action action)
	{
		try
		{
			Byte[] bytes = new Byte[1024];
			while (isConnected)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
						action();
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	public void SendMessage(string clientMessage)
	{
		if (socketConnection == null)
		{
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				Debug.Log("[MSG Sent] " + clientMessage);
				byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);
				int msgLength = clientMessageAsByteArray.Length;
				string msgLengthMessage = msgLength.ToString();
				byte[] msgLengthMessageAsByteArray = Encoding.UTF8.GetBytes(msgLengthMessage);
				byte[] msgLengthMessageAsByteArrayPadded = new byte[HEADER];
				for (int i = 0; i < msgLengthMessageAsByteArrayPadded.Length; i++)
				{
					msgLengthMessageAsByteArrayPadded[i] = Encoding.UTF8.GetBytes(" ")[0];
				}

				for (int i = 0; i < msgLengthMessageAsByteArray.Length; i++)
				{
					msgLengthMessageAsByteArrayPadded[i] = msgLengthMessageAsByteArray[i];
				}
				stream.Write(msgLengthMessageAsByteArrayPadded, 0, msgLengthMessageAsByteArrayPadded.Length);
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log(socketException);
		}
	}

	public static void ConnectSendReceive(string message, Action response)
    {
		MyTCPClient client = new MyTCPClient();
		client.ConnectToTcpServer(response);
		client.SendMessage(message);
		client.DisconnectFromTcpServer();
	}
}
