using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/*
    carries the object held by the player.
    allows this to be thrown.
    For 3rd Person
*/
[RequireComponent(typeof(Player_TP), typeof(InteractionChecker))]
public class ObjectCarrier : MonoBehaviour
{
    public Transform CarrierTransform;

    [SerializeField] private bool isCarrying = false;
    [SerializeField] private float throwPower = 0;
    [SerializeField] private int lerpValue = 50;
    [SerializeField] private int maxThrowPower = 50;
    private Player_TP player;
    private Transform carriedObj;
    private Quaternion carriedObjRot;
    private Mouse input;
    private Keyboard keyB;
    private Vector3 carrierPos;
    private bool inputPress = false;
    private bool fireIsHeld;
    private bool throwThisShit = false;
    private float timeToHold = 0;
    private float inc = 0;


    void Start()
    {
        player = GetComponent<Player_TP>();
        if (CarrierTransform == null) Debug.Log("Player has no carrier transform");
        input = Mouse.current;
        keyB = Keyboard.current;
        carrierPos = CarrierTransform.localPosition;
    }


    void Update() 
    {
        if (isCarrying)
        {
            // throw behaviour // xxx currently in the style of "Broken Time" better like LoZ
            if (fireIsHeld)
            {
                //throwPower = Mathf.PingPong(inc+=lerpValue*Time.deltaTime, 50); // for panning 
                throwPower = Mathf.Clamp(inc+=lerpValue*Time.deltaTime, 0,maxThrowPower);
                if (input.leftButton.wasReleasedThisFrame)
                {
                    if (throwPower > 1) throwThisShit = true;
                    inc = 0;    
                }
            }

            // let go of object by clicking " Right Mouse Button "
            // if (keyB.eKey.wasReleasedThisFrame) LetGoOfObject();


            // Checks if action button is held for a specific time
            ButtonIsHeld(0.3f);

            // xxx allow players to hold the object above them. 
            // move carrier transform with player look direction
            /* float heightOffset = _Utilities.MapRange(player.bodyForward.y, -0.2f, 0.99f, 0,1);
            heightOffset = Mathf.Clamp(heightOffset, 0, 1);
            CarrierTransform.localPosition = new Vector3(carrierPos.x, carrierPos.y + heightOffset*4, carrierPos.z); */
            CarrierTransform.localPosition = new Vector3(carrierPos.x, carrierPos.y, carrierPos.z);
            //Debug.Log("Carrier adding : " + player.lookDirection.y + " = " + heightOffset);

            // move carrier transform back and forth if charging a throw 
            CarrierTransform.position -= transform.TransformDirection(Vector3.forward) * throwPower/100;

        }

    }


    void FixedUpdate() 
    {
        //Push carried object away
        if (throwThisShit)
        {
            Debug.Log("throwPower in ObjectCarrier : " + throwPower);
            Vector3 throwDir = transform.TransformDirection(Vector3.forward) + (Vector3.up * 0.5f);
            isCarrying = false;
            carriedObj.GetComponent<Portable>().isLifted = false;
            carriedObj.GetComponent<Rigidbody>().AddForce(
                throwDir * throwPower + player.BodyVelocity, ForceMode.Impulse);
            throwThisShit = false;
            throwPower = 0;
        }
    }

    // already carring? -> let go, else check
    public void CarrierHandler(GameObject _obj) 
    {
        if (isCarrying) 
        {   
            //if (heldValue > 0) ThrowObject(heldValue); 
            //else LetGoOfObject();
        }
        else if(_obj != null) LiftCheck(_obj);
    }


    public void LetGoOfObject() 
    {
        isCarrying = false;
        carriedObj.GetComponent<Portable>().isLifted = false;
    }

    public bool IsCarrying => isCarrying;

    // check if liftable Object is infront of player ? lift : nothing
    void LiftCheck(GameObject _obj)
    {
        if (_obj.GetComponent<Portable>() != null)
        {
            if (_obj.GetComponent<Portable>().isLiftable && !isCarrying)
            {
                if (!isCarrying) CarryObject(_obj.transform);
            }
        }
        //Debug.Log("p rotation: " + transform.eulerAngles + "\n obj rotation 1: " + obj.transform.eulerAngles);
    }

    // Sets object to lifting
    void CarryObject(Transform _target) 
    {
        //Debug.Log("is Liftig");
        carriedObj = _target;
        carriedObjRot = carriedObj.rotation;
        isCarrying = true;
        carriedObj.GetComponent<Portable>().isLifted = true;
        carriedObj.GetComponent<Portable>().endPos = transform.localPosition;
        carriedObj.GetComponent<Portable>().initRotation = carriedObjRot;
    }

    // Checks if action button is held for a specific time
    void ButtonIsHeld(float heldThreshold)
    {
        // left mouse button is pressed
        if (input.leftButton.wasPressedThisFrame) inputPress = true;
        // left mouse button is released
        else if (input.leftButton.wasReleasedThisFrame) 
        {
            fireIsHeld = false;
            inputPress = false;
            timeToHold = 0;
        }

        // if left mouse button is pressed count up to "heldThreshold"
        // if bigger -> the button is held
        if (inputPress == true && fireIsHeld == false)
        {
            timeToHold += Time.deltaTime;
            if (timeToHold >= heldThreshold) fireIsHeld = true;
        }
    }
}
