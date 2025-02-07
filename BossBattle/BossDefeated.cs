using UnityEngine;
using UnityEngine.Rendering;


namespace BossBattle {
  public class BossDefeated : State {


    public BossDefeated() : base("BossDefeated"){

    }

    public override void Enter(){
      base.Enter();
      
      var gameManager = GameManager.Instance;
      var boss = gameManager.boss;
      var sequencePrefab = gameManager.bossDeathSequence;
      Object.Instantiate(sequencePrefab, boss.transform.position, sequencePrefab.transform.rotation);
    }

    public override void Exit(){
      base.Exit();
    }
  }
}