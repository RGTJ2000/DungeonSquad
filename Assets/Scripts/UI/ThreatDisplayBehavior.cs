using TMPro;
using UnityEngine;

public class ThreatDisplayBehavior : MonoBehaviour
{
    private Camera _mainCamera;
    public GameObject objectToTrack = null;
    private Vector3 verticalOffset = new Vector3(0, 2.5f, 0);
    private ThreatTracker _threatTracker = null;

    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

   

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _mainCamera.transform.rotation;
        if (objectToTrack != null && _threatTracker != null)
        {
            transform.position = objectToTrack.transform.position + verticalOffset;

            int threatLevel = (int)_threatTracker.GetThreatLevel(objectToTrack);

            _textMeshProUGUI.text = threatLevel.ToString();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetObjectToTrack(GameObject obj, ThreatTracker tracker)
    {
        objectToTrack = obj;
        _threatTracker = tracker;
    }
}
