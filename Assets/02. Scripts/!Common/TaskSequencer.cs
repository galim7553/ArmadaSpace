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
    Coroutine _runningCoroutine;  // ���� ���� ���� �ڷ�ƾ
    UnityAction _onComplete;
    MonoBehaviour _coroutineOwner;  // �ڷ�ƾ�� ������ ��ü

    public TaskSequencer(MonoBehaviour coroutineOwner, UnityAction onComplete = null)
    {
        _coroutineOwner = coroutineOwner;  // �ڷ�ƾ�� ������ ��ü ����
        _onComplete = onComplete;
    }

    // �׼� �߰�
    public TaskSequencer AddAction(UnityAction action)
    {
        _taskQueue.Enqueue(new Task(action));
        return this;
    }

    // �ڷ�ƾ �߰�
    public TaskSequencer AddCoroutine(IEnumerator coroutine)
    {
        _taskQueue.Enqueue(new Task(coroutine));
        return this;
    }

    // ��� �߰�
    public TaskSequencer AddWait(float waitTime)
    {
        _taskQueue.Enqueue(new Task(WaitForSeconds(waitTime)));
        return this;
    }

    // ������ ����
    public void Play()
    {
        if (!_isRunning && _taskQueue.Count > 0)
        {
            _isRunning = true;
            _runningCoroutine = _coroutineOwner.StartCoroutine(RunSequence());  // �ڷ�ƾ ����
        }
    }

    // ������ ����
    public void Stop()
    {
        if (_runningCoroutine != null)
        {
            _coroutineOwner.StopCoroutine(_runningCoroutine);  // ���� ���� ���� �ڷ�ƾ ����
            _isRunning = false;  // �ߴ� ���·� ��ȯ
        }
    }

    // ������ ���� �ڷ�ƾ
    private IEnumerator RunSequence()
    {
        while (_taskQueue.Count > 0)
        {
            var task = _taskQueue.Dequeue();

            switch (task.TaskType)
            {
                case TaskType.Action:
                    task.Action?.Invoke();  // �׼� ����
                    break;

                case TaskType.Coroutine:
                    yield return _coroutineOwner.StartCoroutine(task.Coroutine);  // �ڷ�ƾ ����
                    break;
            }
        }

        _onComplete?.Invoke();  // �Ϸ� �ݹ� ����
        _isRunning = false;  // ������ ���� �Ϸ�
    }

    // WaitForSeconds ��� �ڷ�ƾ
    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
