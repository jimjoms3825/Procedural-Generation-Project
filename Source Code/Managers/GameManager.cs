using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: GameManager
 * Description: Controls the execution of the game. Contains methods for changing the scene and game state. 
 * Has references to important objects that can be accessed by other scripts.
 */
public class GameManager : MonoBehaviour
{
    //Static references. 
    public static GameManager instance;
    public static GameObject player;
    public static GameObject uiManager;
    public static Canvas sceneTransitionCanvas;

    //Whether the player has been killed. 
    public static bool playerDeath;

    //Prefabs for instantiation. 
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject uiPrefab;
    [SerializeField] public GameObject soundManagerPrefab;
    [SerializeField] public GameObject bulletFactoryPrefab;
    [SerializeField] private Canvas sceneTransitionCanvasPrefab;

    //Scene transition flags. 
    private bool readyForTransition = false;
    public bool readyForDetransition = false;

    //If player has paused the game. 
    public bool paused = false;

    //enum for tracking which scene the player is in. 
    public enum GameState { MainMenu, Overworld, Floor1, Floor2, Floor3, BossFloor, EndScreen, CreditLevel }
    public GameState currentGameState = GameState.MainMenu;

    private void Awake()
    {
        //Singleton pattern. 
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Instantiate(soundManagerPrefab);
        }
        else if(instance != this)
        {
            Debug.Log("ERROR: more than one game managers active at a time. Deleting new Game Manager.");
            Destroy(gameObject);
        }
    }

    //Changes the level to the next level in sequence. 
    public void changeLevel()
    {
        switch (currentGameState)
        {
            case GameState.MainMenu:
                goToLevel(GameState.Overworld);
                break;
            case GameState.Overworld:
                goToLevel(GameState.Floor1);
                break;
            case GameState.Floor1:
                goToLevel(GameState.Floor2);
                break;
            case GameState.Floor2:
                goToLevel(GameState.Floor3);
                break;
            case GameState.Floor3:
                goToLevel(GameState.BossFloor);
                break;
            case GameState.BossFloor:
                goToLevel(GameState.EndScreen);
                break;
            case GameState.EndScreen:
                goToLevel(GameState.MainMenu);
                break;
            case GameState.CreditLevel:
                goToLevel(GameState.MainMenu);
                break;
        }
    }

    //Sends the player to the level with the provided index.
    public void goToLevel(int newState)
    {
        goToLevel((GameState)newState);
    }

    //Sends the player to the level matching the GameState passed. 
    public void goToLevel(GameState newState)
    {
        int nextSceneIndex = 0;
        bool spawnPlayer = false;
        switch (newState)
        {
            case GameState.MainMenu:
                nextSceneIndex = 0;
                SoundManager.setMusicIntensity(0);
                break;
            case GameState.Overworld:
                SoundManager.setMusicIntensity(50);
                spawnPlayer = true;
                nextSceneIndex = 1;
                break;
            case GameState.Floor1:
                SoundManager.setMusicIntensity(50);
                spawnPlayer = true;
                nextSceneIndex = 2;
                break;
            case GameState.Floor2:
                SoundManager.setMusicIntensity(50);
                spawnPlayer = true;
                nextSceneIndex = 2;
                break;
            case GameState.Floor3:
                SoundManager.setMusicIntensity(50);
                spawnPlayer = true;
                nextSceneIndex = 2;
                break;
            case GameState.BossFloor:
                SoundManager.setMusicIntensity(1f);
                spawnPlayer = true;
                nextSceneIndex = 3;
                break;
            case GameState.EndScreen:
                SoundManager.setMusicIntensity(0f);
                nextSceneIndex = 4;
                break;
            case GameState.CreditLevel:
                SoundManager.setMusicIntensity(0f);
                spawnPlayer = true;
                nextSceneIndex = 5;
                break;
        }
        currentGameState = newState;
        //Reset the playerDeath variable when spawning a new player. 
        if(spawnPlayer) { playerDeath = false; }
        //Starts the multithreaded routine forscene transition. 
        StartCoroutine(nextLevel(nextSceneIndex, spawnPlayer));
    }
    
    //Pauses game execution. 
    public void pause()
    {
        paused = true;
        Time.timeScale = 0;
        uiManager.GetComponent<UIManager>().pause();
    }

    //Unpauses game excecution.
    public void unPause()
    {
        paused = false;
        Time.timeScale = 1f;
        uiManager.GetComponent<UIManager>().unPause();
    }

    //Sends the player to the credit level. 
    public void changeToCreditLevel()
    {
        goToLevel(GameState.CreditLevel);
    }

    //Sends the player to the main menu. 
    public void changeToMainMenu()
    {
        goToLevel(GameState.MainMenu);
    }

    //Called when the player is killed. 
    public void playerDead()
    {
        playerDeath = true;
        goToLevel(GameState.EndScreen);
    }

    //Transitions to the level matching nextSceneIndex and spawns a player if spawnPlayer is true. 
    private IEnumerator nextLevel(int nextSceneIndex, bool spawnPlayer)
    {
        readyForTransition = false;
        readyForDetransition = false;
        if (player != null && player.GetComponent<Rigidbody>() != null) { player.GetComponent<Rigidbody>().useGravity = false; } // Disable player gravity so he doesnt fly around between levels.
        //Black out the screen. 
        StartCoroutine(enableTransitionCanvas(0.5f));
        //yeild until screen black. 
        while(readyForTransition == false)
        {
            yield return null;
        }

        //Create a new player if necessary. 
        if (spawnPlayer && player == null)
        {
            player = Instantiate(playerPrefab);
            player.transform.position = new Vector3(0, 1, 0);
            DontDestroyOnLoad(player);
        }
        else if(player != null)
        {
            player.transform.position = new Vector3(0, 2, 0);
        }
        //Load the new scene using multithreading. 
        SceneManager.LoadSceneAsync(nextSceneIndex);
        Scene nextScene = SceneManager.GetSceneByBuildIndex(nextSceneIndex);

        //Yield until next scene is loaded. 
        while (!nextScene.isLoaded)
        {
            Time.timeScale = 1f;
            yield return new WaitForFixedUpdate();
        }
        //Set the new scene as the active scene. 
        SceneManager.SetActiveScene(nextScene);

        //if going to a scene with generation, wait for the readyForDetransition flag. 
        while (!readyForDetransition && (nextSceneIndex == 1 || nextSceneIndex == 2))
        {
            yield return null;
        }

        if (spawnPlayer) // Spawn the UI in if new player is needed
        {
            uiManager = Instantiate(uiPrefab);
        }
        else 
        {
            if (player != null)
                Destroy(player);
            if(uiManager != null)
                Destroy(uiManager);
        }
        //Disable the transition canvas. 
        StartCoroutine(disableTransitionCanvas(1f));
        //Re-enable player gravity if necessary. 
        if (player != null && player.GetComponent<Rigidbody>() != null) { player.GetComponent<Rigidbody>().useGravity = true; }
    }

    //Creates a fading black canvas over f seconds for scene transition. 
    public IEnumerator enableTransitionCanvas(float f)
    {
        //If no scene transition canvas is currently in the scene. 
        if(sceneTransitionCanvas == null) { 
            sceneTransitionCanvas = Instantiate(sceneTransitionCanvasPrefab);
            sceneTransitionCanvas.transform.SetParent(transform);
        }
        Image sceneTransitionPanel = sceneTransitionCanvas.GetComponentInChildren<Image>();
        TextMeshProUGUI sceneTransitionText = sceneTransitionCanvas.GetComponentInChildren<TextMeshProUGUI>();
        float alpha = 0;
        //Increase the alpha until screen is completely black. 
        while (sceneTransitionPanel.color.a < 1)
            {
            alpha += Time.unscaledDeltaTime / f;
            sceneTransitionPanel.color = new Color(0, 0, 0, alpha);
            sceneTransitionText.alpha = alpha;
            yield return null;
        }
        //Tell the system that the scene can now be changed. 
        readyForTransition = true;
    }

    //Disables the transition canvas over f seconds. 
    public IEnumerator disableTransitionCanvas(float f)
    {
        if(sceneTransitionCanvas != null) {
            Image sceneTransitionPanel = sceneTransitionCanvas.GetComponentInChildren<Image>();
            TextMeshProUGUI sceneTransitionText = sceneTransitionCanvas.GetComponentInChildren<TextMeshProUGUI>();

            float alpha = 1;
            while (sceneTransitionPanel.color.a > 0)
            {
                alpha -= Time.unscaledDeltaTime / f;
                sceneTransitionPanel.color = new Color(0, 0, 0, alpha);
                sceneTransitionText.alpha = alpha;
                yield return null;
            }
        }
    }
}


