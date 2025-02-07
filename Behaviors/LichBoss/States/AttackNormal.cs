using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours.LichBoss.States{
    public class AttackNormal : State {

        private LichBossController controller;
        private LichBossHelper helper;

        private float endAttackCooldown;

        public AttackNormal(LichBossController controller) : base("AttackNormal"){
            this.controller = controller;
            this.helper = controller.helper;

        }

        public override void Enter() {
            base.Enter();

            endAttackCooldown = controller.attackNormalDuration;

            Debug.Log("Atacou com Normal");
            controller.stateMachine.ChangeState(controller.idleState);

            controller.thisAnimator.SetTrigger("tAttackNormal");

            helper.StartStateCoroutine(ScheduleAttack(controller.attackNormalMagicDelay));
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

            var spawnTransform = controller.staffTop;
            var projectile = Object.Instantiate(controller.fireballPrefab, spawnTransform.position, spawnTransform.rotation);

            var projectileCollision = projectile.GetComponent<ProjectileCollision>();
            projectileCollision.attacker = controller.gameObject;
            projectileCollision.damage = controller.attackDamage;

            
            var player = GameManager.Instance.player;
            var projectileRigidbody = projectile.GetComponent<Rigidbody>(); 

            var vectorToPlayer = (player.transform.position + controller.aimOffset - spawnTransform.position).normalized;
            var forceVector = spawnTransform.rotation * Vector3.forward;
            forceVector = new Vector3(forceVector.x, vectorToPlayer.y, forceVector.z);
            forceVector *= controller.attackNormalImpulse;
            projectileRigidbody.AddForce(forceVector, ForceMode.Impulse);

            Object.Destroy(projectile, 30);


            
        }

        
    }
}