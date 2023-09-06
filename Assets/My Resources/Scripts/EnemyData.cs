using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float normalSpeed = 3.0f;
    public float chasingSpeed = 5.0f;
    public float rotationSpeed = 120.0f;
    public float stopDistance = 2.0f;
    public float wanderRadius = 10.0f;
    public float wanderDelay = 5.0f;
    public int fovRayCount = 8;
    public float fovRayDistance = 10.0f;
    public float fovRayHeight = 1.0f;
    public float fovAngle = 60.0f;
    public float attackDamage = 10.0f;
    public string targetTag = "Player";
    public AudioClip slashSFX;
    public AudioClip shootSFX;
}
