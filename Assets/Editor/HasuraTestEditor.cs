using System;
using System.Collections;
using System.Collections.Generic;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public enum Options {desc, asc};

[CustomEditor(typeof(HasuraTest))]
public class HasuraTestEditor : Editor
{
    private HasuraTest hasuraTest;
    private string _playerName;
    private int _score;

    private void OnEnable()
    {
        hasuraTest = (HasuraTest) target;
        OnSubscriptionDataReceived.RegisterListener(DisplayData);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Get Players"))
        {
            GetPlayers();
        }

        if (GUILayout.Button("Get Live Players"))
        {
            GetLivePlayers();
        }

        _playerName = EditorGUILayout.TextField("Player Name", _playerName);

        if (GUILayout.Button("Add Player"))
        {
            AddPlayer(_playerName);
        }

        _score = EditorGUILayout.IntField("Score", _score);

        if (GUILayout.Button("Update Score"))
        {
            UpdateScore(PlayerPrefs.GetString("localPlayerId"), _score);
        }

        if (GUILayout.Button("Display Top Live Players"))
        {
          DisplayTopLivePlayers();
        }
        
        if (GUILayout.Button("Display Top Players"))
        {
          DisplayTopPlayers();
        }
        
        EditorGUILayout.Space();
    }


    private void OnDisable(){
        OnSubscriptionDataReceived.UnregisterListener(DisplayData);
    }

    public void DisplayData(OnSubscriptionDataReceived subscriptionDataReceived){
        Debug.Log(subscriptionDataReceived.data);

        string responseText = subscriptionDataReceived.data;
        JObject j = JObject.Parse(responseText);
        JArray players = (JArray)(j["payload"]["data"]["players"]);
        DisplayPlayers(players);
    }

    public void DisplayPlayers(JArray players) {
        List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

        string localId = PlayerPrefs.GetString("localPlayerId");

        for (int i = 0; i < players.Count; i++)
        {
          JObject entry = (JObject) players[i];
          ScoreEntry scoreEntry = new ScoreEntry();
          scoreEntry.userName = (string) entry["name"];
          scoreEntry.score = (int) entry["score"];
          scoreEntry.isLocalPlayer = (string) entry["id"] == localId;
          scoreEntry.rank = i + 1;

          scoreEntries.Add(scoreEntry);
        }

        FindObjectOfType<LeaderBoardOverlay>().DisplayScores(scoreEntries);
    }

    public void GetLivePlayers()
    {
        hasuraTest.leaderboardApi.Subscribe("livePlayers", GraphApi.Query.Type.Subscription);
    }

    public async void GetPlayers()
    {
        UnityWebRequest request = await hasuraTest.leaderboardApi.Post("getPlayers", GraphApi.Query.Type.Query);
    }

    public async void AddPlayer(string playerName)
    {
        GraphApi.Query addPlayerMutation =
            hasuraTest.leaderboardApi.GetQueryByName("addPlayer", GraphApi.Query.Type.Mutation);
        addPlayerMutation.SetArgs(new {objects = new {name = playerName}});
        UnityWebRequest request = await hasuraTest.leaderboardApi.Post(addPlayerMutation);
        string responseText = request.downloadHandler.text;
        JObject j = JObject.Parse(responseText);
        JArray returnValues = (JArray)(j["data"]["insert_players"]["returning"]);
        string newPlayerId = (string) (returnValues[0])["id"];
        PlayerPrefs.SetString("localPlayerId", newPlayerId);
        PlayerPrefs.Save();
    }

    public async void UpdateScore(string playerId, int score)
    {
        GraphApi.Query updateScoreMutation =
            hasuraTest.leaderboardApi.GetQueryByName("UpdateScoreAction", GraphApi.Query.Type.Mutation);
        updateScoreMutation.SetArgs(new {playerId = playerId, score = score});
        UnityWebRequest request = await hasuraTest.leaderboardApi.Post(updateScoreMutation);
    }

    public void DisplayTopLivePlayers()
    {
        GraphApi.Query topLivePlayersQuery =
            hasuraTest.leaderboardApi.GetQueryByName("topLivePlayers", GraphApi.Query.Type.Subscription);
        topLivePlayersQuery.SetArgs(new {limit = 10, order_by = new {score = Options.desc}, where = new {score = new {_is_null = false}}});
        hasuraTest.leaderboardApi.Subscribe(topLivePlayersQuery);
    }

    public async void DisplayTopPlayers()
    {
        GraphApi.Query topPlayersQuery =
            hasuraTest.leaderboardApi.GetQueryByName("topPlayers", GraphApi.Query.Type.Query);
        topPlayersQuery.SetArgs(new {limit = 10, order_by = new {score = Options.desc}, where = new {score = new {_is_null = false}}});
        UnityWebRequest request = await hasuraTest.leaderboardApi.Post(topPlayersQuery);
        string responseText = request.downloadHandler.text;
        JObject j = JObject.Parse(responseText);
        JArray players = (JArray)(j["data"]["players"]);
        DisplayPlayers(players);
    }
}
