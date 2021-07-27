using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovingController : MonoBehaviour
{
    
    private NavMeshAgent meshAgent;
    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private Animator animator;

    //[Space]
    [Header("AgentSettings")]
    //public float MotionGap;
    //private float MotionGapCalculator = 0;
    public float MaxTravelDistance;
    public bool CanMove;

    //[Space]
    [Header("MeshSettings")]
    public bool IsSkinnedMesh;
    public GameObject MeshObject;
    public Material Standard;
    public Material Outline;

    [Header("GameSettings")]
    public string Player;
    public Transform Target;
    [Space]
    public bool PermittedToFire;
    public bool IsSelected;
    public GameObject DistanceIndicator;
    public GameObject RangeIndicator;

    [Header("WeaponsSettings")]
    public List<WeaponTurretScript> Weapons = new List<WeaponTurretScript>();


    // Start is called before the first frame update
    void Start()
    {
        //Obtaining NavMeshAgent
        meshAgent = GetComponent<NavMeshAgent>();
        
        //Obtaining Animator
        animator = GetComponent<Animator>();

        //Getting Mesh Component
        //Defining whatever the object is using SkinnedMeshRenderer or not
        if (IsSkinnedMesh)
        {
            skinnedMeshRenderer = MeshObject.GetComponent<SkinnedMeshRenderer>();
        }
        else
        meshRenderer = MeshObject.GetComponent<MeshRenderer>();

        //Hide DistanceIndicator
        DistanceIndicator.SetActive(false);
        //Set the scale of DI in accordance of MaxTravelDistance
        DistanceIndicator.transform.localScale = new Vector3(MaxTravelDistance, DistanceIndicator.transform.localScale.y, MaxTravelDistance);

        //Assigning Weapons
        WeaponTurretScript[] weaponTurretScripts = transform.GetComponentsInChildren<WeaponTurretScript>(true);
        for(int i = 0; i<weaponTurretScripts.Length; i++)
        {
            Weapons.Add(weaponTurretScripts[i]);
        }

    }



    // Update is called once per frame
    void Update()
    {

        #region Mouse Command
        if (Input.GetMouseButtonDown(0) && IsSelected==true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //if Mouse Command is deselecting the object
                if (hit.collider.gameObject == gameObject)
                {
                    IsSelected = false;
                    if(!IsSkinnedMesh)
                        meshRenderer.material=Standard;
                    else
                        skinnedMeshRenderer.material=Standard;

                    DistanceIndicator.SetActive(false);
                    RangeIndicator.SetActive(false);

                    return;
                }
                //if Object is not movable
                if (!CanMove)//if (MotionGapCalculator > 0)
                    return;
                //if Target Location is too far
                if (Vector3.Distance(transform.position, hit.point) > MaxTravelDistance)
                    return;
                meshAgent.SetDestination(hit.point);
                CanMove = false; //MotionGapCalculator = MotionGap;

                //if targeting an enemy
                if (PermittedToFire && hit.collider.gameObject.GetComponent<UnitMovingController>())
                {
                    if (hit.collider.gameObject.GetComponent<UnitMovingController>().Player != Player)
                    {
                        Target = hit.collider.gameObject.transform;
                    }
                }
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
                        meshRenderer.material=new Material(Outline);
                    else
                        skinnedMeshRenderer.material= new Material(Outline);

                    DistanceIndicator.SetActive(true);
                    RangeIndicator.SetActive(true);
                }
            }
        }
        #endregion

        #region Mesh Command
        /*
        //Calculating Motion Gap
        if (MotionGapCalculator > 0)
        {
            MotionGapCalculator -= Time.deltaTime;
        }
        else if (MotionGapCalculator < 0)
        {
            MotionGapCalculator = 0;
        }

        //Visualizing Time Gap Calculator

        float FFvalue = 1 - (1 / MotionGap) * MotionGapCalculator;
        
        if (!IsSkinnedMesh && IsSelected)
        {
            meshRenderer.material.SetFloat("Vector1_287BDE35", FFvalue);
        }
        if (IsSkinnedMesh && IsSelected)
        {
            skinnedMeshRenderer.material.SetFloat("Vector1_287BDE35", FFvalue);
        }
        */

        //Visualizing if the unit is still able to move
        float FFvalue;
        if (CanMove)
            FFvalue = 1;
        else
            FFvalue = 0.1f;

        if (!IsSkinnedMesh && IsSelected)
        {
            meshRenderer.material.SetFloat("Vector1_287BDE35", FFvalue);
        }
        if (IsSkinnedMesh && IsSelected)
        {
            skinnedMeshRenderer.material.SetFloat("Vector1_287BDE35", FFvalue);
        }
        #endregion

        #region Animator Command
        //Animator Sync
        animator.SetBool("FireReady", PermittedToFire);
        #endregion
    }
}
