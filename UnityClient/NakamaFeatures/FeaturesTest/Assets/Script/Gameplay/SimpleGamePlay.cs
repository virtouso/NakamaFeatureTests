using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace Script.Gameplay
{
    public class SimpleGamePlay : MonoBehaviour
    {
        public Action<string> OnGameFinished { get; set; }
        public Action<int> OnRoundStarted { get; set; }

        private string emailText = "";
        private string inputText = "";
        // private string matchNameText = "";

        private string matchResultText = "";
        private string roundNumberText = "";


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

        private List<IUserPresence> presences = new();


        private void OnGUI()
        {
            GUILayout.Label("email");
            emailText = GUILayout.TextField(emailText, 25, GUILayout.Width(200));
            // GUILayout.Label("match name");
            // matchNameText = GUILayout.TextField(matchNameText, 25, GUILayout.Width(200));
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
                SendGuess(int.Parse(inputText));
            }

            if (GUILayout.Button("Leave", GUILayout.Width(100)))
            {
                socket.LeaveMatchAsync(match);
            }

            GUILayout.Label(roundNumberText);
            GUILayout.Label(matchResultText);
        }


        private async void CreateMatchWithName()
        {
            // Debug.LogError($"match name: {matchNameText}");
            // match = await socket.CreateMatchAsync(matchNameText);
            // Debug.LogError($"match: {match.Id}");

            var ticket = await socket.AddMatchmakerAsync("*", 2, 2);

            Debug.LogError($"ticket id:{ticket.Ticket}");
        }

        private async void SendGuess(int guessNumber)
        {
            if (match == null)
            {
                Debug.LogError("there is no active match");
                return;
            }

            if (_isHost)
            {
                CalculateGuess(session.Username, guessNumber);
                return;
            }

            byte[] data = System.Text.Encoding.UTF8.GetBytes(guessNumber.ToString());
            await socket.SendMatchStateAsync(match.Id, (long)OpCodes.CalculateGuess, data);
            Debug.LogError($"Sent message: {guessNumber}");
        }

        private async void SendStartRound(int roundNumber)
        {
            if (match == null)
            {
                Debug.LogError("there is no active match");
                return;
            }

            byte[] data = System.Text.Encoding.UTF8.GetBytes(roundNumber.ToString());
            await socket.SendMatchStateAsync(match.Id, (long)OpCodes.RoundStart, data);
            Debug.LogError($"sent message: {roundNumber}");
        }


        private async void Authenticate()
        {
            client = new Client(scheme, host, port, serverKey);
            socket = Nakama.Socket.From(client);

            // Authenticate with device ID
            session = await client.AuthenticateEmailAsync($"{emailText}@yahoo.com", "123456789", emailText);
            Debug.LogError($"Authenticated: {session.UserId}");

            // Connect socket
            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");

            // Listen to match data
            socket.ReceivedMatchState += OnReceivedMatchState;
            socket.ReceivedMatchmakerMatched += OnMatched;
            socket.ReceivedMatchPresence += OnMatchPresence;
        }


        private async void OnMatchPresence(IMatchPresenceEvent message)
        {
            Debug.LogError($"{message.MatchId} presence updated");

            foreach (var item in message.Joins)
            {
                Debug.LogError($"{item.Username} joined the match. {item.Persistence}  {item.Status}");
                presences.Add(item);
            }

            foreach (var item in message.Leaves)
            {
                Debug.LogError($"{item.Username} left the match {item.Persistence}  {item.Status}");
                presences.Remove(item);
            }

            //   if(presences.Count()<2) return;
            // await StartScenario();
        }

        private async void OnMatched(IMatchmakerMatched newMatch)
        {
            Debug.LogError($"matched:{newMatch.MatchId}");
            match = await socket.JoinMatchAsync(newMatch);
            await StartScenario(newMatch.Users.Select(x => x.Presence));
        }


        private void OnReceivedMatchState(IMatchState matchState)
        {
            string receivedMessage = System.Text.Encoding.UTF8.GetString(matchState.State);
            Debug.LogError($"Received from network match: {receivedMessage}");

            

            Debug.LogError($"received match enum:{(OpCodes)matchState.OpCode}");

            switch ((OpCodes)matchState.OpCode)
            {
                case OpCodes.RoundStart:
                    Debug.LogError($"Received Round Start:{matchState.State}");
                    ShowRoundNumberText(int.Parse(receivedMessage));
                    OnRoundStarted?.Invoke(int.Parse(receivedMessage));
                 
                    break;
                case OpCodes.CalculateGuess:
                    int guessedNumber = int.Parse(receivedMessage);
                    CalculateGuess(matchState.UserPresence.Username, guessedNumber);
                    break;
                case OpCodes.MatchFinish:
                    Debug.LogError("received match finish state");
                    ShowFinalText(receivedMessage);
                    OnGameFinished?.Invoke(receivedMessage);
                 
                    break;
            }
        }


        private void CalculateGuess(string senderId, int guessNumber)
        {
            Debug.LogError($"received calculated guess: {senderId}_{guessNumber}");
            if (!_isHost) return;

            if (!_gameData.Rounds.Last().Players.ContainsKey(senderId))
                _gameData.Rounds.Last().Players.Add(senderId,
                    new PlayerRoundData
                    {
                        UserId = senderId, GuessNumber = guessNumber,
                        RightGuess = guessNumber == _gameData.Rounds.Last().Secret
                    });
            else
            {
                _gameData.Rounds.Last().Players[senderId].GuessNumber = guessNumber;
                _gameData.Rounds.Last().Players[senderId].RightGuess = guessNumber == _gameData.Rounds.Last().Secret;
            }
        }

        private async Task SendResponse()
        {
            var response = JsonConvert.SerializeObject(matchResults);
            var responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
           // OnGameFinished?.Invoke(response);
            await socket.SendMatchStateAsync(match.Id, (long)OpCodes.MatchFinish, responseBytes);
        }


        private const int RoundCount = 7;
        private const int waitMiliseconds = 5000;
        private GameData _gameData = new();

        private Random _random = new();

        private async Task StartScenario(IEnumerable<IUserPresence> users)
        {
            if (!ShouldRunMatch(users)) return;

            for (int i = 0; i < RoundCount; i++)
            {
                _gameData.Rounds.Add(new Round { Secret = _random.Next(0, 2) });
                 HandleStartRound(i);
                
                await Task.Delay(waitMiliseconds);
            }

            CastScores();
            ShowFinalText(JsonConvert.SerializeObject(matchResults));
            await SendResponse();
        }


        
        
        
        private void HandleStartRound(int roundNumber)
        {
            ShowRoundNumberText(roundNumber);
            SendStartRound(roundNumber );
        }

        private Dictionary<string, int> matchResults = new();

        private void CastScores()
        {
            foreach (var round in _gameData.Rounds)
            {
                foreach (var item in round.Players)
                {
                    if (matchResults.ContainsKey(item.Key))
                        matchResults[item.Key] += Convert.ToInt32(item.Value.RightGuess);
                    else matchResults.Add(item.Key, Convert.ToInt32(item.Value.RightGuess));
                }
            }
        }


        private bool _isHost;

        private bool ShouldRunMatch(IEnumerable<IUserPresence> users)
        {
            _isHost = session.Username == users.ToList().OrderBy(x => x.Username).First().Username;
            Debug.LogError($"im the host: {match.Self.Username}");
            return _isHost;
        }

        private async void ShowRoundNumberText(int obj)
        {
            roundNumberText = $"New Round:{obj+1}";
        //    await Task.Delay(1000);
          //  roundNumberText = "";
        }


        private void ShowFinalText(string obj)
        {
            
            Debug.LogError($"-final data method--{obj}");
            
            var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(obj);
            StringBuilder result = new StringBuilder();

            result.Append("-----").Append(Environment.NewLine);
            
            foreach (var item in data)
            {
                result.Append($"{item.Key}: {item.Value}").Append(Environment.NewLine);
            }

            matchResultText = result.ToString();

            // roundText.text = "";
            // matchFinishText.color = Color.blue;
            // matchFinishText.alpha = 1;
            // matchFinishText.text = result.ToString();
        }
    }
}