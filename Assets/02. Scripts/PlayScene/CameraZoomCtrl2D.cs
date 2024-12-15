using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoomCtrl2D : MonoBehaviour
{
    [SerializeField] float _minCameraSize = 2.5f;
    [SerializeField] float _maxCmaeraSize = 10.0f;
    [SerializeField] float _zoomSpeed = 1.0f;

    Camera _camera;

    float _originOrthographicSize = 5.0f;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera.orthographic == false)
            Debug.LogError($"This Camera is not orthographic. gameObject name : {gameObject.name}");
        _originOrthographicSize = _camera.orthographicSize;
    }
    private void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > Util.EPSILON)
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - _zoomSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime,
                _minCameraSize, _maxCmaeraSize);
    }
    public void ResetZoom()
    {
        _camera.orthographicSize = _originOrthographicSize;
    }
}
