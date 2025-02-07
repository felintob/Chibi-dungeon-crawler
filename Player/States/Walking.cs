using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : State {

  private PlayerController controller;
  private float footstepCooldown;

  public Walking(PlayerController controller) : base("Walking") {
    this.controller = controller;

  }

  public override void Enter() {
      base.Enter();

      
  }

  public override void Exit() {
      base.Exit();
  }
 
  public override void Update() {
      base.Update();

       if(controller.AttemptToAttack()) {
        return;
      }

      if(controller.hasDefenseInput) {
        controller.stateMachine.ChangeState(controller.defendState);
        return;
      }

      if(controller.hasJumpInput) {
        controller.stateMachine.ChangeState(controller.jumpState);
        return;
      }

      if(controller.movementVector.IsZero()) {
        controller.stateMachine.ChangeState(controller.idleState);
        return;
      }

            float velocity = controller.thisRigidbody.velocity.magnitude;
            float velocityRate = velocity / controller.maxSpeed;
            footstepCooldown -= Time.deltaTime * velocityRate;
            if (footstepCooldown <= 0f) {
                footstepCooldown = controller.footstepInterval;
                var audioClip = controller.footstepSounds[Random.Range(0, controller.footstepSounds.Count - 1)];
                var volumeScale = Random.Range(0.8f, 1f);
                controller.footstepAudioSource.PlayOneShot(audioClip, volumeScale);
            }

        
  }

  public override void LateUpdate() {
      base.LateUpdate();
  }

  public override void FixedUpdate() {
      base.FixedUpdate();

      Vector3 walkVector = new Vector3(controller.movementVector.x, 0, controller.movementVector.y);

      walkVector = controller.GetFoward() * walkVector;
      walkVector = Vector3.ProjectOnPlane(walkVector, controller.slopeNormal);
      walkVector *= controller.movementSpeed;


      controller.thisRigidbody.AddForce(walkVector, ForceMode.Force);

      controller.RotateBodyToFaceInput();
  }
  
}