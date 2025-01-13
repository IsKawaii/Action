using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterToggleManager : MonoBehaviour
{
    public static CharacterToggleManager Instance; // シングルトンインスタンス

    public GameObject[] predefinedCharacters; // プリセットキャラクターリスト
    private GameObject[] characters; // 実際のキャラクターインスタンスリスト
    public int currentCharacterIndex = 0; // 現在のキャラクター

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeWithPlayer(GameObject playerInstance)
    {
        // プリセットキャラクターリストを複製し、最初のキャラクターにプレイヤーを設定
        characters = new GameObject[predefinedCharacters.Length];
        characters[0] = playerInstance;

        // 他のキャラクターを生成しリストに登録
        for (int i = 1; i < predefinedCharacters.Length; i++)
        {
            characters[i] = Instantiate(predefinedCharacters[i]);
            characters[i].SetActive(false); // 最初は非アクティブ
            DontDestroyOnLoad(characters[i]);
        }

        // 最初のキャラクターを有効化
        currentCharacterIndex = 0;
        characters[currentCharacterIndex].SetActive(true);
    }

    public void SwitchCharacter()
    {
        // 現在のキャラクターを無効化
        characters[currentCharacterIndex].SetActive(false);

        // 次のキャラクターを選択
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;

        // 新しいキャラクターを有効化
        characters[currentCharacterIndex].SetActive(true);
    }

    public GameObject GetCurrentCharacter()
    {
        return characters[currentCharacterIndex];
    }
}