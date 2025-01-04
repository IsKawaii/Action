using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // シリアライズしてUnityのInspectorでアサイン
    [SerializeField] private CharacterManager characterManager;

    private void Start()
    {
        // CharacterManagerがアサインされていれば好感度を更新
        if (characterManager != null)
        {
            characterManager.ParseCharacterLikabilityCommand("Alice+1");
        }
    }

}
