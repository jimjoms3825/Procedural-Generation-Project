using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: CrossHair
 * Description: A programatically created crosshair.
 */
public class CrossHair : MonoBehaviour
{
    [SerializeField] private float maxDistance; //The maximum spread of the hairs. 
    [SerializeField] private GameObject crosshairSpriteComponent; //The image object which will be used for the hairs. 
    [SerializeField] private int numberOfCrosshairs; // The number of hairs to generate. 

    private GameObject[] crossHairs; // The collection of instantiated hairs. 

    private float crossHairAngleSegment; // The number of degrees between crosshairs.

    //Instantiation. 
    private void Awake()
    {
        //Calculate the rotation and direction of crosshairs. 
        crossHairAngleSegment = 360 / numberOfCrosshairs;
        //Create the array and instantiate the hairs. 
        crossHairs = new GameObject[numberOfCrosshairs];
        for(int i = 0; i < numberOfCrosshairs; i++)
        {
            crossHairs[i] = Instantiate(crosshairSpriteComponent);
            crossHairs[i].transform.SetParent(transform);
            crossHairs[i].transform.Rotate(0, 0, i * crossHairAngleSegment);
        }
    }

    //Set the distance from the center for each hair. 
    public void setDistance(float newDistance)
    {
        if(newDistance > maxDistance)
        {
            newDistance = maxDistance;
        } 
        else if(newDistance < 0)
        {
            newDistance = 0;
        }

        foreach(GameObject hair in crossHairs)
        {
            hair.transform.position = transform.position + hair.transform.up * newDistance;
        }
    }
}
