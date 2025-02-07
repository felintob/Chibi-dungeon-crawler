using System;
using System.Collections;
using System.Collections.Generic;
using EventArgs;
using Player.States;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public Idle idleState;
    [HideInInspector] public Walking walkingState;
    [HideInInspector] public Jump jumpState;
    [HideInInspector] public Attack attackState;
    [HideInInspector] public Defend defendState;
    [HideInInspector] public Hurt hurtState;
    [HideInInspector] public Dead deadState;

    [HideInInspector] public Rigidbody thisRigidbody;
    [HideInInspector] public Animator thisAnimator;
    [HideInInspector] public LifeScript thisLife;

    [Header("Movement")]
    public float movementSpeed = 10;
    public float maxSpeed = 10;
    [HideInInspector] public Vector2 movementVector;

    [Header("Footsteps")]
    public List<AudioClip> footstepSounds;
    public AudioSource footstepAudioSource;
    public float footstepInterval = 0.33f;

    [Header("Jump")]
    public float jumpPower = 10;
    public float jumpMovementFactor = 1f;
    [HideInInspector] public bool hasJumpInput;


    [Header("Slope")]
    public float maxSlopeAngle = 45;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isOnSlope;
    [HideInInspector] public Vector3 slopeNormal;

    [Header("Attack")]
    public int attackStages;
    public List<float> attackStageDurations; 
    public List<float> attackStageMaxIntervals; 
    public List<float> attackStageImpulses;
    public GameObject swordHitbox;
    public float swordKnockbackImpulse = 10;
    public List<int> damageByStage;
    public float swordActiveThreshold = 0.5f;
    internal int currentAttackStage = 1;
    internal float timeLeftToAdvanceAttackStages;

    [Header("Defend")]
    public GameObject shieldHitbox;
    public float shieldKnockbackImpulse = 10;
    public float shieldSelfKnockbackImpulse = 4;
    [HideInInspector] public bool hasDefenseInput;

    [Header("Hurt")]
    public float hurtDuration = 0.2f;
    public float invulnerabilityDuration = 1f;
    internal float invulnerabilityTimeLeft;

    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject shieldEffect;



    void Awake() {
        thisRigidbody = GetComponent<Rigidbody>();
        thisAnimator = GetComponent<Animator>();
        thisLife = GetComponent<LifeScript>();

        
        if (thisLife != null) {
            thisLife.OnDamage += OnDamage;
            thisLife.OnHeal += OnHeal;
            thisLife.canInflictDamageDelegate += CanInflictDamage;
        }
        GlobalEvents.Instance.OnGameWon += OnGameWon;
    }


    
    void Start()
    {
        stateMachine = new StateMachine();
        idleState = new Idle(this);
        walkingState = new Walking(this);
        jumpState = new Jump(this);
        attackState = new Attack(this);
        defendState = new Defend(this);
        hurtState = new Hurt(this);
        deadState = new Dead(this);
        stateMachine.ChangeState(idleState);

        swordHitbox.SetActive(false);
        shieldHitbox.SetActive(false);

        var gameplayUI = GameManager.Instance.gameplayUI;
        gameplayUI.playerHealthBar.SetMaxHealth(thisLife.maxHealth);
    }

    
    void Update()
    {
        

      bool isUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
      bool isDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
      bool isLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
      bool isRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
      float inputX = isRight ? 1 : isLeft ? -1 : 0;
      float inputY = isUp ? 1 : isDown ? -1 : 0;
      movementVector = new Vector2(inputX, inputY);
      hasJumpInput = Input.GetKey(KeyCode.Space);

      hasDefenseInput = Input.GetMouseButton(1);

      float velocity = thisRigidbody.velocity.magnitude;
      float velocityRate = velocity / maxSpeed;
      thisAnimator.SetFloat("fVelocity", velocityRate);
      

      DetectGround();
      DetectSlope();
    
        var gameManager = GameManager.Instance;
        if (!gameManager.isGameOver && !gameManager.isGameWon) {
            if (invulnerabilityTimeLeft > 0 && !thisLife.isVulnerable) {
                if ((invulnerabilityTimeLeft -= Time.deltaTime) <= 0f) {
                    thisLife.isVulnerable = true;
                }
            }
        }

        timeLeftToAdvanceAttackStages -= Time.deltaTime;
        if (timeLeftToAdvanceAttackStages <= 0f) currentAttackStage = 1;
      
      var bossBattleHandler = GameManager.Instance.bossBattleHandler;
      var isInCutscene = bossBattleHandler.IsInCutscene();
      if(isInCutscene && stateMachine.currentStateName != idleState.name) {
        stateMachine.ChangeState(idleState);
      } 
      stateMachine.Update();

    }

    void LateUpdate() {
        stateMachine.LateUpdate();
    }

    void FixedUpdate() {
        Vector3 gravityForce = Physics.gravity * (isOnSlope ? 0.25f : 1f);
        thisRigidbody.AddForce(gravityForce, ForceMode.Acceleration);

        LimitSpeed();

        stateMachine.FixedUpdate();
    }

    private void OnGameWon(object sender, GameWonArgs args) {
        stateMachine.ChangeState(idleState);
        thisLife.isVulnerable = false;
    }

    private void OnDamage(object sender, DamageEventArgs args){
        if(GameManager.Instance.isGameOver) return;
        if (GameManager.Instance.isGameWon) return;
        Debug.Log("Player recebeu um dano de " + args.damage + " do " + args.attacker.name);
        stateMachine.ChangeState(hurtState);
    }
    private void OnHeal(object sender, HealEventArgs args) {
        var gameplayUI = GameManager.Instance.gameplayUI;
        gameplayUI.playerHealthBar.SetHealth(thisLife.health);
        Debug.Log("Player recebeu uma cura.");
    }


    public void OnSwordCollisionEnter(Collider other) {
        var otherObject = other.gameObject;
        var otherRigidbody = otherObject.GetComponent<Rigidbody>();
        var otherCollider = otherObject.GetComponent<Collider>();
        var otherLife = otherObject.GetComponent<LifeScript>();

        int bit = 1 << otherObject.layer;
        int mask = LayerMask.GetMask("Target", "Creatures");
        bool isBitInMask = (bit & mask) == bit;
        bool isTarget = isBitInMask; 

        if(isTarget && otherRigidbody != null){

            if(otherLife != null){
                var damage = damageByStage[currentAttackStage - 1];
                otherLife.InflictDamage(gameObject, damage);
            }
            
            if(otherRigidbody != null){
                var isLastStage = currentAttackStage == attackStages;
                var stageFactor = isLastStage ? 1.5f : 1f;
                var positionDiff = otherObject.transform.position - gameObject.transform.position;
                var impulseVector = new Vector3(positionDiff.normalized.x, 0, positionDiff.normalized.z);
                impulseVector *= swordKnockbackImpulse * stageFactor;
                otherRigidbody.AddForce(impulseVector, ForceMode.Impulse);
            }

            if(hitEffect != null){
                var hitPosition = otherCollider.ClosestPointOnBounds(swordHitbox.transform.position);
                var hitRotation = hitEffect.transform.rotation;
                Instantiate(hitEffect, hitPosition, hitRotation);
            }

        }
    }

    public void OnShieldCollisionEnter(Collider other){
        OnShieldCollisionEnter(other.gameObject);
    }

    public void OnShieldCollisionEnter(GameObject otherObject){
        var otherRigidbody = otherObject.GetComponent<Rigidbody>();
        var layerMask = LayerMask.GetMask("Creatures", "Projectile");
        var isEligible = layerMask == (layerMask | (1 << otherObject.layer));
        if (isEligible) {

            // Calculate knockback
            var positionDiff = otherObject.transform.position - gameObject.transform.position;
            var attackerImpulseVec = new Vector3(positionDiff.normalized.x, 0, positionDiff.normalized.z);
            attackerImpulseVec *= shieldKnockbackImpulse;
            var victimImpulseVec = new Vector3(-positionDiff.normalized.x, 0, -positionDiff.normalized.z);
            victimImpulseVec *= shieldSelfKnockbackImpulse;

            // Knockback attacker
            if (otherRigidbody != null) {
                otherRigidbody.AddForce(attackerImpulseVec, ForceMode.Impulse);
            }

            thisRigidbody.AddForce(victimImpulseVec, ForceMode.Impulse);

            if (shieldEffect != null) {
                var hitPosition = shieldHitbox.transform.position;
                var hitRotation = shieldEffect.transform.rotation;
                Instantiate(shieldEffect, hitPosition, hitRotation);
            }
        }
    }

    private bool CanInflictDamage(GameObject attacker, int damage){
        var isDefending = stateMachine.currentStateName == defendState.name;
        if(isDefending){
            Vector3 playerDirection = transform.TransformDirection(Vector3.forward);
            Vector3 attackDirection = (transform.position - attacker.transform.position).normalized;
            float dot = Vector3.Dot(playerDirection, attackDirection);
            if(dot < -0.25){
                OnShieldCollisionEnter(attacker);
                return false;
            }
        }
        return true;
    }

    public Quaternion GetFoward() {
        Camera camera = Camera.main;
        float eulerY = camera.transform.eulerAngles.y;
        return Quaternion.Euler(0, eulerY, 0);
    }

    public void RotateBodyToFaceInput(float alpha = 0.225f){
        if(movementVector.IsZero()) return;

         Camera camera = Camera.main;
         Vector3 inputVector = new Vector3(movementVector.x, 0, movementVector.y);
         Quaternion q1 = Quaternion.LookRotation(inputVector, Vector3.up);
         Quaternion q2 = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);
         Quaternion toRotation = q1 * q2;
         Quaternion newRotation = Quaternion.LerpUnclamped(transform.rotation, toRotation, alpha);

         thisRigidbody.MoveRotation(newRotation);
    }

    public bool AttemptToAttack() {
        if (currentAttackStage == attackStages) return false;

        if(Input.GetMouseButtonDown(0)) {
            var isAttacking = stateMachine.currentStateName == attackState.name;
            var canAttack = !isAttacking || attackState.CanSwitchStages();
            if(canAttack) {
                if (timeLeftToAdvanceAttackStages > 0f) currentAttackStage++;
                stateMachine.ChangeState(attackState);
                return true;
        }

    }
    return false;
    }



    private void DetectGround() {
        isGrounded = false;
        
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float maxDistance = 0.1f;
        LayerMask groundLayer = GameManager.Instance.groundLayer;
        if(Physics.Raycast(origin, direction, maxDistance, groundLayer)) {
            isGrounded = true;
        }
        
    }

    private void DetectSlope(){
        isOnSlope = false;
        slopeNormal = Vector3.zero;
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float maxDistance = 0.2f;

        if(Physics.Raycast(origin, direction, out var slopeHitInfo, maxDistance)) {
            float angle = Vector3.Angle(Vector3.up, slopeHitInfo.normal);
            isOnSlope = angle < maxSlopeAngle && angle != 0;
            slopeNormal = isOnSlope ? slopeHitInfo.normal : Vector3.zero;
            
        }
    }

    private void LimitSpeed(){
        Vector3 flatVelocity = new Vector3(thisRigidbody.velocity.x, 0, thisRigidbody.velocity.z);
        if(flatVelocity.magnitude > maxSpeed){
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;

            thisRigidbody.velocity = new Vector3(limitedVelocity.x, thisRigidbody.velocity.y, limitedVelocity.z);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("BossRoomSensor")) {
            GlobalEvents.Instance.InvokeBossRoomEnter(this, new BossRoomEnterArgs());
            Destroy(other.gameObject);
        }
    }

    
  }

