using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using TMPro;
using Lean.Localization;
using UnityEditor;
public class StoreManager : MonoBehaviour
{

    public UIView lastTab;
    public Material labelMat;
    public float previewTime;
    private void Awake()
    {
        UpdateColorLabel();
        UpdateStoreName();
        FixMaterial();
        UpdateBeatCoinText();
    }

    public void ChangeTab(UIView tab)
    {


        if(lastTab != tab)
        {
            lastTab.Hide();


            tab.Show();
            lastTab = tab;
        }

    }

    private void UpdateBeatCoinText()
    {
        //format: 0000000

        GameObject.FindWithTag("BCText").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("BeatCoins").ToString("0000000");
    }

    public void UpdateStoreName()
    {
        GameObject[] b = GameObject.FindGameObjectsWithTag("ItemToBuy");

        foreach(GameObject _b in b)
        {


            PaletteController storePalette = _b.GetComponent<StoreDesc>().palete;
            TextMeshProUGUI[] storeText = _b.GetComponentsInChildren<TextMeshProUGUI>();

            for(int x = 0; x < storeText.Length; x++)
            {
                if (!storeText[x].CompareTag("Price"))
                {
                    if (storePalette.languagesList[storePalette.paletteLocal[x].languageID] == LeanLocalization.CurrentLanguage)
                    {
                        //Title first
                        storeText[0].text = storePalette.paletteLocal[x].paletteName;

                        //Description second
                        storeText[1].text = storePalette.paletteLocal[x].paletteStoreDescription;
                    }
                }
                else
                {
                    int iPrice = storeText[x].transform.parent.transform.parent.GetComponent<StoreDesc>().itemPrice;
                    storeText[x].text = iPrice.ToString();

                    if(iPrice > PlayerPrefs.GetInt("BeatCoins"))
                    {
                        storeText[x].transform.parent.GetComponent<UIButton>().Interactable = false;
                    }
                }

            }
        }
    }



    ///////////////////
    //PALETTE SWAPPER//
    ///////////////////

    public PaletteController currentPalette;
    private PaletteController testPalette;

    private bool isPreviewing = false;

    public void FixMaterial()
    {
        currentPalette = Resources.Load<PaletteController>("VFX_Store/Palette/" + PlayerPrefs.GetString("SavedPalette"));
            

        for (int x = 0; x < currentPalette.paletteColor.Count; x++)
        {

            foreach (Material mat in currentPalette.paletteColor[x].materialRespective)
            {

                //mat.SetColor("_TintColor", currentPalette.paletteColor[x].hexColorCode);
                mat.SetColor("_TintColor", new Color(currentPalette.paletteColor[x].hexColorCode.r, currentPalette.paletteColor[x].hexColorCode.g, currentPalette.paletteColor[x].hexColorCode.b, mat.GetColor("_TintColor").a));


                if (mat.name == "Color2MAT")
                {
                    Shader.SetGlobalColor("_TintColor", currentPalette.paletteColor[x].hexColorCode);
                }
            }



        }
    }

    private void UpdateColorLabel()
    {
        TextMeshProUGUI[] allLabel = Resources.FindObjectsOfTypeAll(typeof(TextMeshProUGUI)) as TextMeshProUGUI[];

        foreach (TextMeshProUGUI label in allLabel)
        {
            label.color = labelMat.GetColor("_TintColor");
        }
    }

    public void NewPalette(PaletteController palette, int price)
    {
        if(currentPalette != palette)
        {
            Debug.Log(price);
            int newbc = PlayerPrefs.GetInt("BeatCoins") - price;
            Debug.Log(newbc);
            PlayerPrefs.SetInt("BeatCoins", newbc);
            Debug.Log(PlayerPrefs.GetInt("BeatCoins"));
        }

        currentPalette = palette;

        PlayerPrefs.SetString("SavedPalette", currentPalette.name);

        for (int x = 0; x < currentPalette.paletteColor.Count; x++)
        {

            foreach (Material mat in currentPalette.paletteColor[x].materialRespective)
            {
                mat.SetColor("_TintColor", new Color(currentPalette.paletteColor[x].hexColorCode.r, currentPalette.paletteColor[x].hexColorCode.g, currentPalette.paletteColor[x].hexColorCode.b, mat.GetColor("_TintColor").a));

                if (mat.name == "Color2MAT")
                {
                    Shader.SetGlobalColor("_TintColor", currentPalette.paletteColor[x].hexColorCode);
                }
            }


        }
        


        UpdateBeatCoinText();
        UpdateColorLabel();

    }

    public void TestPalette(PaletteController palette)
    {
        if (!isPreviewing)
        {
            testPalette = palette;

            StartCoroutine(PreviewPalette());
        }
    }

    IEnumerator PreviewPalette()
    {
        isPreviewing = true;

        for (int x = 0; x < testPalette.paletteColor.Count; x++)
        {

            foreach (Material mat in currentPalette.paletteColor[x].materialRespective)
            {

                mat.SetColor("_TintColor", new Color(currentPalette.paletteColor[x].hexColorCode.r, currentPalette.paletteColor[x].hexColorCode.g, currentPalette.paletteColor[x].hexColorCode.b, mat.GetColor("_TintColor").a));

                if (mat.name == "Color2MAT")
                {
                    Shader.SetGlobalColor("_TintColor", testPalette.paletteColor[x].hexColorCode);
                }
            }

        }

        UpdateColorLabel();

        yield return new WaitForSeconds(previewTime);

        isPreviewing = false;

        for (int x = 0; x < currentPalette.paletteColor.Count; x++)
        {

            foreach (Material mat in currentPalette.paletteColor[x].materialRespective)
            {

                mat.SetColor("_TintColor", new Color(currentPalette.paletteColor[x].hexColorCode.r, currentPalette.paletteColor[x].hexColorCode.g, currentPalette.paletteColor[x].hexColorCode.b, mat.GetColor("_TintColor").a));

                if (mat.name == "Color2MAT")
                {
                    Shader.SetGlobalColor("_TintColor", currentPalette.paletteColor[x].hexColorCode);
                }
            }

        }

        UpdateColorLabel();
    }
}
