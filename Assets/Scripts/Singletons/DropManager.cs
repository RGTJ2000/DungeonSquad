using UnityEngine;
using System.Collections.Generic;

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

   

    public void DropAllLoot(GameObject deadEntity)
    {
        Inventory _inventory = deadEntity.GetComponent<Inventory>();
        if (_inventory != null)
        {
            int copper_count = _inventory.copper_count;
            _inventory.copper_count = 0;
            if (copper_count > 0)
            {
                ThrowCoins(copperDrop_prefab, copper_count, deadEntity);
            }

            int silver_count = _inventory.silver_count;
            _inventory.silver_count = 0;
            if (silver_count > 0)
            {
                ThrowCoins(silverDrop_prefab, silver_count, deadEntity);
            }

            //get gold count
            int gold_count = _inventory.gold_count;
            _inventory.gold_count = 0;
            if (gold_count > 0)
            {
                ThrowCoins(goldDrop_prefab, gold_count, deadEntity);
            }

            int platinum_count = _inventory.platinum_count;
            _inventory.platinum_count = 0;
            if(platinum_count > 0)
            {
                ThrowCoins(platinumDrop_prefab, platinum_count, deadEntity);
            }

            if (copper_count+silver_count+gold_count+platinum_count > 0)
            {
                SoundManager.Instance.PlaySoundByKeyAtPosition("coin_drop", deadEntity.transform.position, SoundCategory.sfx);
            }

            //drop items

            List<RuntimeItem> allItems = _inventory.AllItems;
            foreach (RuntimeItem item in allItems)
            {
                ThrowRuntimeItem(item, deadEntity);
            }

            //clear items
            _inventory.Clear();
        }

    }

    private void ThrowCoins(GameObject dropPrefab, int amount, GameObject entity)
    {
        GameObject thisDrop;
        float thisLaunchSpeed;

        for (int i = 0; i < amount; i++)
        {
            Vector3 launchPosition;
            if (entity.CompareTag("Chest"))
            {
                launchPosition = entity.transform.position + entity.transform.forward;
                thisLaunchSpeed = dropLaunchSpeed * 2f;
            }
            else
            {
                launchPosition = entity.transform.position;
                thisLaunchSpeed = dropLaunchSpeed;
            }

            thisDrop = Instantiate(dropPrefab,launchPosition, Quaternion.identity);

            

            Rigidbody _rb = thisDrop.GetComponent<Rigidbody>();
            if (_rb != null)
            {

                _rb.linearVelocity = RandomizeLaunchVector(dropLaunchVector) * thisLaunchSpeed;


            }
        }

    }

    public void ThrowRuntimeItem(RuntimeItem item, GameObject entity)
    {
        GameObject _prefab = item.item_prefab;

        if (_prefab != null)
        {
            Debug.Log("Instatiating drop");
            GameObject thisDrop;

            Vector3 launchPosition;
            if (entity.CompareTag("Chest"))
            {
                launchPosition = entity.transform.position + entity.transform.forward;
            }
            else
            {
                launchPosition = entity.transform.position;
            }

            thisDrop = Instantiate(_prefab, launchPosition, Quaternion.identity);
            //set the RuntimeItem reference in prefab
            DroppedItemBehavior _droppedItemBehavior = thisDrop.GetComponent<DroppedItemBehavior>();
            _droppedItemBehavior.SetRuntimeItem(item);

            
            //throw item
            Rigidbody _rb = thisDrop.GetComponent<Rigidbody>();
            {

                _rb.linearVelocity = RandomizeLaunchVector(dropLaunchVector) * dropLaunchSpeed *3f;

            }
             SoundManager.Instance.PlaySoundByKeyAtPosition(item.baseItem.dropAudio_ID, entity.transform.position, SoundCategory.sfx);
            Debug.Log("drop audio = " + item.baseItem.dropAudio_ID);
        }
        

    }


    private Vector3 RandomizeLaunchVector(Vector3 vector)
    {
        float randomDegrees = Random.Range(0f, 360f); // Random angle between 0-360

        // Rotate around Y-axis
        Vector3 rotatedVector = Quaternion.Euler(0, randomDegrees, 0) * vector;
        return rotatedVector;
    }
}
