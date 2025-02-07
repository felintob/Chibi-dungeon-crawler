using UnityEngine;
using System;
using System.Collections;

namespace BossBattle {
  public class BossBattleHandler {
    
    public StateMachine stateMachine;
    public Disabled stateDisabled;
    public Battle stateBattle;
    public Intro stateIntro;
    public Waiting stateWaiting;
    public BossDefeated stateDefeated;
    public BossVictorious stateVictorious;

    public BossBattleHandler() {
      stateMachine = new StateMachine();
      stateDisabled = new Disabled();
      stateBattle = new Battle();
      stateIntro = new Intro();
      stateWaiting = new Waiting();
      stateDefeated = new BossDefeated();
      stateVictorious = new BossVictorious();
      stateMachine.ChangeState(stateDisabled);

      GameManager.Instance.bossBattleParts.SetActive(false);

      var globalEvents = GlobalEvents.Instance;
      globalEvents.OnBossDoorOpen += (sender, args) => stateMachine.ChangeState(stateWaiting);
      globalEvents.OnBossRoomEnter += (sender, args) => stateMachine.ChangeState(stateIntro);
      globalEvents.OnGameOver += (sender, args) => stateMachine.ChangeState(stateVictorious);
      globalEvents.OnGameWon += (sender, args) => stateMachine.ChangeState(stateDefeated);

    }

    public void Update(){
      stateMachine.Update();
    }

    public bool IsActive(){
      return stateMachine.currentStateName == stateBattle.name;
    }

    public bool IsInCutscene() {
      return stateMachine.currentStateName == stateIntro.name;
    }
  }
}