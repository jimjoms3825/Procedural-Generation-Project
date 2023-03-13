using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: DisplayStand
 * Description: A display stand which displays an item in the credit menu.
 */

public class DisplayStand : MonoBehaviour
{
    [SerializeField] private GameObject displayObject;
    [SerializeField] private float spinSpeed;
    [SerializeField] private Vector3 rotationPoint;

    private void Start()
    {
        if(!displayObject) {
            if (transform.GetChild(0))
            {
                displayObject = transform.GetChild(0).gameObject;
            }
            else
            {
                Destroy(this);
            }
        }
        if(displayObject.GetComponent<Gun>() != null)
        {
            displayObject = WeaponPickup.createWeaponPickup(displayObject.GetComponent<Gun>());
        }

        rotationPoint += transform.position;
        displayObject.transform.position = rotationPoint;

    }

    //Calculate rotation every frame. 
    void LateUpdate()
    {
        if (!displayObject) return;
        displayObject.transform.RotateAround(rotationPoint, Vector3.up, spinSpeed * Time.deltaTime); //spin
    }

}
