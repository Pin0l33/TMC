using UnityEngine; 

public class PersistentBootstrap : MonoBehaviour 

{ private static PersistentBootstrap instance; void Awake() 
{ if (instance != null) 
{ Destroy(gameObject); return; } 
instance = this; DontDestroyOnLoad(gameObject); } }