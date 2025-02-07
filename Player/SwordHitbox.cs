using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{

    public PlayerController playerController;
    

    private void OnTriggerEnter(Collider other) {
        playerController.OnSwordCollisionEnter(other);
    }
}
