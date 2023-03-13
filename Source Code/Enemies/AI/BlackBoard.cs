using System.Collections.Generic;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: BlackBoard
 * Description: A class used to store and access data by one or more AI
 * agents. Uses a dictionary as the main data structure. 
 */
public class BlackBoard
{
    // Start is called before the first frame update
    private Dictionary<string, object> data;

    /*
     * Simple constructor which initializes the dictionary. 
     */
    public BlackBoard()
    {
        data = new Dictionary<string, object>();
    }

    /*
     * Adds the passed value under the passed key to the blackboard. 
     */
    public void addData(string key, object val)
    {
        data[key] = val;
    }

    /*
     * Returns the data associated with the key passed as an argument, if 
     * there is valid data under that key.
     */
    public object getData(string key)
    {
        object value = null;
        data.TryGetValue(key, out value);
        return value;
    }

    /*
     * Removes the data associated with the passed key. 
     */
    public bool ClearData(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
            return true;
        }
        return false;
    }

    /*
     * Returns a bool representing whether the blackboard contains data
     * assocated with the passed key. 
     */
    public bool hasData(string key)
    {
        if (data.ContainsKey(key)) { return true; }
        return false;
    }
}
