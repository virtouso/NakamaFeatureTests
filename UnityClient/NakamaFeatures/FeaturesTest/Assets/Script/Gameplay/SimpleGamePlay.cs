using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nakama;
using UnityEngine;

namespace Script.Gameplay
{
    public class SimpleGamePlay : MonoBehaviour
    {
        private string emailText = "";
        private string inputText = "";
        private string matchNameText = "";

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


        private void OnGUI()
        {
            GUILayout.Label("email");
            emailText = GUILayout.TextField(emailText, 25, GUILayout.Width(200));
            GUILayout.Label("match name");
            matchNameText = GUILayout.TextField(matchNameText, 25, GUILayout.Width(200));
            GUILayout.Label("guess text");
            inputText = GUILayout.TextField(inputText, 25, GUILayout.Width(200));
            
           GUILayout.Space(20); 
            
           
           if (GUILayout.Button("Auth", GUILayout.Width(100)))
           {
               Authenticate();
           }
           
           if (GUILayout.Button("Create", GUILayout.Width(100)))
           {
               CreateMatchWithName();
               
           }

           if (GUILayout.Button("Send", GUILayout.Width(100)))
           {
               SendGuess(inputText);
           }

           if (GUILayout.Button("Leave", GUILayout.Width(100)))
           {
               socket.LeaveMatchAsync(match);
           }
           
           
           
        }
        
        
        private async void CreateMatchWithName()
        {
            Debug.LogError($"match name: {matchNameText}");
            match = await socket.CreateMatchAsync(matchNameText);
            Debug.LogError($"match: { match.Id}");
        }
        
        private async void SendGuess(string message)
        {
            if (match != null)
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                await socket.SendMatchStateAsync(match.Id, 0, data);
                Debug.LogError($"Sent message: {message}");
            }
        }
        
        
        private async void Authenticate()
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


        private const int RoundCount = 7;
        private GameData _gameData = new ();

        private async void StartScenario()
        {
            if (!ShouldRunMatch(match)) return;
            
            
        }



        private bool ShouldRunMatch(IMatch match)
        {
            return match.Self.Username == match.Presences.OrderBy(x => x.Username).First().Username;
        }
        
    }
}