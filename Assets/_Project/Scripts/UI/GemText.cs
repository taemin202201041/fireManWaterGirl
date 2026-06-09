using TMPro;
using UnityEngine;

public class GemText : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"¾òÀº Àë¼ö: {GameManager.Instance.gemCount}/21";
    }
}
