using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace Script.Gameplay
{
    public class SimpleGameplayUi : MonoBehaviour
    {
        [SerializeField] private SimpleGamePlay simpleGamePlay;
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI matchFinishText;

        private void Awake()
        {
            simpleGamePlay.OnGameFinished += GameFinished;
            simpleGamePlay.OnRoundStarted += OnRoundStart;
        }


        private void Start()
        {
            roundText.text = "";
            matchFinishText.text = "";
        }

        private async void OnRoundStart(int obj)
        {
            // matchFinishText.text = "";
            roundText.color = Color.blue;
            roundText.alpha = 1;
            roundText.text = $"New Round:{obj}";
            await Task.Delay(1000);
            roundText.text = "";
        }

        private void GameFinished(string obj)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(obj);
            StringBuilder result = new StringBuilder();

            foreach (var item in data)
            {
                result.Append($"{item.Key}: {item.Value}").Append(Environment.NewLine);
            }

            // roundText.text = "";
            matchFinishText.color = Color.blue;
            matchFinishText.alpha = 1;
            matchFinishText.text = result.ToString();
        }
    }
}