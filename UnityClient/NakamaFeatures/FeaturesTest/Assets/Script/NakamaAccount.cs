using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

namespace Script
{
    public class NakamaAccount : MonoBehaviour
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
            await client.LinkDeviceAsync(session, SystemInfo.deviceUniqueIdentifier);
            
            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");
        }

        private async Task MetaTest()
        {
            var result = await client.RpcAsync(session, "account/update_meta");
            Debug.Log(result.Payload);
        }

        private async void Start()
        {
            await Authenticate();
            await MetaTest();
        }
    }
}