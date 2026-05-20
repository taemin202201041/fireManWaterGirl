using UnityEngine;
using UnityEngine.SceneManagement; // 씬 재시작용

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 대상이 플레이어인지 확인
        if (collision.CompareTag("Player"))
        {
            Debug.Log(collision.name + " 사망!");
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        // 현재 씬을 다시 로드 (나중에 GameManager에서 처리하는게 좋음)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}