using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script.NonAuthoritative
{
    public class Manager: MonoBehaviour
    {
        [SerializeField] private Button clientButton;
        [SerializeField] private Button serverButton;

        [SerializeField] private GameObject clientUi;
        [SerializeField] private GameObject serverUi;


        private void StartClient()
        {
            clientUi.gameObject.SetActive(true);
        }


        private void StartServer()
        {
            serverUi.gameObject.SetActive(true);
        }
        private void Awake()
        {
            clientButton.onClick.AddListener(StartClient);
            serverButton.onClick.AddListener(StartServer);
        }
    }
}