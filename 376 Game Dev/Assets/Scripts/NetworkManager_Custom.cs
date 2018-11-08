using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using System;

public class NetworkManager_Custom : NetworkManager {

    public void startupHost()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            setPort();
            NetworkManager.singleton.StartHost();
        }
    }

    public void joinGame()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            setIPAddress();
            setPort();
            NetworkManager.singleton.StartClient();
        }
    }

    void setIPAddress()
    {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        Debug.Log(ipAddress);
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void setPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            StartCoroutine(setupMenuSceneButton());
        }
        else
        {
            setupOtherSceneButton();
        }
    }

    IEnumerator setupMenuSceneButton()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject.Find("StartHostButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("StartHostButton").GetComponent<Button>().onClick.AddListener(startupHost);

        GameObject.Find("JoinGameButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("JoinGameButton").GetComponent<Button>().onClick.AddListener(joinGame);

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
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);
    }
}
