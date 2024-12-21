using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using Script.dto;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

namespace Script
{
    public class NakamaCustom : MonoBehaviour
    {
        private const string scheme = "http";
        private const string host = "127.0.0.1";
        private const int port = 7350;
        private const string serverKey = "defaultkey";

        private IClient client;
        private ISocket socket;
        private ISession session;


        private async Task<string> AuthMany()
        {
            client = new Client(scheme, host, port, serverKey);
            socket = Nakama.Socket.From(client);

            for (int i = 0; i < 175; i++)
            {
                session = await client.AuthenticateEmailAsync($"u{i}@yahoo.com", "123456789", $"u{i}");
             //   if (Random.Range(1, 5) > 3)
                {
                    var res = await client.RpcAsync(session, "match_result_test", "{}");
                    Debug.LogError(res.Payload);
                }
            }

            return "dddd";
        }

        private List<string> leaderboards = new()
        {
            "marble_3", "marble_2", "marble_1",
            "amber_3", "amber_2", "amber_1",
            "turquoise_3", "turquoise_2", "turquoise_1",
            "ruby_3", "ruby_2", "ruby_1",
            "emerald_3", "emerald_2", "emerald_1",
            "diamond_3", "diamond_2", "diamond_1",
        };


        private async Task<string> GetLeaderboardConfig()
        {
            var dd = await client.RpcAsync(session, "leaderboard_tiers_list");
            Debug.Log(dd.Payload);
            return "ddd";
        }
        private async Task<string> ReadLeaderboards()
        {
            foreach (var leaderboard in leaderboards)
            {
                var data = await client.ListLeaderboardRecordsAsync(session, leaderboard, limit: 200);
                Debug.Log("Leaderboard:"+leaderboard+":"+ JsonConvert.SerializeObject( data.Records));
            }
            return "dddd";
        }

        private async Task<string> ReadLeaderboardsAroundUser()
        {
            var dd = await client.RpcAsync(session, "leaderboard_past_period_time");
            Debug.Log(dd.Payload);
            var des = JsonConvert.DeserializeObject<KeyValue>(dd.Payload);

            foreach (var leaderboard in leaderboards)
            {
                var data = await client.ListLeaderboardRecordsAroundOwnerAsync(session, 
                    leaderboard, session.UserId, limit: 100);
                Debug.Log("Leaderboard Around User:"+leaderboard+":"+ JsonConvert.SerializeObject( data.Records));
            }

            return "ddd";
        }

        private async Task<string> ReadPastWeekLeaderboard()
        {
            var dd = await client.RpcAsync(session, "leaderboard_past_period_time");
            Debug.Log(dd.Payload);
            var des = JsonConvert.DeserializeObject<KeyValue>(dd.Payload);

            foreach (var leaderboard in leaderboards)
            {
                var data = await client.ListLeaderboardRecordsAsync(session, 
                    leaderboard,  limit: 200, expiry: long.Parse(des.Data));
                Debug.Log("Past Leaderboard"+leaderboard+":"+ JsonConvert.SerializeObject( data.Records));
            }

            return "ddd";
        }


        private async void Start()
        {
         await   AuthMany();
         await ReadLeaderboards();
        await    GetLeaderboardConfig();
         await   ReadLeaderboardsAroundUser();
        await    ReadPastWeekLeaderboard();
        }

        private void Update()
        {
        }
    }
}