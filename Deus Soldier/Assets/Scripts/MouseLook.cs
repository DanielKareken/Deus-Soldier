using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform playerRoot, lookRoot;
    [SerializeField] private bool invert;
    [SerializeField] private bool canUnlock = true;
    [SerializeField] private float mouseSensitivity = 5f;
    [SerializeField] private int smoothSteps = 10;
    [SerializeField] private float smoothWeight = 0.4f;
    [SerializeField] private float rollAngle = 0f;
    [SerializeField] private float rollSpeed = 3f;
    [SerializeField] private Vector2 defaultLookLimits = new Vector2(-85f, 85f);

    private Vector2 lookAngles;
    private Vector2 currentMouseLook;
    private Vector2 smoothMove;
    private float xRot = 0f;
    private float yRot = 0f;
    private float currentRollAngle;
    private int lastLookFrame;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleLockCursor();

        if(Cursor.lockState == CursorLockMode.Locked)
        {
            LookAround();
        }
    }

    void ToggleLockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    //allow camera to look around environment
    void LookAround()
    {
        /*currentMouseLook = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        lookAngles.x += currentMouseLook.x * mouseSensitivity * (invert ? 1f : -1f);
        lookAngles.y += currentMouseLook.y * mouseSensitivity;

        //cap look angle at given interval
        lookAngles.x = Mathf.Clamp(lookAngles.x, defaultLookLimits.x, defaultLookLimits.y);

        //currentRollAngle = Mathf.Lerp(currentRollAngle, Input.GetAxisRaw(MouseAxis.MOUSE_X) * rollAngle, Time.deltaTime * rollSpeed);

        lookRoot.localRotation = Quaternion.Euler(lookAngles.x, 0f, currentRollAngle);
        playerRoot.localRotation = Quaternion.Euler(0f, lookAngles.y, 0f);*/

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 5;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * 5;
        xRot = invert ? xRot + mouseY : xRot - mouseY;
        xRot = Mathf.Clamp(xRot, defaultLookLimits.x, defaultLookLimits.y);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerRoot.Rotate(Vector3.up * mouseX);
    }
}
