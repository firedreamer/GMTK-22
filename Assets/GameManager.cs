using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    //Singleton boilerplate
    private static GameManager instance;
    public static GameManager Instance { get => instance; }
    public bool isEnemyTurn = false;
    public PowerAimScript powerAimScript;
    public SpriteAnimationHandler enemyWizard;

    public bool ballHitPlayerCup = false;
    public bool hitSuccessful = false;
    public bool shielded = false;

    public bool playBall = false;
    
    public int playerHP = 3;
    public int enemyHP = 3;

    public string[] missFlavorText;
    public string[] hitFlavorText;
    public string[] turnFlavorText;
    public string[] selfFlavorText;
    public string[] shieldFlavorText;
    public string[] diceFlavorText;

    public GameObject[] playerShield;
    public GameObject[] enemyShield;
    public CinemachineVirtualCamera mainMenuCamera;
    public CanvasGroup menuUI;
    public CanvasGroup winUI;
    public CanvasGroup loseUI;

    public GameObject playerBall;
    public GameObject enemyBall;
    public AudioSource menuSource;
    public AudioClip winClip;
    public AudioClip loseClip;


    bool win = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        powerAimScript.Init();

        enemyWizard.SetState("Enemy", "Idle");

    }

    // Update is called once per frame
    void Update()
    {
        if(!win)
        {
            if(enemyHP <=0)
            {
                win = true;
                Destroy(playerBall);
                Destroy(enemyBall);
                mainMenuCamera.Priority = 200000;
                winUI.DOFade(1, 0.75f);
                menuSource.PlayOneShot(winClip);
                StartCoroutine(DelayedEndGame());
            }
            else if(playerHP <=0)
            {
                win = true;
                Destroy(playerBall);
                Destroy(enemyBall);
                mainMenuCamera.Priority = 200000;
                loseUI.DOFade(1, 0.75f);
                menuSource.PlayOneShot(loseClip);
                StartCoroutine(DelayedEndGame());
            }
        }
        
    }

    public string EvaluateFlavorText()
    {
        if(!isEnemyTurn && ballHitPlayerCup)
        {
            return selfFlavorText[Random.Range(0, selfFlavorText.Length)];
        }
        else if (hitSuccessful)
        {
            return hitFlavorText[Random.Range(0, hitFlavorText.Length)];
        }
        else if (shielded)
        {
            return shieldFlavorText[Random.Range(0, shieldFlavorText.Length)];
        }
        else
        {
            //logic here for random flavortext
            return missFlavorText[Random.Range(0, missFlavorText.Length)];
        }
    }
    public string RandomizeTurnText()
    {
        return turnFlavorText[Random.Range(0, turnFlavorText.Length)];
    }
    public string RandomizeDiceText()
    {
        return diceFlavorText[Random.Range(0, diceFlavorText.Length)];
    }

    public void PlayBallAttack()
    {
        playBall = true;

    }
    public void PlayShield()
    {
        playerShield[Random.Range(0, playerShield.Length)].GetComponent<MeshRenderer>().material.DOFade(1, 0.75f);
    }
    public void PlayArenaMod()
    {

    }
    public void EnemyPlayShield()
    {

    }

    public void StartGame()
    {
        
        mainMenuCamera.Priority = -10;
        menuUI.DOFade(0, 0.75f);
        menuUI.blocksRaycasts = false;
        menuUI.interactable = false;
        StartCoroutine(DelayedActivate());
    }

    IEnumerator DelayedActivate()
    {
        yield return new WaitForSeconds(2.5f);
        PlayBallAttack();
    }
    IEnumerator DelayedEndGame()
    {
        yield return new WaitForSeconds(3.75f);
        Application.LoadLevel(0);
    }
}
