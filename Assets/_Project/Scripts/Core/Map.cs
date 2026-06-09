using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    public Vector2 fireManStartPositon;
    public Vector2 WaterGirlStartPositon;
    void Start()
    {
        PlayerSetting();
        GameManager.Instance.Map = this;
        GameManager.Instance.gemCountTemp = 0;
    }
    public void PlayerSetting() 
    {
        GameManager.Instance.fireMan.transform.position = fireManStartPositon;
        GameManager.Instance.waterGirl.transform.position = WaterGirlStartPositon;
    }
}
