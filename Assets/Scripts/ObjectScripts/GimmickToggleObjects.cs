using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickToggleObjects : MonoBehaviour
{
    public Player player;
    protected bool savedToggle, toggle;

    protected virtual void OnEnable()
    {
        Player.OnPlayerCreated += HandlePlayerCreated;
    }

    protected virtual void OnDisable()
    {
        Player.OnPlayerCreated -= HandlePlayerCreated;
        player.creatSave -= SavePosition; 
        player.backToSave -= LoadPosition;
    }

    protected virtual void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
        player.creatSave += SavePosition;
        player.backToSave += LoadPosition;
    }

    protected virtual void SavePosition()
    {
        savedToggle = toggle;
    }

    protected virtual void LoadPosition()
    {
        toggle = savedToggle;
    }
}