using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{

    public GameObject hitEffect;
    [HideInInspector] public GameObject attacker;
    [HideInInspector] public int damage;

    void Awake(){

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
        var hitObject = collision.gameObject;
        var hitLayer = hitObject.layer;
        var collidedWithPlayer = hitLayer == LayerMask.NameToLayer("Player");
        if(collidedWithPlayer) {
            var hitLife = hitObject.GetComponent<LifeScript>();
            if(hitLife != null) {
                hitLife.InflictDamage(attacker, damage);
            }
        }

        if(hitEffect != null) {
            var effect = Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
            Destroy(effect, 10);
        }

        Destroy(gameObject);
    }
}
