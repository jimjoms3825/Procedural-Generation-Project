using UnityEngine;
using TMPro;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: EndMenu
 * Description: Menu script with simple access methods for compatability in the inspector. 
 */
public class EndMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endText;
    void Awake()
    {
        if (GameManager.playerDeath)
        {
            endText.text = "You have died";
            endText.color = Color.red;
        }
        else
        {
            endText.text = "You have won!";
        }
    }

    public void returnToMainMenu()
    {
        GameManager.instance.changeToMainMenu();
    }

}
