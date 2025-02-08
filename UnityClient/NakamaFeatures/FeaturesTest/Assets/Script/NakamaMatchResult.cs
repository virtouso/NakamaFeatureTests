using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

namespace Script
{
    public class NakamaMatchResult : MonoBehaviour
    {
        [SerializeField] private bool isChangiz;
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

        private async Task AuthenticateAsChangiz()
        {
            client = new Client(scheme, host, port, serverKey);
            socket = Nakama.Socket.From(client);

            // Authenticate with device ID
            session = await client.AuthenticateEmailAsync("changiz@yahoo.com", "123456789", emailText.Split('@')[0]);

            Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");
            await client.LinkDeviceAsync(session, SystemInfo.deviceUniqueIdentifier);

            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");
            Debug.Log(session.UserId);
            await SendMatchResult();
        }


        private async Task AuthenticateAsEssi()
        {
            client = new Client(scheme, host, port, serverKey);
            socket = Nakama.Socket.From(client);

            // Authenticate with device ID
            session = await client.AuthenticateEmailAsync("essi@yahoo.com", "123456789", emailText.Split('@')[0]);

            Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");
          //  await client.LinkDeviceAsync(session, SystemInfo.deviceUniqueIdentifier);

            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");
            Debug.Log(session.UserId);
           await SendMatchResult();
        }


        private async Task SendMatchResult()
        {
            var obj = new { match_id = "1234", winner = "ec50892f-fb12-4381-8ea7-2c24deb34b1b", loser = "c0e25509-5224-4626-9b9e-cb0ee218c115", winner_hero = "hunter", loser_hero = "hunter" };
            var result = await client.RpcAsync(session, "match/match_result", JsonConvert.SerializeObject(obj));
            Debug.Log(result.Payload);
        }


        private async void Start()
        {
    
                await AuthenticateAsChangiz();
        
                await AuthenticateAsEssi();
       //     await SendMatchResult();
        }
    }
}