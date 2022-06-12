using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform LookAt;
    public Transform CamTransform;
    [Space]
    [SerializeField]
    private float headBobAmount = 0.1f; // FirstPerson only
    [SerializeField]
    private float minCameraDistance = 4.0f;
    [SerializeField]
    private float maxCameraDistance = 12.0f;
    [SerializeField]
    private Vector2 camSensitivity = new Vector2(0.8f, 0.5f);
    [SerializeField]
    private bool enableFirstPerson = false;

    private Camera cam;
    private Player_TP playerSc;
    private float distance = 10.0f;
    private float scrollValue = 10.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float playerHeight;
    private float headMovement = 0.0f; // FirstPerson only
    private bool isFirstPerson = false; // FirstPerson only


    private void Start()
    {
        cam = Camera.main;
        playerSc = FindObjectOfType<Player_TP>();
        playerHeight = playerSc.PlayerHeight;
    }


    private void LateUpdate() 
    {
        isFirstPerson = distance < 1.0f;
        Vector3 playerHead = Vector3.zero;
        Vector3 dir = new Vector3(0,0,-distance);


        if (isFirstPerson) // scrolled in or out?
            playerHead = new Vector3(LookAt.position.x, LookAt.position.y + playerHeight + headMovement, LookAt.position.z);
        else
            playerHead = new Vector3(LookAt.position.x, LookAt.position.y + playerHeight, LookAt.position.z);
            
        
        // Stop player from looking 360 around x-Axis
        currentX = Mathf.Clamp(currentX, -65, 75);
        Quaternion rotation = Quaternion.Euler(currentX,currentY,0);

        CamTransform.position = playerHead + rotation * dir;
        
        if (cam.transform.position.y <= (LookAt.position.y - playerHeight/2)) 
            cam.transform.position = new Vector3(cam.transform.position.x,LookAt.position.y - playerHeight/2,cam.transform.position.z);
        
        CamTransform.LookAt(playerHead);
        
        playerSc.CamForwardDirection = rotation * Vector3.forward;
    }


    public void Look(Vector2 _v) 
    {
        //Debug.Log("Look: " + v);
        currentX -= _v.y * camSensitivity.y;
        currentY += _v.x * camSensitivity.x;
    }


    public void ZoomCam(Vector2 _v) 
    {
        Vector2 vN = _v.normalized;
        
        //Debug.Log("Zoom " + vN + " Distance = " + distance);
        scrollValue -= vN.y/1.5f;
        scrollValue = Mathf.Clamp(scrollValue, minCameraDistance-0.1f, maxCameraDistance);

        if (scrollValue < minCameraDistance && enableFirstPerson) 
            distance = 0.01f;
        else 
            distance = Mathf.Clamp(scrollValue, minCameraDistance, maxCameraDistance);
        
    }

    // Head bobbing while in first person view
    public void HeadMove(float _amount, bool _sprinting) // FirstPerson only
    {
        if (enableFirstPerson)
        {
            //Debug.Log(sprinting);
            _amount = Mathf.Clamp(_amount, 0,1);

            float stepLerp = _Utilities.MapRange(_amount, 0,1, Mathf.PI,Mathf.PI*2);
            float sinMove = Mathf.Sin(stepLerp); 
            float headBob = _sprinting ? 0.25f : headBobAmount;
            float camLerp = _Utilities.MapRange(sinMove, -1,1, -headBob,headBob);

            headMovement = Mathf.Sin(camLerp);
        }
    }
}
