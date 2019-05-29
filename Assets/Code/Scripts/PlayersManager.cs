using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public GameObject playersHolder;

    public Dictionary<int, GameObject> playersDictionary = new Dictionary<int, GameObject>();

    public GameObject SpawnNewPlayer(int connectionId) {
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, playersHolder.transform);
        playersDictionary.Add(connectionId, player);
        return player;
    }

}
