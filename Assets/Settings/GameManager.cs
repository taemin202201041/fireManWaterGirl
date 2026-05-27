using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴 적용 (어디서나 GameManager에 쉽게 접근할 수 있도록 함)
    public static GameManager Instance { get; private set; }

    [Header("Player Status")]
    public bool isFireboyReady = false;
    public bool isWatergirlReady = false;
    private bool isGameOver = false;
    public NetworkManager netWorkManager;

    private void Awake()
    {
        // 싱글톤 인스턴스 중복 방지
        if (Instance == null)
        {
            Instance = AwakeInstance();
            DontDestroyOnLoad(gameObject); //파괴방지
            DontDestroyOnLoad(netWorkManager.gameObject); //파괴방지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private GameManager AwakeInstance()
    {
        return this;
    }

    /// 플레이어가 클리어 문에 도착했을 때 호출되는 함수
    /// <param name="isFireboy">파이어보이면 true, 워터걸이면 false</param>
    /// <param name="isReady">도착 상태</param>
    public void SetPlayerReady(bool isFireboy, bool isReady)
    {
        if (isGameOver) return;

        if (isFireboy)
        {
            isFireboyReady = isReady;
        }
        else
        {
            isWatergirlReady = isReady;
        }

        // 둘 다 도착했는지 확인
        CheckStageClear();
    }
    /// 양쪽 플레이어가 모두 도착했는지 확인하고 스테이지를 클리어 처리합니다.
    private void CheckStageClear()
    {
        if (isFireboyReady && isWatergirlReady)
        {
            Debug.Log("스테이지 클리어! 다음 스테이지로 이동합니다.");
            // 다음 씬 로드 (Build Settings에 등록된 순서 기준)
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            // 다음 씬이 존재하면 로드, 없으면 처음으로 돌아가거나 메인화면 이동
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("모든 스테이지를 클리어했습니다!");
            }
        }
    }

    /// 플레이어가 속성에 맞지 않는 함정에 빠졌을 때 호출되는 함수
    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("게임 오버! 3초 후 스테이지를 재시작합니다.");

        // 3초 후 재시작 함수 호출
        Invoke(nameof(RestartStage), 3f);
    }
    /// 현재 스테이지(씬)를 다시 로드합니다.
    private void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
