using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class NakamaAutomaticMatch : MonoBehaviour
{
    private string _emailText = "";
    private string _inputText = "";
    private string _rankText = "";
    private string _healthText = "";
    private string _regionText = "";

    private  string _threshold="";
    private const string Scheme = "http";
    private const string Host = "localhost";
    private const int Port = 7350;
    private const string ServerKey = "defaultkey";

    private IClient _client;
    private ISocket _socket;
    private ISession _session;

    private IMatch _match;
    private string _matchId = "";
    private string _joinMatchId = "";

    private bool _includeHealth;
    private bool _strictHealth;
    private bool _includeRegion;
    private bool _strictRegion;
    private bool _includeRank;
    private bool _strictRank;


    void OnGUI()
    {
        GUILayout.Label("email");
        _emailText = GUILayout.TextField(_emailText, 25, GUILayout.Width(200));
        GUILayout.Label("input");
        _inputText = GUILayout.TextField(_inputText, 25, GUILayout.Width(200));
        GUILayout.Label("rank");
        _rankText = GUILayout.TextField(_rankText, 25, GUILayout.Width(200));
        GUILayout.Label("health");
        _healthText = GUILayout.TextField(_healthText, 25, GUILayout.Width(200));
        GUILayout.Label("region");
        _regionText = GUILayout.TextField(_regionText, 25, GUILayout.Width(200));
        GUILayout.Label("threshold");
        _threshold = GUILayout.TextField(_threshold, 25, GUILayout.Width(200));

        _includeHealth = GUILayout.Toggle(_includeHealth, "Include Health");
        _strictHealth = GUILayout.Toggle(_strictHealth, "Strict Health");
        _includeRegion = GUILayout.Toggle(_includeRegion, "Include Region");
        _strictRegion = GUILayout.Toggle(_strictRegion, "Strict Region");
        _includeRank = GUILayout.Toggle(_includeRank, "Include Rank");
        _strictRank = GUILayout.Toggle(_strictRank, "Strict Rank");


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
            SendMessage(_inputText);
        }

        if (GUILayout.Button("Leave", GUILayout.Width(100)))
        {
            _socket.LeaveMatchAsync(_match);
        }
    }

    private async Task Authenticate()
    {
        _client = new Client(Scheme, Host, Port, ServerKey);
        _socket = Nakama.Socket.From(_client);

        // Authenticate with device ID
        _session = await _client.AuthenticateEmailAsync(_emailText, "123456789");

        Debug.LogError($"Authenticated: {_session.UserId}");

        // Connect socket
        await _socket.ConnectAsync(_session);
        Debug.LogError("Socket connected.");

        // Listen to match data
        _socket.ReceivedMatchState += OnReceivedMatchState;

        _socket.ReceivedMatchmakerMatched += OnMatched;
        _socket.ReceivedMatchPresence += OnMatchPresence;
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
        _match = await _socket.JoinMatchAsync(newMatch);


        StringBuilder builder = new StringBuilder();

        builder.Append("on matched:");

        foreach (var item in newMatch.Users)
        {
            builder.Append(item.Presence.Username).Append(Environment.NewLine);
        }

        Debug.LogError(builder.ToString());
    }


    private void OnReceivedMatchState(IMatchState matchState)
    {
        string receivedMessage = System.Text.Encoding.UTF8.GetString(matchState.State);
        Debug.LogError($"Received from match: {receivedMessage}");
    }


    private async void SendMessage(string message)
    {
        if (_match != null)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            await _socket.SendMatchStateAsync(_match.Id, 0, data);
            Debug.LogError($"Sent message: {message}");
        }
    }

    private async void CreateMatch()
    {
        var numDic = new Dictionary<string, double>
            { { HealthKey, int.Parse(_healthText) }, { RankKey, int.Parse(_rankText) } };
        var stringDic = new Dictionary<string, string>() { { RegionKey, _regionText } };

        var intThreshold = int.Parse(_threshold);
        
        
        var ticket = await _socket.AddMatchmakerAsync(CreateQuery(), 2, 2, stringDic, numDic);

        Debug.LogError($"ticket id:{ticket.Ticket}");
    }


    private string CreateQuery()
    {
        StringBuilder builder = new();

        int threshold = int.Parse(_threshold);
        int currentHealth = int.Parse(_healthText);
        int currentRank = int.Parse(_rankText);
        
        if (_includeRegion)
        {
            if (!_regions.Contains(_regionText))
                throw new Exception("region not exist");

            if (_strictRegion)
                builder.Append("+");
            builder.Append($"properties.{RegionKey}:{_regionText}");
            builder.Append(" ");
        }

        if (_includeHealth)
        {
            if (_strictHealth)
                builder.Append("+");
            builder.Append($"properties.{HealthKey}:<={currentHealth+threshold}");
            builder.Append(" ");
            if (_strictHealth)
                builder.Append("+");
            builder.Append($"properties.{HealthKey}:>={currentHealth-threshold}");
            builder.Append(" ");
            
        }

        if (_includeRank)
        {
            if (_strictRank)
                builder.Append("+");
            builder.Append($"properties.{RankKey}:<={currentRank+threshold}");
            builder.Append(" ");
            if (_strictHealth)
                builder.Append("+");
            builder.Append($"properties.{RankKey}:>={currentRank-threshold}");
            builder.Append(" ");
        }


        var result = builder.ToString();
        if (result == string.Empty)
            result = "*";
        
        Debug.LogError($"query is: {result.ToString()}");
        return result;
    }

    private static List<string> _regions = new() { "asia", "eu", "africa" };
    private const string HealthKey = "health";
    private const string RankKey = "rank";
    private const string RegionKey = "region";
}