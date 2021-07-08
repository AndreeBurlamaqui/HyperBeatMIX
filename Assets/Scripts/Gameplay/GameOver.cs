using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using Doozy.Engine.Progress;
using TMPro;
public class GameOver : MonoBehaviour
{

    public UIHandler _uiHandler;
    public UIPopup gameOverPopup;
    public SpawnNote _spawnNote;
    public Progressor goldBeatProgressor;
    public TextMeshProUGUI timerText;
    public UIView timerView;
    public TextMeshProUGUI beatCoins;

    private float finalBeatScore, finalTimer;
    private int finalBeatGold;
    
    private void OnDestroy()
    {
        PlayerPrefs.SetInt("BeatBar", (int)goldBeatProgressor.Value);
    }

    public void Lose()
    {
        GameObject[] noteMiss = GameObject.FindGameObjectsWithTag("Note");

        foreach(GameObject nM in noteMiss)
        {
            Destroy(nM);
        }

        _spawnNote.isAlive = false;

        finalBeatScore = _uiHandler.beatPoints;
        finalBeatGold = (int)_uiHandler.goldBeat;
        finalTimer = _uiHandler.timer;

        PlayerPrefs.SetInt("BeatCoins" ,finalBeatGold + PlayerPrefs.GetInt("BeatCoins"));

        gameOverPopup.Show();
    }

    IEnumerator GameOverScreen()
    {
        //Barra de beatcoins
        int playerprefsbar = PlayerPrefs.GetInt("BeatBar");
        Debug.Log(playerprefsbar);
        goldBeatProgressor.InstantSetValue(playerprefsbar);

        yield return new WaitForSeconds(1f);

        float newBarValue = finalBeatScore + playerprefsbar;
        goldBeatProgressor.SetValue(newBarValue);

        //Beatcoins
        StartCoroutine(UpdateBeatCoins());


        //Timer
        timerView.Show();
        int minutes = Mathf.FloorToInt(finalTimer / 60F);
        int seconds = Mathf.FloorToInt(finalTimer - minutes * 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);



    }

    public void HowMuchGoldBeat()
    {
        StartCoroutine(GameOverScreen());
    }

    IEnumerator UpdateBeatCoins()
    {
        beatCoins.text = PlayerPrefs.GetInt("BeatCoins").ToString();
        beatCoins.transform.parent.GetComponent<UIView>().StartLoopAnimation();

        yield return new WaitForSeconds(0.4f);
        beatCoins.transform.parent.GetComponent<UIView>().StopLoopAnimation();
    }

    public void BeatBarProgression()
    {
        float progress = goldBeatProgressor.Progress;

        if (progress >= 1)
        {
            goldBeatProgressor.ResetValueTo(ResetValue.ToMinValue);
            finalBeatScore -= goldBeatProgressor.MaxValue;
            goldBeatProgressor.SetValue(finalBeatScore);
            Debug.Log(finalBeatScore);

            PlayerPrefs.SetInt("BeatCoins", 1 + PlayerPrefs.GetInt("BeatCoins"));

            StartCoroutine(UpdateBeatCoins());

        }
        

            //PlayerPrefs.SetInt("BeatBar", (int)goldBeatProgressor.Value);
        
    }
}
