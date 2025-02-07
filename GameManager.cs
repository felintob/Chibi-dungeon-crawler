using System;
using System.Collections;
using System.Collections.Generic;
using BossBattle;
using Item;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {get; private set;}

    [HideInInspector] public bool isGameOver; 
    [HideInInspector] public bool isGameWon; 

    public GameObject player;
    [SerializeField] public List<Interaction> interactionList;

    [Header("Rendering")]
    public Camera worldUiCamera;

    [Header("Physics")]
    [SerializeField] public LayerMask groundLayer;

    [Header("Inventory")]
    public int keys;
    public bool hasBossKey;

    [Header("Boss")]
    public GameObject boss;
    public GameObject bossBattleParts;
    public BossBattleHandler bossBattleHandler;
    public GameObject bossDeathSequence;

    [Header("UI")]
    public GameplayUI gameplayUI;

    [Header("Music")]
    public AudioSource gameplayMusic;
    public AudioSource bossMusic;
    public AudioSource ambienceMusic;



    void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }

        

        
    }

    public void Start() {
        bossBattleHandler = new BossBattleHandler();

        var MusicTargetVolume = gameplayMusic.volume;
        gameplayMusic.volume = 0;
        gameplayMusic.Play();
        StartCoroutine(FadeAudioSource.StartFade(gameplayMusic, MusicTargetVolume, 1f));

        var ambienceTargetVolume = ambienceMusic.volume;
        ambienceMusic.volume = 0;
        ambienceMusic.Play();
        StartCoroutine(FadeAudioSource.StartFade(ambienceMusic, ambienceTargetVolume, 1f));

        GlobalEvents.Instance.OnGameOver += (sender, args) => isGameOver = true;
    }

    public void Update() {
        bossBattleHandler.Update();
        
    }

    

    

    

    
    
}
