using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: Menu
 * Description: Menu script with simple access methods for compatability in the inspector. 
 */

public class Menu : MonoBehaviour
{
    
    public void goToLevel(int i)
    {
        GameManager.instance.goToLevel(i);
    }

    public void startNewGame()
    {
        GameManager.instance.changeLevel();
    }

    public void startCreditLevel()
    {
        GameManager.instance.changeToCreditLevel();
    }
}
