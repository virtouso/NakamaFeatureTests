using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using Script.dto;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


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

        [SerializeField] private bool sendScores;

        private async Task<string> AuthMany()
        {
            client = new Client(scheme, host, port, serverKey);

            for (int i = 0; i < 400; i++)
            {
                session = await client.AuthenticateEmailAsync($"u{i}@yahoo.com", "123456789", $"u{i}");
                //     var dd = await client.RpcAsync(session, "player_current_leaderboard_tier");
                //    Debug.Log(dd.Payload);
                //    var ddd = await client.RpcAsync(session, "leaderboard_past_period_time");
                //    Debug.Log(ddd.Payload);

                if (!sendScores) continue;
                //  if (Random.Range(1, 5) > 1)
                //  {
                var res = await client.RpcAsync(session, "test_leaderboard_score",
                    JsonConvert.SerializeObject(new KeyValueInteger { Data = i }));
                Debug.Log(res.Payload);
                //  }
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
                var data = await client.ListLeaderboardRecordsAsync(session, leaderboard, limit: 1000);
                Debug.Log("Leaderboard :" + leaderboard + ":" + data.Records.Count());

                StringBuilder leaderData = new StringBuilder();
                foreach (var item in data.Records)
                {
                    leaderData.Append(
                        $"leaderboard:{leaderboard}:{item.Rank}|{item.Username}|{item.Score}{Environment.NewLine}");
                }

                Debug.Log(leaderData.ToString());
            }

            return "dddd";
        }

        private async Task<string> ReadLeaderboardsAroundUser()
        {
            foreach (var leaderboard in leaderboards)
            {
                var data = await client.ListLeaderboardRecordsAroundOwnerAsync(session,
                    leaderboard, session.UserId, limit: 100);
                Debug.Log("Leaderboard Around User:" + leaderboard + ":" + data.Records.Count());

                StringBuilder leaderData = new StringBuilder();
                foreach (var item in data.Records)
                {
                    leaderData.Append(
                        $"around leaderboard:{leaderboard}:{item.Rank}|{item.Username}|{item.Score}{Environment.NewLine}");
                }

                Debug.Log(leaderData.ToString());
            }

            return "ddd";
        }

        private async Task<string> ReadPastWeekLeaderboard()
        {
            if (session == null)
            {
                client = new Client(scheme, host, port, serverKey);
                session = await client.AuthenticateEmailAsync($"u399@yahoo.com", "123456789", $"u399");
            }

            foreach (var item in leaderboards)
            {
                for (int i = 1; i <= 6; i++)
                {
                    var ser = JsonConvert.SerializeObject(new LeaderboardRequest
                    {
                        LeaderboardId = item,
                        PastCount = i,
                    });
                    Debug.Log(ser);

                    var dd = await client.RpcAsync(session, "leaderboard_past_period_players",
                        ser);
                    var des = JsonConvert.DeserializeObject<LeaderboardResponse>(dd.Payload);
                    Debug.Log($"past|name:{item}|number:{i}|players count:{des.Records.Count}|prev:{des.PrevCursor}|next:{des.NextCursor}");

                    StringBuilder builder = new StringBuilder();

                    foreach (var sub in des.Records)
                    {
                        builder.Append($"{sub.rank}|{sub.username.value}|{sub.score}|").Append(Environment.NewLine);
                    }

                    Debug.Log(builder.ToString());
                }
            }


            return "ddd";
        }

        void CopyToClipboard(string str)
        {
            TextEditor textEditor = new TextEditor();
            textEditor.text = str;
            textEditor.SelectAll();
            textEditor.Copy();
        }


        private async void Start()
        {
            // var ser =   JsonConvert.SerializeObject( new LeaderboardRequest
            // {
            //     LeaderboardId = "ddd",
            //     PastCount = 1,
            // });
            // Debug.Log(ser);
            await AuthMany();
            await ReadLeaderboards();
            await GetLeaderboardConfig();
            await ReadLeaderboardsAroundUser();
            await ReadPastWeekLeaderboard();
        }
    }
}