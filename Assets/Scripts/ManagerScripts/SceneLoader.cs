using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Player player;
    public PlayerShooter shooter;
    public FadeImage fade;
    public string nextSceneName;
    private Collider2D myCollider;
    private bool isNextScenePlace = false, canGoNextScene = false;

    private void OnEnable()
    {
        Player.OnPlayerCreated += HandlePlayerCreated;
    }

    private void OnDisable()
    {
        Player.OnPlayerCreated -= HandlePlayerCreated;
    }

    private void HandlePlayerCreated(Player newPlayer)
    {
        player = newPlayer;
    }

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isNextScenePlace && InputManager.instance.GetKeyDown(KeyCode.N))
        {
            isNextScenePlace = false;
            PrepareNextScene();      
        }

        if (canGoNextScene && fade.IsFadeOutComplete())
        {
            canGoNextScene = false;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void PrepareNextScene()
    {
        fade.StartFadeOut();
        if (player != null)
        {
            player.isSceneChanging = true;
            PlayerManager.Instance.SavePlayerState();
        }
        canGoNextScene = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isNextScenePlace = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isNextScenePlace = false;   
        }
    }
}
