using UnityEngine;

public class ScriptCamara : MonoBehaviour
{
    
    void Start()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

}
