using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private string zoneName;

    public string Id => id;
    public string ZoneName => zoneName;
}