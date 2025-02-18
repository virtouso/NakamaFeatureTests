using System;
using System.Threading.Tasks;
using Nakama;
using Nakama.Console;
using Newtonsoft.Json;
using UnityEngine;
using ApiResponseException = Nakama.ApiResponseException;

namespace Script.Shop
{
    public class NakamaTest : MonoBehaviour
    {
        private const string scheme = "http";
        private const string host = "127.0.0.1";
        private const int port = 7350;
        private const string serverKey = "defaultkey";

        private IClient client;
        private ISocket socket;
        private ISession session;

        private string emailText = "";
        private string groupNameText = "";
        private string userIdText = "";

        private async Task Authenticate()
        {
            client = new Client(scheme, host, port, serverKey);
            socket = Nakama.Socket.From(client);

            
            
            // Authenticate with device ID
            session = await client.AuthenticateEmailAsync("changiz@yahoo.com", "123456789", emailText.Split('@')[0]);
            Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");

            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");
        }
        
        
        
        
        

        private async void Start()
        {
            await Authenticate();
           // await TestAll();
        }


  public      class Response<T>
        {
            public T Payload { get; set; }
            public string message { get; set; }
            public int StatusCode { get; set; }
            public string Code { get; set; }
        }
        
        
        
        
    }
}