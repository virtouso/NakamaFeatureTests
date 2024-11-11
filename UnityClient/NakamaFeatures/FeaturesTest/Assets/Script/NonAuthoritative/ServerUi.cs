using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;
using UnityEngine.UI;

namespace Script.NonAuthoritative
{
    public class ServerUi: MonoBehaviour
    {
        private NakamaNetworkManager _nakamaNetworkManager = new NakamaNetworkManager();

        [SerializeField] private InputField numberInput;
        [SerializeField] private InputField matchNameInput;
        
        [SerializeField] private Button connectButton;
        [SerializeField] private Button sendButton;
        

        private void Create()
        {
            _nakamaNetworkManager.CreateServer(matchNameInput.text);
        }


        private long _messageOpCode;
        private readonly Dictionary<string, string> _message = new Dictionary<string, string>();
        private void Send()
        {
            _nakamaNetworkManager.SendMessageToAll(matchNameInput.text,_messageOpCode,_message);
        }
        
        private void OnMessage(MessageData obj)
        {
            Debug.Log(obj.ToJson());
        }

        private void OnUserPresence(UserPresenceData obj)
        {
            Debug.Log(obj.ToJson());
        }
        
        

        private void Awake()
        {
            connectButton.onClick.AddListener(Create);
            sendButton.onClick.AddListener(Send);
            _nakamaNetworkManager.OnUserPresence+= OnUserPresence;
            _nakamaNetworkManager.OnMessage+= OnMessage;
        }
    }
}