using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: UIManager
 * Description: Manages the HUD UI elements and pause Menu. 
 */
public class UIManager : MonoBehaviour
{
    //HUD text element references. 
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI grenades;
    [SerializeField] private TextMeshProUGUI powerName;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponModifier;
    [SerializeField] private TextMeshProUGUI clipAmmo;
    [SerializeField] private TextMeshProUGUI totalAmmo;

    //HUD element that is enabled when looking at interactable objects. 
    [SerializeField] private TextMeshProUGUI interactText;

    //Slider references. 
    [SerializeField] private Slider maxAmmoSlider;
    [SerializeField] private Slider magazineSlider;
    [SerializeField] private Slider healthSlider;

    //Crosshair script reference. 
    [SerializeField] private CrossHair crosshair;

    //Prefab of pause menu. 
    [SerializeField] private GameObject pauseCanvasPrefab;
    
    private GameObject pauseCanvas;
    private int interactableMask;

    private void Awake()
    {
        interactableMask = LayerMask.GetMask("Interactable");
        DontDestroyOnLoad(this); //Keep this persistant between scenes. 
        magazineSlider.maxValue = 1f;
        maxAmmoSlider.maxValue = 1f;
        healthSlider.maxValue = 1f;
    }

    //Once per frame.
    private void LateUpdate()
    {
        //Output of a physics raycast. 
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3, interactableMask)
            && hit.transform.gameObject.GetComponent<Interactable>() != null) // when player is looking at an interactable
        {
            interactText.text = hit.transform.gameObject.GetComponent<Interactable>().displayText;
        }
        else
        {
            interactText.text = "";
        }

        //If player is present in scene, assign HUD variables. 
        if(GameManager.player != null)
        {
            Player player = GameManager.player.GetComponent<Player>();
            grenades.text = player.currentGrenades.ToString();
            int _health = (int)player.life.getLife();
            health.text = _health.ToString();
            healthSlider.maxValue = player.life.getMaxLife();
            healthSlider.value = player.life.getLife();
            if (player.life.invulnerable)
            {
                healthSlider.fillRect.GetComponent<Image>().color = Color.white;
            }
            else
            {
                healthSlider.fillRect.GetComponent<Image>().color = Color.red;
            }

            //Assign crosshair spread. 
            if (crosshair != null)
            {
                crosshair.setDistance(player.currentAccuracy);
            }

            //Gun variables.
            if(player.getCurrentWeapon() != null)
            {
                Gun gun = player.getCurrentWeapon();
                //Clip in gun. 
                if (gun.hasMag)
                {
                    clipAmmo.color = Color.white;
                    clipAmmo.text = gun.CurrentMagazine.ToString();
                    magazineSlider.value = (float)gun.CurrentMagazine / (float)gun.gunStats.magazineSize;
                    maxAmmoSlider.value = (float)gun.TotalAmmo / (float)gun.gunStats.maxAmmo;
                }
                else // When clip is ejected
                {
                    clipAmmo.color = Color.red;
                    magazineSlider.value = 0;
                    clipAmmo.text = "-";
                }
                totalAmmo.text = gun.TotalAmmo.ToString();
                weaponName.text = gun.gunStats.weaponShortName;
                weaponModifier.text = gun.gunStats.modifierName;
            }
            //If no gun is equipped. 
            else
            {
                clipAmmo.text = "0";
                totalAmmo.text = "0";
                weaponName.text = "";
                weaponModifier.text = "";
            }

            //If player has power. 
            if (player.getCurrentPower() != null)
            {
                Power power = player.getCurrentPower();
                powerName.text = power.powerStats.powerName;
                if (!power.canUse)
                {
                    powerName.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
                else
                {
                    powerName.color = new Color(1f, 1f, 1f, 1f);
                }
            }
            else
            {
                powerName.text = "-";
            }
        }
    }

    public void unPause()
    {
        Destroy(pauseCanvas);
    }

    public void pause()
    {
        pauseCanvas = Instantiate(pauseCanvasPrefab);
    }
}
