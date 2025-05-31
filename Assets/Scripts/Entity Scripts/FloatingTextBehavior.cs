using TMPro;
using UnityEngine;
using System.Collections;

public class FloatingTextBehavior : MonoBehaviour
{
    private Camera mainCamera;
    public Transform parentTransform; // Cache parent for position sync
    private Vector3 lastParentPosition;
    private Vector3 initialLocalOffset; // Store the starting offset
    private Vector3 activeFloatVector = Vector3.up;
    public float duration = 1.0f;

    private float floatSpeed = 2.0f;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No MainCamera found!");
            return;
        }
        //Debug.Log("transform position" + transform.position + "  parent position" + transform.parent.position);
        //parentTransform = transform.parent; // Store parent before unparenting
        //initialLocalOffset = transform.localPosition; // Capture offset before unparenting
        //transform.SetParent(null, false);


        initialLocalOffset = transform.position - parentTransform.position;
        lastParentPosition = transform.position;
        StartCoroutine(FloatAndFade());
    }

    // Update is called once per frame
    void LateUpdate()
    {
    
    }

    public void SetFloatVector(Vector3 floatVector)
    {
        activeFloatVector = floatVector;
        Debug.Log("Set float vector = " + activeFloatVector);
    }

    IEnumerator FloatAndFade()
    {
        //float duration = 1.0f; // Duration of the animation
        float elapsedTime = 0f;

       
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            
            
            // Sync position with parent, then float up in world space
            if (parentTransform != null)
            {
            

                transform.position = parentTransform.position + initialLocalOffset + activeFloatVector*floatSpeed*elapsedTime;
                lastParentPosition = parentTransform.position;

            }
            else
            {
                transform.position = lastParentPosition + initialLocalOffset + activeFloatVector * floatSpeed * elapsedTime;
            }
            

            // Face camera in world space
            if (mainCamera != null)
            {
               
                transform.rotation = mainCamera.transform.rotation;
            }

            TextMeshPro textMeshProComponent = GetComponent<TextMeshPro>();
            if (textMeshProComponent != null)
            {
                Color textColor = textMeshProComponent.color;
                textColor.a = 1 - (elapsedTime / duration); // Reduce alpha over time
                textMeshProComponent.color = textColor;
            }

            yield return null; // Wait for the next frame
        }
        Destroy(gameObject);
        
    }




}
