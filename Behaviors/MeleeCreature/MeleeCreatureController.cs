using System;
using Behaviors.MeleeCreature.States;
using EventArgs;
using UnityEngine;
using UnityEngine.AI;

public class MeleeCreatureController : MonoBehaviour
{
    [HideInInspector]public MeleeCreatureHelper helper;
    [HideInInspector]public NavMeshAgent thisAgent;
    [HideInInspector]public Animator thisAnimator;
    [HideInInspector]public LifeScript thisLife;
    [HideInInspector]public Collider thisCollider;
    [HideInInspector] public Rigidbody thisRigidbody;

    [HideInInspector]public StateMachine stateMachine;
    [HideInInspector]public Idle idleState;
    [HideInInspector]public Follow followState;
    [HideInInspector]public Attack attackState;
    [HideInInspector]public Hurt hurtState;
    [HideInInspector]public Dead deadState;

    [Header("General")]
    public float searchRadius = 5f;

    [Header("Idle")]
    public float targetSearchInterval = 1;

    [Header("Follow")]
    public float ceaseFollowInterval = 4f;

    [Header("Attack")]
    public float distanceToAttack = 1f;
    public float attackRadius = 1.5f;
    public float attackSphereRadius = 1.5f;
    public float damageDelay = 0f;
    public float attackDuration = 1f;
    public int attackDamage = 1;

    [Header("Hurt")]
    public float hurtDuration = 1f;

    [Header("Dead")]
    public float destroyIfFar = 30f;

    [Header("Effects")]
    public GameObject knockoutEffect;

    [Header("Debug")]
    public string currentStateName;

    private void Awake() {
        thisAgent = GetComponent<NavMeshAgent>();
        thisAnimator = GetComponent<Animator>();
        thisLife = GetComponent<LifeScript>();
        thisCollider = GetComponent<Collider>();
        thisRigidbody = GetComponent<Rigidbody>();

        this.helper = new MeleeCreatureHelper(this);
    }

    private void Start() {

        stateMachine = new StateMachine();
        idleState = new Idle(this);
        followState = new Follow(this);
        attackState = new Attack(this);
        hurtState = new Hurt(this);
        deadState = new Dead(this);
        stateMachine.ChangeState(idleState);

        thisLife.OnDamage += OnDamage;
    }

    private void OnDamage(object sender, DamageEventArgs args) {
        Debug.Log("Criatura recebeu " + args.damage + " de dano de " + args.attacker.name);
        stateMachine.ChangeState(hurtState);
    }

    public void Update(){
            stateMachine.Update();
            currentStateName = stateMachine.currentStateName;

            var velocityRate = thisAgent.velocity.magnitude;
            thisAnimator.SetFloat("fVelocity", velocityRate);
        }

    public void LateUpdate(){
            stateMachine.LateUpdate();
        }

    public void FixedUpdate(){
            stateMachine.FixedUpdate();
        }
}
