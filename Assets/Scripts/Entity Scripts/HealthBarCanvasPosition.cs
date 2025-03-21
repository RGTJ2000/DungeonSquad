using UnityEngine;

public class HealthBarCanvasPosition : MonoBehaviour
{

    private Camera _mainCamera;
    private Vector3 offset_vector;
    private GameObject entity_to_follow;

    Quaternion inverseRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCamera = Camera.main;
        offset_vector = new Vector3(0f, 1.25f, 0f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = _mainCamera.transform.rotation;
        //transform.rotation = Quaternion.Euler(_mainCamera.transform.rotation.x, _mainCamera.transform.rotation.y -transform.parent.rotation.y, _mainCamera.transform.rotation.z);
       // transform.rotation = _mainCamera.transform.rotation;
      if (entity_to_follow != null )
        {
            transform.position = entity_to_follow.transform.position + offset_vector;

        }
       

    }

    public void SetEntityToFollow(GameObject entity)
    {
        entity_to_follow = entity;
    }
}
