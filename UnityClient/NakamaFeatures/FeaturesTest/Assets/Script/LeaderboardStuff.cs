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

        private async Task GetMyTier()
        {
            try
            {
                //    var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "leaderboard/get_my_tier");
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
                    JsonConvert.SerializeObject(new LeaderboardTierRewardConfig
                    {
             TierId = "marble_1",
                        RankRangeRewards= new List<LeaderboardTierRankRewardConfig>()
                        {
                            {new LeaderboardTierRankRewardConfig
                            {
                                Assets = new Dictionary<string, List<int>>(){ 
                                    ["ground"]= new List<int>{1,2 ,6},
                                    ["card_back"]=new List<int>{1,2 ,6}
                                },
                                RankRangeEnd = 0,
                                RankRangeStart = 0,
                                WalletResources = new Dictionary<string, long>
                                {
                                    ["stone"]=100
                                }
                            }}
                        }
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
            await GetMyTier();
        }
    }
}


// models 
public class LeaderboardTierRewardConfig
{
    [JsonProperty("tier_id")]
    public string TierId { get; set; }

    [JsonProperty("rank_range_rewards")]
    public List<LeaderboardTierRankRewardConfig> RankRangeRewards { get; set; }
}

public class LeaderboardTierRankRewardConfig
{
    [JsonProperty("rank_range_start")]
    public int RankRangeStart { get; set; }

    [JsonProperty("rank_range_end")]
    public int RankRangeEnd { get; set; }

    [JsonProperty("wallet_resources")]
    public Dictionary<string, long> WalletResources { get; set; }

    [JsonProperty("assets")]
    public Dictionary<string, List<int>> Assets { get; set; }
}

