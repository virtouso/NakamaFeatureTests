using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

namespace Script
{
    public class LeaderboardStuff : MonoBehaviour
    {
        private const string scheme = "http";
        private const string host = "127.0.0.1";
        private const int port = 7350;
        private const string serverKey = "defaultkey";

        private IClient client;
        private ISocket socket;
        private ISession session;

        private string emailText = "";
        private string groupNameText = "";
        private string userIdText = "";


        private async Task Authenticate()
        {
            client = new Client(scheme, host, port, serverKey);
            socket = Nakama.Socket.From(client);

            // Authenticate with device ID
            session = await client.AuthenticateEmailAsync("chan@yahoo.com", "123456789", emailText.Split('@')[0]);

            Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");
            //  await client.LinkDeviceAsync(session, SystemInfo.deviceUniqueIdentifier);

            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");
        }


        private async Task GetTiersList()
        {
            try
            {
                //    var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "leaderboard_tiers_list");
                Debug.Log(result.Payload);
            }
            catch (ApiResponseException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private async Task GetTiersRewards()
        {
            try
            {
                //    var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "leaderboard/tiers_rewards");
                Debug.Log(result.Payload);
            }
            catch (ApiResponseException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private async Task UpdateTierConfig()
        {
            try
            {
                //    var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "admin/leaderboard/set_reward_for_tier",
                    JsonConvert.SerializeObject(new
                    {
                        tier_id = "marble_1",
                        wallet_resources = new Dictionary<string, long>() { { "stone", 10 } },
                        assets = new Dictionary<string, List<int>>() { { "ground", new List<int>(){1} } },
                    }));
                Debug.Log(result.Payload);
            }
            catch (ApiResponseException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private async void Start()
        {
            await Authenticate();
            await GetTiersList();
            await UpdateTierConfig();
            await GetTiersRewards();
        }
    }
}