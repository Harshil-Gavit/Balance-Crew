using System;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    
    [SerializeField] PlayerController playerController;

    void Start()
    {
        playerController = GameObject.FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        playerController.isGrounded = true;
    }
}
