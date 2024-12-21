using System;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;


public class NakamaOnGuiTest : MonoBehaviour
{
    private const string scheme = "http";
    private const string host = "127.0.0.1"; // Your Nakama server IP if remote
    private const int port = 7350;
    private const string serverKey = "defaultkey";

    private IClient client;
    private ISocket socket;
    private ISession session;

    private IMatch match;
    private string matchId = "";
    private string joinMatchId = "";

    private List<string> logMessages = new List<string>();

    private async void Start()
    {
        client = new Client(scheme, host, port, serverKey);
        socket = Nakama.Socket.From(client);

        // Authenticate with device ID
        session = await client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        LogMessage($"Authenticated: {session.UserId}");

        // Connect socket
        await socket.ConnectAsync(session);
        LogMessage("Socket connected.");
   
        // Listen to match data
        socket.ReceivedMatchState += OnReceivedMatchState;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 500));

        GUILayout.Label("Nakama Test Client");

        // Create Match Button
        if (GUILayout.Button("Create Match"))
        {
            CreateMatch();
        }

        // Join Match Input Field
        GUILayout.Label("Enter Match ID to Join:");
        joinMatchId = GUILayout.TextField(joinMatchId, GUILayout.Width(250));

        // Join Match Button
        if (GUILayout.Button("Join Match"))
        {
            JoinMatch(joinMatchId);
        }

        // Display Current Match ID
        if (!string.IsNullOrEmpty(matchId))
        {
            GUILayout.Label($"Current Match ID: {matchId}");
        }

        // Send Message Button
        if (match != null && GUILayout.Button("Send Message"))
        {
            SendMessage("Hello from Unity!");
        }

        // Display Logs
        GUILayout.Label("Logs:");
        foreach (var log in logMessages)
        {
            GUILayout.Label(log);
        }

        GUILayout.EndArea();
    }

    private async void CreateMatch()
    {
        match = await socket.CreateMatchAsync();
        matchId = match.Id;
        LogMessage($"Match created with ID: {matchId}");
        Debug.Log(matchId);
    }

    private async void JoinMatch(string inputMatchId)
    {
        try
        {
            match = await socket.JoinMatchAsync(inputMatchId);
            matchId = match.Id;
            LogMessage($"Joined match with ID: {inputMatchId}");
        }
        catch (Exception e)
        {
            LogMessage($"Failed to join match: {e.Message}");
        }
    }

    private async void SendMessage(string message)
    {
        if (match != null)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            await socket.SendMatchStateAsync(match.Id, 0, data);
            LogMessage($"Sent message: {message}");
        }
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        string receivedMessage = System.Text.Encoding.UTF8.GetString(matchState.State);
        LogMessage($"Received from match: {receivedMessage}");
    }

    private void LogMessage(string message)
    {
        Debug.Log(message);
        logMessages.Add(message);
    }

    private async void OnApplicationQuit()
    {
        if (socket != null)
        {
            await socket.CloseAsync();
        }
    }
}
