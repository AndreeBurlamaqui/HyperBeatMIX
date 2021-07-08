using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHit : MonoBehaviour
{
    public UIHandler _uiHandler;


    private GameObject hitOKPF, hitEarlyPF, hitLatePF, coinFlipPF;
    public bool isNoteAbove;
    public GameObject noteAbove;
    public Vector3 startPos, currentPos;

    private void Start()
    {
        hitOKPF = Resources.Load<GameObject>("Prefabs/HitOK");
        hitEarlyPF = Resources.Load<GameObject>("Prefabs/HitEarly");
        hitLatePF = Resources.Load<GameObject>("Prefabs/HitLate");
        coinFlipPF = Resources.Load<GameObject>("Prefabs/CoinFlip");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            isNoteAbove = true;
            noteAbove = collision.gameObject;
            startPos = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            isNoteAbove = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            currentPos = collision.transform.position;
        }
    }

    public void TestClick()
    {
        if (isNoteAbove)
        {
            float percent = GetPercentageAlong(startPos, Vector3.zero, currentPos);


            if (percent <= 0.25f)             //Early = percent <= 0.2f 

            {
                ResetStreak();
                GameObject pfGO = Instantiate(hitEarlyPF, noteAbove.transform.position, Quaternion.identity);
                pfGO.transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;

            }
            else if (0.25f < percent && percent < 0.6f) // Perfect = 0.2 > percent < 0.6;
            {
                _uiHandler.UpdateStreak();
                Instantiate(hitOKPF, noteAbove.transform.position, Quaternion.identity);

                if (noteAbove.GetComponent<Note>().hasCoin)
                {
                    _uiHandler.GoldBeatCollected();
                    Instantiate(coinFlipPF, noteAbove.transform.position, Quaternion.identity);
                }

            }
            else if (percent >= 0.6f)             //Late = percent >= 0.6f
            {
                ResetStreak();
                GameObject pfGO = Instantiate(hitLatePF, noteAbove.transform.position, Quaternion.identity);
                pfGO.transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;

            }

            _uiHandler.UpdateScore();
            DestroyWarningLine();

            Destroy(noteAbove);
        }
    }

    public void ResetStreak()
    {
        _uiHandler.StreakReset();

    }

    public void DestroyWarningLine()
    {
        _uiHandler.HideWarningLine();
    }

    public static float GetPercentageAlong(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;

        return Vector3.Dot(ac, ab) / ab.sqrMagnitude;
    }

}
