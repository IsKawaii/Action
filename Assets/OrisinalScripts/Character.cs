using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string Name { get; private set; }   // キャラクターの名前
    public int Likability { get; private set; } // キャラクターの好感度

    public Character(string name, int initialLikability)
    {
        Name = name;
        Likability = initialLikability;
    }

    public void UpdateLikability(int change)
    {
        Likability += change;
        Debug.Log($"{Name} の好感度が {change} 変わりました。現在の好感度: {Likability}");
    }
}

