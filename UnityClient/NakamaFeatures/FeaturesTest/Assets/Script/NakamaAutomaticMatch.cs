using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class NakamaAutomaticMatch : MonoBehaviour
{
    private string emailText = "";
    private string inputText = "";


    private const string scheme = "http";
    private const string host = "127.0.0.1";
    private const int port = 7350;
    private const string serverKey = "defaultkey";

    private IClient client;
    private ISocket socket;
    private ISession session;

    private IMatch match;
    private string matchId = "";
    private string joinMatchId = "";


    void OnGUI()
    {
        emailText = GUILayout.TextField(emailText, 25, GUILayout.Width(200));
        inputText = GUILayout.TextField(inputText, 25, GUILayout.Width(200));

        GUILayout.Space(10);


        if (GUILayout.Button("Auth", GUILayout.Width(100)))
        {
            Authenticate();
        }

        if (GUILayout.Button("Create", GUILayout.Width(100)))
        {
            CreateMatch();
        }

        if (GUILayout.Button("Send", GUILayout.Width(100)))
        {
            SendMessage(inputText);
        }

        if (GUILayout.Button("Leave", GUILayout.Width(100)))
        {
            socket.LeaveMatchAsync(match);
        }
    }

    private async Task Authenticate()
    {
        client = new Client(scheme, host, port, serverKey);
        socket = Nakama.Socket.From(client);

        // Authenticate with device ID
        session = await client.AuthenticateEmailAsync(emailText, "123456789");
        Debug.LogError($"Authenticated: {session.UserId}");

        // Connect socket
        await socket.ConnectAsync(session);
        Debug.LogError("Socket connected.");

        // Listen to match data
        socket.ReceivedMatchState += OnReceivedMatchState;
        socket.ReceivedMatchmakerMatched += OnMatched;
        socket.ReceivedMatchPresence += OnMatchPresence;
    }

    private void OnMatchPresence(IMatchPresenceEvent message)
    {
        Debug.LogError($"{message.MatchId} presence updated");

        foreach (var item in message.Joins)
        {
            Debug.LogError($"{item.Username} joined the match. {item.Persistence}  {item.Status}");
        }

        foreach (var item in message.Leaves)
        {
            Debug.LogError($"{item.Username} left the match {item.Persistence}  {item.Status}"); 
        }
    }

    private async void OnMatched(IMatchmakerMatched newMatch)
    {
        Debug.LogError(newMatch.MatchId);
        match = await socket.JoinMatchAsync(newMatch);
    }


    private void OnReceivedMatchState(IMatchState matchState)
    {
        string receivedMessage = System.Text.Encoding.UTF8.GetString(matchState.State);
        Debug.LogError($"Received from match: {receivedMessage}");
    }


    private async void SendMessage(string message)
    {
        if (match != null)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            await socket.SendMatchStateAsync(match.Id, 0, data);
            Debug.LogError($"Sent message: {message}");
        }
    }

    private async void CreateMatch()
    {
        var ticket = await socket.AddMatchmakerAsync("*", 2, 2);

        Debug.LogError($"ticket id:{ticket.Ticket}");
    }

    private async void JoinMatch(string inputMatchId)
    {
        try
        {
            match = await socket.JoinMatchAsync(inputMatchId);
            matchId = match.Id;
            Debug.LogError($"Joined match with ID: {inputMatchId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join match: {e.Message}");
        }
    }
}