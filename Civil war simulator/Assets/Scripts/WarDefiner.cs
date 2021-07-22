using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

public class WarDefiner : MonoBehaviour
{
    [Header("WarInfo")]
    public List<GameObject> Units = new List<GameObject>();
    List<UnitMovingController> unitMovingControllers = new List<UnitMovingController>();
    UnitMovingController unitInControll;
    public string GameCondition; //Active, Controll, pause

    [Header("Camera (Cinemachine)")]
    public CinemachineVirtualCamera virtualCamera;


    //Sync camera target as the UnitInControll
    void SyncUnitInControllInRefofCameraTarget()
    {
        unitInControll = virtualCamera.m_Follow.gameObject.GetComponent<UnitMovingController>();
    }


    //Method to switch camera target to next unit of the list
    public void SwitchCameraTargetUnitToNext()
    {
        //Getting Index of next unit
        int nextIndex = Units.IndexOf(virtualCamera.m_Follow.gameObject) + 1;

        if (Units.Count > nextIndex)
        {
            //Follow next unit of the list
            virtualCamera.m_Follow = Units[nextIndex].transform;
            virtualCamera.m_LookAt = Units[nextIndex].transform;
        }
        else
        {
            //Follow First unit of the list
            virtualCamera.m_Follow = Units[0].transform;
            virtualCamera.m_LookAt = Units[0].transform;
        }
        SyncUnitInControllInRefofCameraTarget();
    }


    //Method to switch camera target to previous unit of the list
    public void SwitchCameraTargetUnitToPrevious()
    {
        //Getting Index of previous unit
        int nextIndex = Units.IndexOf(virtualCamera.m_Follow.gameObject) - 1;

        if (nextIndex >= 0)
        {
            //Follow previous unit of the list
            virtualCamera.m_Follow = Units[nextIndex].transform;
            virtualCamera.m_LookAt = Units[nextIndex].transform;
        }
        else
        {
            //Follow Last unit of the list
            virtualCamera.m_Follow = Units[Units.Count-1].transform;
            virtualCamera.m_LookAt = Units[Units.Count - 1].transform;
        }
        SyncUnitInControllInRefofCameraTarget();
    }


    //Method to Remove a unit from both lists
    public void RemoveUnitFromLists(GameObject unit)
    {
        Units.Remove(unit);
        unitMovingControllers.Remove(unit.GetComponent<UnitMovingController>());

        //switch camera target if current target is the removing object
        if(unitInControll.gameObject == unit)
        {
            SwitchCameraTargetUnitToPrevious();
        }
    }


    //Method to turn GameCondition to Controll
    public void ToControllCondition()
    {
        //Make All Units Movable
        for(int i = 0; i < Units.Count; i++)
        {
            unitMovingControllers[i].CanMove = true;
            unitMovingControllers[i].PermittedToFire = false;
            Units[i].GetComponent<NavMeshAgent>().isStopped = true;
        }

        GameCondition = "Controll";
    }

    //Method to turn GameCondition to Controll
    public void ToActiveCondition()
    {
        //Make All Units Movable
        for (int i = 0; i < Units.Count; i++)
        {
            unitMovingControllers[i].CanMove = false;
            Units[i].GetComponent<NavMeshAgent>().isStopped = false;
        }

        GameCondition = "Active";
    }


    public void FirePermission()
    {
        //unitInControll.PermittedToFire = true;
        for (int i = 0; i < Units.Count; i++)
        {
            if (unitMovingControllers[i].IsSelected)
            {
                unitMovingControllers[i].PermittedToFire = true;
            }
        }
    }


    public void StandByCommand()
    {
        for (int i = 0; i < Units.Count; i++)
        {
            if (unitMovingControllers[i].IsSelected)
            {
                Units[i].GetComponent<NavMeshAgent>().SetDestination(Units[i].transform.position);
                unitMovingControllers[i].CanMove = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        //Obtaining Units
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for(int i = 0; i < units.Length; i++)
        {
            Units.Add(units[i]);
            unitMovingControllers.Add(units[i].GetComponent<UnitMovingController>());
        }

        //Defining Condition
        // GameCondition = "Controll";
        ToControllCondition(); 
    }


    // Update is called once per frame
    void Update()
    {
        //When active, check to determine when to return to Controll Condition
        if(GameCondition == "Active")
        {
            //Check if all the navigations were finished
            bool toControll = true;
            for(int i = 0; i < Units.Count; i++)
            {
                if (!(Vector3.Distance(Units[i].transform.position, Units[i].GetComponent<NavMeshAgent>().destination ) <= 1) )
                //if(Units[i].GetComponent<NavMeshAgent>().isStopped)
                {
                    toControll = false;
                }
            }

            if (toControll)
            {
                print("toControll");
                ToControllCondition();
            }
            
        }
    }
}
