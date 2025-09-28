using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Rotation
        float rotation = Input.GetAxisRaw("Horizontal") * rotateSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);

        // Movement (local space)
        float movement = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = transform.forward * movement * moveSpeed;

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep grounded
        }
        velocity.y += gravity * Time.deltaTime;

        // Combine movement and gravity
        Vector3 move = (moveDirection + new Vector3(0, velocity.y, 0)) * Time.deltaTime;

        controller.Move(move);
    }
}
