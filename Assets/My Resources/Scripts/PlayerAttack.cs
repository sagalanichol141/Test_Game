using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee")]
    public string meleeLayer = "Melee Layer";
    public string meleeState = "Melee";
    public string targetTag = "Enemy";
    public LayerMask targetLayer;
    public float attackDamage = 30f;
    public GameObject meleeWeapon;
    public List<string> meleeStateList = new List<string>();
    public AudioClip slashSFX;

    [Header("Ranged")]
    public string rangedLayer = "Ranged Layer";
    public string rangedState = "Ranged";
    public float bulletDamage = 10;
    public int maxBulletAmmo = 12;
    public GameObject rangedWeapon;
    public List<string> rangedStateList = new List<string>();
    public Transform pooledBulletsParent;
    public Transform bulletOut;
    public TMP_Text bulletAmmoText;
    public AudioClip shootSFX;
    public AudioClip ammoOutSFX;

    [Header("Area")]
    public LayerMask overlapLayer;
    public string areaLayer = "Area Layer";
    public string areaState = "Area";
    public float bombDamage = 75f;
    public int maxBombAmmo = 5;
    public GameObject areaWeapon;
    public List<string> areaStateList = new List<string>();
    public Transform pooledBombParent;
    public Transform bombOut;
    public TMP_Text bombAmmoText;
    public AudioClip throwSFX;
    public AudioClip throwOutSFX;

    [Header("Animation")]
    [Range(0, 2)] public float meleExitOffset = 0.25f; // Untuk mengurangi delay exit time animasi melee
    [Range(0, 2)] public float rangedExitOffset = 0.25f; // Untuk mengurangi delay exit time animasi ranged
    [Range(0, 2)] public float areaExitOffset = 0.25f; // Untuk mengurangi delay exit time animasi area
    public AudioClip switchWeaponSFX;

    [Header("Output")]
    public bool isAttacking = false;
    [Range(1, 3)] public int weaponIndex = 1;
    public string currentWeapon = "Melee"; // Default weapon type
    public List<GameObject> overlappedObjectList;
    public float exitOffset;
    public int bulletAmmo;
    public int bombAmmo;

    private Collider attackTrigger;
    private Animator animator;
    private PlaySound sound;

    private void Start()
    {
        sound = new PlaySound();
        bulletAmmo = maxBulletAmmo;
        bombAmmo = maxBombAmmo;
        attackTrigger = meleeWeapon.GetComponent<Collider>();
        TriggerOff();
        animator = GetComponent<Animator>();
        SwitchWeapon(currentWeapon);
    }

    private void Update()
    {
        // Scroll mouse untuk mengganti senjata
        int scrollWheel = Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10);
        weaponIndex += scrollWheel;
        weaponIndex = Mathf.Clamp(weaponIndex, 1, 3);

        bulletAmmo = Mathf.Clamp(bulletAmmo, 0, maxBulletAmmo);
        bombAmmo = Mathf.Clamp(bombAmmo, 0, maxBombAmmo);
        bulletAmmoText.text = bulletAmmo.ToString() + "/" + maxBulletAmmo.ToString();
        bombAmmoText.text = bombAmmo.ToString() + "/" + maxBombAmmo.ToString();

        if (scrollWheel != 0f)
        {
            sound.PlaySoundAt(switchWeaponSFX, transform.position, 1, "Switch", true);
            if (weaponIndex == 1)
            {
                // Senjata awal
                SwitchWeapon("Melee");
            }
            else if (weaponIndex == 2)
            {
                // Scroll ke atas (ganti ke senjata berikutnya)
                SwitchWeapon("Ranged");
            }
            else if (weaponIndex == 3)
            {
                // Scroll ke bawah (ganti ke senjata sebelumnya)
                SwitchWeapon("Area");
            }
        }


        // Cek jika pemain sedang melakukan serangan
        if (!isAttacking && Input.GetMouseButton(0))
        {
            // Memilih state serangan sesuai dengan senjata saat ini
            string currentAttackState = "";

            if (currentWeapon == "Melee" && meleeStateList.Count > 0)
            {
                int randomIndex = Random.Range(0, meleeStateList.Count);
                currentAttackState = meleeStateList[randomIndex];
            }
            else if (currentWeapon == "Ranged")
            {
                currentAttackState = rangedStateList[0];
            }
            else if (currentWeapon == "Area")
            {
                currentAttackState = areaStateList[0];
            }

            if (!string.IsNullOrEmpty(currentAttackState))
            {
                // Memainkan state serangan sesuai senjata saat ini
                animator.Play(currentAttackState, animator.GetLayerIndex(currentWeapon + " Layer"));

                // Reset semua layer weight
                animator.SetLayerWeight(animator.GetLayerIndex(rangedLayer), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex(meleeLayer), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex(areaLayer), 0f);

                // Mengatur layer weight menjadi 1 sesuai senjata saat ini
                if (currentWeapon == "Ranged")
                {
                    animator.Play(rangedState);
                    animator.SetLayerWeight(animator.GetLayerIndex(rangedLayer), 1f);
                    exitOffset = rangedExitOffset;
                    Shoot();
                }
                else if (currentWeapon == "Area")
                {
                    animator.Play(areaState);
                    animator.SetLayerWeight(animator.GetLayerIndex(areaLayer), 1f);
                    exitOffset = areaExitOffset;
                }
                else
                {
                    animator.SetLayerWeight(animator.GetLayerIndex(meleeLayer), 1f);
                    exitOffset = meleExitOffset;
                }

                // Mengatur pemain sedang melakukan serangan
                isAttacking = true;

                // Memanggil method untuk mengembalikan layer weight ke 0 setelah animasi selesai
                StartCoroutine(ResetLayerWeight());
            }
        }
    }

    private IEnumerator ResetLayerWeight()
    {
        // Menunggu hingga animasi selesai
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(currentWeapon + " Layer")).length - exitOffset);

        // Mengatur layer weight menjadi 0 sesuai senjata saat ini
        if (currentWeapon == "Ranged")
        {
            animator.SetLayerWeight(animator.GetLayerIndex(rangedLayer), 0f);
        }
        else if (currentWeapon == "Area")
        {
            animator.SetLayerWeight(animator.GetLayerIndex(areaLayer), 0f);
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex(meleeLayer), 0f);
        }

        // Mengatur pemain telah selesai melakukan serangan
        isAttacking = false;
    }

    private void SwitchWeapon(string newWeapon)
    {
        // Nonaktifkan senjata saat ini
        if (currentWeapon == "Melee")
        {
            meleeWeapon.SetActive(false);
        }
        else if (currentWeapon == "Ranged")
        {
            rangedWeapon.SetActive(false);
        }
        else if (currentWeapon == "Area")
        {
            areaWeapon.SetActive(false);
        }

        // Aktifkan senjata baru dan sesuaikan animasi
        if (newWeapon == "Melee")
        {
            meleeWeapon.SetActive(true);
            animator.Play(meleeState);
        }
        else if (newWeapon == "Ranged")
        {
            rangedWeapon.SetActive(true);
            animator.Play(rangedState);
        }
        else if (newWeapon == "Area")
        {
            areaWeapon.SetActive(true);
            animator.Play(areaState);
        }

        currentWeapon = newWeapon;
    }

    public void TriggerOn()
    {
        sound.PlaySoundAt(slashSFX, transform.position, 1, "Slash", true);
        attackTrigger.enabled = true;
        GetTargetOverlap();
        foreach (GameObject item in overlappedObjectList)
        {
            if (item.CompareTag(targetTag))
            {
                item.GetComponent<Health>().TakeDamage(attackDamage);
            }
        }
    }

    public void TriggerOff()
    {
        attackTrigger.enabled = false;
        overlappedObjectList.Clear();
    }

    private void GetTargetOverlap()
    {
        // Menentukan posisi tengah bola tumpang tindih sesuai dengan posisi objek ini
        Collider meleeWeaponCollider = meleeWeapon.GetComponent<Collider>();
        Vector3 center = meleeWeaponCollider.bounds.center;

        // Menentukan radius sesuai dengan ukuran collider objek ini
        float radius = Mathf.Max(meleeWeaponCollider.bounds.size.x, meleeWeaponCollider.bounds.size.y, meleeWeaponCollider.bounds.size.z) / 2.0f;

        // Menggunakan Physics.OverlapSphere untuk mendeteksi objek pada lapisan target dalam radius tumpang tindih
        Collider[] colliders = Physics.OverlapSphere(center, radius, targetLayer);

        // Mengosongkan daftar tumpang tindih
        overlappedObjectList.Clear();

        // Menambahkan objek yang tumpang tindih ke dalam daftar
        foreach (Collider col in colliders)
        {
            overlappedObjectList.Add(col.gameObject);
        }
    }

    private void Shoot()
    {
        sound.PlaySoundAt(ammoOutSFX, transform.position, 1, "Ammo Out", true);
        if (bulletAmmo > 0)
        {
            sound.PlaySoundAt(shootSFX, transform.position, 1, "Shoot", true);
            bulletAmmo -= 1;
            foreach (Transform bullet in pooledBulletsParent)
            {
                if (!bullet.gameObject.activeSelf)
                {
                    bullet.position = bulletOut.position;
                    bullet.rotation = transform.rotation;
                    bullet.gameObject.SetActive(true);
                    bullet.gameObject.GetComponent<Bullet>().targetTag = targetTag;
                    bullet.gameObject.GetComponent<Bullet>().hitDamage = bulletDamage;
                    return;
                }
            }
        }
    }

    public void Throw()
    {
        sound.PlaySoundAt(throwOutSFX, transform.position, 1, "Throw Out", true);
        if (bombAmmo > 0)
        {
            sound.PlaySoundAt(throwSFX, transform.position, 1, "Throw", true);
            bombAmmo -= 1;
            foreach (Transform bomb in pooledBombParent)
            {
                if (!bomb.gameObject.activeSelf)
                {
                    bomb.position = bombOut.position;
                    bomb.rotation = transform.rotation;
                    bomb.gameObject.SetActive(true);
                    bomb.gameObject.GetComponent<Bomb>().targetLayer = overlapLayer;
                    bomb.gameObject.GetComponent<Bomb>().hitDamage = bombDamage;
                    return;
                }
            }
        }
    }
}
