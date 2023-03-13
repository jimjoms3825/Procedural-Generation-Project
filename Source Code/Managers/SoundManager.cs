using UnityEngine;
using FMOD.Studio;
using FMODUnity;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: SoundManager
 * Description: Provides a simple interface for playing 2D/3D sounds and music. 
 */

public class SoundManager : MonoBehaviour
{
    //Enum for control. 
    public enum SoundGroups { Master, Music, SFX, UI }

    //A static instance for singleton pattern. 
    private static SoundManager instance;

    //Audio Bus' for controlling sounds by grouping. 
    private static Bus master;
    private static Bus music;
    private static Bus SFX;
    private static Bus UI;

    //The instance of the music track.
    private EventInstance musicEvent;

    public void Awake()
    {
        //Singleton pattern.
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogError("Second SoundManager script present. Destroying new instance.");
            Destroy(this);
        }

        //Assign references to bus'.
        master = RuntimeManager.GetBus("bus:/");
        music = RuntimeManager.GetBus("bus:/Music");
        SFX = RuntimeManager.GetBus("bus:/SFX");
        UI = RuntimeManager.GetBus("bus:/UI");

        //Read the saved values for the sound groups and assign them.
        setLevel(PlayerPrefs.GetFloat("MasterVolume"), SoundGroups.Master);
        setLevel(PlayerPrefs.GetFloat("UIVolume"), SoundGroups.UI);
        setLevel(PlayerPrefs.GetFloat("SFXVolume"), SoundGroups.SFX);
        setLevel(PlayerPrefs.GetFloat("MusicVolume"), SoundGroups.Music);

        //Create the instance of the main music.
        musicEvent = RuntimeManager.CreateInstance("event:/armos");
        //Set the intensity variable to 0.
        setMusicIntensity(0);
        //Start the music playback. 
        musicEvent.start();
    }

    //Play a sound by passing the directory of the sound in FMOD as a string. 
    public void PlaySound(string s)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(s);
        newInstance.start();
    }

    //Plays a sound with directory s with pitch and volume variability. 
    public void PlaySound(string s, float pitchVariability, float volumeVariability)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(s);
        float v;
        float p;
        newInstance.getVolume(out v);
        newInstance.getPitch(out p);
        newInstance.setPitch(Random.Range(p - pitchVariability, p + pitchVariability));
        newInstance.setVolume(Random.Range(v - volumeVariability, v + volumeVariability));
        newInstance.start();
    }

    //Plays a sound and attaches it to a Gameobject for 3D positioning. 
    public void playSoundAtPosition(string s, GameObject gameObject)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(s);
        newInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        newInstance.start();
    }

    //Plays a sound with 3d positioning and pitch / volume variability. 
    public void playSoundAtPosition(string s, GameObject gameObject, float pitchVariability, float volumeVariability)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(s);
        float v;
        float p;
        newInstance.getVolume(out v);
        newInstance.getPitch(out p);
        newInstance.setPitch(Random.Range(p - pitchVariability, p + pitchVariability));
        newInstance.setVolume(Random.Range(v - volumeVariability, v + volumeVariability));
        newInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        newInstance.start();
    }

    //Gets the instance of the sound manager. 
    public static SoundManager GetInstance()
    {
        return instance;
    }

    //Sets the volume percentage of the passed group to the set float. 
    public static void setLevel(float f, SoundGroups group)
    {
        switch (group)
        {
            case SoundGroups.Master:
                master.setVolume(Mathf.Clamp(f, 0, 1));
                break;
            case SoundGroups.SFX:
                SFX.setVolume(Mathf.Clamp(f, 0, 1));
                break;
            case SoundGroups.Music:
                music.setVolume(Mathf.Clamp(f, 0, 1));
                break;
            case SoundGroups.UI:
                UI.setVolume(Mathf.Clamp(f, 0, 1));
                break;
        }
    }

    //Returns the volume level of the passed group. 
    public static float getLevel(SoundGroups group)
    {
        float f;
        switch (group)
        {
            case SoundGroups.Master:
                master.getVolume(out f);
                return f;
            case SoundGroups.SFX:
                SFX.getVolume(out f);
                return f;
            case SoundGroups.Music:
                music.getVolume(out f);
                return f;
            case SoundGroups.UI:
                UI.getVolume(out f);
                return f;
        }
        Debug.LogError("Could not find soundgroup for getLevel() call.");
        return 0;
    }

    //Sets the intensity of the music. 
    public static void setMusicIntensity(float intensity)
    {
        intensity = Mathf.Clamp(intensity, 0, 100f);
        instance.musicEvent.setParameterByName("intensity", intensity);
    }

}
