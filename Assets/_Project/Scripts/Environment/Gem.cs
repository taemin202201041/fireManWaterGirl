using UnityEngine;

public class Gem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 대상이 플레이어인지 확인
        if (collision.CompareTag("Player"))
        {
            Debug.Log(collision.name + " 젬 획득!");
            //게임메니저에서 젬획득 카운트
            GameManager.Instance.gemCountTemp += 1;
            GameObject.Destroy(gameObject);
        }
    }
}
