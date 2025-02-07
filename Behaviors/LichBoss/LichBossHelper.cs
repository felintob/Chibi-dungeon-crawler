using System;
using System.Collections;
using System.Collections.Generic;
using Behaviours.LichBoss.States;
using EventArgs;
using UnityEngine;
using UnityEngine.AI;

namespace Behaviours.LichBoss{
    public class LichBossHelper {

        private LichBossController controller;

        public LichBossHelper(LichBossController controller){
            this.controller = controller;
        }
        
        public float GetDistanceToPlayer(){
            var player = GameManager.Instance.player;
            var playerPosition = player.transform.position;
            var origin = controller.transform.position;
            var positionDifference = playerPosition - origin;
            var distance = positionDifference.magnitude;
            return distance;
        }

        public bool HasLowHealth() {
            var life = controller.thisLife;
            float lifeRate = (float) life.health / (float) life.maxHealth;
            return lifeRate <= controller.lowHealthThreshold;
        }

        public void StartStateCoroutine(IEnumerator enumerator){
            controller.StartCoroutine(enumerator);
            controller.stateCoroutines.Add(enumerator);
        }

        public void ClearStateCoroutines(){
            foreach(var enumerator in controller.stateCoroutines){
                controller.StopCoroutine(enumerator);
            }
            controller.stateCoroutines.Clear();
        }


    }

}