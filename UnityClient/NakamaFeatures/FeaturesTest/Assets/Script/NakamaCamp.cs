using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using Script.config;
using Script.dto;
using UnityEngine;
using CardConfig = Script.dto.CardConfig;

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
        session = await client.AuthenticateEmailAsync("changiz@yahoo.com", "123456789", emailText.Split('@')[0]);
        Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");

        await socket.ConnectAsync(session);
        Debug.LogError("Socket connected.");
    }


    private async void UpdateServerConfigs()
    {
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
                    CardId = item.Id
                }
            };
            var data = JsonConvert.SerializeObject(req);
            var res = await client.RpcAsync(session, "send_card_config", data);
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
        Debug.LogError(result.Payload);
    }

    private async Task<Camp> GetPlayerCampAllData()
    {
        var result = await client.RpcAsync(session, "camp/get_player_camp_all_data");
        Debug.LogError(result.Payload);
        var des = JsonConvert.DeserializeObject<Camp>(result.Payload);
        return des;
    }


    private async Task GetPlayerDeck()
    {
        var result = await client.RpcAsync(session, "camp/get_player_deck", JsonConvert.SerializeObject(new GetPlayerDeckRequest
        {
            DeckIndex = 0,
            HeroID = "hunterdsds"
        }));
        Debug.LogError(result.Payload);
        Debug.LogError(result.HttpKey);
        Debug.LogError(result.Id);
    }


    private async Task AddOrUpdatePlayerData(Camp camp)
    {
        var req = new Dictionary<string, CardRequest>();

        var dddd = camp.AllCards.Take(2).ToList();
        dddd.Add(camp.AllCards.First(x=>x.Value.CardId=="c6"));
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
    }

    private async Task BuyCard()
    {
        var result = await client.RpcAsync(session, "camp/buy_card", JsonConvert.SerializeObject(new BuyCardRequest { CardId = "c6" }));
        Debug.LogError(result.Payload);
    }

    private async Task SellCard()
    {
        var result = await client.RpcAsync(session, "camp/sell_card", JsonConvert.SerializeObject(new SellCardRequest() { CardGuid = "ca8e4258-ba9e-4c41-9f2b-61aca7130203" }));
        Debug.LogError(result.Payload);
    }

    private async Task Start()
    {
        await Authenticate();
       
        await SellCard();
        var camp = await GetPlayerCampAllData();
        //  await AddOrUpdatePlayerData(camp);

        //   await SellCard();

        //  await BuyCard();
        //  await GetPlayerCampAllData();
        //  var account= await client.GetAccountAsync(session);
        //  Debug.LogError(account.Wallet);

        //   UpdateServerConfigs();
        //   await GetPlayerCards();
        //   var camp = await GetPlayerCampAllData();
        //   await AddOrUpdatePlayerData(camp);
        //     await GetPlayerDeck();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        UpdateServerConfigs();
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

public partial class Camp
{
    [JsonProperty("AllCards")] public Dictionary<string, Card> AllCards { get; set; }

    [JsonProperty("heroesDecks")] public HeroesDecks HeroesDecks { get; set; }
}

public partial class Card
{
    [JsonProperty("guid")] public string Guid { get; set; }

    [JsonProperty("card_id")] public string CardId { get; set; }
}

public partial class HeroesDecks
{
    [JsonProperty("hunter")] public Hunter[] Hunter { get; set; }
}

public partial class Hunter
{
    [JsonProperty("cards")] public Dictionary<string, Card> Cards { get; set; }
}


public class BuyCardRequest
{
    [JsonProperty("card_id")] public string CardId { get; set; }
}

public class SellCardRequest
{
    [JsonProperty("card_guid")] public string CardGuid { get; set; }
}