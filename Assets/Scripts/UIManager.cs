using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public Slider slider;
    public Text HPText;
    public Text timerText;
    public Text scoreText;
    public GameObject gameOverScreen;
    public GameObject gameClearScreen;

    private int maxHP;
    private float playTime;
    private IEnumerator timeCoroutine;

    public void init(int hp)
    {
        maxHP = hp;
        slider.value = 1f;
        slider.maxValue = 1f;
        HPText.text = $"HP: {hp}";
        timeCoroutine = Timer();
        StartCoroutine(timeCoroutine);
    }

    public void SetHP(int hp)
    {
        HPText.text = $"HP: {hp}";
        slider.value = (float)hp / maxHP;
    }
    
    public void RetryButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void GameClear(int itemCount, int hitCount)
    {
        int score = 0;
        StopCoroutine(timeCoroutine);

        //1초당 5점
        //시간 최대점수는 900점
        score = Math.Max((180 - (int)playTime) * 5 + itemCount * 150 - hitCount * 100, 0);
        var text = $"플레이 시간: {playTime:F1}초\n시간: {(180 - (int)playTime) * 5}점\n" +
            $"아이템 {itemCount}개: {itemCount * 150}점\n" +
            $"충돌 {hitCount}회: {hitCount * -100}점\n" +
            $"합계: {score}점";
        scoreText.text = text;
        gameClearScreen.SetActive(true);
    }

    private IEnumerator Timer()
    {
        playTime = 0f;
        while (true)
        {
            timerText.text = $"Time: {playTime:F1}s";
            playTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
