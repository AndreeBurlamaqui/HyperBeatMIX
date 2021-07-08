using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNote : MonoBehaviour
{
    [Header("Public Variables")]

    public float actualNoteSpeed;
    public int multipleNotes;
    public float totalTimePlayed = 0;
    public UIHandler _uiHandler;

    [HideInInspector] public bool isAlive = true;

    [Header("Private Variables")]
    
    [SerializeField] private Transform[] spawnNoteLocation = new Transform[7];
    // 0 - Down
    // 1 - Down Right
    // 2 - Right
    // 3 - Up Right
    // 4 - Up Left
    // 5 - Left
    // 6 - Down Left

    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float startNoteSpeed;
    [SerializeField] private float maximumNoteSpeed;


    [Header("Difficulties Variables")]

    private float tpNoteVelMultiplier; // multiplicador de Velocidade das notas
    [SerializeField] private float tpNoteDivisor; //divisor pra velocidade das notas crescer lerdo mas aumentar conforme o tempo
    [SerializeField] private float tpCooldownSpawnMultiplier; // cooldown de spawn
    [SerializeField] private float lastDiffCooldown; // cooldown de spawn ultima dificuldade
    private bool sndDiffActivated = false, lastDiffActivated = false;
    public Vector3 tpButtonDifficulty; // x = primeira dificuldade , y = segunda dificuldade , z = terceira dificuldade
    [SerializeField] private float noteCooldownSpawn, spawnNoteTimer = 0; //Cooldown

    private int buttonRandom; //Dificuldade por botoes


    [Header("Coin System")]

    [SerializeField] private float percentageCoin = 0;
    [SerializeField] private Vector2 luckVariable;

    void Start()
    {
        noteCooldownSpawn = tpCooldownSpawnMultiplier;
        actualNoteSpeed = startNoteSpeed;
    }

    private void Update()
    {
        if (isAlive)
        {
            totalTimePlayed += Time.deltaTime;
            tpNoteVelMultiplier += Time.deltaTime;

            CooldownDifficultyIncreaser();
            SpawnTimer();
        }
    }

    private void FixedUpdate()
    {
        if(isAlive)
            _uiHandler.UpdateTimePlayed(totalTimePlayed);
    }

    public void SpawnTimer()
    {
        spawnNoteTimer += Time.deltaTime;

        if(spawnNoteTimer >= noteCooldownSpawn)
        {
            spawnNoteTimer = 0;

            SpawnNewNote();
        }
    }
    public void SpawnNewNote()
    {

        if(totalTimePlayed <= tpButtonDifficulty.x)
        {
            int[] noteButton = new int[] { 0, 3, 4 };
            int newRandom = Random.Range(0,noteButton.Length);

            buttonRandom = noteButton[newRandom];
        }
        else if(totalTimePlayed <= tpButtonDifficulty.y)
        {
            int[] noteButton = new int[] { 0, 1, 3, 4, 6};
            int newRandom = Random.Range(0, noteButton.Length);

            buttonRandom = noteButton[newRandom];

            if(!sndDiffActivated)
            {
                sndDiffActivated = true;

                tpNoteVelMultiplier = 20f;

                _uiHandler.SecondButtons();
                luckVariable *= 2;
            }

        }
        else if(totalTimePlayed <= tpButtonDifficulty.z)
        {
            int newRandom = Random.Range(0, 6);
            buttonRandom = newRandom;

            if (!lastDiffActivated)
            {
                lastDiffActivated = true;

                tpCooldownSpawnMultiplier *= lastDiffCooldown;

                tpNoteVelMultiplier = totalTimePlayed / 2f;


                _uiHandler.ThirdButtons();
                luckVariable *= 2;
            }

        }

        _uiHandler.ShowWarningLine(buttonRandom);

        Transform newNoteLocation = spawnNoteLocation[buttonRandom];
        Quaternion newNoteRotation = spawnNoteLocation[buttonRandom].rotation;
        GameObject newNoteGO = Instantiate(notePrefab, newNoteLocation.position, newNoteRotation);
        newNoteGO.GetComponent<Note>().speed = actualNoteSpeed;

        //Coin or not
        luckVariable.y = Mathf.Clamp(_uiHandler.streakPoints,1,100);
        float randomLuck = Random.Range(luckVariable.x, luckVariable.y) * 0.1f;
        percentageCoin = totalTimePlayed / (500f / (randomLuck /2f ));
        float randomValue = Random.value;

        if (randomValue <= Mathf.Clamp01(percentageCoin))
        {
            newNoteGO.transform.GetChild(0).gameObject.SetActive(true);
            newNoteGO.GetComponent<Note>().hasCoin = true;

        }
        else
        {
            newNoteGO.transform.GetChild(0).gameObject.SetActive(false);
            newNoteGO.GetComponent<Note>().hasCoin = false;
        }

        spawnNoteTimer = 0;

        UpdateNoteVelocity();
    }

    private void CooldownDifficultyIncreaser()
    {
        noteCooldownSpawn = tpCooldownSpawnMultiplier / totalTimePlayed;
    }

    public void UpdateNoteVelocity()
    {
        float newSpeed = startNoteSpeed / tpNoteDivisor * tpNoteVelMultiplier;
        actualNoteSpeed = Mathf.Clamp(newSpeed, startNoteSpeed, maximumNoteSpeed);
    }
}
