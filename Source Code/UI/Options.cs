using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: Options. 
 * Description: A script used in options menus to allow for the changing of variables. 
 */

public class Options : MonoBehaviour
{
    //Sliders for volume. 
    [SerializeField] private Slider master;
    [SerializeField] private Slider UI;
    [SerializeField] private Slider SFX;
    [SerializeField] private Slider music;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 1);
        }
        if (!PlayerPrefs.HasKey("UIVolume"))
        {
            PlayerPrefs.SetFloat("UIVolume", 1);
        }
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 1);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
        }
        master.value = PlayerPrefs.GetFloat("MasterVolume");
        UI.value = PlayerPrefs.GetFloat("UIVolume");
        SFX.value = PlayerPrefs.GetFloat("SFXVolume");
        music.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    public void setMasterVolume(float f)
    {
        SoundManager.setLevel(f, SoundManager.SoundGroups.Master);
        PlayerPrefs.SetFloat("MasterVolume", f);
    }

    public void setSFXVolume(float f)
    {
        SoundManager.setLevel(f, SoundManager.SoundGroups.SFX);
        PlayerPrefs.SetFloat("SFXVolume", f);
    }

    public void setUIVolume(float f)
    {
        SoundManager.setLevel(f, SoundManager.SoundGroups.UI);
        PlayerPrefs.SetFloat("UIVolume", f);
    }

    public void setMusicVolume(float f)
    {
        SoundManager.setLevel(f, SoundManager.SoundGroups.Music);
        PlayerPrefs.SetFloat("MusicVolume", f);
    }

    public void exitToMainMenu()
    {
        GameManager.instance.changeToMainMenu();
    }

    public void unPause()
    {
        GameManager.instance.unPause();
    }

}
