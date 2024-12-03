using UnityEngine;
using UnityEngine.UI;

// スキルツリーのボタンにアタッチするスクリプト
public class SkillTreeButton : MonoBehaviour
{
    public GameObject skillPrefab; // このボタンが担当するスキル
    public Button skillButton;
    public Skill requiredSkill; // 前提スキル
    private Player player;       // プレイヤーのスクリプトへの参照

    void Start()
    {
        player = FindObjectOfType<Player>();

        if (player != null)
        {
            // スキルがアンロックされたときに UpdateButtonState() を呼び出す
            player.OnSkillUnlocked += OnSkillUnlocked;
        }

        UpdateButtonState();
    }

    void OnDestroy()
    {
        // イベント購読を解除
        if (player != null)
        {
            player.OnSkillUnlocked -= OnSkillUnlocked;
        }
    }

    private void OnSkillUnlocked(Skill unlockedSkill)
    {
        UpdateButtonState();
    }

    public void UpdateButtonState() // ボタンの状態を更新
    {
        Debug.Log("ツリーボタンを更新");
        if (player != null)
        {
        Player player = FindObjectOfType<Player>();
        }
        if (player != null && skillPrefab != null)
        {
            // 前提スキルが解放されているかどうかをチェック
            if (player.HasSkill(requiredSkill) || requiredSkill == null)
            {
                skillButton.interactable = true; // 押せるようにする
                SetButtonColor(Color.white); // ボタンの色を通常色に戻す
            }
            else
            {
                skillButton.interactable = false; // 押せないようにする
                SetButtonColor(Color.gray); // ボタンの色を変える
            }
        }
        else
        {
            Debug.Log("プレイヤーかスキルプレハブが見つからない");
        }
    }

    void SetButtonColor(Color color) // ボタンの色を設定
    {
        ColorBlock colorBlock = skillButton.colors;
        colorBlock.normalColor = color;
        colorBlock.disabledColor = color;
        skillButton.colors = colorBlock;
    }

    public void OnSkillButtonClick() // スキル開放ボタンが押された時に呼ばれるメソッド
    {   
        Player player = FindObjectOfType<Player>();
        if (player != null && skillPrefab != null && skillButton.interactable)
        {
            if( (requiredSkill == null) || (requiredSkill != null && player.HasSkill(requiredSkill)) )
            {
                // プレハブからスキルオブジェクトを生成
                GameObject skillObject = Instantiate(skillPrefab);
                Skill skill = skillObject.GetComponent<Skill>();
                if (skill != null)
                {
                    // スキルをアンロック
                    player.UnlockSkill(skill);
                }
                // スキルオブジェクトを非アクティブ化してから削除
                //skillObject.SetActive(false);
                //Destroy(skillObject);

                //UpdateButtonState(); // 他のボタンの状態も更新
            }
            else if (!player.HasSkill(requiredSkill))
            {
                Debug.Log("前提スキル持ってない");
            }
            else
            {
                Debug.LogError("前提スキル周りが変かも");
            }
        }
        else
        {
            Debug.LogError("プレイヤーかスキルプレハブかボタンが変かも");
        }

    }
}

    
