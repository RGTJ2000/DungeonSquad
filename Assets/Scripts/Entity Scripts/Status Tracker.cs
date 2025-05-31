using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusTracker : MonoBehaviour
{
    public GameObject status_tracker_prefab;
    private GameObject _statusStack;
    private Camera _mainCamera;
    private Health _health;

    private Vector3 directionToCamera;
    private Vector3 stackOffset = new Vector3(0, 4.1f, 0);

    public enum StatusType
    {
        confusion,
        fear,
        fire,
        frost,
        poison,
        sleep


    }

    [System.Serializable]
    public struct StatusData : IComparable<StatusData>
    {
        public StatusType statusType;
        public float statusCount;
        public Sprite statusIcon;

        public StatusData(StatusType type, float count, Sprite icon)
        {
            statusType = type;
            statusCount = count;
            statusIcon = icon;
        }

        public int CompareTo(StatusData other)
        {
            return other.statusCount.CompareTo(statusCount);
        }
    }

    [SerializeField]
    private StatusData[] statusArray = new StatusData[6];

    private bool confusionActive = false;
    private bool fearActive = false;
    private bool fireActive = false;
    private bool frostActive = false;
    private bool poisonActive = false;
    private bool sleepActive = false;

    private bool fireInCooldown = false;
    private bool fireStopped = true;
    private bool frostInCooldown = false;
    /*
    [SerializeField] private Sprite fearIcon;
    [SerializeField] private Sprite confusionIcon;
    [SerializeField] private Sprite sleepIcon;
    [SerializeField] private Sprite poisonIcon;
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite frostIcon;
    

    private float fearCount = 10f;
    private float confusionCount = 10f;
    private float sleepCount = 10f;
    private float poisonCount = 0f;
    private float fireCount = 0f;
    private float frostCount = 0f;
    */

    EntityStats _entityStats;

    private Image[] slots_array;

    void Start()
    {
        _statusStack = Instantiate(status_tracker_prefab, transform.position + stackOffset, Quaternion.identity);

        _mainCamera = Camera.main;

        _entityStats = GetComponent<EntityStats>();

        _health = GetComponent<Health>();

        slots_array = _statusStack.GetComponentsInChildren<Image>();

        System.Array.Sort(slots_array, (a, b) => a.name.CompareTo(b.name));

    }

    private void Update()
    {
        ShowActiveStatuses();
        PerformDamage();
        CountdownStatuses();

    }

    void LateUpdate()
    {
        // Get the direction from the Canvas to the camera
        directionToCamera = _mainCamera.transform.position - transform.position;
        // Flatten the direction to the XZ plane (ignore Y)
        directionToCamera.y = 0;
        // If direction is zero (directly above/below), skip to avoid errors
        directionToCamera.Normalize();
        transform.rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);

        _statusStack.transform.position = transform.position + stackOffset;

    }

    void ShowActiveStatuses()
    {
        List<StatusData> activeList = new List<StatusData>();

        foreach (var status in statusArray)
        {
            switch (status.statusType)
            {
                case StatusType.confusion:
                    if (confusionActive)
                    {
                        activeList.Add(status);
                    }
                    
                    break;
                case StatusType.fear:
                    if (fearActive)
                    {
                        activeList.Add(status);
                    }
                    break;
                case StatusType.fire:
                    if (fireActive)
                    {
                        activeList.Add(status);
                    }
                    break;
                case StatusType.frost:
                    if (frostActive)
                    {
                        activeList.Add(status);
                    }
                    break;
                case StatusType.poison:
                    if (poisonActive)
                    {
                        activeList.Add(status);
                    }
                    break;
                case StatusType.sleep:
                    if (sleepActive)
                    {
                        activeList.Add(status);
                    }
                    break;
                default:
                    break;
            }

        }

        activeList.Sort();

        for (int i = 0; i < activeList.Count; i++)
        {
            slots_array[i].enabled = true;
            
            Color color = slots_array[i].color;
            float visibilityTier = Mathf.Ceil(activeList[i].statusCount / 5);
            color.a = Mathf.Clamp01(visibilityTier * 0.4f);
            slots_array[i].color = color;
            

            slots_array[i].sprite = activeList[i].statusIcon;

        }

        for (int i = activeList.Count; i < slots_array.Length; i++)
        {
            slots_array[i].enabled = false;
        }


    }

    private void CountdownStatuses()
    {
        for (int i = 0; i < statusArray.Length; i++)
        {
            

            switch (statusArray[i].statusType)
            {
                case StatusType.confusion:

                    statusArray[i].statusCount = statusArray[i].statusCount - (1f * Time.deltaTime * _entityStats.confusion_dissipationRate);

                    if (statusArray[i].statusCount < 0)
                    {
                        statusArray[i].statusCount = 0;
                    }

                    if (statusArray[i].statusCount >= _entityStats.confusion_AL)
                    {
                        confusionActive = true;
                    } else if (statusArray[i].statusCount <= 0)
                    {
                        confusionActive = false;
                    }
                    
                    break;
                case StatusType.fear:
                    statusArray[i].statusCount = statusArray[i].statusCount - (1f * Time.deltaTime * _entityStats.fear_dissipationRate);

                    if (statusArray[i].statusCount < 0)
                    {
                        statusArray[i].statusCount = 0;
                    }

                    if (statusArray[i].statusCount >= _entityStats.fear_AL)
                    {
                        fearActive = true;
                    }
                    else if (statusArray[i].statusCount <= 0)
                    {
                        fearActive = false;
                    }

                    break;
                case StatusType.fire:
                    statusArray[i].statusCount = statusArray[i].statusCount - (1f * Time.deltaTime * _entityStats.fire_dissipationRate);

                    if (statusArray[i].statusCount < 0)
                    {
                        statusArray[i].statusCount = 0;
                    }

                    if (statusArray[i].statusCount >= _entityStats.fire_AL)
                    {
                        fireActive = true;
                    }
                    else if (statusArray[i].statusCount <= 0)
                    {
                        fireActive = false;
                        fireStopped = true;
                    }

                    break;
                case StatusType.frost:
                    statusArray[i].statusCount = statusArray[i].statusCount - (1f * Time.deltaTime * _entityStats.frost_dissipationRate);

                    if (statusArray[i].statusCount < 0)
                    {
                        statusArray[i].statusCount = 0;
                    }

                    if (statusArray[i].statusCount >= _entityStats.frost_AL)
                    {
                        frostActive = true;
                    }
                    else if (statusArray[i].statusCount <= 0)
                    {
                        frostActive = false;
                    }

                    break;
                case StatusType.poison:
                    statusArray[i].statusCount = statusArray[i].statusCount - (1f * Time.deltaTime * _entityStats.poison_dissipationRate);

                    if (statusArray[i].statusCount < 0)
                    {
                        statusArray[i].statusCount = 0;
                    }

                    if (statusArray[i].statusCount >= _entityStats.poison_AL)
                    {
                        poisonActive = true;
                    }
                    else if (statusArray[i].statusCount <= 0)
                    {
                        poisonActive = false;
                    }

                    break;
                case StatusType.sleep:
                    statusArray[i].statusCount = statusArray[i].statusCount - (1f * Time.deltaTime * _entityStats.sleep_dissipationRate);

                    if (statusArray[i].statusCount < 0)
                    {
                        statusArray[i].statusCount = 0;
                    }

                    if (statusArray[i].statusCount >= _entityStats.sleep_AL)
                    {
                        sleepActive = true;
                    }
                    else if (statusArray[i].statusCount <= 0)
                    {
                        sleepActive = false;
                    }

                    break;
                default:
                    break;
            }


        }

    }

    private void PerformDamage()
    {
        if (fireActive && !fireInCooldown)
        {
            if (fireStopped)
            {
                StartCoroutine(StartFireCooldown()); //warm up the fire damage
                
                fireStopped = false;
                fireInCooldown = true;

            } else
            {
                _health.TakeDamage(1f * _entityStats.fire_damageMultiplier);
                fireInCooldown = true;
                StartCoroutine(StartFireCooldown());

            }

            
        }
    }

    IEnumerator StartFireCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        fireInCooldown = false;
    }
 
    public void ReceiveStatusCount(float damage, string type)
    {
        switch (type)
        {
            case "fire":
                statusArray[(int)StatusType.fire].statusCount += damage;
                break;
        }
    }
    private void OnDestroy()
    {
        Destroy(_statusStack);
    }
}
