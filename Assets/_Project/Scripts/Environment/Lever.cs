using UnityEngine;

public class Lever : MonoBehaviour
{
    public Door targetDoor;
    public bool State; //¿ÞÂÊ false ¿À¸¥ÂÊ true
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log(gameObject.transform.rotation.z);
        if(gameObject.transform.rotation.z > 0.3) 
        {
            State = false;
            targetDoor.Close();
        }
        else if(gameObject.transform.rotation.z < -0.3) 
        {
            State = true;
            targetDoor.Open();
        }
    }
}
