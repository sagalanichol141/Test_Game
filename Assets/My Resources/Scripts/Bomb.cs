using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [HideInInspector] public string targetTag;
    [HideInInspector] public LayerMask targetLayer;
    public float hitDamage;
    public float speed;
    public float radius;
    public float lifeTime;
    public string pooledImpactFXName;
    public AudioClip explosionSFX;

    private Transform pooledExplodeFX;
    private ParticleSystem explodeFX;
    private ParticleSystem.MainModule mainModule;
    private Rigidbody rb;
    private PlaySound sound;

    private void HandleExplosion() {
        foreach (Transform fx in pooledExplodeFX)
        {
            if (!fx.gameObject.activeSelf)
            {
                sound.PlaySoundAt(explosionSFX, transform.position, 1, "Bomb Explosion", true);
                explodeFX = fx.GetComponent<ParticleSystem>();
                explodeFX.transform.position = transform.position;
                explodeFX.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                mainModule = explodeFX.main;
                mainModule.stopAction = ParticleSystemStopAction.Disable;
                fx.gameObject.SetActive(true);

                Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
                foreach (Collider col in colliders)
                {
                    col.gameObject.GetComponent<Health>().TakeDamage(hitDamage);
                }
                
                gameObject.SetActive(false);
                return;
            }
        }
    }

    private IEnumerator Alive()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        HandleExplosion();
        gameObject.SetActive(false);
    }

    private void OnEnable() {
        sound = new PlaySound();
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * speed * Time.fixedUnscaledDeltaTime, ForceMode.VelocityChange);
        pooledExplodeFX = GameObject.Find(pooledImpactFXName).transform;
        StartCoroutine(Alive());
    }
}
