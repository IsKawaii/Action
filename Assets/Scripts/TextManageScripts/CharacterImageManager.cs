using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImageData
{
    public string imageName;   // 画像名
    public Sprite image;       // 画像
}

[System.Serializable]
public class CharacterImageData
{
    public string characterName;           // キャラクター名
    public List<ImageData> images = new(); // ImageDate(画像データ)のリスト
}

public class CharacterImageManager : MonoBehaviour
{
    public List<CharacterImageData> characterImageList = new();

    private Dictionary<string, Dictionary<string, Sprite>> characterImageDictionary;

    private void Start()
    {
        characterImageDictionary = new Dictionary<string, Dictionary<string, Sprite>>();

        foreach (var characterData in characterImageList)
        {
            var imageDictionary = new Dictionary<string, Sprite>();
            foreach (var imageData in characterData.images)
            {
                if (!imageDictionary.ContainsKey(imageData.imageName))
                {
                    imageDictionary.Add(imageData.imageName, imageData.image);
                }
            }

            if (!characterImageDictionary.ContainsKey(characterData.characterName))
            {
                characterImageDictionary.Add(characterData.characterName, imageDictionary);
            }
        }
    }

    public Sprite GetCharacterImage(string characterName, string imageName)
    {
        if (characterImageDictionary.TryGetValue(characterName, out var imageDictionary))
        {
            if (imageDictionary.TryGetValue(imageName, out var sprite))
            {
                return sprite;
            }
        }
        return null;
    }
}
