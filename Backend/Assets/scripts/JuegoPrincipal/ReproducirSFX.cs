using UnityEngine;

public class ReproducirSFX : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlaySFXPitched(AudioClip clip, float minPitch, float maxPitch)
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip);
    }

    
    public void PlayInterrupting(AudioClip clip, float minPitch, float maxPitch, float volume = 1f)
    {
        audioSource.Stop();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();
    }
}