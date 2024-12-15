using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveCtrl2D : MonoBehaviour
{
    [SerializeField] KeyCode _upKeyCode;
    [SerializeField] KeyCode _downKeyCode;
    [SerializeField] KeyCode _rightKeyCode;
    [SerializeField] KeyCode _leftKeyCode;

    [SerializeField] Vector2 _moveSpeed;

    [SerializeField] float _focusMoveSpeed;

    bool _isLimited = false;
    Vector2 _maxPos;
    Vector2 _minPos;

    Vector3 _pos;

    bool _isFreezed = false;

    Vector3 _originPos = Vector3.zero;
    private void Awake()
    {
        _originPos = transform.position;
    }

    public void SetLimit(Vector2 maxPos, Vector2 minPos)
    {
        _maxPos = maxPos;
        _minPos = minPos;

        _isLimited = true;
    }

    private void Update()
    {
        if (_isFreezed == true)
            return;

        if (_isFocusMoving == true)
            return;

        if(Input.GetKey(_upKeyCode))
            transform.Translate(Vector3.up * _moveSpeed.y * Time.deltaTime);
        if (Input.GetKey(_downKeyCode))
            transform.Translate(Vector3.down * _moveSpeed.y * Time.deltaTime);
        if(Input.GetKey(_rightKeyCode))
            transform.Translate(Vector3.right * _moveSpeed.x * Time.deltaTime);
        if (Input.GetKey(_leftKeyCode))
            transform.Translate(Vector3.left * _moveSpeed.x * Time.deltaTime);

        if(_isLimited == true)
        {
            _pos = transform.position;
            _pos.x = Mathf.Clamp(_pos.x, _minPos.x, _maxPos.x);
            _pos.y = Mathf.Clamp(_pos.y, _minPos.y, _maxPos.y);
            transform.position = _pos;
        }
    }

    public void ResetPos()
    {
        if (_isFreezed == true)
            return;
        if (_moveToFocusedPos != null)
        {
            StopCoroutine(_moveToFocusedPos);
            _moveToFocusedPos = null;
        }
        transform.position = _originPos;
    }

    public void Freeze(bool value)
    {
        _isFreezed = value;
    }
    public void Focus(Vector2 pos, bool delayed = false)
    {
        if (delayed == false)
        {
            _pos = transform.position;
            _pos.x = pos.x;
            _pos.y = pos.y;
            transform.position = _pos;
        }
        else
        {
            if (_moveToFocusedPos != null)
            {
                StopCoroutine(_moveToFocusedPos);
                _moveToFocusedPos = null;
            }
            _moveToFocusedPos = StartCoroutine(MoveToFocusedPos(pos));
        }
    }

    Coroutine _moveToFocusedPos;
    bool _isFocusMoving = false;
    IEnumerator MoveToFocusedPos(Vector2 pos)
    {
        _isFocusMoving = true;
        _pos = transform.position;
        Vector3 targetPos = new Vector3(pos.x, pos.y, _pos.z);
        while (Vector3.Distance(_pos, targetPos) > Util.EPSILON)
        {
            yield return null;
            _pos = transform.position;
            transform.position = Vector3.MoveTowards(_pos, targetPos, _focusMoveSpeed * Time.deltaTime);
        }
        _isFocusMoving = false;
    }
}
