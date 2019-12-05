/*Copyright (C) Katherine Brough & Noah Greaves in association with Vancouver Film School*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ObjectPooling;
using Cinemachine;

public class GameMaster : MonoBehaviour
{
    public PlayerController Player;
    public MusicContorller MusicController;
    public static GameMaster instance;                                                  //Kate:makes the gamemaster accessible from other scripts
    [SerializeField] public bool _loadAgain = false;
    public Dictionary<string, int> DeathCount = new Dictionary<string, int>();          // Kate:the list of the dead enemies by tag and number for the ressurection spell
    public  Dictionary<string, int> _ressurectionbuildup = new Dictionary<string, int>();
    [SerializeField] private PoolableObject _hand;
    [SerializeField] public GameObject _bosstitanController;                                                        
    private HandsOfDead _hands;                                                          //Noah: Handes of Dead Spell
    //public float healerPercentage => (_ressurectionbuildup["Healer"] * 1.0f) / 3.0f;
    public float rangerPercentage => (_ressurectionbuildup["Ranger"] * 1.0f)/ 3.0f;
    public float tankPercentage => (_ressurectionbuildup["Tank"] * 1.0f)/ 3.0f;

    [SerializeField] private GameOverScreen _gameOverScreen;
    [HideInInspector] public GameOverScreen _gameOverScreenClone;

    [SerializeField] private GameObject _gameOverParent;

    private int _numOfHands;

    [SerializeField] public CinemachineImpulseSource _damageshake;
    [SerializeField] public CinemachineImpulseSource _hurtshake;
    [SerializeField] public CinemachineImpulseSource _fireshake;
    [SerializeField] public CinemachineImpulseSource _damageshake2;
    [SerializeField] public CinemachineImpulseSource _hurtshake2;
    [SerializeField] public CinemachineImpulseSource _explosionshake;

    [HideInInspector] public bool beenCalled = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);                         //Kate: makes sure this does not get destroyed on load when character dies and respawns
        }
        Player = FindObjectOfType<PlayerController>();
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            Player.transform.position = new Vector3(1001.2f, -59, 312);
           Player.transform.RotateAround(Player.transform.position, Vector3.up,90);
        }
        Cursor.visible = false;
    }

    private void Start() 
    {
        
    }

    // Create the object pools for the game to use
    public void ObjectPooling()
    {
        PoolManager.CreatePool(_hand, _numOfHands);               //Noah: Object Pooling for the hands of the dead
    }

    public void DisplayGameOver()
    {
        if(beenCalled == false)
        {
            _gameOverScreenClone = Instantiate(_gameOverScreen);
            _gameOverScreenClone.transform.SetParent(_gameOverParent.transform);
            Time.timeScale = 0.0f;
            beenCalled = true;
        }
    }

    public void ReloadScene()
    {
        Tank.AIList.Clear();
        Time.timeScale = 1.0f;
        StartCoroutine(DoReloadScene());
    }

    private IEnumerator DoReloadScene()
    {
        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3, LoadSceneMode.Single);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            GameObject playerObject =  GameObject.Find("Beta_SC_Kiin_Prefabv2");
        
            if(playerObject != null)
            {
               // Debug.Log("player assigned");
                Player = playerObject.GetComponent<PlayerController>();
            }
            else
            {
                Debug.Log("Couldn't find player");
            }
        }
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(5, LoadSceneMode.Single);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            GameObject playerObject =  GameObject.Find("Beta_SC_Kiin_Prefabv21");
            if(playerObject != null)
            {
               // Debug.Log("player assigned");
                Player = playerObject.GetComponent<PlayerController>();
                beenCalled = false;
            }
            else
            {
                Debug.Log("Couldn't find player");
            }
        }
        beenCalled = false;
    }
}

