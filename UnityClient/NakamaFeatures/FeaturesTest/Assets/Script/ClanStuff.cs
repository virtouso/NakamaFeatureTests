using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Nakama;
using UnityEngine;

public class ClanStuff : MonoBehaviour
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

    private async void Authenticate()
    {
        client = new Client(scheme, host, port, serverKey);
        socket = Nakama.Socket.From(client);

        // Authenticate with device ID
        session = await client.AuthenticateEmailAsync(emailText, "123456789", emailText.Split('@')[0]);
        Debug.LogError($"Authenticated: {session.Username}****{session.UserId}");

        await socket.ConnectAsync(session);
        Debug.LogError("Socket connected.");
    }


    private void OnGUI()
    {
        emailText = GUILayout.TextField(emailText, 100, GUILayout.Width(200));
        groupNameText = GUILayout.TextField(groupNameText, 100, GUILayout.Width(200));
        userIdText = GUILayout.TextField(userIdText, 100, GUILayout.Width(200));
        if (GUILayout.Button("Auth", GUILayout.Width(100)))
        {
            Authenticate();
        }

        if (GUILayout.Button("Create", GUILayout.Width(100)))
        {
            CreateGroup();
        }

        if (GUILayout.Button("Join", GUILayout.Width(100)))
        {
            RequestJoinGroup();
        }

        if (GUILayout.Button("Promote", GUILayout.Width(100)))
        {
            PromoteUser();
        }

        if (GUILayout.Button("Demote", GUILayout.Width(100)))
        {
            DemoteUser();
        }

        if (GUILayout.Button("Accept", GUILayout.Width(100)))
        {
            AddUserToGroup();
        }

        if (GUILayout.Button("List", GUILayout.Width(100)))
        {
            ListUsers();
        }
    }


    private async void CreateGroup()
    {
        var res = await client.CreateGroupAsync(session, groupNameText, "good bond", null, null, false);
        Debug.LogError($"created:{res.Id}");
    }

    private async void PromoteUser()
    {
        await client.PromoteGroupUsersAsync(session, groupNameText, new[] { userIdText });
    }

    private async void DemoteUser()
    {
        await client.DemoteGroupUsersAsync(session, groupNameText, new[] { userIdText });
    }

    private async void RequestJoinGroup()
    {
        // try
        // {
        //     await client.JoinGroupAsync(session, groupNameText);
        //     Debug.Log($"Successfully joined group with ID: {groupNameText}");
        // }
        // catch (ApiResponseException e)
        // {
        //     Debug.LogError($"Failed to join group: {e.Message}");
        // }

        using (HttpClient client = new HttpClient())
        {
            // Set the base address (Nakama server)
            client.BaseAddress = new Uri("http://127.0.0.1:7350");

            // Set the Authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",session.AuthToken);

            // Set Accept header
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Make the POST request
            HttpResponseMessage response = await client.PostAsync($"/v2/group/{groupNameText}/join", null);

            // Read the response
            string responseContent = await response.Content.ReadAsStringAsync();

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                Debug.LogError("Successfully joined the group!");
                Debug.LogError("Response: " + responseContent);
            }
            else
            {
                Debug.LogError($"Failed to join the group. Status Code: {response.StatusCode}");
                Debug.LogError("Error: " + responseContent);
            }
        }


    }

    private async void AddUserToGroup()
    {
        try
        {
            var groupUsers = await client.ListGroupUsersAsync(session, groupNameText,null,100);

            // Step 2: Filter users with a 'JOIN_REQUEST' state
            List<string> pendingUserIds = new List<string>();
            foreach (var user in groupUsers.GroupUsers)
            {
                if (user.State == 3) // State 3 represents 'JOIN_REQUEST'
                {
                    pendingUserIds.Add(user.User.Id);
                }
            }

            // Step 3: Approve the join requests by adding the users to the group
            if (pendingUserIds.Count > 0)
            {
                await client.AddGroupUsersAsync(session, groupNameText, pendingUserIds);
                Debug.Log($"Approved {pendingUserIds.Count} join requests.");
            }
            else
            {
                Debug.Log("No pending join requests to approve.");
            }
        }
        catch (ApiResponseException e)
        {
            Debug.LogError($"Failed to accept join requests: {e.Message}");
        }
    }

    private async void ListUsers()
    {
        var result = await client.ListGroupUsersAsync(session, groupNameText,null,100);

        foreach (var item in result.GroupUsers)
        {
            Debug.LogError($"{item.User.Username}....{item.User.Online}...{item.State}");
        }
    }
}