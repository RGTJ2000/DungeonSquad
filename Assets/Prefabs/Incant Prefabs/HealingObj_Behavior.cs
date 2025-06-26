using System.Collections;
using UnityEngine;

public class HealingObj_Behavior : MonoBehaviour
{
    GameObject target;
    float healAmount;
    private ParticleSystem ps;

    void Awake()
    {
        // Cache the particle system on the object (assumes it's on the same GameObject)
        ps = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (target != null)
        {
            gameObject.transform.position = target.transform.position;
        }
        else
        {
            StopAndDestroyAfterParticles();
        }
    }
    public void SetAndStartLifetime(GameObject _target, float time, float _healAmount)
    {
        target = _target;
        healAmount = _healAmount;
        StartCoroutine(StartLifetime(time));
    }

    IEnumerator StartLifetime(float _time)
    {
        yield return new WaitForSeconds(_time);
        target.GetComponent<Health>().Heal(healAmount);
        StopAndDestroyAfterParticles();
    }

    private void StopAndDestroyAfterParticles()
    {
        if (ps != null && ps.isPlaying)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting); // Stop emission
            float lifetime = ps.main.duration + ps.main.startLifetime.constantMax;
            StartCoroutine(WaitAndDestroy(lifetime));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator WaitAndDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }


}
