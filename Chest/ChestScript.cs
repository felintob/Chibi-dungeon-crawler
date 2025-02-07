using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EventArgs;
using Item;

namespace Chest{

  public class ChestScript : MonoBehaviour {


    public Interaction interaction;
    public GameObject itemHolder;
    public Item.Item item;

    public ChestOpenEvent onOpen = new();

    private Animator thisAnimator;

    private void Awake(){
      thisAnimator = GetComponent<Animator>();
    }

    private void Start() {
      interaction.OnInteraction += OnInteraction;
    }
    
    private void Update() {

    }

    private void OnInteraction(object sender, InteractionEventArgs args) {
      Debug.Log("Jogador acabou de interagir com o baú, que contem " + item.displayName);
      interaction.SetAvailable(false);
      thisAnimator.SetTrigger("tOpen"); 

      var itemObjectPrefab = item.objectPrefab;
      var position = itemHolder.transform.position;
      var rotation = itemObjectPrefab.transform.rotation;
      var itemObject = Instantiate(itemObjectPrefab, position, rotation);
      itemObject.transform.localScale = new Vector3(2, 2, 2);
      itemObject.transform.SetParent(itemHolder.transform);

      var itemType = item.itemType;
      if(itemType == ItemType.Key){
        GameManager.Instance.keys++;
      } else if(itemType == ItemType.BossKey){
        GameManager.Instance.hasBossKey = true;
      } else if(itemType == ItemType.Potion) {
        var player = GameManager.Instance.player;
        var playerLife = player.GetComponent<LifeScript>();
        playerLife.Heal();
      }

      onOpen?.Invoke(gameObject);

      var gameplayUI = GameManager.Instance.gameplayUI;
      gameplayUI.AddObject(itemType);
    }

    }

    [Serializable] public class ChestOpenEvent : UnityEvent<GameObject> {}
  }
