using UnityEngine;

public class PlaySound
{
    public void PlaySoundAt(AudioClip audioClip, Vector3 position, float volume, string soundName, bool is3D)
    {
        GameObject soundObject = new GameObject(soundName);
        soundObject.transform.position = position;

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = is3D ? 1.0f : 0.0f; // Set spatial blend to 1 for 3D sound, 0 for 2D sound

        audioSource.Play();

        // Hancurkan objek setelah clip selesai dimainkan
        MonoBehaviour.Destroy(soundObject, audioClip.length);
    }
}
