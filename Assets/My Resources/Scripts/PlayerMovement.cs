using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [Range(0, 10)] public float moveSpeed = 5f; // Kecepatan bergerak
    [Range(0, 10)] public float aimSpeed = 2f; // Kecepatan saat melihat ke arah mouse
    public Transform pointIndicator;
    
    private CharacterController characterController;
    private Animator animator;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Input horizontal dan vertical
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Mengatur vektor input
        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Menghitung arah pandang mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 lookDirection = transform.forward;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            pointIndicator.position = targetPoint;
            lookDirection = (targetPoint - transform.position).normalized;
        }

        // Mengatur rotasi pemain selalu menghadap ke arah mouse
        transform.forward = Vector3.Slerp(transform.forward, lookDirection, Time.deltaTime * aimSpeed);

        // Mengatur animasi berdasarkan input horizontal dan vertical
        animator.SetFloat("Input X", horizontalInput);
        animator.SetFloat("Input Z", verticalInput);

        // Mengatur pergerakan pemain berdasarkan local space
        Vector3 moveDirection = transform.TransformDirection(inputVector) * moveSpeed;

        // Memindahkan pemain menggunakan CharacterController
        characterController.SimpleMove(moveDirection);
    }
}
