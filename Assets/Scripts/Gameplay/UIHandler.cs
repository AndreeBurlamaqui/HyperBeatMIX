using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public float beatMultiplier, streakMultiplier;
    public TextMeshProUGUI score, timePlayed, streak;
    public UIView streakView;
    public AnimationCurve warningLineCurve, warningLineCurveHide;
    public float warningLineLerp, warningLerpSpeed;


    private RectTransform warningLineActionShow, warningLineActionHide;
    private bool isHiding, isShowing;
    [HideInInspector]public float beatPoints = 0, streakPoints = 0, goldBeat = 0, timer;
    [SerializeField] private float streakShowTime;
    private List<int> spawnedNotes = new List<int>();

    [SerializeField] private Button[] noteButton = new Button[7];
    private GameObject[] noteWarning = new GameObject[7];
    // 0 - Down
    // 1 - Down Right
    // 2 - Right
    // 3 - Up Right
    // 4 - Up Left
    // 5 - Left
    // 6 - Down Left

    [HideInInspector] public float streakHit = 0;

    private void Start()
    {
        streakView.AutoHideAfterShowDelay = streakShowTime;
        FirstButtons();

        for(int x = 0; x < noteButton.Length; x++)
        {
            noteWarning[x] = noteButton[x].transform.GetChild(0).gameObject;
            //noteWarning[x].SetActive(false);
        }
    }


    public void FirstButtons()
    {
        //{ 0, 3, 4 };
        for (int x = 0; x < 7; x++)
        {
            if (x == 0 || x == 3 || x == 4)
            {
                noteButton[x].interactable = true;
            }
            else
            {
                noteButton[x].interactable = false;
            }
        }

        Debug.Log("NEW BUTTONS");
    }

    public void SecondButtons()
    {
        //{ 0, 1, 3, 4, 6};
        for (int x = 0; x < 7; x++)
        {
            if (x == 0 || x == 1 || x == 3 || x == 4 || x == 6)
            {
                noteButton[x].interactable = true;
            }
            else
            {
                noteButton[x].interactable = false;
            }
        }
        Debug.Log("NEW BUTTONS");

    }


    public void ThirdButtons()
    {
        //everyone
        for (int x = 0; x < 7; x++)
        {
            noteButton[x].interactable = true;
        }
        Debug.Log("NEW BUTTONS");


    }

    public void UpdateTimePlayed(float timeNow)
    {
        timer = timeNow;

        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);

        timePlayed.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateScore()
    {
        beatPoints += beatMultiplier + (streakMultiplier * streakPoints);

        score.text = "" + beatPoints.ToString("00000");
    }

    public void GoldBeatCollected()
    {
        goldBeat += 1;
    }

    public void UpdateStreak()
    {

        streakPoints += 1;

        if (streakPoints < 50)
        {
            if (streakPoints % 5 == 0)
            {

                streakView.Show();
            }
        }
        else 
        {
            if (streakPoints % 10 == 0 && streakPoints > 0)
            {

                streakView.Show();
            }
        }
        
        if (!streakView.IsHidden)
        {
            streak.text = "" + streakPoints.ToString("00");
        }
    }

    public void StreakReset()
    {

        if (streakPoints > 0)
        {
            streakView.Show();
        }


        streakPoints = 0;
        streak.text = "" + streakPoints.ToString("00");

    }

    private void Update()
    {
        // 0 to 1 = show
        // 1 to 0 = hide

        if (isHiding)
        {

                warningLineLerp -= Time.deltaTime * warningLerpSpeed;
                warningLineActionHide.anchoredPosition = new Vector3(0, warningLineCurveHide.Evaluate(warningLineLerp), 0);

            if(warningLineLerp <= 0)
            {
                isHiding = false;
                warningLineActionHide = null;
            }
            
        }
        
        if (isShowing)
        {

                warningLineLerp += Time.deltaTime * warningLerpSpeed;
                warningLineActionShow.anchoredPosition = new Vector3(0, warningLineCurve.Evaluate(warningLineLerp), 0);

            if(warningLineLerp >= 1)
            {
                isShowing = false;
                warningLineActionShow = null;
            }

        }
    }

    public void ShowWarningLine(int whichOne)
    {
        warningLineLerp = 0f;
        isShowing = true;
        warningLineActionShow = noteWarning[whichOne].GetComponent<RectTransform>();
        spawnedNotes.Add(whichOne);
    }

    public void HideWarningLine()
    {
        if (spawnedNotes.Count > 1)
        {
            List<int> resultNotes = spawnedNotes.FindAll(x => x == spawnedNotes[0]);

            if (resultNotes.Count <= 1)
            {
                warningLineLerp = 1f;
                isHiding = true;
                warningLineActionHide = noteWarning[spawnedNotes[0]].GetComponent<RectTransform>();
            }

        }
        else
        {
            warningLineLerp = 1f;
            isHiding = true;
            warningLineActionHide = noteWarning[spawnedNotes[0]].GetComponent<RectTransform>();
        }


        spawnedNotes.RemoveAt(0);
    }
}