using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskSequencer
{
    public enum TaskType
    {
        Action,
        Coroutine
    }

    private class Task
    {
        TaskType _taskType;
        IEnumerator _coroutine;
        UnityAction _action;

        public TaskType TaskType => _taskType;
        public IEnumerator Coroutine => _coroutine;
        public UnityAction Action => _action;

        public Task(UnityAction action)
        {
            _action = action;
            _coroutine = null;
            _taskType = TaskType.Action;
        }

        public Task(IEnumerator coroutine)
        {
            _coroutine = coroutine;
            _action = null;
            _taskType = TaskType.Coroutine;
        }
    }

    Queue<Task> _taskQueue = new Queue<Task>();
    bool _isRunning = false;
    Coroutine _runningCoroutine;  // 현재 실행 중인 코루틴
    UnityAction _onComplete;
    MonoBehaviour _coroutineOwner;  // 코루틴을 실행할 객체

    public TaskSequencer(MonoBehaviour coroutineOwner, UnityAction onComplete = null)
    {
        _coroutineOwner = coroutineOwner;  // 코루틴을 실행할 객체 저장
        _onComplete = onComplete;
    }

    // 액션 추가
    public TaskSequencer AddAction(UnityAction action)
    {
        _taskQueue.Enqueue(new Task(action));
        return this;
    }

    // 코루틴 추가
    public TaskSequencer AddCoroutine(IEnumerator coroutine)
    {
        _taskQueue.Enqueue(new Task(coroutine));
        return this;
    }

    // 대기 추가
    public TaskSequencer AddWait(float waitTime)
    {
        _taskQueue.Enqueue(new Task(WaitForSeconds(waitTime)));
        return this;
    }

    // 시퀀스 실행
    public void Play()
    {
        if (!_isRunning && _taskQueue.Count > 0)
        {
            _isRunning = true;
            _runningCoroutine = _coroutineOwner.StartCoroutine(RunSequence());  // 코루틴 실행
        }
    }

    // 시퀀스 중지
    public void Stop()
    {
        if (_runningCoroutine != null)
        {
            _coroutineOwner.StopCoroutine(_runningCoroutine);  // 현재 실행 중인 코루틴 중지
            _isRunning = false;  // 중단 상태로 전환
        }
    }

    // 시퀀스 실행 코루틴
    private IEnumerator RunSequence()
    {
        while (_taskQueue.Count > 0)
        {
            var task = _taskQueue.Dequeue();

            switch (task.TaskType)
            {
                case TaskType.Action:
                    task.Action?.Invoke();  // 액션 실행
                    break;

                case TaskType.Coroutine:
                    yield return _coroutineOwner.StartCoroutine(task.Coroutine);  // 코루틴 실행
                    break;
            }
        }

        _onComplete?.Invoke();  // 완료 콜백 실행
        _isRunning = false;  // 시퀀스 실행 완료
    }

    // WaitForSeconds 대기 코루틴
    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
