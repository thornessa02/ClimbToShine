using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public Toggle seedBool;
    public TMP_InputField seedNb;
    void Start()
    {
        if (PlayerPrefs.HasKey("UseSeed")) 
        {
            if(PlayerPrefs.GetInt("UseSeed")==1) seedBool.isOn = true;
            else seedBool.isOn = false;
        }

        if (PlayerPrefs.HasKey("SeedNb"))
        {
            seedNb.text = PlayerPrefs.GetString("SeedNb");
        }
        seedBool.onValueChanged.AddListener(UseSeed);
    }

    // Update is called once per frame
    public void UseSeed(bool value)
    {
        if (value)
            PlayerPrefs.SetInt("UseSeed", 1);
        else PlayerPrefs.SetInt("UseSeed", 0);

        PlayerPrefs.Save();
    }
    public void SeedNb(string value)
    {
        print(value);
        PlayerPrefs.SetString("SeedNb", value);
        PlayerPrefs.Save();
    }
}
