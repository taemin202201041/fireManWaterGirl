using UnityEngine;

public class Lever : MonoBehaviour
{
    public bool State; //¿ŞÂÊ false ¿À¸¥ÂÊ true
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(gameObject.transform.rotation.z > 35) 
        {
            State = false;
        }
        else if(gameObject.transform.rotation.z > -35) 
        {
            State = true;
        }
    }
}
