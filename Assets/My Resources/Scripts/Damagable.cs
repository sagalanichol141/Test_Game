using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    public string targetTag;
    public float minHitDamage;
    public float maxHitDamage;
    public float hitDelay;

    private bool hit;

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag(targetTag) && !hit)
        {
            StartCoroutine(DoDamage(other.gameObject));
        }
    }

    private IEnumerator DoDamage(GameObject target)
    {
        hit = true;
        target.GetComponent<Health>().TakeDamage(Random.Range(minHitDamage, maxHitDamage));
        yield return new WaitForSecondsRealtime(hitDelay);
        hit = false;
    }
}
