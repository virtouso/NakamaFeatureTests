using System;
using System.Collections;
using System.Collections.Generic;
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

    private async void Authenticate()
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


    private void Start()
    {
        Authenticate();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        UpdateServerConfigs();
    }
}