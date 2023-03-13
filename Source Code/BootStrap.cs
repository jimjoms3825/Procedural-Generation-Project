using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: BootStrap
 * Description: Simple bootstrap to avoid duplication of GameManager. 
 */

public class BootStrap : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
        Destroy(gameObject);
    }
}
