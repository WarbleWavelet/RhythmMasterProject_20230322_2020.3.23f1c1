using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryEffect : MonoBehaviour {

    public RhythmGameController gameController;
    public bool isHitted;
    public float animationTime;



    private void OnEnable()
    {
        Invoke("ReturnToPool",animationTime);
    }


    void ReturnToPool()
    {
        if (isHitted)
        {
            gameController.ReturnEffectGoToPool(gameObject,gameController.hitEffectObjectPool);
            gameObject.SetActive(false);
        }
        else
        {
            gameController.ReturnEffectGoToPool(gameObject, gameController.downEffectObjectPool);
            gameObject.SetActive(false);
        }
    }
}
