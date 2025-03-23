using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{

    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SpawnManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<SpawnManager>();
                    singletonObject.name = typeof(SpawnManager).ToString() + " (Singleton)";

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private SquadSpawner _squadSpawner;
    private EnemySpawner _enemySpawner;


    void Awake()
    {
        // Ensure singleton integrity
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _squadSpawner = GetComponent<SquadSpawner>();
        if (_squadSpawner != null )
        {
            _squadSpawner.InstantiateCoreAndCharacters();

        }

        _enemySpawner = GetComponent<EnemySpawner>();
        if (_enemySpawner != null )
        {
            Debug.Log("Calling SpawnEnemies");
            _enemySpawner.SpawnEnemies();
        }
    }

    void Start()
    {
        _squadSpawner.FillSquadManagerWithSquad(); 
    }

    

}