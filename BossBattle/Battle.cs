using UnityEngine;
using System;
using System.Collections;

namespace BossBattle {
  public class Battle : State {

    public Battle() : base("Battle"){

    }

    public override void Enter(){
      base.Enter();

      var gameplayUI = GameManager.Instance.gameplayUI;
      gameplayUI.ToggleBossBar(true);
      var boss = GameManager.Instance.boss;
      var bossLife = boss.GetComponent<LifeScript>();
      gameplayUI.bossHealthBar.SetMaxHealth(bossLife.maxHealth);
    }

    public override void Exit(){
      base.Exit();
      var gameplayUI = GameManager.Instance.gameplayUI;
        gameplayUI.ToggleBossBar(false);
    }

    public override void Update(){
      base.Update();
      var gameplayUI = GameManager.Instance.gameplayUI;
      var boss = GameManager.Instance.boss;
      var bossLife = boss.GetComponent<LifeScript>();
      gameplayUI.bossHealthBar.SetHealth(bossLife.health);
    }
  }
}