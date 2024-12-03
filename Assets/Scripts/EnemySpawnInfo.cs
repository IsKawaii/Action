using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public Transform spawnPoint; // スポーンポイント
    public GameObject[] enemyPrefabs; // スポーンポイントに対応する複数の敵Prefab
}
