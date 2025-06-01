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


    private StatusData[] statusArray = new StatusData[6];

    [SerializeField] private Sprite confusionIcon;
    [SerializeField] private Sprite fearIcon;
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite frostIcon;
    [SerializeField] private Sprite poisonIcon;
    [SerializeField] private Sprite sleepIcon;


    private bool confusionActive = false;
    private bool fearActive = false;
    private bool fireActive = false;
    private bool frostActive = false;
    private bool poisonActive = false;
    private bool sleepActive = false;

    private bool confusionInCooldown = false;
    private bool confusionStopped = true;

    private bool fearInCooldown = false;
    private bool fearStopped = true;

    private bool fireInCooldown = false;
    private bool fireStopped = true;

    private bool frostInCooldown = false;
    private bool frostStopped = true;

    private bool poisonInCooldown = false;
    private bool poisonStopped = true;

    private bool sleepInCooldown = false;
    private bool sleepStopped = true;


   

    EntityStats _entityStats;
    FloatTextDisplay _floatTextDisplay;

    private Image[] imageStack_array;

    void Start()
    {
        _statusStack = Instantiate(status_tracker_prefab, transform.position + stackOffset, Quaternion.identity);

        _mainCamera = Camera.main;

        _entityStats = GetComponent<EntityStats>();
        _health = GetComponent<Health>();
        _floatTextDisplay = GetComponent<FloatTextDisplay>();

        imageStack_array = _statusStack.GetComponentsInChildren<Image>();

        System.Array.Sort(imageStack_array, (a, b) => a.name.CompareTo(b.name));

        InitializeStatusArray();

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

        _statusStack.transform.rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);

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
            imageStack_array[i].enabled = true;
            
            Color color = imageStack_array[i].color;
            float visibilityTier = Mathf.Ceil(activeList[i].statusCount / 5);
            color.a = Mathf.Clamp01(visibilityTier * 0.4f);
            imageStack_array[i].color = color;
            
            //set the icon in the stack to that status icon
            imageStack_array[i].sprite = activeList[i].statusIcon;

        }

        for (int i = activeList.Count; i < imageStack_array.Length; i++)
        {
            //count up from the top of the used stack to top of stack, turning off unused image slots
            imageStack_array[i].enabled = false;
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
        if (confusionActive && !confusionInCooldown)
        {
            confusionInCooldown = true;
            StartCoroutine(StartConfusionCooldown());

            if (confusionStopped)
            {
                confusionStopped = false;
            }
          
        }

        if (fearActive && !fearInCooldown)
        {
            fearInCooldown = true;
            StartCoroutine(StartFearCooldown());

            if (fearStopped)
            {
                fearStopped = false;
            }
         
        }

        if (fireActive && !fireInCooldown)
        {
            fireInCooldown = true;

            StartCoroutine(StartFireCooldown());

            if (fireStopped)
            {
                fireStopped = false;
            }
            else
            {
                float damage = 1f * _entityStats.fire_damageMultiplier;
                _health.TakeDamage(damage);
                _floatTextDisplay.ShowFloatDamage(damage, CombatResultType.hit, DamageType.fire);
            }
        }

        if (frostActive && !frostInCooldown)
        {
            frostInCooldown = true;
            StartCoroutine(StartFrostCooldown());
           

            if (frostStopped)
            {
                frostStopped = false;
            }
            else
            {
                float damage = 1f * _entityStats.frost_damageMultiplier;
                _health.TakeDamage(damage);
                _floatTextDisplay.ShowFloatDamage(damage, CombatResultType.hit, DamageType.frost);
            }
        }

        if (poisonActive && !poisonInCooldown)
        {
            poisonInCooldown = true;
            StartCoroutine(StartPoisonCooldown());
            

            if (poisonStopped)
            {
                poisonStopped = false;
            }
            else
            {
                float damage = 1f * _entityStats.poison_damageMultiplier;
                _health.TakeDamage(damage);
                _floatTextDisplay.ShowFloatDamage(damage, CombatResultType.hit, DamageType.physical);
            }
        }

        if (sleepActive && !sleepInCooldown)
        {
            sleepInCooldown = true;
            StartCoroutine(StartSleepCooldown());
          
            if (sleepStopped)
            {
                sleepStopped = false;
            }
            
        }
    }

    IEnumerator StartConfusionCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        confusionInCooldown = false;
    }

    IEnumerator StartFearCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        fearInCooldown = false;
    }
    IEnumerator StartFireCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        fireInCooldown = false;
    }
    IEnumerator StartFrostCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        frostInCooldown = false;
    }

    IEnumerator StartPoisonCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        poisonInCooldown = false;
    }

    IEnumerator StartSleepCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        sleepInCooldown = false;
    }
    public void ReceiveStatusCount(float damage, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.confusion:
                statusArray[(int)StatusType.confusion].statusCount += damage;
                break;
            case DamageType.fear:
                statusArray[(int)StatusType.fear].statusCount += damage;
                break;
            case DamageType.fire:
                statusArray[(int)StatusType.fire].statusCount += damage;
                break;
            case DamageType.frost:
                statusArray[(int)StatusType.frost].statusCount += damage;
                break;
            case DamageType.poison:
                statusArray[(int)StatusType.poison].statusCount += damage;
                break;
            case DamageType.sleep:
                statusArray[(int)StatusType.sleep].statusCount += damage;
                break;
            default:
                break;
        }
    }
    private void OnDestroy()
    {
        Destroy(_statusStack);
    }


    private void InitializeStatusArray()
    {
        
        statusArray[0] = new StatusData(StatusType.confusion, 0f, confusionIcon);
        statusArray[1] = new StatusData(StatusType.fear, 0f, fearIcon);
        statusArray[2] = new StatusData(StatusType.fire, 0f, fireIcon);
        statusArray[3] = new StatusData(StatusType.frost, 0f, frostIcon);
        statusArray[4] = new StatusData(StatusType.poison, 0f, poisonIcon);
        statusArray[5] = new StatusData(StatusType.sleep, 0f, sleepIcon);

    }
}
