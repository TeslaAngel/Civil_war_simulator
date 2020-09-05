using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovingController : MonoBehaviour
{
    private NavMeshAgent meshAgent;
    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    public bool IsSelected;
    public bool IsSkinnedMesh;
    public GameObject MeshObject;
    public Material Standard;
    public Material Outline;

    // Start is called before the first frame update
    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();

        if (IsSkinnedMesh)
        {
            skinnedMeshRenderer = MeshObject.GetComponent<SkinnedMeshRenderer>();
        }
        else
        meshRenderer = MeshObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Mouse Command
        if (Input.GetMouseButtonDown(0) && IsSelected==true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    IsSelected = false;
                    if(!IsSkinnedMesh)
                        meshRenderer.material=Standard;
                    else
                        skinnedMeshRenderer.material=Standard;
                    return;
                }
                meshAgent.SetDestination(hit.point);
                
            }
        }
        if (Input.GetMouseButtonDown(0) && IsSelected == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    IsSelected = true;
                    if (!IsSkinnedMesh)
                        meshRenderer.material=Outline;
                    else
                        skinnedMeshRenderer.material=Outline;
                }
            }
        }
        
    }
}
