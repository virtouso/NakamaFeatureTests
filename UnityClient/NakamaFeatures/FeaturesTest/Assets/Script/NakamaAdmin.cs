using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Script
{
    public class NakamaAdmin : MonoBehaviour
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
            session = await client.AuthenticateEmailAsync("changdgfgfsdsiz@yahoo.com", "123456789", emailText.Split('@')[0]);

            Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");
          //  await client.LinkDeviceAsync(session, SystemInfo.deviceUniqueIdentifier);

            await socket.ConnectAsync(session);
            Debug.LogError("Socket connected.");
        }


        private async Task GivePlayerPack()
        {
            var obj = new { player_id = session.UserId, common = 10, legendary = 9 };
            var result = await client.RpcAsync(session, "admin/card_pack/give_player_pack", JsonConvert.SerializeObject(obj));
            Debug.Log(result.Payload);
        }

        private async Task RemovePack()
        {
            var obj = new { pack_id = "" };
            var result = await client.RpcAsync(session, "admin/card_pack/remove_pack", JsonConvert.SerializeObject(obj));
            Debug.Log(result.Payload);
        }

        private async Task AddPack()
        {
            var obj = new { pack_id = "1011", rarities = new List<string> { "common", "legendary", "common", "epic" }, chance = 10, rarity_type = "common" };
            var result = await client.RpcAsync(session, "admin/card_pack/add_pack", JsonConvert.SerializeObject(obj));
            Debug.Log(result.Payload);
        }

        private async Task ListPacks()
        {
            var result = await client.RpcAsync(session, "admin/card_pack/list_packs");
            Debug.Log(result.Payload);
        }


        private async Task RemoveUser()
        {
            var result = await client.RpcAsync(session, "admin/user/remove_user",
                JsonConvert.SerializeObject( new {user_id = "ac364758-2bf0-4d03-b93a-e206a599570e"}));
            Debug.Log(result.Payload);
        }

        private async void Start()
        {
            await Authenticate();
            await RemoveUser();
            //   await AddPack();
            //   await RemovePack();
            // await GivePlayerPack();
            //await ListPacks();
        }
    }
}