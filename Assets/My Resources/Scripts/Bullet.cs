using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public string targetTag;
    [HideInInspector] public float hitDamage;
    public float speed;
    public float lifeTime;
    public string pooledImpactFXName;
    public AudioClip impactSFX;

    private Transform pooledImpactFX;
    private ParticleSystem impactFX;
    private ParticleSystem.MainModule mainModule;
    private PlaySound sound;

    private void HandleImpact() {
        foreach (Transform fx in pooledImpactFX)
        {
            if (!fx.gameObject.activeSelf)
            {
                impactFX = fx.GetComponent<ParticleSystem>();
                impactFX.transform.position = transform.position;
                impactFX.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                mainModule = impactFX.main;
                mainModule.stopAction = ParticleSystemStopAction.Disable;
                fx.gameObject.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
        }
    }

    private void Update() {
        transform.Translate(Vector3.forward * speed * Time.unscaledDeltaTime);
    }

    private IEnumerator Alive()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        gameObject.SetActive(false);
        HandleImpact();
    }

    private void OnTriggerStay(Collider other) {
        sound.PlaySoundAt(impactSFX, transform.position, 1, "Bullet", true);
        if (other.gameObject.CompareTag(targetTag))
        {
            other.gameObject.GetComponent<Health>().TakeDamage(hitDamage);
            HandleImpact();
        }

        else
            HandleImpact();
    }

    private void OnEnable() {
        sound = new PlaySound();
        pooledImpactFX = GameObject.Find(pooledImpactFXName).transform;
        StartCoroutine(Alive());
    }
}
