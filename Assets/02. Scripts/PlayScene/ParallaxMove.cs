using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이 클래스는 게임오브젝트를 MainCamera 위치에 따라 Parallax 이동 하게 합니다.
/// </summary>
public class ParallaxMove : MonoBehaviour
{
    // ----- Reference ----- //
    Transform _mainCamTransform;
    // ----- Reference ----- //

    // ----- Info ----- //
    [Tooltip("카메라의 움직임 대비 저항도\n" +
        "1: 카메라와 함께 이동\n" +
        "0: 월드상에서 정지\n" +
        "(Update 기반)")]
    [Range(0f, 1f)]
    [SerializeField] float _resistance;
    // ----- Info ----- //

    // ----- Calculation ----- //
    Vector3 _prevCamPos = Vector3.zero;
    Vector3 _movedCamVec = Vector3.zero;
    // ----- Calculation ----- //


    private void Awake()
    {
        _mainCamTransform = Camera.main.transform;
        if (_mainCamTransform == null)
        {
            Debug.LogError("Main Camera Transfrom is missing!");
            return;
        }

        _resistance = Mathf.Clamp(_resistance, 0f, 1f);
    }
    private void Start()
    {
        _prevCamPos = _mainCamTransform.position;
    }

    private void LateUpdate()
    {
        MoveOnUpdate();
    }

    void MoveOnUpdate()
    {
        _movedCamVec = _mainCamTransform.position - _prevCamPos;
        _movedCamVec.z = 0f;

        transform.Translate(_movedCamVec * _resistance);

        _prevCamPos = _mainCamTransform.position;
    }
}
