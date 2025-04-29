using System.Net;
using UnityEditor.PackageManager;
using UnityEngine;

public class ReturnLinePlot : MonoBehaviour
{

    //public LineRenderer _returnLineRenderer;
    private LineRenderer _returnLineRenderer;
    public bool active_line = false;
    public GameObject target_obj = null;

    public float visibleDistance = 50f;




    LayerMask layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject lineObj = new GameObject("OtherLineRenderer");
        lineObj.transform.SetParent(transform, false); // Parent to this object
        _returnLineRenderer = lineObj.AddComponent<LineRenderer>();
        _returnLineRenderer.enabled = false;

        // Configure the LineRenderer
        _returnLineRenderer.startWidth = 0.1f;      // Set the start width of the line
        _returnLineRenderer.endWidth = 0.1f;        // Set the end width of the line
        _returnLineRenderer.useWorldSpace = true;
        _returnLineRenderer.numCapVertices = 10;
        // Set the number of points in the line
        _returnLineRenderer.positionCount = 2;

        int layer1 = LayerMask.NameToLayer("Characters");
        layerMask = (1 << layer1);

    }

    private void Update()
    {
        if (active_line && target_obj != null)
        {



            //RaycastAll to target to check if visible, if visible, then turn on line rendered and plotline

            RaycastHit[] hits_info = (Physics.RaycastAll(transform.position, target_obj.transform.position - transform.position, visibleDistance));

            if (hits_info.Length > 0)
            {
                System.Array.Sort(hits_info, (a, b) => (a.distance.CompareTo(b.distance)));

                bool hit_wall = false;
                bool hit_target_before_wall = false;
                bool hit_target = false;

                for (int i = 0; i < hits_info.Length; i++)
                {
                    if (hits_info[i].transform.gameObject.CompareTag("Wall"))
                    {
                        hit_wall = true;

                    }
                    if (hits_info[i].transform.gameObject == target_obj)
                    {
                        hit_target = true;

                        if (!hit_wall)
                        {
                            hit_target_before_wall = true;

                        }
                    }


                }

                if (hit_target && hit_target_before_wall)
                {
                    _returnLineRenderer.enabled = true;
                    PlotActiveLine(target_obj.transform.position);
                }
                else
                {
                    //if not set active line to false and target_obj to null
                    _returnLineRenderer.enabled = false;
                    active_line = false;
                }


            }
            else //if no hits from Raycast
            {
                _returnLineRenderer.enabled = false;
                active_line = false;
            }
         

        }
        else
        {
           // Debug.Log("ReturnlinePlot - disabling ReturnLineRendere");
            //if no hit at all then set active line to false and target_obj to null
            _returnLineRenderer.enabled = false;
            active_line = false;
        }

    }


    public void SetLineMaterial(Material material)
    {
        _returnLineRenderer.material = material;
    }
    public void SetReturnLineWidth(float thisTargetWidth, float originWidth)
    {
        _returnLineRenderer.startWidth = thisTargetWidth;
        _returnLineRenderer.endWidth = originWidth;
    }
  
    public void PlotActiveLine(Vector3 target_position)
    {
        _returnLineRenderer.SetPosition(0, transform.position);
        _returnLineRenderer.SetPosition(1, target_position);
    }

    public void SetSolidLine(Material material)
    {
        _returnLineRenderer.material = material;
        _returnLineRenderer.textureMode = LineTextureMode.Stretch;
    }

    public void SetDottedLine(Material material)
    {
        _returnLineRenderer.material = material;
        _returnLineRenderer.textureMode = LineTextureMode.Tile;
    }
}
