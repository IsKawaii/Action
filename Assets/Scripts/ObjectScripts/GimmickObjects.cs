using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GimmickObjects : MonoBehaviour
{
    public Player player;
    protected Vector2 savedPosition;
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
        savedPosition = transform.position;
        savedToggle = toggle;
    }

    protected virtual void LoadPosition()
    {
        transform.position = savedPosition;
        toggle = savedToggle;
    }
}
