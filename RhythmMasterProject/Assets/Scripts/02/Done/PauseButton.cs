using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour {

    public Sprite[] sprites;

    public RhythmGameController rhythmGameController;

    /// <summary>实际就是暂停按钮</summary>
    Button button;

    Image image;

    // Use this for initialization
    void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(PauseOrPlayMusic);
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {

    }

    void PauseOrPlayMusic()
    {
        rhythmGameController.isPauseState = !rhythmGameController.isPauseState;
        if (rhythmGameController.isPauseState)
        {
            image.sprite = sprites[1];
            rhythmGameController.PauseMusic();
        }
        else
        {
            image.sprite = sprites[0];
            rhythmGameController.PlayMusic();
        }
    }
}
