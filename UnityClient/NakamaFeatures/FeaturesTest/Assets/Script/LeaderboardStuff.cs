using System;
using Nakama;
using UnityEngine;

namespace Script
{
    public class LeaderboardStuff: MonoBehaviour
    {
        private const string ServerKey = "defaultkey";
        private const string Host = "127.0.0.1";
        private const int Port = 7350;
        private const string Scheme = "http";

        private IClient _client;
        private ISession _session;
        private IApiAccount _account;


        private void SendScoreRequest()
        {
           // _client.RpcAsync();
        }
        
        
        
    }
}