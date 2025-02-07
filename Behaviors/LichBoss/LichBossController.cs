using System.Collections;
using System.Collections.Generic;
using Behaviours.LichBoss.States;
using EventArgs;
using UnityEngine;
using UnityEngine.AI;

namespace Behaviours.LichBoss
{
    public class LichBossController : MonoBehaviour {
        
        [HideInInspector] public LichBossHelper helper;

        [HideInInspector] public NavMeshAgent thisAgent;
        [HideInInspector] public Animator thisAnimator;
        [HideInInspector] public LifeScript thisLife;

        [HideInInspector] public StateMachine stateMachine;
        [HideInInspector] public Idle idleState;
        [HideInInspector] public Follow followState;
        [HideInInspector] public AttackNormal attackNormalState;
        [HideInInspector] public AttackSuper attackSuperState;
        [HideInInspector] public AttackRitual attackRitualState;
        [HideInInspector] public Hurt hurtState;
        [HideInInspector] public Dead deadState;

        [Header("General")]
        public float lowHealthThreshold = 0.4f;
        public Transform staffTop;
        public Transform staffBottom;

        [Header("Idle")]
        public float idleDuration = 0.3f;

        [Header("Follow")]
        public float ceaseFollowInterval = 4f;

        [Header("Attack Normal")]
        public int attackDamage = 1;
        public Vector3 aimOffset = new Vector3(0, 1.4f, 0);
        
        public float attackNormalMagicDelay = 0f;
        public float attackNormalDuration = 0f;
        public float attackNormalImpulse = 10;

        [Header("Attack Super")]
        public float attackSuperMagicDelay = 0f;
        public float attackSuperDuration = 1f;
        public float attackSuperMagicDuration = 1f;
        public int attackSuperMagicCount = 5;
        public float attackSuperImpulse = 10;



        [Header("Attack Ritual")]
        public float distanceToRitual = 2.5f;
        public float attackRitualDelay = 0f;
        public float attackRitualDuration= 0f;

        [Header("Hurt")]
        public float hurtDuration = 0.5f;

        [Header("Magic")]
        public GameObject fireballPrefab;
        public GameObject energyBallPrefab;
        public GameObject ritualPrefab;

        [Header("Debug")]
        public string currentStateName;

        [HideInInspector]public List<IEnumerator> stateCoroutines = new List<IEnumerator>();

        private void Awake(){

            thisAgent = GetComponent<NavMeshAgent>();
            thisAnimator = GetComponent<Animator>();
            thisLife = GetComponent<LifeScript>();

            helper = new LichBossHelper(this);
        }

        private void Start(){

            stateMachine = new StateMachine();
            idleState = new Idle(this);
            followState = new Follow(this);
            attackNormalState = new AttackNormal(this);
            attackSuperState = new AttackSuper(this);
            attackRitualState = new AttackRitual(this);
            hurtState = new Hurt(this);
            deadState = new Dead(this);
            stateMachine.ChangeState(idleState);

            thisLife.OnDamage += OnDamage;
        }

        private void OnDamage(object sender, DamageEventArgs args){
            Debug.Log("Boss recebeu" + args.damage + " de dano de " + args.attacker.name);
            stateMachine.ChangeState(hurtState);
        }

        public void Update(){
            var bossBattleHandler = GameManager.Instance.bossBattleHandler;
            if(bossBattleHandler.IsActive()){
                stateMachine.Update(); 

            }
            currentStateName = stateMachine.currentStateName;

            var velocityRate = thisAgent.velocity.magnitude / thisAgent.speed;
            thisAnimator.SetFloat("fVelocity", velocityRate);

            if(!thisLife.IsDead()){
                var player = GameManager.Instance.player;
                var vecToPlayer = player.transform.position - transform.position;
                vecToPlayer.y = 0;
                vecToPlayer.Normalize();
                var desiredRotation = Quaternion.LookRotation(vecToPlayer);
                var newRotation = Quaternion.LerpUnclamped(transform.rotation, desiredRotation, 0.2f);
                transform.rotation = newRotation;

            }
        }

        public void LateUpdate(){
            stateMachine.LateUpdate();
        }

        public void FixedUpdate(){
            stateMachine.FixedUpdate();
        }

    }
    
}