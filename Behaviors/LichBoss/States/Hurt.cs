using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Behaviours.LichBoss.States{
    public class Hurt : State {

        private LichBossController controller;
        private LichBossHelper helper;

        private float timePassed;

        public Hurt(LichBossController controller) : base("Hurt"){
            this.controller = controller;
            this.helper = controller.helper;

        }

        public override void Enter() {
            base.Enter();

            timePassed = 0;

            controller.thisLife.isVulnerable = false;

            controller.thisAnimator.SetTrigger("tHurt");
        }

        public override void Exit(){
            base.Exit();

            controller.thisLife.isVulnerable = true;

        }

        public override void Update(){
            base.Update();

            timePassed += Time.deltaTime;

            if(timePassed >= controller.hurtDuration) {
                if(controller.thisLife.IsDead()){
                    controller.stateMachine.ChangeState(controller.deadState);
                } else {
                    controller.stateMachine.ChangeState(controller.idleState);
                }
                return;
            }
        }

        public override void LateUpdate(){
            base.LateUpdate();
        }
    }
}