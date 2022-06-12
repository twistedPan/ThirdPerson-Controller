using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Handles Player interaction with world
    checks if object infront of player is
        - portable 
*/
public class InteractionChecker : MonoBehaviour
{
    [SerializeField] private float actionDistance = 4.0f;
    private Player_TP playerSc;
    private ObjectCarrier carrierSc;
    private GameObject target = null;


    void Start()
    {
        playerSc = GetComponent<Player_TP>();
        carrierSc = GetComponent<ObjectCarrier>();
    }

    // checks if action is possible by casting a ray along the camera.forward axis
    public void ActionCheck()
    {
        // the position of the players head 
        Vector3 playerPos = transform.position + Vector3.up*playerSc.PlayerHeight;
        // ray cast along camera forward direction 
        Debug.DrawRay(playerPos, playerSc.CamForwardDirection*actionDistance, Color.yellow, 0.2f);

        RaycastHit hit;
        if (Physics.Raycast(playerPos, playerSc.CamForwardDirection*actionDistance, out hit, actionDistance))
        {
            //Debug.Log("Hit : " + hit.transform.name);
            if (hit.transform.gameObject.GetComponent<Portable>() != null) 
            {
                target = hit.transform.gameObject;
                if (hit.transform.GetComponent<Portable>() != null) 
                    carrierSc.CarrierHandler(target);
            }
            else 
                target = null;
        }
        else if (carrierSc.IsCarrying)
                carrierSc.LetGoOfObject();
    }

}
