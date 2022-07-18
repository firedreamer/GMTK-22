using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using DG.Tweening;

public class PowerAimScript : MonoBehaviour
{
    //Singleton boilerplate
    private static PowerAimScript instance;
    public static PowerAimScript Instance { get => instance; }

    bool powerFlip = false;
    [SerializeField]
    Transform powerArrow;
    [SerializeField]
    Transform angleArrow;
    [SerializeField]
    Transform angleArrowEnemy;
    [SerializeField]
    float moveRate = 0.75f;
    bool directionSwitch = false;
    [SerializeField]
    CinemachineVirtualCamera playerBallCamera;
    [SerializeField]
    CinemachineVirtualCamera playerTableCamera;
    [SerializeField]
    CinemachineVirtualCamera enemyFaceCamera;
    [SerializeField]
    CinemachineVirtualCamera enemyTurnCamera;
    [SerializeField]TMP_Text _tmpProText;
    string writer;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;
    [SerializeField] GameObject enemyProjectile;

    float enemyYVal;
    bool enemyRotate;
    public void Init()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
    }

    public void Restart()
    {
        if(GameManager.Instance.isEnemyTurn)
        {
            powerFlip = false;
            angleArrow.GetComponent<MeshRenderer>().enabled = true;
            powerArrow.GetComponent<MeshRenderer>().enabled = false;
            transform.parent.gameObject.GetComponent<Projectile>().ResetToInitialState();
            enemyProjectile.GetComponent<Projectile>().ResetToInitialState();
            playerBallCamera.Priority = 10;
            enemyFaceCamera.Priority = 10;
            enemyTurnCamera.Priority = 10;
            playerTableCamera.Priority = 5000;
            
            StopAllCoroutines();
            //REPLACE WITH CHOICE LOGIC
            GameManager.Instance.playBall = true;
            GameManager.Instance.isEnemyTurn = false;
        }
        else
        {
            GameManager.Instance.playBall = false;
            powerFlip = false;
            angleArrow.GetComponent<MeshRenderer>().enabled = false;
            powerArrow.GetComponent<MeshRenderer>().enabled = false;
            transform.parent.gameObject.GetComponent<Projectile>().ResetToInitialState();
            playerBallCamera.Priority = 10;
            playerTableCamera.Priority = 10;
            enemyTurnCamera.Priority = 10;
            enemyFaceCamera.Priority = 100;
            enemyProjectile.GetComponent<Projectile>().ResetToInitialState();
            StartCoroutine(EnemyBehavior());
            GameManager.Instance.isEnemyTurn = true;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.playBall)
        {
            if (!GameManager.Instance.isEnemyTurn)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    powerFlip = true;
                    angleArrow.GetComponent<MeshRenderer>().enabled = false;
                    powerArrow.GetComponent<MeshRenderer>().enabled = true;
                }
                if (!powerFlip)
                {
                    float yVal = ((Input.mousePosition.x / 768f) - 0.5f) * 57f;
                    transform.localRotation = Quaternion.Euler(0, yVal, 0);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    powerFlip = false;
                    angleArrow.GetComponent<MeshRenderer>().enabled = false;
                    powerArrow.GetComponent<MeshRenderer>().enabled = false;
                    transform.parent.gameObject.GetComponent<Projectile>().Launch();
                    playerTableCamera.Priority = 10;
                    playerBallCamera.Priority = 100;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(powerFlip)
        {
            if(!directionSwitch)
            {
                if (powerArrow.transform.localPosition.z > -65)
                {
                    powerArrow.transform.Translate(Vector3.down * moveRate * Time.deltaTime);
                }
                else
                {
                    directionSwitch = true;
                }
            }
            else
            {
                if (powerArrow.transform.localPosition.z <= 0)
                {
                    powerArrow.transform.Translate(Vector3.up * moveRate * Time.deltaTime);
                }
                else
                {
                    directionSwitch = false;
                }
            }
        }
    }

    IEnumerator EnemyBehavior()
    {
        yield return new WaitForSeconds(1.5f);
        _tmpProText.text =  GameManager.Instance.EvaluateFlavorText();
        _tmpProText.transform.parent.GetComponent<CanvasGroup>().DOFade(1, 0.75f);
        _tmpProText.gameObject.GetComponent<typewriterUI>().Start();
        yield return new WaitForSeconds(7f);
        GameManager.Instance.ballHitPlayerCup = false;
        GameManager.Instance.hitSuccessful = false;
        GameManager.Instance.shielded = false;
        _tmpProText.text = GameManager.Instance.RandomizeTurnText();
        _tmpProText.gameObject.GetComponent<typewriterUI>().Start();
        yield return new WaitForSeconds(6.75f);
        _tmpProText.transform.parent.GetComponent<CanvasGroup>().DOFade(0, 0.75f);
        yield return new WaitForSeconds(1);
        enemyTurnCamera.Priority = 100;
        /*int turn = Random.Range(0, 3);
        switch (turn)
        {
            case 0:
                StartCoroutine(EnemyAim());
                break;
            case 1:
                GameManager.Instance.EnemyPlayShield();
                break;
            case 2:
                GameManager.Instance.PlayArenaMod();
                break;
            default:

                break;
        }*/
        yield return new WaitForSeconds(0.1f);
        float yVal = ((Random.Range(0f, 765f) / 768f) - 0.5f) * 57f;
        print(yVal);
        angleArrowEnemy.transform.rotation = Quaternion.Euler(0, yVal, 0);
        yield return new WaitForSeconds(0.65f);
        enemyProjectile.GetComponent<Projectile>().Launch();
    }
}
