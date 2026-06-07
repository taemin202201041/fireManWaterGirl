using UnityEngine;

public class Hazard : MonoBehaviour
{
    public enum HazardType { Fire, Water, Both }
    public HazardType type = HazardType.Both;

    [SerializeField] private LayerMask fireboyLayer;
    [SerializeField] private LayerMask watergirlLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int objLayer = 1 << collision.gameObject.layer;
        bool isFireboy   = (fireboyLayer.value & objLayer) != 0;
        bool isWatergirl = (watergirlLayer.value & objLayer) != 0;

        bool shouldDie = type switch
        {
            HazardType.Fire  => isWatergirl,
            HazardType.Water => isFireboy,
            HazardType.Both  => isFireboy || isWatergirl,
            _ => false
        };

        if (shouldDie)
        {
            Debug.Log(collision.name + " 사망!");
            GameManager.Instance.TriggerGameOver();
        }
    }
}
