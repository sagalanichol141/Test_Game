using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Pickup : MonoBehaviour
{
    public PickupType pickupType;
    public int maxBulletAdd;
    public int maxBombAdd;
    public int maxHealthAdd;
    public ParticleSystem explodeFX;
    public AudioClip poofSFX;
    public AudioClip explodeSFX;
    public CinemachineImpulseSource cinemachineImpulseSource;
    private ParticleSystem.MainModule mainFX;

    private void OnTriggerStay(Collider other) {
        PlaySound sound = new PlaySound();
        if (other.gameObject.CompareTag("Player"))
        {
            sound.PlaySoundAt(poofSFX, transform.position, 1, "Poof", true);
            sound.PlaySoundAt(explodeSFX, transform.position, 1, "Explode Poof", true);
            cinemachineImpulseSource = GameObject.Find("CM Impulse").GetComponent<CinemachineImpulseSource>();
            if (pickupType == PickupType.Bullet)
                other.gameObject.GetComponent<PlayerAttack>().bulletAmmo += Random.Range(1, maxBulletAdd);
            
            else if (pickupType == PickupType.Bomb)
                other.gameObject.GetComponent<PlayerAttack>().bombAmmo += Random.Range(1, maxBombAdd);
            
            else
                other.gameObject.GetComponent<Health>().currentHealth += Random.Range(1, maxHealthAdd);

            GameObject cloneFX = Instantiate(explodeFX.gameObject, transform.position, Quaternion.identity);
            mainFX = cloneFX.GetComponent<ParticleSystem>().main;
            mainFX.stopAction = ParticleSystemStopAction.Destroy;
            cinemachineImpulseSource.GenerateImpulse();
            Destroy(gameObject);
        }
    }
}

public enum PickupType
{
    Bullet,
    Bomb,
    Health
}
