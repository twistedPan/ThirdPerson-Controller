using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Portable : MonoBehaviour
{
    public bool isLiftable = true;
    public bool isLifted = false;
    public bool isCounted = false;
    public float weight = 0;
    public Quaternion initRotation = Quaternion.identity;
    private Transform playerCarryTrans;
    private ObjectCarrier carrierSc;
    private Rigidbody rb;
    private bool hasChild = false;
    private Vector3 origin;
    public Vector3 endPos; // end pos at carrier


    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        playerCarryTrans = FindObjectOfType<ObjectCarrier>().CarrierTransform;
        hasChild = transform.childCount > 0;
        carrierSc = FindObjectOfType<ObjectCarrier>();
    }


    private void Start() 
    {
        origin = transform.position;
        if(weight == 0) 
            weight = Random.Range(1.0f, 3.0f);
        //weight = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 4;
    }


    private void Update() 
    {
        if (isLifted) // Move with Player if carried
            GetCarried();
        else if (rb.useGravity == false) 
            rb.useGravity = true;


        // Return to origin if out of level xxx testing
        if(transform.position.y <= -20) 
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = origin;
        }
    }


    private void FixedUpdate() 
    {
        // fall physics
        if (rb.velocity.y < 0 && !isLifted) 
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (weight - 1) * Time.deltaTime;
        } 
        else if (rb.velocity.y == 0)
        {
            rb.velocity = Vector3.zero;
        }
    }


    public void StopCarried()
    {
        carrierSc.LetGoOfObject();
    }


    private void GetCarried() 
    {
        Vector3 objSize = transform.localScale;
        if (hasChild) 
            objSize = transform.GetChild(0).localScale; // if it has a body gameObject

        // stop right there criminal scum
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        
        // set distance to player according to localScale
        float longSide = objSize.x > objSize.z ? objSize.x : objSize.z;
        //if (longSide < 2.5f) longSide = 2.5f;
        //float longSide = objSize.z; // currently allways facing to z

        Vector3 facing = playerCarryTrans.transform.position + 
            Vector3.Scale(transform.forward,new Vector3(longSide/2, 0, longSide/2));

        // snap to carrier position
        //transform.position = new Vector3(facing.x, objSize.y/2 + facing.y, facing.z);

        // lerp to carrier position
        endPos = new Vector3(facing.x, objSize.y/2 + facing.y, facing.z);
        transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime*25);

        transform.rotation = playerCarryTrans.parent.rotation;

    }

    private IEnumerator Move_Routine(Transform transform, Vector3 from, Vector3 to)
    {
        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime*16;
            transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
