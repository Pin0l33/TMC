using UnityEngine;

public class ReproducirSFX : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }



}

