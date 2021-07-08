using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteSwapper : MonoBehaviour
{

    [SerializeField]
    private PaletteController currentPalette;


    public void NewPalette(PaletteController palette)
    {
        currentPalette = palette;


        for(int x = 0; x < currentPalette.paletteColor.Count; x++)
        {
            foreach (Material mat in currentPalette.paletteColor[x].materialRespective) { 

                mat.color = currentPalette.paletteColor[x].hexColorCode;
            }
        }

        
    }
}
