using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

namespace Script.MetaClientExample
{
    public class WebClientReference : ScriptableObject
    {
        [SerializeField] private List<RequestData> requestsList;

        private Dictionary<RequestNames, string> _requests= new ();


        private void OnEnable()
        {
            requestsList.ForEach(x=>_requests.Add(x.RequestName,x.Url));
        }


        public async Task< MetaResponse<GetHeroDeckResponse>> GetHeroDeck(Session session, Client client, GetHeroDeckRequest request)
        {
            var url = _requests[RequestNames.GetHeroDeck];
            var result= await SendRequest<GetHeroDeckResponse, GetHeroDeckRequest>(session, client, url, request);
            return result;
        }
        
        
        
        private async Task<MetaResponse<T>> SendRequest<T, S>(Session session, Client client,  string method, S request)
        {
            MetaResponse<T> result = new MetaResponse<T>();
            var deserializedResponse = new ServerMetaResponse<T>();
            string rawPayload= "";
            try
            {
                var jsonInput = JsonConvert.SerializeObject(request);
                var response = await client.RpcAsync(session, method, jsonInput);
                rawPayload = response.Payload;
                deserializedResponse = JsonConvert.DeserializeObject<ServerMetaResponse<T>>(response.Payload);
                result = new MetaResponse<T>
                {
                    Payload = deserializedResponse.Payload,
                    DeveloperMessage = deserializedResponse.DeveloperMessage,
                    MessageCode = deserializedResponse.MessageCode,
                    GrpcCode = 0,
                    HttpCode = 200,
                    SecondaryRawPayload = rawPayload
                };
            }
            catch (ApiResponseException e)
            {
                result = new MetaResponse<T>
                {
                    Payload = default,
                    DeveloperMessage = deserializedResponse.DeveloperMessage,
                    MessageCode = deserializedResponse.MessageCode,
                    GrpcCode = e.GrpcStatusCode,
                    HttpCode = e.StatusCode,
                    SecondaryRawPayload = rawPayload
                };
            }
            catch (Exception e)
            {
                result = new MetaResponse<T>
                {
                    Payload = default,
                    DeveloperMessage = e.Message,
                    MessageCode = "unknown",
                    GrpcCode =2,
                    HttpCode = 500,
                    SecondaryRawPayload = ""
                };
                
                
            }

            return result;
        }




  
        

        [Serializable]
        public class RequestData
        {
            [field:SerializeField] public string Url{get;private set;}
            [field:SerializeField] public RequestNames RequestName { get; private set; } 
        }

        public enum RequestNames
        {
            GetHeroDeck
        }

        public class GetHeroDeckRequest
        {
            
        }

        public class GetHeroDeckResponse
        {
            
        }
        public class ServerMetaResponse<T>
        {
            [JsonProperty("payload")] public T Payload { get; set; }

            [JsonProperty("message_code")] public string MessageCode { get; set; }

            //developer_message does not exist in production response. just for detailed error data and log
            [JsonProperty("developer_message")] public string DeveloperMessage { get; set; }
        }

        public class MetaResponse
        {
            [JsonProperty("message_code")] public string MessageCode { get; set; }
            [JsonProperty("developer_message")] public string DeveloperMessage { get; set; }
            [JsonProperty("http_code")] public long HttpCode { get; set; }
            [JsonProperty("grpc_code")] public int GrpcCode { get;  set; }
            public bool Success => HttpCode == 200 || GrpcCode == 0;
        }
        
        public class MetaResponse<T>:  MetaResponse
        {
            [JsonProperty("payload")] public T Payload { get; set; }

            [JsonProperty("secondary_raw_payload")] public string SecondaryRawPayload { get; set; }
         
        }

        
 





        void Start()
        {
            Dictionary<string, Action> handlers = new Dictionary<string, Action>();
            
            handlers.Add("not_enough_resource", HandleNoresourceError);
            
        }


        void HandleNoresourceError()
        {
            
        }
        
        
        
        
        
        
    }
}