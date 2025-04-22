using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

namespace Script
{
    public class NakamaPlayerResource : MonoBehaviour
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


        private async Task GiveCardBack()
        {
            try
            {
                var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "user_resource/give_card_back", JsonConvert.SerializeObject(obj));
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

        private async Task GiveEmojiPack()
        {
            try
            {
                var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "user_resource/give_emoji_pack", JsonConvert.SerializeObject(obj));
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

        private async Task GiveGround()
        {
            try
            {
                var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "user_resource/give_ground", JsonConvert.SerializeObject(obj));
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

        private async Task GetResources()
        {
            try
            {
                //   var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "user_resource/get_resources");
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

        private async Task UpdateActiveGround()
        {
            try
            {
                var obj = new { id = 2 };
                var result = await client.RpcAsync(session, "user_resource/update_active_ground", JsonConvert.SerializeObject(obj));
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

        private async Task UpdateActiveCardBack()
        {
            try
            {
                var obj = new { id = 1 };
                var result = await client.RpcAsync(session, "user_resource/update_active_card_back",JsonConvert.SerializeObject(obj));
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

        private async Task UpdateActiveEmojis()
        {
            try
            {
                var obj = new { ids = new List<int>{1001,1201} };
                var result = await client.RpcAsync(session, "user_resource/update_active_emojis",JsonConvert.SerializeObject(obj));
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
            await GetResources();
            await GiveCardBack();
            await GiveEmojiPack();
            await GiveGround();
            await UpdateActiveGround();
            await UpdateActiveCardBack();
            await UpdateActiveEmojis();
        }
    }
}