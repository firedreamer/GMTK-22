using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

[System.Serializable]
public class AnimateObject
{
    public string objectIdentifier;
    public SpriteAnimationElement stateObject;
    public List<AnimateState> animateStates;
}

[System.Serializable]
public class AnimateState
{
    public string stateIdentifier;
    public List<Sprite> states;
}

public class SpriteAnimationHandler : MonoBehaviour
{
    public List<AnimateObject> animateObjects;

    public void SetState(string objectID, string stateID, Action playNextAnim = null, bool flip = false, float loopTime = 0, float stagger = -1)
    {
        var animateObject = animateObjects.Where(x => x.objectIdentifier.Equals(objectID));
        if (animateObject != null && animateObject.Count() > 0)
        {
            AnimateObject tempObj = animateObject.First();
            var stateObject = tempObj.animateStates.Where(x => x.stateIdentifier.Equals(stateID));

            if (stateObject != null && stateObject.Count() > 0)
            {
                AnimateState animateState = stateObject.First();
                tempObj.stateObject.Animate(animateState.states, playNextAnim, flip, loopTime, stagger);
            }
        }
    }

    public bool CheckStateExist(string objectID, string stateID)
    {
        bool isExist = false;
        var animateObject = animateObjects.Where(x => x.objectIdentifier.Equals(objectID));
        if (animateObject != null && animateObject.Count() > 0)
        {
            AnimateObject tempObj = animateObject.First();
            for (int i = 0; i < tempObj.animateStates.Count; i++)
            {
                if (tempObj.animateStates[i].stateIdentifier == stateID)
                {
                    isExist = true;
                }
            }
        }
        return isExist;
    }

    public List<string> GetAllStates(string objectID)
    {
        List<string> ids = new List<string>();
        var animateObject = animateObjects.Where(x => x.objectIdentifier.Equals(objectID));
        if (animateObject != null && animateObject.Count() > 0)
        {
            AnimateObject tempObj = animateObject.First();
            for (int i = 0; i < tempObj.animateStates.Count; i++)
            {
                ids.Add(tempObj.animateStates[i].stateIdentifier);
            }
        }
        return ids;
    }

    public void AnimateAllObjects(string stateID)
    {
        foreach (var obj in animateObjects)
        {
            foreach (var state in obj.animateStates)
            {
                if (state.stateIdentifier==stateID)
                {
                    obj.stateObject.Animate(state.states);
                    break;
                }
            }
        }
    }

    public Transform GetObjectTransform(string objectID)
    {
        var animateObject = animateObjects.Where(x => x.objectIdentifier.Equals(objectID));
        if (animateObject != null && animateObject.Count() > 0)
        {
            AnimateObject tempObj = animateObject.First();
            return tempObj.stateObject.transform;
        }
        else
        {
            return null;
        }
    }
}
