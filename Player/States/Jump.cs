using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Jump : State{

  private PlayerController controller;

  private bool hasJump;
  private float cooldown;

  public Jump(PlayerController controller) : base("Jump"){
    this.controller = controller;
    
  }

  public override void Enter() {
      base.Enter();
     
      hasJump = false;
      cooldown = 0.5f;
     
     controller.thisAnimator.SetBool("bJumping", true);
  }

  public override void Exit() {
      base.Exit();

      controller.thisAnimator.SetBool("bJumping", false);
  }
 
  public override void Update() {
      base.Update();
      cooldown -= Time.deltaTime;
      if(hasJump && controller.isGrounded && cooldown <= 0) {
          controller.stateMachine.ChangeState(controller.idleState);
          return;
      }

      
  }

  public override void LateUpdate() {
      base.LateUpdate();
  }

  public override void FixedUpdate() {
      base.FixedUpdate();

      if(!hasJump) {
        hasJump = true;
        ApplyImpulse();
      }

      Vector3 walkVector = new Vector3(controller.movementVector.x, 0, controller.movementVector.y);

      walkVector = controller.GetFoward() * walkVector;
      walkVector *= controller.movementSpeed * controller.jumpMovementFactor;



      controller.thisRigidbody.AddForce(walkVector, ForceMode.Force);

      controller.RotateBodyToFaceInput();
  }

  private void ApplyImpulse() {
    Vector3 forceVector = Vector3.up * controller.jumpPower;
    controller.thisRigidbody.AddForce(forceVector, ForceMode.Impulse);
  }

 

}