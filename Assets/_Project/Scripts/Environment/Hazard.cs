using UnityEngine;

public class Hazard : MonoBehaviour
{
    public enum HazardType { Fire, Water, Both }
    public HazardType type = HazardType.Both;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        string name = collision.gameObject.name;
        bool isFireboy   = name.Contains("Fire");
        bool isWatergirl = name.Contains("Water");

        bool shouldDie = type switch
        {
            HazardType.Fire  => isWatergirl, // 불 웅덩이: 물소녀만 죽음
            HazardType.Water => isFireboy,   // 물 웅덩이: 불소년만 죽음
            HazardType.Both  => true,
            _ => false
        };

        if (shouldDie)
        {
            Debug.Log(collision.name + " 사망!");
            GameManager.Instance.TriggerGameOver();
        }
    }
}
