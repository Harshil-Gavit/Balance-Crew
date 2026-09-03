using System;
using UnityEngine;

public class tempFishSpawn : MonoBehaviour
{
    [SerializeField] private GameObject fish;
    [SerializeField] private Transform playerTransform;

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Instantiate(fish, playerTransform.position, playerTransform.rotation);
        }
    }
}
