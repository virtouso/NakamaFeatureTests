using System;
using System.Collections.Generic;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;

namespace Script
{
    public class ChatStuff : MonoBehaviour
    {
        private const string ServerKey = "defaultkey";
        private const string Host = "127.0.0.1"; // Change to your server's IP
        private const int Port = 7350;
        private const bool UseSSL = false;

        private IClient Client { get; set; }
        private ISocket Socket { get; set; }
        private ISession Session { get; set; }
        private IChannel Channel { get; set; }

        private string emailText = "";
        private string friendText = "";
        private string messageText = "";

        private void OnGUI()
        {
            emailText = GUILayout.TextField(emailText, 400, GUILayout.Width(200));
            friendText = GUILayout.TextField(friendText, 400, GUILayout.Width(200));
            messageText = GUILayout.TextField(messageText, 400, GUILayout.Width(200));

            
            
            if (GUILayout.Button("Auth"))
            {
                Auth();
            }

            if (GUILayout.Button("Join"))
            {
                Join();
            }

            if (GUILayout.Button("Send"))
            {
                Send();
            }
        }

        private async void Auth()
        {
            Client = new Client("http", Host, Port, ServerKey );
            Socket = Nakama.Socket.From(Client);
        
            Session = await Client.AuthenticateEmailAsync(emailText, "123456789", emailText.Split('@')[0]);
            Debug.LogError($"{Session.Username} is logged in####{Session.UserId}");
            await Socket.ConnectAsync(Session);
            Debug.LogError(Session.Username+"--"+  Session.AuthToken);
            Socket.ReceivedChannelMessage += OnChannelMessageReceived;
        }

        private void OnChannelMessageReceived(IApiChannelMessage obj)
        {
            Debug.LogError($"{obj.Username}: {obj.Content} : {obj.ToJson()}");
        }

        private async void Join()
        {
            Channel = await Socket.JoinChatAsync(friendText, ChannelType.DirectMessage, true);
            var result = await Client.ListChannelMessagesAsync(Session, Channel.Id, 10, true);

            foreach (var m in result.Messages)
            {
                Debug.LogError($"{m.Username}, {m.Content}");
            }
        }

        async void Send()
        {
            await Socket.WriteChatMessageAsync(Channel.Id, new Dictionary<string, string> {{"message", messageText}}.ToJson());
        }
    }
}