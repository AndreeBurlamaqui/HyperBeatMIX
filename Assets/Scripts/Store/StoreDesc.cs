using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;


public class StoreDesc : MonoBehaviour
{

    public PaletteController palete;
    public StoreManager _storeManager;
    public int itemPrice;

    private void Awake()
    {
        GetComponentInChildren<UIButton>().OnClick.OnTrigger.Event.AddListener(() => _storeManager.NewPalette(palete, itemPrice));

        UIButton[] ub = GetComponentsInChildren<UIButton>();

        foreach(UIButton _ub in ub)
        {
            if(_ub.name == "BuyButton")
            {
                _ub.OnClick.OnTrigger.Event.AddListener(() => _storeManager.NewPalette(palete, itemPrice));
            }
            else
            {
                _ub.OnClick.OnTrigger.Event.AddListener(() => _storeManager.TestPalette(palete));
            }
        }
    }

}
