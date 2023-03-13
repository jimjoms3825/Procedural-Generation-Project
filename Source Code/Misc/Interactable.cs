using UnityEngine;
/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: Interactable
 * Description: Abstract class which all interactable objects inherit from. 
 */
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] public string displayText = "Press G to interact";
    //Inhereting classes put their interaction code here. 
    public abstract void Interact();
}
