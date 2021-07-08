using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public ButtonHit lastNote;
    public bool hasCoin;

    private bool isDead = false;

    void Update()
    {
        //rb.AddForce(Vector3.up * speed * Time.deltaTime);

        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ButtonHit"))
        {

            lastNote = collision.gameObject.GetComponent<ButtonHit>();
        }

        if (collision.CompareTag("Goal"))
        {
            if (!isDead)
            {
                Debug.Log("Missed Note");
                lastNote.ResetStreak();
                lastNote.DestroyWarningLine();
                isDead = true;

                GameObject.FindWithTag("GameOver").GetComponent<GameOver>().Lose();

                Destroy(gameObject);
            }
        }
    }
}
