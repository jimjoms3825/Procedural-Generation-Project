using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: MiniMap
 * Description: Script which displays a minimap for the player. 
 */
public class MiniMap : MonoBehaviour
{
    [SerializeField] private float height; // The height of the projection camera. 
    [SerializeField] private GameObject playerCursor; // The image cursor of the player. 
    [SerializeField] private Teleporter teleporter; // A reference for distance calculation. 
    [SerializeField] private TextMeshProUGUI TPText; // Displays the distance to the teleporter. 

    //Update once per frame. 
    private void LateUpdate()
    {
        if(GameManager.player == null) { return; }
        transform.position = GameManager.player.transform.position + new Vector3(0, height, 0);
        transform.rotation = Quaternion.Euler(90,0, 0);
        playerCursor.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -GameManager.player.transform.rotation.eulerAngles.y);
        if (teleporter != null)
        {
            TPText.text = "Target is " + (int)Vector3.Distance(GameManager.player.transform.position, teleporter.transform.position) + " away";
        }
        else
        {
            teleporter = FindObjectOfType<Teleporter>();
            TPText.text = "";
        }

    }
}
