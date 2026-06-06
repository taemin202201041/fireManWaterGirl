using UnityEngine;

public class HorizontalLeber : MonoBehaviour
{
    public GameObject leftEnd; //왼쪽끝
    public GameObject rightEnd; //오른쪽끝
    public float state; // 현제 상태 0(왼쪽)~1(오른쪽) 대략적
    float len; // 길이
    private void Start()
    {
        len = rightEnd.transform.position.x - leftEnd.transform.position.x;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        state = ((gameObject.transform.position.x - leftEnd.transform.position.x)/len-0.09f)*1.2f; //0~1 범위 맞추기위해 0.09 빼고 1.2 곱함
    }
}
