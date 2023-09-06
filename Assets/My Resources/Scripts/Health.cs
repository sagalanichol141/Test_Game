using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using HighlightPlus;
using Cinemachine;
using TMPro;
using UnityEngine.AI;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    [Header("Input")]
    public float maxHealth = 100f;
    public float damagedDelay = 1f;
    public float damagedKnockBack;
    public bool hitReaction = true;
    public bool healthBarLook = false;
    public Image healthBar;
    public TMP_Text healthText;
    public string[] dieStateList;
    public string[] damagedStateList;
    public string cinemachineImpulseName = "CM Impulse";
    public MonoBehaviour[] behavioursToDisable;
    public GameObject[] dropableList;
    public Transform damagedFXParent;
    public AudioClip damagedSFX;
    public AudioClip dieSFX;
    public AudioClip dropSFX;

    [Header("Output")]
    public float currentHealth;
    public bool isDamaged = false;
    public bool isDead = false;
    private Animator animator;
    private float timer = 0f;
    private NavMeshAgent navmesh;
    private HighlightEffect highlightEffect;
    private ParticleSystem.MainModule mainFX;
    private CinemachineImpulseSource cinemachineImpulse;
    private PlaySound sound;

    private void Start()
    {
        sound = new PlaySound();
        cinemachineImpulse = GameObject.Find(cinemachineImpulseName).GetComponent<CinemachineImpulseSource>();
        highlightEffect = GetComponent<HighlightEffect>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        navmesh = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (isDamaged && hitReaction)
        {
            Time.timeScale = 0;
            timer += Time.unscaledDeltaTime;
            if (timer >= damagedDelay)
            {
                isDamaged = false;
                timer = 0f;
                Time.timeScale = 1;
                if (!isDead)
                    EnableBehaviours();
            }
        }

        // Update health bar
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }

        if (healthBarLook)
        {
            healthBar.transform.parent.LookAt(Camera.main.transform);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        sound.PlaySoundAt(damagedSFX, transform.position, 1, "Damaged", true);
        if (cinemachineImpulse != null)
            cinemachineImpulse.GenerateImpulse();

        if (highlightEffect != null)
            highlightEffect.HitFX();

        if (!isDamaged)
        {
            if (hitReaction && damagedStateList.Length > 0)
            {
                DisableBehaviours();
                int randomIndex = Random.Range(0, damagedStateList.Length);
                animator.Play(damagedStateList[randomIndex]);
                transform.position -= transform.forward * Random.Range(0, -damagedKnockBack) * Time.deltaTime;
                isDamaged = true;
            }

            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        if (damagedFXParent != null)
        {
            foreach (Transform fx in damagedFXParent)
            {
                if (!fx.gameObject.activeSelf)
                {
                    mainFX = fx.gameObject.GetComponent<ParticleSystem>().main;
                    mainFX.stopAction = ParticleSystemStopAction.Disable;
                    fx.gameObject.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
                    fx.gameObject.SetActive(true);
                    return;
                }
            }
        }
    }

    public void Die()
    {
        sound.PlaySoundAt(dieSFX, transform.position, 1, "Die", true);
        isDead = true;
        if (healthBar != null)
            healthBar.transform.parent.gameObject.SetActive(false);

        if (dieStateList.Length > 0)
        {
            int randomIndex = Random.Range(0, dieStateList.Length);
            animator.Play(dieStateList[randomIndex]);
        }

        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
        DisableBehaviours();
        Drop();
        Destroy(gameObject);
    }

    private void DisableBehaviours()
    {
        foreach (var item in behavioursToDisable)
        {
            item.enabled = false;
        }

        if (navmesh != null)
            navmesh.enabled = false;
    }

    private void EnableBehaviours()
    {
        foreach (var item in behavioursToDisable)
        {
            item.enabled = true;
        }

        if (navmesh != null)
            navmesh.enabled = true;
    }

    private void Drop()
    {
        sound.PlaySoundAt(dropSFX, transform.position, 1, "Drop", true);
        if (dropableList.Length > 0)
        {
            Instantiate(dropableList[Random.Range(0, dropableList.Length)], new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity);
        }
    }
}
