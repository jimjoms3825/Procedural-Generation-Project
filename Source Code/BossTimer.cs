using UnityEngine;
using TMPro;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: BossTimer
 * Description: Timer based on player distance which counts down player survival at the end of the game. 
 */

public class BossTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private float timer = 60;
    //Flag for player first entering the zone. 
    private bool firstTime = false;

    // Called every physics update. 
    void FixedUpdate()
    {
        if(GameManager.player != null && !GameManager.playerDeath && Vector3.Distance(transform.position, GameManager.player.transform.position) < 40)
        {
            timer -= Time.deltaTime;
            text.text = "CountDown:\n" + timer.ToString("0.00");
            if (!firstTime)
            {
                foreach(Squad s in FindObjectsOfType<Squad>())
                {
                    s.blackBoard.addData("In Combat", true);
                    s.blackBoard.addData("Player Position", GameManager.player.transform.position);
                }
                firstTime = true;
            }
        }
        else
        {
            text.text = "Subject Too Far";
        }

        if(timer < 0)
        {
            GameManager.instance.goToLevel(GameManager.GameState.EndScreen);
        }

    }
}
