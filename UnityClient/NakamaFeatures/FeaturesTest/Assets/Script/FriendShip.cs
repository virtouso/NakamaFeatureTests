using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nakama;
using UnityEngine;

public class FriendShip : MonoBehaviour
{
    private const string scheme = "http";
    private const string host = "127.0.0.1";
    private const int port = 7350;
    private const string serverKey = "defaultkey";

    private IClient client;
    private ISocket socket;
    private ISession session;

    private string emailText = "";
    private string friendNameText = "";

    private void OnGUI()
    {
        emailText = GUILayout.TextField(emailText, 25, GUILayout.Width(200));
        friendNameText = GUILayout.TextField(friendNameText, 25, GUILayout.Width(200));
        if (GUILayout.Button("Auth", GUILayout.Width(100)))
        {
            Authenticate();
        }


        if (GUILayout.Button("Send", GUILayout.Width(100)))
        {
            SendFriendRequest();
        }

        if (GUILayout.Button("List", GUILayout.Width(100)))
        {
            ListFriends();
        }

        if (GUILayout.Button("Accept", GUILayout.Width(100)))
        {
            AcceptFriendsRequest();
        }
    }


    private async void Authenticate()
    {
        client = new Client(scheme, host, port, serverKey);
        socket = Nakama.Socket.From(client);

        // Authenticate with device ID
        session = await client.AuthenticateEmailAsync(emailText, "123456789",emailText.Split('@')[0]);
        Debug.LogError($"Authenticated: {session.Username}");
        var result =await client.ListFriendsAsync(session);
 
    
        await socket.ConnectAsync(session);
        Debug.LogError("Socket connected.");
    }


    private void SendFriendRequest()
    {
       client.AddFriendsAsync(session, Array.Empty<string>(),new string[]{friendNameText});
       

    }

    private async void ListFriends()
    {
        var res =await client.ListFriendsAsync(session);

        foreach (var item in res.Friends)
        {
            Debug.LogError($"log: {item.User.Username}.....{item.UpdateTime}....{item.State}...{item.User.Online}");
            
        }
        
        
    }

    private async void AcceptFriendsRequest()
    {
        var res =await client.ListFriendsAsync(session);
       await client.AddFriendsAsync(session, Array.Empty<string>(),res.Friends.Select(x=>x.User.Username));
       var ress =await client.ListFriendsAsync(session);

       foreach (var item in ress.Friends)
       {
           Debug.LogError($"log: {item.User.Username}.....{item.UpdateTime}....{item.State}");
       }
    }
}