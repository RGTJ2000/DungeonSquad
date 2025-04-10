using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField] private Vector3 dropLaunchVector = new Vector3(1, 0.5f, 0).normalized;
    [SerializeField] private float dropLaunchSpeed = 5f;

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

    public GameObject goldDrop_prefab;

    public void DropLoot(GameObject deadEntity)
    {
        Inventory _inventory = deadEntity.GetComponent<Inventory>();
        if (_inventory != null)
        {
            //get gold count
            int gold_count = _inventory.gold_count;
            _inventory.gold_count = 0;


            Debug.Log($"{deadEntity.name} DROPPED {gold_count} GOLD.");

            if (gold_count > 0)
            {
                ThrowLoot(goldDrop_prefab, gold_count, deadEntity);
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
