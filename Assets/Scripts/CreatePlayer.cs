using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class CreatePlayer : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera
    public Vector3 spawnPointforPlayer;
    public Quaternion startRotation;

    private void Start()
    {
        GameObject player = PlayerManager.Instance.GetOrCreatePlayer(spawnPointforPlayer, startRotation);
        PlayerManager.Instance.LoadPlayerState();

        if (virtualCamera != null)
        {
            //virtualCamera.Follow = player.transform;
        }
    }

}
