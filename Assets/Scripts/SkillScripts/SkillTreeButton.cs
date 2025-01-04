using UnityEngine;
using UnityEngine.UI;
using TMPro;

// スキルツリーのボタンにアタッチするスクリプト
public class SkillTreeButton : MonoBehaviour
{
    public GameObject skillPrefab; // このボタンが担当するスキル
    public Button skillButton; // このボタン
    public TextMeshProUGUI buttonText; // このボタンのテキスト
    public Skill requiredSkill; // 前提スキル
    private Player player;       // プレイヤーのスクリプトへの参照
    private bool havaThisSkill; 

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
        if (player != null)
        {
            Player player = FindObjectOfType<Player>();
        }
        if (player != null && skillPrefab != null)
        {
            if (!havaThisSkill && (player.HasSkill(requiredSkill) || requiredSkill == null))
            {
                skillButton.interactable = true; // 押せるようにする
                SetButtonColor(Color.white); // ボタンの色を通常色に戻す
                buttonText.color = Color.white;
            }
            else
            {
                skillButton.interactable = false; // 押せないようにする
                SetButtonColor(Color.gray); // ボタンの色を変える
                buttonText.color = Color.gray;
            }
        }
        else
        {
            Debug.Log("プレイヤーかスキルプレハブが見つからない");
        }
    }

    private void SetButtonColor(Color color) // ボタンの色を設定
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
            if ( (requiredSkill == null) || (requiredSkill != null && player.HasSkill(requiredSkill)) )
            {
                // プレハブからスキルオブジェクトを生成
                GameObject skillObject = Instantiate(skillPrefab);
                Skill skill = skillObject.GetComponent<Skill>();
                if (skill != null)
                {
                    player.UnlockSkill(skill); // スキルをアンロック
                    skillButton.interactable = false; // 押せないようにする
                    havaThisSkill = true;
                    SetButtonColor(Color.gray); // ボタンの色を変える
                    buttonText.color = Color.gray;
                }
                skillObject.SetActive(false);
                //Destroy(skillObject);
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

    
