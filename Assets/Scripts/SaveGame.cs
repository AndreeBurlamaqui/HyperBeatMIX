using UnityEngine;


public class SaveGame : MonoBehaviour
{
    private void Awake()
    {
        if(!PlayerPrefs.HasKey("BeatCoins"))
        {
            PlayerPrefs.SetInt("BeatCoins", 0);
        }

        if (!PlayerPrefs.HasKey("BeatBar"))
        {
            PlayerPrefs.SetInt("BeatBar", 0);
        }
    }

}