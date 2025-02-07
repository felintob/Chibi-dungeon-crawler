using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours.LichBoss.States{
    public class AttackSuper : State {

        private LichBossController controller;
        private LichBossHelper helper;

        private float endAttackCooldown;

        public AttackSuper(LichBossController controller) : base("AttackSuper"){
            this.controller = controller;
            this.helper = controller.helper;

        }

        public override void Enter() {
            base.Enter();

            endAttackCooldown = controller.attackSuperDuration;

            Debug.Log("Atacou com Super");
            controller.stateMachine.ChangeState(controller.idleState);

            controller.thisAnimator.SetTrigger("tAttackSuper");

            for(int i = 0; i < controller.attackSuperMagicCount - 1; i++){
                var delayStep = controller.attackSuperMagicDuration / (controller.attackSuperMagicCount - 1);
                var delay = controller.attackSuperMagicDelay + delayStep * i;
            helper.StartStateCoroutine(ScheduleAttack(delay));
            }

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
            var projectile = Object.Instantiate(controller.energyBallPrefab, spawnTransform.position, spawnTransform.rotation);

            var projectileCollision = projectile.GetComponent<ProjectileCollision>();
            projectileCollision.attacker = controller.gameObject;
            projectileCollision.damage = controller.attackDamage;

            
            var player = GameManager.Instance.player;
            var projectileRigidbody = projectile.GetComponent<Rigidbody>(); 

            var vectorToPlayer = (player.transform.position + controller.aimOffset - spawnTransform.position).normalized;
            var forceVector = spawnTransform.rotation * Vector3.forward;
            forceVector = new Vector3(forceVector.x, vectorToPlayer.y, forceVector.z);
            forceVector *= controller.attackSuperImpulse;
            projectileRigidbody.AddForce(forceVector, ForceMode.Impulse);

            Object.Destroy(projectile, 30);
            
        }
    }
}