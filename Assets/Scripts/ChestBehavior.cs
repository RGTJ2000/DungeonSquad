using System.Collections;
using UnityEngine;

public class ChestBehavior : MonoBehaviour
{
    private Transform lidPivot;
    private float openAngle = -80f;
    private float openSpeed = 4f;

    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private ReturnLinePlot _returnLinePlot;

    private void Start()
    {
        lidPivot = transform.Find("lidPivot");

        closedRotation = lidPivot.localRotation;
        openRotation = Quaternion.Euler(openAngle, 0f, 0f);

        _returnLinePlot = GetComponent<ReturnLinePlot>();

    }
    public void OpenChest()
    {
        if (!isOpen)
        {
            StartCoroutine(RotateLid(openRotation));
            DropManager.Instance.DropAllLoot(gameObject);
            gameObject.tag = "OpenedChest";

            _returnLinePlot.active_line = false;
            _returnLinePlot.target_obj = null;

            isOpen = true;
        }

    }

   
    IEnumerator RotateLid(Quaternion targetRotation)
    {
        while (Quaternion.Angle(lidPivot.localRotation, targetRotation) > 0.1f)
        {
            lidPivot.localRotation = Quaternion.Slerp(lidPivot.localRotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        lidPivot.localRotation = targetRotation;

    }

}
