using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallDisplayUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerShooter shooter;
    public Transform iconContainer;   // アイコンを配置する親オブジェクト

    public static BallDisplayUI Instance { get; private set; }
    [SerializeField] private GameObject ballIconPrefab;
    public List<GameObject> ballIcons = new List<GameObject>();

    private int remainballs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("BallDisplayUIが生成");
        }
        else
        {
            Destroy(gameObject); // 重複したインスタンスを破棄
        }
    }

    private void OnEnable()
    {
        // プレイヤー生成イベントを購読
        Player.OnPlayerCreated += HandlePlayerCreated;
    }

    private void OnDisable()
    {
        // プレイヤー生成イベントの購読を解除
        Player.OnPlayerCreated -= HandlePlayerCreated;
    }

    private void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
    }

    public void  InitializeIcons(int remainBall, Sprite ballSprite)
    {
        Debug.Log("アイコン更新");
        // 既存のアイコンをクリア 
        foreach (GameObject icon in ballIcons)
        {
            Destroy(icon);
        }
        ballIcons.Clear();

        // 最大球数分のアイコンを生成
        for (int i = 0; i < shooter.maxShots; i++)
        {
            GameObject icon = Instantiate(ballIconPrefab, iconContainer);
            Image iconImage = icon.GetComponent<Image>();

            if (iconImage != null)
            {
                Debug.Log("アイコン画像を設定");
                iconImage.sprite = ballSprite; // アイコン画像を設定
            }

            icon.SetActive(false); // 初期状態は非表示
            
            ballIcons.Add(icon);
        } 
        remainballs = shooter.remainingShots;
        Debug.Log("remainballs" + remainballs);
        UpdateBallIcons(remainballs);
    }

    public void UpdateBallIcons(int remainballs)
    {
        for (int i = 0; i < shooter.maxShots; i++)
        {
            if (ballIcons[i] != null) 
            {
                ballIcons[i].SetActive(i < remainballs);
            }
        }
    }
}