using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class NetworkManager_Custom : NetworkManager {

    private float nextRefreshTime;
    private bool lookForMatch = true;
    private MatchListPanel gameList;

    void Start()
    {
        gameList = GameObject.Find("GameList").GetComponent<MatchListPanel>();
    }

    public void StartHosting()
    {
        StartMatchMaker();
        matchMaker.CreateMatch(GameObject.Find("RoomName").transform.Find("Text").GetComponent<Text>().text, 4, true, "", "", "", 0, 0, OnMatchCreated);
    }

    private void OnMatchCreated(bool success, string extendedinfo, MatchInfo responsedata)
    {
        base.StartHost(responsedata);
        RefreshMatches();
    }

    private void Update()
    {
        if (lookForMatch && Time.time >= nextRefreshTime)
        {
            RefreshMatches();
        }
    }

    private void RefreshMatches()
    {
        nextRefreshTime = Time.time + 5f;

        if (matchMaker == null)
            StartMatchMaker();

        matchMaker.ListMatches(0, 10, "", true, 0, 0, HandleListMatchesComplete);
    }

    private void HandleListMatchesComplete(bool success, string extendedinfo, List<MatchInfoSnapshot> responsedata)
    {
        if (lookForMatch)
        {
            gameList.updateList(responsedata);
        }
    }

    public void JoinMatch(MatchInfoSnapshot match)
    {
        if (matchMaker == null)
            StartMatchMaker();

        matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, HandleJoinedMatch);
    }

    private void HandleJoinedMatch(bool success, string extendedinfo, MatchInfo responsedata)
    {
        StartClient(responsedata);
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            StartCoroutine(setupMenuSceneButton());
            lookForMatch = true;
        }
        else
        {
            lookForMatch = false;
            setupOtherSceneButton();
        }
    }

    IEnumerator setupMenuSceneButton()
    {
        gameList = GameObject.Find("GameList").GetComponent<MatchListPanel>();
        yield return new WaitForSeconds(0.3f);
        GameObject.Find("StartHostButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("StartHostButton").GetComponent<Button>().onClick.AddListener(StartHosting);

        GameObject[] g = Resources.FindObjectsOfTypeAll<GameObject>();

        for (int i = 0; i < g.Length; i++)
        {
            if (g[i].tag == "GameController")
            {
                Destroy(g[i]);
                break;
            }
        }
    }

    void setupOtherSceneButton()
    {
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.AddListener(base.StopHost);
    }
}
