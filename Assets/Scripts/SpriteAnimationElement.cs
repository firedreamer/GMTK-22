using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpriteAnimationElement : MonoBehaviour
{
    public bool isLooped = true;
    [SerializeField]
    protected float loopIntervalTime = 0.5f;
    [SerializeField]
    private Image image;
    [SerializeField]
    private SpriteRenderer animSprite;
    [SerializeField]
    private MeshRenderer meshRenderer;
    protected Coroutine animationCorutine;
    private bool flipState;
    private bool updateEmission=false;
    protected Action<Sprite> SetSprite;

    private void Awake()
    {
        if (animSprite != null)
        {
            flipState = animSprite.flipX;
        }
        // No renderer item is set
        if (image == null && animSprite == null && meshRenderer == null)
        {
            image = this.GetComponent<Image>();
            if(image == null)
            {
                animSprite = this.GetComponent<SpriteRenderer>();
            }
            if(animSprite == null)
            {
                meshRenderer = this.GetComponent<MeshRenderer>();
            }
        }

        if (meshRenderer!=null && ((meshRenderer.material.shader == Shader.Find("Universal Render Pipeline/Lit") && meshRenderer.material.IsKeywordEnabled("_EMISSION"))))
        {
            updateEmission = true;
        }
    }

    public void Animate(List<Sprite> states, Action nextAnim = null, bool flip=false, float loopTime = 0, float stagger = -1)
    {
        if (animationCorutine != null)
        {
            StopCoroutine(animationCorutine);
            animationCorutine = null;
        }
        if (animSprite!=null && flip)
        {
            animSprite.flipX = !flipState;
        }
        if (!gameObject.activeSelf)
        {
            return;
        }

        AssignAnimationType();
        var time = Mathf.Approximately(loopTime, 0) ? loopIntervalTime : loopTime;
        animationCorutine = StartCoroutine(StateAnimation(time, states, nextAnim, stagger));
    }

    IEnumerator StateAnimation(float loopIntervalTime, List<Sprite> states, Action nextAnim = null, float stagger = -1)
    {
        if(SetSprite == null)
        {
            AssignAnimationType();
        }

        if (states.Count == 1)
        {
            SetSprite(states[0]);
            if (nextAnim != null)
            {
                nextAnim.Invoke();
            }
            yield break;
        }
        while (true)
        {
            if (stagger > 0 && !isLooped)
            {
                yield return new WaitForSeconds(stagger);
            }
            for (int i = 0; i < states.Count; i++)
            {
                SetSprite(states[i]);
                yield return new WaitForSeconds(loopIntervalTime);
            }
            if (nextAnim != null)
            {
                nextAnim.Invoke();
                break;
            }
            if (!isLooped)
                break;
            if (stagger > 0)
            {
                yield return new WaitForSeconds(stagger);
            }
        }
    }

    private void AssignAnimationType()
    {
        if (image != null)
        {
            SetSprite = (sprite) => image.sprite = sprite;
        }
        else if (animSprite != null)
        {
            SetSprite = (sprite) => animSprite.sprite = sprite;
        }
        else if (meshRenderer != null)
        {
            if (updateEmission)
            {
                SetSprite = ((sprite) =>
                {
                    meshRenderer.material.mainTexture = sprite.texture;
                    meshRenderer.material.SetTexture("_EmissionMap", sprite.texture);
                });
            }
            else
            {
                SetSprite = ((sprite) => meshRenderer.material.mainTexture = sprite.texture);
            }
            
        }
    }
}
