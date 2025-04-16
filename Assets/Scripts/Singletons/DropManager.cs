using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class DropManager : MonoBehaviour
{


    [SerializeField] private Vector3 dropLaunchVector = new Vector3(1, 0, 0).normalized;
    [SerializeField] private float dropLaunchSpeed = 5f;

    [SerializeField] private GameObject copperDrop_prefab;
    [SerializeField] private GameObject silverDrop_prefab;
    [SerializeField] private GameObject goldDrop_prefab;
    [SerializeField] private GameObject platinumDrop_prefab;

    private static bool isQuitting = false;
    void OnApplicationQuit() => isQuitting = true;

    private static DropManager _instance;
    public static DropManager Instance
    {
        get
        {
            if (isQuitting) return null; // Prevent creation during shutdown

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<DropManager>(FindObjectsInactive.Include);
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<DropManager>();
                    singletonObject.name = typeof(DropManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

   

    public void DropLoot(GameObject deadEntity)
    {
        Inventory _inventory = deadEntity.GetComponent<Inventory>();
        if (_inventory != null)
        {
            int copper_count = _inventory.copper_count;
            _inventory.copper_count = 0;
            if (copper_count > 0)
            {
                ThrowLoot(copperDrop_prefab, copper_count, deadEntity);
            }

            int silver_count = _inventory.silver_count;
            _inventory.silver_count = 0;
            if (silver_count > 0)
            {
                ThrowLoot(silverDrop_prefab, silver_count, deadEntity);
            }

            //get gold count
            int gold_count = _inventory.gold_count;
            _inventory.gold_count = 0;
            if (gold_count > 0)
            {
                ThrowLoot(goldDrop_prefab, gold_count, deadEntity);
            }

            int platinum_count = _inventory.platinum_count;
            _inventory.platinum_count = 0;
            if(platinum_count > 0)
            {
                ThrowLoot(platinumDrop_prefab, platinum_count, deadEntity);
            }

            if (copper_count+silver_count+gold_count+platinum_count > 0)
            {
                SoundManager.Instance.PlaySoundByKeyAtPosition("coin_drop", deadEntity.transform.position, SoundCategory.sfx);
            }

        }

    }

    private void ThrowLoot(GameObject dropPrefab, int amount, GameObject entity)
    {
        GameObject thisDrop;
        for (int i = 0; i < amount; i++)
        {
            thisDrop = Instantiate(dropPrefab, entity.transform.position, Quaternion.identity);
            Rigidbody _rb = thisDrop.GetComponent<Rigidbody>();
            if (_rb != null)
            {

                _rb.linearVelocity = RandomizeLaunchVector(dropLaunchVector) * dropLaunchSpeed;


            }
        }

    }

    private Vector3 RandomizeLaunchVector(Vector3 vector)
    {
        float randomDegrees = Random.Range(0f, 360f); // Random angle between 0-360

        // Rotate around Y-axis
        Vector3 rotatedVector = Quaternion.Euler(0, randomDegrees, 0) * vector;
        return rotatedVector.normalized;
    }
}
