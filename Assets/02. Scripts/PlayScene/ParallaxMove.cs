using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� Ŭ������ ���ӿ�����Ʈ�� MainCamera ��ġ�� ���� Parallax �̵� �ϰ� �մϴ�.
/// </summary>
public class ParallaxMove : MonoBehaviour
{
    // ----- Reference ----- //
    Transform _mainCamTransform;
    // ----- Reference ----- //

    // ----- Info ----- //
    [Tooltip("ī�޶��� ������ ��� ���׵�\n" +
        "1: ī�޶�� �Բ� �̵�\n" +
        "0: ����󿡼� ����\n" +
        "(Update ���)")]
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
