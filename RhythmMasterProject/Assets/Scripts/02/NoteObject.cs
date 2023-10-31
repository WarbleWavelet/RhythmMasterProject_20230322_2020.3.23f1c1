using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;


/// <summary>音符，绿色，蓝色，透明的音符</summary>
public class NoteObject : MonoBehaviour 
{
    #region 字属

    RhythmGameController gameController;
    LaneController laneController;

    /// <summary>音符身上挂的组件</summary>
    public SpriteRenderer visuals;

    /// <summary>不同轨道（6个）上音符的图像，和透明、蓝色、绿色三种音符（蓝色到透明=长按的开始和结束，绿色=短按）</summary>
    public Sprite[] noteSprites;

    KoreographyEvent trackedEvent;

    /// <summary>长按开始</summary>
    public bool isLongNote;
    /// <summary>长按结束</summary>
    public bool isLongNoteEnd;

    public int hitOffset;


    #endregion



    #region 生命


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameController.isPauseState)
        {
            return;
        }

        UpdatePosition();
        GetHitOffset();
        if (transform.position.z<=laneController.targetBottomTrans.position.z)
        {
            gameController.ReturnNoteObjectToPool(this);
            ResetNote();
        }
    }
    #endregion

    #region 辅助


    //初始化方法
    public void Initialize(KoreographyEvent evt,
        int noteNum,LaneController laneCont,
        RhythmGameController gameCont,
        bool isLongStart,
        bool isLongEnd)
    {
        trackedEvent = evt;
        laneController = laneCont;
        gameController = gameCont;
        isLongNote = isLongStart;
        isLongNoteEnd = isLongEnd;
        int spriteNum = noteNum;
        if (isLongNote)
        {
            spriteNum+=6;
        }
        else if (isLongNoteEnd)
        {
            spriteNum += 12;
        }
        visuals.sprite = noteSprites[spriteNum - 1];
    }

    //将Note对象重置
    private void ResetNote()
    {


    }


    /// <summary>返回对象池</summary>
    void ReturnToPool()
    {
        gameController.ReturnNoteObjectToPool(this);
        ResetNote();
    }


    /// <summary>击中音符对象</summary>
    public void OnHit()
    {
        ReturnToPool();
    }


    /// <summary>更新位置的方法</summary>
    void UpdatePosition()
    {
        Vector3 pos = laneController.TargetPosition;

        pos.z -= (gameController.DelayedSampleTime - trackedEvent.StartSample) / (float)gameController.SampleRate * gameController.noteSpeed;

        transform.position = pos;
    }


    void GetHitOffset()
    {
        int curTime = gameController.DelayedSampleTime;
        int noteTime = trackedEvent.StartSample;
        int hitWindow = gameController.HitWindowSampleWidth;
        hitOffset = hitWindow - Mathf.Abs(noteTime-curTime);
    }

    /// <summary>当前音符是否已经Miss</summary>
    public bool IsNoteMissed()
    {
        bool bMissed = true;
        if (enabled)
        {
            int curTime = gameController.DelayedSampleTime;
            int noteTime = trackedEvent.StartSample;
            int hitWindow = gameController.HitWindowSampleWidth;

            bMissed = curTime - noteTime > hitWindow;
        }
        return bMissed;
    }


    /// <summary>音符的命中等级</summary>
    public int IsNoteHittable()
    {
        int hitLevel = 0;
        if (hitOffset>=0)
        {
            if (hitOffset>=2000&&hitOffset<=9000)
            {
                hitLevel = 2;
            }
            else
            {
                hitLevel = 1;
            }
        }
        else
        {
            this.enabled = false;
        }

        return hitLevel;
    }
    #endregion

}
