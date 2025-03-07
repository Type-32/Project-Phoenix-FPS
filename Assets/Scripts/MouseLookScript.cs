using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLookScript : MonoBehaviour
{

    [SerializeField] PlayerControllerManager player;
    private Vector2 MouseInput;
    private float mouseX;
    private float mouseY;
    public float mouseSensitivityValve;
    private float regularSensitivity;
    private float aimingSensitivity;
    [SerializeField] Camera itemLayerCamera;
    public Camera playerMainCamera;
    public Camera minimapCamera;
    [SerializeField] GameObject cameraHolder;
    //float xRot = 0f;

    void Start()
    {
        if (!player.pv.IsMine)
        {
            Destroy(minimapCamera.gameObject);
            Destroy(itemLayerCamera.gameObject);
            return;
        }
        else
        {
            playerMainCamera.fieldOfView = player.stats.cameraFieldOfView;
            Cursor.lockState = CursorLockMode.Locked;
            mouseSensitivityValve = player.stats.mouseSensitivity;
            mouseX = 0f;
            mouseY = 0f;
            regularSensitivity = mouseSensitivityValve;
            aimingSensitivity = mouseSensitivityValve / 1.3f;
        }
    }
    public void ResetAimingSensitivity(float sensitivity)
    {
        regularSensitivity = sensitivity;
        aimingSensitivity = sensitivity / 1.3f;
    }
    public void SetPlayerFOV(float fov)
    {
        playerMainCamera.fieldOfView = fov;
        player.stats.cameraFieldOfView = fov;
    }
    void Update()
    {
        if (!player.pv.IsMine) return;
        CameraInput();
        CameraMovement();
    }
    void CameraInput()
    {
        
        if (player.stats.isAiming)
        {
            mouseSensitivityValve = aimingSensitivity;
        }
        else
        {
            mouseSensitivityValve = regularSensitivity;
        }
        if (player.stats.mouseMovementEnabled)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityValve * Time.deltaTime;
            mouseY += Input.GetAxisRaw("Mouse Y") * mouseSensitivityValve * Time.deltaTime;
        }
        mouseY = Mathf.Clamp(mouseY, player.stats.ClampCamRotX, player.stats.ClampCamRotZ);
        //xRot -= mouseY;
        //xRot = Mathf.Clamp(xRot, player.stats.ClampCamRotX, player.stats.ClampCamRotZ);
    }
    void CameraMovement()
    {
        if (player.stats.mouseMovementEnabled)
        {
            transform.localRotation = Quaternion.Euler(-mouseY, 0, 0);
            player.transform.Rotate(Vector3.up * mouseX);
            //itemLayerCamera.transform.localRotation = transform.localRotation;
        }
    }
    void CameraRotationClamp(float y, float x, float z)
    {
        mouseY = Mathf.Clamp(y, x, z);
    }
}
