
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Player _Player;
    [SerializeField] private float _SmoothMovementSpeed = 0.3f;
    [SerializeField] private float _SmoothDragMovementSpeed = 5f;
    [SerializeField] private float _SmoothViewMovement = 10f;
    [SerializeField] private float _CameraMovementUnit = 5f;
    [SerializeField] private float _MinFov = 5f;
    [SerializeField] private float _MaxFov = 50f;
    [SerializeField] private float _Sensitivity = 20f;
    [SerializeField] private float _PanLimitX = 200f;
    [SerializeField] private float _PanLimitY = 200f;
    [SerializeField] private float _RotationSpeed = 10f;
    [SerializeField] private float _RotationSpeedCalvin = 3f;

    private bool _CenterOnCharacter = true;
    private bool _IsTopDown = false;
    private float _AngleRotated;
    private bool _IsCameraMoving = false;


    private void LateUpdate()
    {
        // adding zoom in and zoom out 
        float fov = Camera.main.fieldOfView;
        float targetFov = fov;
        targetFov -= Input.GetAxis("Mouse ScrollWheel") * _Sensitivity;
        targetFov = Mathf.Clamp(targetFov, _MinFov, _MaxFov);
        Camera.main.fieldOfView = Mathf.Lerp(fov, targetFov, _SmoothViewMovement * Time.deltaTime);

        Vector3 cameraPosition = transform.position;

        if (!_IsTopDown)
        {
            if (Input.GetKey("q"))
            {
                transform.Rotate(0, _RotationSpeedCalvin, 0);
            }

            if (Input.GetKey("e"))
            {
                transform.Rotate(0, -_RotationSpeedCalvin, 0);
            }
        }

        if (Input.GetKey("w"))
        {
            // cameraPosition.z += _CameraMovementUnit;
            // cameraPosition.x -= _CameraMovementUnit;
            cameraPosition += transform.forward;
            _CenterOnCharacter = false;
        }
        if (Input.GetKey("a"))
        {
            cameraPosition -= transform.right   ;
            // cameraPosition.x -= _CameraMovementUnit;
            // cameraPosition.z -= _CameraMovementUnit;
            _CenterOnCharacter = false;
        }
        if (Input.GetKey("s"))
        {
            cameraPosition -= transform.forward;
            // cameraPosition.z -= _CameraMovementUnit;
            // cameraPosition.x += _CameraMovementUnit;
            _CenterOnCharacter = false;
        }
        if (Input.GetKey("d"))
        {
            cameraPosition += transform.right;
            // cameraPosition.x += _CameraMovementUnit;
            // cameraPosition.z += _CameraMovementUnit;
            _CenterOnCharacter = false;
        }
        if (Input.GetButtonDown("Jump"))
        {
            _CenterOnCharacter = true;
        }
        if (Input.GetKeyDown(KeyCode.V) && !_IsTopDown && !_IsCameraMoving)
        {
            StartCoroutine(RotateCameraToTopDown());

        }
        if (Input.GetKeyDown(KeyCode.V) && _IsTopDown && !_IsCameraMoving)
        {
            StartCoroutine(RotateCameraBack());

        }

        if (!_CenterOnCharacter)
        {
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, -_PanLimitX, _PanLimitX);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, -_PanLimitY, _PanLimitY);

            transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPosition, _SmoothDragMovementSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 target = _Player.transform.position;
            // create a smooth camera movement
            transform.position = Vector3.Lerp(transform.position, target, _SmoothMovementSpeed * Time.deltaTime);
        }
    }

    IEnumerator RotateCameraBack()
    {
        _IsCameraMoving = true;
        while (_AngleRotated <= 45)
        {
            transform.Rotate(new Vector3(-1f, 0f, 0f), _RotationSpeed * Time.deltaTime);
            _AngleRotated += _RotationSpeed * Time.deltaTime;
            yield return null;

        }
        _AngleRotated = 0f;
        _IsCameraMoving = false;
        _IsTopDown = false;
    }

    IEnumerator RotateCameraToTopDown()
    {
        _IsCameraMoving = true;
        while (_AngleRotated <= 45)
        {

            transform.Rotate(new Vector3(1f, 0f, 0f), _RotationSpeed * Time.deltaTime);
            _AngleRotated += _RotationSpeed * Time.deltaTime;
            yield return null;
        }
        _AngleRotated = 0f;
        _IsCameraMoving = false;
        _IsTopDown = true;
    }

    public void SetCenterToCharacter()
    {
        _CenterOnCharacter = true;
    }

}
