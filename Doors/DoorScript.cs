using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventArgs;
using Item;

public class DoorScript : MonoBehaviour
{

    public Interaction interaction;
    public Item.Item requiredKey;
    private bool isOpen;

    private Animator thisAnimator;

    private void Awake(){
      thisAnimator = GetComponent<Animator>();
    }
    
    void Start()
    {
        interaction.OnInteraction += OnInteraction;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isOpen){
            var hasKey = false;
            if(requiredKey == null){
                hasKey = true;
            } else if(requiredKey.itemType == ItemType.Key){
                hasKey = GameManager.Instance.keys > 0;
            } else if(requiredKey.itemType == ItemType.BossKey) {
                hasKey = GameManager.Instance.hasBossKey;
            }

            interaction.SetAvailable(hasKey);
        }


    }

    private void OnInteraction(object sender, InteractionEventArgs args) {
      Debug.Log("Jogador acabou de interagir com a porta!");



      if(!isOpen){
        OpenDoor();
      } else {
        CloseDoor();
      }
      
    }

    private void OpenDoor(){
        isOpen = true;
        
        if(requiredKey != null){
            if(requiredKey.itemType == ItemType.Key){
                GameManager.Instance.keys--;
            } else if(requiredKey.itemType == ItemType.BossKey){
                GameManager.Instance.hasBossKey = false;
            }
        }

        
        var gameplayUI = GameManager.Instance.gameplayUI;
        gameplayUI.RemoveObject(requiredKey.itemType);

        

        interaction.SetAvailable(false);
        
        thisAnimator.SetTrigger("tOpen"); 

        var isBossDoor = requiredKey.itemType == ItemType.BossKey;
        if(isBossDoor){
            GlobalEvents.Instance.InvokeBossDoorOpen(this, new BossDoorOpenArgs());
        }

    }

    private void CloseDoor(){
        isOpen = false;
        
        
        
        thisAnimator.SetTrigger("tClose"); 

    }
}
