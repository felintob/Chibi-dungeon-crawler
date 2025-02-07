using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours.LichBoss.States{
    public class AttackRitual : State {

        private LichBossController controller;
        private LichBossHelper helper;

        private float endAttackCooldown;

        public AttackRitual(LichBossController controller) : base("AttackRitual"){
            this.controller = controller;
            this.helper = controller.helper;

        }

        public override void Enter() {
            base.Enter();

            endAttackCooldown = controller.attackRitualDuration;

            Debug.Log("Atacou com Ritual");
            controller.stateMachine.ChangeState(controller.idleState);

            controller.thisAnimator.SetTrigger("tAttackRitual");

            helper.StartStateCoroutine(ScheduleAttack(controller.attackRitualDelay));
        }

        public override void Exit(){
            base.Exit();
            helper.ClearStateCoroutines();

        }

        public override void Update(){
            base.Update();

            if((endAttackCooldown -= Time.deltaTime) <= 0f){
                controller.stateMachine.ChangeState(controller.idleState);
                return;
            }
        }

        public override void LateUpdate(){
            base.LateUpdate();
        }

        private IEnumerator ScheduleAttack(float delay){
            yield return new WaitForSeconds(delay);


            var gameObject = Object.Instantiate(controller.ritualPrefab, controller.staffBottom.position, controller.ritualPrefab.transform.rotation);

            Object.Destroy(gameObject, 10);

            if(helper.GetDistanceToPlayer() <= controller.distanceToRitual){
                var playerLife = GameManager.Instance.player.GetComponent<LifeScript>();
                playerLife.InflictDamage(controller.gameObject, controller.attackDamage);
            }
            
        }
    }
}