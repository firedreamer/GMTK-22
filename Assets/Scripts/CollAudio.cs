using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollAudio : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip collClip;
    [SerializeField] AudioClip[] randomCollectionClip;
    [SerializeField] bool randomize;
    [SerializeField] bool isScoreColl;
    [SerializeField] GameObject florishFX;
    public bool isPlayerCup;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(randomize)
        {
            audioSource.PlayOneShot(randomCollectionClip[Random.Range(0,randomCollectionClip.Length)]);
        }
        else
        {
            audioSource.PlayOneShot(collClip);
        }
        if(isScoreColl)
        {
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collision.transform.position = transform.position;
            florishFX.SetActive(true);
            StartCoroutine(DelayAndRestart(3f));
            if(isPlayerCup)
            {
                GameManager.Instance.ballHitPlayerCup = true;
                GameManager.Instance.playerHP--;
            }
            else
            {
                GameManager.Instance.hitSuccessful = true;
                GameManager.Instance.enemyHP--;
            }
        }
    }

    IEnumerator DelayAndRestart(float delay)
    {
        yield return new WaitForSeconds(delay);
        florishFX.SetActive(false);
        PowerAimScript.Instance.Restart();
        Destroy(transform.parent.gameObject);
    }
}
