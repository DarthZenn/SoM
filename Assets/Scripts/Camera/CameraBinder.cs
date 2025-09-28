using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBinder : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(BindToPlayer());
    }

    private IEnumerator BindToPlayer()
    {
        yield return null; // wait one frame, let Unity kill the duplicate player

        var cam = GetComponent<CinemachineVirtualCamera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            cam.LookAt = player.transform;
        }
    }
}
