using System.Collections.Generic;
using System.Linq;
using Nakama;
using Newtonsoft.Json;
using Script.config;
using Script.dto;
using Script.MetaClientExample;
using UnityEditor.VersionControl;
using UnityEngine;
using CardConfig = Script.dto.CardConfig;
using Task = System.Threading.Tasks.Task;

namespace Script
{
    public class NakamaCamp : MonoBehaviour
    {
        [SerializeField] private CardsList cardsList;


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
            session = await client.AuthenticateEmailAsync("changdsdsizz@yahoo.com", "123456789", emailText.Split('@')[0]);
       //     session = await client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);          
            
            Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");

            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");
        }

        [ContextMenu("UpdateCardsConfig")]
        private async void UpdateServerConfigs()
        {
            if (session == null)
                await
                    Authenticate();
            foreach (var item in cardsList.Cards)
            {
                var req = new SendCardConfigRequest
                {
                    Secret = "moeen1234",
                    CardId = item.Id,
                    JsonConfig = new CardConfig
                    {
                        Attack = item.Attack,
                        Health = item.Health,
                        Mana = item.Mana,
                        Marketable = item.Marketable,
                        Rarity = item.Rarity,
                        Story = item.Story,
                        BuyPrice = ToDict(item.BuyPrice),
                        DisplayName = item.DisplayName,
                        SellPrice = ToDict(item.SellPrice),
                        CardId = item.Id,
                        OpenPackChance = item.OpenPackChance,
                    }
                };
                var data = JsonConvert.SerializeObject(req);
                var res = await client.RpcAsync(session, "admin/cards/send_card_config", data);
           //     var modeled = JsonConvert.DeserializeObject<string>(res.Payload); // only result is ok!
                Debug.Log(res.Payload);
            }


            Dictionary<string, long> ToDict(List<Script.config.CardConfig.CurrencyData> input)
            {
                var result = new Dictionary<string, long>();

                foreach (var item in input)
                    result.Add(item.CurrencyId, item.Amount);

                return result;
            }
        }

        private async Task GetPlayerCards()
        {
            var result = await client.RpcAsync(session, "camp/get_player_cards");
            var modeled = JsonConvert.DeserializeObject<WebClientReference.MetaResponse<Dictionary<string, PlayerCardData>>>(result.Payload);
        }

        private async Task GetPlayerCampAllData()
        {
            var result = await client.RpcAsync(session, "camp/get_player_camp_all_data");
            Debug.LogError(result.Payload);
            var des = JsonConvert.DeserializeObject<WebClientReference.MetaResponse<Camp>>(result.Payload); // no secondary response model
        }


        public async Task GetPlayerCampConfigs()
        {
            var result = await client.RpcAsync(session, "camp/get_camp_configs");
            Debug.LogError(result.Payload);
      
            

            var des = JsonConvert.DeserializeObject<WebClientReference.MetaResponse<CampConfigResponse>>(result.Payload); // no secondary response model
            //    return des;
        }

        public class CampConfigResponse
        {
            [JsonProperty("max_permitted_deck_size")]
            public int MaxPermittedDeckSize { get; set; }

            [JsonProperty("max_permitted_same_legendary_type")]
            public int MaxPermittedSameLegendaryType { get; set; }

            [JsonProperty("max_permitted_same_non_legendary_type")]
            public int MaxPermittedSameNonLegendaryType { get; set; }
        }        
        
 
                
        private async Task GetHeroesConfig()
        {
            var dd = await client.RpcAsync(session, "general_configs/heroes_data");
            Debug.Log(dd.Payload);
            
        }
        
        private async Task GetHeroesConfigsModeled()
        {
            var dd = await client.RpcAsync(session, "general_configs/heroes_data_modeled");
            Debug.Log(dd.Payload);
            
        }

        private async Task RemoveCardWithId()
        {
            var dd = await client.RpcAsync(session, "admin/cards/delete_card_with_id","c2");
            Debug.Log(dd.Payload);
            
        }
        private async Task RemoveAllCards()
        {
            var dd = await client.RpcAsync(session, "admin/cards/delete_all_cards");
            Debug.Log(dd.Payload);
            
        }
        
        
        private async Task GetPlayerAllHeroes()
        {
            var result = await client.RpcAsync(session, "camp/get_player_heroes");
            Debug.LogError(result.Payload);

            //   var des = JsonConvert.DeserializeObject<Camp>(result.Payload);
            var modeled = JsonConvert.DeserializeObject<WebClientReference.MetaResponse<List<string>>>(result.Payload); // no secondary response
        }

   
        private async Task GetPlayerDeck()
        {
            var result = await client.RpcAsync(session, "camp/get_player_deck", JsonConvert.SerializeObject(new GetPlayerDeckRequest
            {
                DeckIndex = 0,
                HeroID = "hunter"
            }));
            var data = JsonConvert.DeserializeObject<WebClientReference.MetaResponse< PlayerHeroDeck>>(result.Payload);
        }


