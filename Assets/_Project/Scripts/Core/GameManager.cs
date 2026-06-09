using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Status")]
    public bool isFireboyReady = false;
    public bool isWatergirlReady = false;
    private bool isGameOver = false;
    public NetworkManager netWorkManager;
    public GameObject fireManSingle;
    public GameObject waterGirlSingle;
    public Move fireMan;
    public Move waterGirl;
    public Map Map;
    public int gemCount;
    public int gemCountTemp;
    bool temp; //맵 중복 실행 방지용

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(netWorkManager.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            isGameOver = false;
            RestartStage();
        }
    }

    public void NextStage()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings && temp ==false)
        { gemCount += gemCountTemp; Invoke(nameof(go), 0.5f); temp = true; }
        else
            Debug.Log("모든 스테이지를 클리어했습니다!");
    }
    public void go() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); temp = false; }

    public void SinglePlayStart()
    {
        fireMan = Instantiate(fireManSingle).GetComponent<Move>();
        waterGirl = Instantiate(waterGirlSingle).GetComponent<Move>();
        NextStage();
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("게임 오버! 3초 후 스테이지를 재시작합니다.");
        Invoke(nameof(RestartStage), 0f);
    }
    private void RestartStage()
    {
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
