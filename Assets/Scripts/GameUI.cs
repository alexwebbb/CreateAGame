﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUi;

	void Start () {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
	}

    void OnGameOver() {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUi.SetActive(true);
        Cursor.visible = true;
    }

    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
	
    // UI Input
    public void StartNewGame() {
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
