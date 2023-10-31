using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;




/// <summary>轨道控制器(在按键上)</summary>
public class LaneController : MonoBehaviour 
{

    #region 字属



    RhythmGameController gameController;

    [Tooltip("此音轨使用的键盘按键，默认SDF JKL")]
    public KeyCode keyboardButton;

    [Tooltip("此音轨对应事件的编号")]
    public int laneID;


    #region 音符的3个位置


    /// <summary>对“目标”位置的键盘按下的视觉效果</summary>
    public Transform targetVisuals;

    //上下边界
    /// <summary>音符生成的位置</summary>
    public Transform targetTopTrans;

    /// <summary>音符销毁的位置</summary>
    public Transform targetBottomTrans;
    #endregion


    /// <summary>包含在此音轨中的所有事件列表</summary> 
    List<KoreographyEvent> laneEvents = new List<KoreographyEvent>();

    /// <summary>包含此音轨当前活动的所有音符对象的队列</summary> 
    Queue<NoteObject> trackedNotes = new Queue<NoteObject>();

    /// <summary>检测此音轨中的生成的下一个事件的索引</summary>
    int pendingEventIdx = 0;

    /// <summary>也就是按键</summary>
    public GameObject downVisual;
    /// <summary>长音符击中特效</summary>
    public GameObject longNoteHitEffectGo;

    /// <summary>音符移动的目标位置</summary>
    public Vector3 TargetPosition 
    {
        get
        {
            return transform.position;
        }
    }

    public bool hasLongNote;

    public float timeVal = 0;


    #endregion



    #region 生命

    // Update is called once per frame
    void Update() {

        if (gameController.isPauseState)
        {
            return;
        }

        while (trackedNotes.Count>0&&trackedNotes.Peek().IsNoteMissed())//清除无效音符
        {
            if (trackedNotes.Peek().isLongNoteEnd)
            {
                hasLongNote = false;
                timeVal = 0;
                downVisual.SetActive(false);
                longNoteHitEffectGo.SetActive(false);
            }
            gameController.comboNum = 0;
            gameController.HideComboNumText();
            gameController.ChangeHitLevelSprite(0);
            gameController.UpdateHP();
            trackedNotes.Dequeue();
        }

        
        CheckSpawnNext();//检测新音符的产生
        
        if (Input.GetKeyDown(keyboardButton))//检测玩家的输入
        {
            CheckNoteHit();
            downVisual.SetActive(true);
        }
        else if (Input.GetKey(keyboardButton))
        {
            
            if (hasLongNote)//检测长音符
            {
                if (timeVal>=0.15f)
                {
                    
                    if (!longNoteHitEffectGo.activeSelf)//显示命中等级（Great Perfect）
                    {
                        gameController.ChangeHitLevelSprite(2);
                        CreateHitLongEffect();
                    }
                    timeVal = 0;
                }
                else
                {
                    timeVal += Time.deltaTime;
                }
            }
        }
        else if (Input.GetKeyUp(keyboardButton))
        {
            downVisual.SetActive(false);
            
            if (hasLongNote)//检测长音符
            {
                longNoteHitEffectGo.SetActive(false);
                CheckNoteHit();
            }
        }

    }
    #endregion

    #region 辅助


    //初始化
    public void Initialize(RhythmGameController controller)
    {
        gameController = controller;
    }



    //检测事件是否匹配当前编号的音轨
    public bool DoesMatch(int noteID)
    {
        return noteID == laneID;
    }



    //如果匹配，则把当前事件添加进音轨所持有的事件列表
    public void AddEventToLane(KoreographyEvent evt)
    {
        laneEvents.Add(evt);
    }



    //音符在音谱上产生的位置偏移量
    int GetSpawnSampleOffset()
    {
        //出生位置与目标点的位置。x，y值不变
        float spawnDistToTarget = targetTopTrans.position.z - transform.position.z;

        //到达目标点的时间
        float spawnPosToTargetTime = spawnDistToTarget / gameController.noteSpeed;

        return (int)spawnPosToTargetTime * gameController.SampleRate;
    }



    //检测是否生成下一个新音符
    void CheckSpawnNext()
    {
        int samplesToTarget = GetSpawnSampleOffset();

        int currentTime = gameController.DelayedSampleTime;

        while (pendingEventIdx < laneEvents.Count
            && laneEvents[pendingEventIdx].StartSample < currentTime + samplesToTarget)
        {
            KoreographyEvent evt = laneEvents[pendingEventIdx];
            int noteNum = evt.GetIntValue();
            NoteObject newObj = gameController.GetFreshNoteObject();
            bool isLongNoteStart = false;
            bool isLongNoteEnd = false;
            if (noteNum > 6)
            {
                isLongNoteStart = true;
                noteNum = noteNum - 6;
                if (noteNum > 6)
                {
                    isLongNoteEnd = true;
                    isLongNoteStart = false;
                    noteNum = noteNum - 6;
                }
            }
            newObj.Initialize(evt, noteNum, this, gameController, isLongNoteStart, isLongNoteEnd);
            trackedNotes.Enqueue(newObj);
            pendingEventIdx++;
        }
    }



    #region 3个特效



    /// <summary>
    /// 生成特效的有关方法
    /// </summary>
    void CreateDownEffect()
    {
        GameObject downEffectGo = gameController.GetFreshEffectObject(gameController.downEffectObjectPool, gameController.downEffectGo);
        downEffectGo.transform.position = targetVisuals.position;
    }



    void CreateHitEffect()
    {
        GameObject hitEffectGo = gameController.GetFreshEffectObject(gameController.hitEffectObjectPool, gameController.hitEffectGo);
        hitEffectGo.transform.position = targetVisuals.position;
    }


    void CreateHitLongEffect()
    {
        longNoteHitEffectGo.SetActive(true);
        longNoteHitEffectGo.transform.position = targetVisuals.position;
    }
    #endregion




    /// <summary>
    /// 检测是否有击中音符对象<para />
    /// 如果是，它将执行命中并删除
    /// </summary>
    public void CheckNoteHit()
    {
        if (!gameController.gameStart)
        {
            CreateDownEffect();
            return;
        }

        if (trackedNotes.Count>0)//轨道上有音符
        {
            NoteObject noteObject = trackedNotes.Peek();
            if (noteObject.hitOffset>-6000)
            {
                trackedNotes.Dequeue();
                int hitLevel= noteObject.IsNoteHittable();
                gameController.ChangeHitLevelSprite(hitLevel);
                if (hitLevel>0)
                {
                    //更新分数
                    gameController.UpdateScoreText(100 * hitLevel);
                    if (noteObject.isLongNote)
                    {
                        hasLongNote = true;
                        CreateHitLongEffect();
                    }
                    else if (noteObject.isLongNoteEnd)
                    {
                        hasLongNote = false;
                    }
                    else
                    {
                        CreateHitEffect();
                    }

                    //增加连接数
                    gameController.comboNum++;
                }
                else
                {
                    //未击中
                    //减少玩家HP
                    gameController.UpdateHP();
                    //断掉玩家命中连接数
                    gameController.HideComboNumText();
                    gameController.comboNum = 0;
                }
                noteObject.OnHit();
            }
            else
            {
                CreateDownEffect();
            }
        }
        else
        {
            CreateDownEffect();
        }
    }
    #endregion
}
