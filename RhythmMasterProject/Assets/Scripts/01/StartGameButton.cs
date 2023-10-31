using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour {



	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(StartGame);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
