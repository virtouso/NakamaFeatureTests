using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

namespace Script.NonAuthoritative
{
    public class NakamaManager : MonoBehaviour
    {
       
        private IClient _client;
        private ISession _session;
        private ISocket _socket;
        private const string MATCHMAKING_QUERY = "*";

        private async void Start()
        {
            // Initialize Nakama client
            _client = new Client("http", "127.0.0.1", 7350, "defaultkey");
            _socket = _client.NewSocket();

            // Authenticate the user
            _session = await _client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
            await _socket.ConnectAsync(_session);

            Debug.Log("Connected to Nakama");

            
            
            // Start matchmaking
            await StartMatchmaking();
        }

        // Start matchmaking with 2 player limit
        private async Task StartMatchmaking()
        {
            var properties = new Dictionary<string, string>
            {
                { "mode", "1" } // You can define custom properties if needed
            };
            await _socket.AddMatchmakerAsync(MATCHMAKING_QUERY, 2, 2, properties);
            Debug.Log("Matchmaking started...");
        }
        
        
        private void OnEnable()
        {
            _socket.ReceivedMatchmakerMatched += OnMatchmakerMatched;
        }

        private void OnDisable()
        {
            _socket.ReceivedMatchmakerMatched -= OnMatchmakerMatched;
        }

        private async void OnMatchmakerMatched(IMatchmakerMatched matched)
        {
            Debug.Log($"Match found! Match ID: {matched.MatchId}");

            // Join the matched game
            var match = await _socket.JoinMatchAsync(matched.MatchId);
            Debug.Log($"Joined match with ID: {match.Id}");
        }

        
        
        
    }
}
