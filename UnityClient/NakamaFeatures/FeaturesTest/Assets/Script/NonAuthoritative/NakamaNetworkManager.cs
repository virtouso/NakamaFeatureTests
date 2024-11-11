using System;
using System.Collections.Generic;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;

namespace Script.NonAuthoritative
{
    public class NakamaNetworkManager
    {
        private IClient _client;
        private ISocket _socket;
        private ISession _session;
        public readonly List<IUserPresence> Presences = new();
        public Action<UserPresenceData> OnUserPresence;
        public Action<MessageData> OnMessage;

        public async void JoinMatch(string matchName)
        {
            _client = new Nakama.Client("http", @"127.0.0.1", 7350, "defaultkey");
            _socket = _client.NewSocket();
            var match = await _socket.JoinMatchAsync(matchName);

            InitPresence(match);
            InitMessageEvents();
        }

        private void InitMessageEvents()
        {
            var enc = System.Text.Encoding.UTF8;
            _socket.ReceivedMatchState += newState =>
            {
                var content = JsonParser.FromJson<Dictionary<string, String>>(enc.GetString(newState.State));
                OnMessage?.Invoke(new MessageData
                {
                    Messages = content, Username = newState.UserPresence.Username,
                    UserId = newState.UserPresence.UserId, OpCode = newState.OpCode
                });
            };
        }

        private void InitPresence(IMatch match)
        {
            foreach (var presence in match.Presences)
            {
                Debug.LogWarning($"user In Match:{presence.UserId}, {presence.Username}");
            }


            _socket.ReceivedMatchPresence += presenceEvent =>
            {
                foreach (var presence in presenceEvent.Leaves)
                {
                    Presences.Remove(presence);
                    OnUserPresence?.Invoke(new UserPresenceData
                        { Join = false, UserId = presence.UserId, Username = presence.Username });
                }

                Presences.AddRange(presenceEvent.Joins);

                foreach (var presence in presenceEvent.Joins)
                {
                    OnUserPresence?.Invoke(new UserPresenceData
                        { Join = false, UserId = presence.UserId, Username = presence.Username });
                }
            };
        }

        public async void SendMessageToAll(string matchName, long opCode, Dictionary<string, string> message)
        {
            await _socket.SendMatchStateAsync(matchName, opCode, message.ToJson());
        }


        public async void RequestLeave(string matchName)
        {
            await _socket.LeaveMatchAsync(matchName);
        }


        public async void CreateServer(string matchName)
        {
            _client = new Nakama.Client("http", @"127.0.0.1", 7350, "defaultkey");
            _socket = _client.NewSocket();
            var match = await _socket.CreateMatchAsync(matchName);



            InitPresence(match);
            InitMessageEvents();
        }
    }
}