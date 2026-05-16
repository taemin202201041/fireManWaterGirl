using UnityEngine;
using UnityEngine.SceneManagement; // 씬 재시작용

public class Hazard : MonoBehaviour
{
    public enum HazardType { Fire, Water, Poison } // 위험 요소 종류
    public HazardType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 대상이 플레이어인지 확인
        if (collision.CompareTag("Player"))
        {
            // 플레이어 이름이나 특정 컴포넌트로 불/물 구분
            string playerName = collision.gameObject.name;

            bool shouldDie = false;

            if (type == HazardType.Fire && playerName.Contains("Water")) shouldDie = true;
            else if (type == HazardType.Water && playerName.Contains("Fire")) shouldDie = true;
            else if (type == HazardType.Poison) shouldDie = true; // 독물은 둘 다 죽음

            if (shouldDie)
            {
                Debug.Log(playerName + " 사망!");
                RestartLevel();
            }
        }
    }

    void RestartLevel()
    {
        // 현재 씬을 다시 로드 (나중에 GameManager에서 처리하는게 좋음)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}