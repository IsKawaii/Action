using UnityEngine;

public class SectionProperties : MonoBehaviour
{
    public float leftHeight; // 左端の高さ
    public float rightHeight; // 右端の高さ
    public float width; // セクションの幅 

    public EnemySpawnInfo[] enemySpawnInfos; // 敵のスポーン情報の配列 
    public int numberOfEnemiesToSpawn; // スポーンさせる敵の数 
    
    public Transform exitPoint; // Exitポイント new
}
