using UnityEngine;
using EventArgs;

namespace Behaviours.LichBoss.States{
    public class Dead : State {

        private LichBossController controller;
        private LichBossHelper helper;

        public Dead(LichBossController controller) : base("Dead"){
            this.controller = controller;
            this.helper = controller.helper;

        }

        public override void Enter() {
            base.Enter();

            controller.thisLife.isVulnerable = false;

            controller.thisAnimator.SetTrigger("tDead");

            GlobalEvents.Instance.InvokeGameWon(this, new GameWonArgs());
        }

        public override void Exit(){
            base.Exit();

        }

        public override void Update(){
            base.Update();
        }

        public override void LateUpdate(){
            base.LateUpdate();
        }
    }
}