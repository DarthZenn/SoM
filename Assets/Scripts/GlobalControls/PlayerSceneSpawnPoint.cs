using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSceneSpawnPoint : MonoBehaviour
{
    public string spawnID; // must match a sceneToSceneID

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (GameManager.Instance != null && GameManager.Instance.sceneToSceneID == spawnID)
        {
            GameManager.Instance.ClearSceneData();

            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
    }
}
