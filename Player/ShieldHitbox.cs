using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHitbox : MonoBehaviour
{

    public PlayerController playerController;
    

    private void OnTriggerEnter(Collider collision){
        playerController.OnShieldCollisionEnter(collision);
    }

}