        private async Task AddOrUpdatePlayerData(Camp camp)
        {
            var req = new Dictionary<string, CardRequest>();

            var dddd = camp.AllCards.Take(2).ToList();
            dddd.Add(camp.AllCards.First(x => x.Value.CardId == "c6"));
            foreach (var item in dddd)
            {
                req.Add(item.Key, new CardRequest { CardId = item.Value.CardId, CardGuid = item.Value.Guid });
            }

            //   req.ElementAt(0).Value.CardId = "hunterdsds";

            var data = new AddOrUpdatePlayerDeckRequest
            {
                DeckIndex = 5,
                HeroID = "hunter",
                RequestDeckCards = req
            };

            var result = await client.RpcAsync(session, "camp/add_or_update_player_deck", JsonConvert.SerializeObject(data));
            Debug.LogError(result.Payload);

            // 1) message_code = bad_request no secondary response
            var res1 = JsonConvert.DeserializeObject<WebClientReference.MetaResponse<string>>(result.Payload);

            //2) message_code = failed_to_read_player_data        database error. same result as above

            // 3) message_code = hero_is_locked_or_not_exist_cant_add_deck_for_it  hero not exist on profile error same as above no secondary
            // 4) message_code = "requested_deck_card_not_exist_on_profile"  secondary is simple string the card that is not on player profile
            // 5) message_code = "invalid_data_sent"  sent a card guid but card id is not the same as on server
            // 6) message_code = "violated_rarity" 

            var res2 = JsonConvert.DeserializeObject<WebClientReference.MetaResponse<ViolatedRarityCardsResponse>>(result.Payload);

            // 7 ) message_code = "failed_to_write_player_data"   no secondary. database error
            // 8 ) message_code "ok" nothing to do. data saved
        }

        private async Task BuyCard()
        {
            var result = await client.RpcAsync(session, "camp/buy_card", JsonConvert.SerializeObject(new BuyCardRequest { CardId = "c6" }));
            Debug.LogError(result.Payload);

            // 1) "card_config_not_found"  no model. config requested not found
            // 2) "card_not_marketable" no model. you cant buy or sell this card
            // 3) "insufficient_resource" these resources are not enough  
            var result1 = JsonConvert.DeserializeObject<WebClientReference.MetaResponse<InsufficientWalletResponse>>(result.Payload);
            // 4) "player_camp_data_not_found" database error. nothing to do no secondary
            // 5)"requested_card_type_already_maxed_out" no secondary. if you already have legendary card or 2 other type you cant buy it
            //6) "failed_to_write_player_data" database error. no secondary
            //7) "ok"
        }


        public class InsufficientWalletResponse
        {
            [JsonProperty("needed_resources")] public Dictionary<string, long> NeededResources { get; set; }
        }

        class ViolatedRarityCardsResponse
        {
            [JsonProperty("violation")] public Dictionary<string, int> Violation { get; set; }
        }

        private async Task SellCard()
        {
            var result = await client.RpcAsync(session, "camp/sell_card", JsonConvert.SerializeObject(new SellCardRequest() { CardGuid = "ca8e4258-ba9e-4c41-9f2b-61aca7130203" }));
            Debug.LogError(result.Payload);
            
            var regularRes= JsonConvert.DeserializeObject<WebClientReference.MetaResponse<string>>(result.Payload);
            
           //  1) "player_does_not_own_the_card" no secondary result
           // 2) "card_config_not_found" no secondary
           // 3) "card_not_marketable" dto or secondary
           // 4)  "error_update_wallet" server database error no secondary
          // 5)  "failed_to_write_player_data" database error. no dto
          // 6) "ok" card sold and resources added to wallet no secondary 
          
    
        }

        private async void Start()
        {
            await Authenticate();
            await GetHeroesConfig();
            await GetHeroesConfigsModeled();
         //   await RemoveCardWithId();
           // await RemoveAllCards(); 

             await GetPlayerCampConfigs();
             await GetPlayerCampAllData();
            // await GetPlayerCards();
            // await GetPlayerAllHeroes();
            // await GetPlayerDeck();
            // await BuyCard();
            // await SellCard();
            //  await AddOrUpdatePlayerData();
        }
    }

    class GetPlayerDeckRequest
    {
        [JsonProperty("hero_id")] public string HeroID { get; set; }
        [JsonProperty("deck_index")] public int DeckIndex { get; set; }
    }

    class AddOrUpdatePlayerDeckRequest
    {
        [JsonProperty("hero_id")] public string HeroID { get; set; }
        [JsonProperty("deck_index")] public int DeckIndex { get; set; }
        [JsonProperty("request_deck_cards")] public Dictionary<string, CardRequest> RequestDeckCards { get; set; }
    }

    class CardRequest
    {
        [JsonProperty("card_guid")] public string CardGuid { get; set; }
        [JsonProperty("card_id")] public string CardId { get; set; }
    }


    public partial class Card
    {
        [JsonProperty("guid")] public string Guid { get; set; }

        [JsonProperty("card_id")] public string CardId { get; set; }
    }

    public partial class HeroesDecks
    {
        private Dictionary<string, Dictionary<string, Card>> Cards { get; set; }
    }


    public class BuyCardRequest
    {
        [JsonProperty("card_id")] public string CardId { get; set; }
    }

    public class SellCardRequest
    {
        [JsonProperty("card_guid")] public string CardGuid { get; set; }
    }


    public class Camp
    {
        public Dictionary<string, PlayerCardData> AllCards { get; set; } = new();

        [JsonProperty("heroesDecks")] public Dictionary<string, List<PlayerHeroDeck>> HeroesDecks { get; set; } = new();
    }

    public class PlayerCardData
    {
        [JsonProperty("guid")] public string Guid { get; set; }

        [JsonProperty("card_id")] public string CardId { get; set; }
    }

    public class PlayerHeroDeck
    {
        [JsonProperty("cards")] public Dictionary<string, PlayerCardData> Cards { get; set; }
    }
}