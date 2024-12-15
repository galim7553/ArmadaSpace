using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Util
{
    // ���� ���
    public static class Ran
    {
        /// <summary>
        /// Ȯ���� ���� ���õ� Index�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="probs"></param>
        /// <returns></returns>
        public static int Choose(IList<float> probs)
        {
            float total = 0;

            foreach (float prob in probs)
            {
                if (prob > 0f)
                    total += prob;
            }

            float ranVal = Random.value * total;

            for (int i = 0; i < probs.Count; i++)
            {
                if (probs[i] <= 0f) continue;

                if (ranVal <= probs[i])
                    return i;
                else
                    ranVal -= probs[i];
            }

            return 0;
        }

        /// <summary>
        /// List�� ������ ������ �����մϴ�.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int k = Random.Range(i, n);
                T temp = list[k];
                list[k] = list[i];
                list[i] = temp;
            }
        }
    }

    


    // ���� �Լ�(ease)
    public static class Ease
    {
        public static float InQuad(float t)
        {
            return t * t;
        }
        public static float InOutQuad(float t)
        {
            return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }

        const float OUTBACK = 1.70158f;
        public static float OutBack(float t)
        {
            float c3 = OUTBACK + 1f;
            return 1 + c3 * Mathf.Pow(t - 1, 3) + OUTBACK * Mathf.Pow(t - 1, 2);
        }
    }


    // ����
    // ���ϴ� ������ ���͸� ȸ����Ű�� �Լ�
    public static Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad; // ������ �������� ��ȯ
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // ȸ�� ��� ����
        float newX = vector.x * cos - vector.y * sin;
        float newY = vector.x * sin + vector.y * cos;

        return new Vector2(newX, newY);
    }
    /// <summary>
    /// 0.01f
    /// </summary>
    public const float EPSILON = 0.01f;

    const string COULD_NOT_FOUND_FORMAT = "{0} could not found in {1} ({2})";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorType"></param>
    /// <param name="values">0: ã�� ����� Class, 1: ã�� ��ü�� Class, 2: ã�� ��ü�� �̸�</param>
    /// <returns></returns>
    public static void PrintCouldNotFoundErrorLog(params string[] values)
    {
        Debug.Log(string.Format(COULD_NOT_FOUND_FORMAT, values));
    }

    /// <summary>
    /// ���ǿ� �ش��ϴ� ù ��° �ڽ� ������Ʈ�� �����մϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>(true))
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        PrintCouldNotFoundErrorLog(typeof(T).Name, "GameObject", go.name);
        return null;
    }

    /// <summary>
    /// ���ǿ� �ش��ϴ� ù ��° �ڽ� ���� ������Ʈ�� �����մϴ�.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
        {
            Debug.LogError($"Could not find GameObject name({name}) in {go.name}");
            return null;
        }
            

        return transform.gameObject;
    }

    /// <summary>
    /// ���� ������Ʈ�� T ������Ʈ�� �߰��ϰų� ã���ϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static AnimationClip GetAnimationClip(Animator animator, string name)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        AnimationClip rst = null;
        foreach(AnimationClip clip in clips)
        {
            if(clip.name == name)
            {
                rst = clip;
                break;
            }
        }
        return rst;
    }

    public static void Destroy(GameObject go)
    {
        PoolObject poolObj = go.GetComponent<PoolObject>();
        if(poolObj != null)
            poolObj.ReturnToPool();
        else 
            Object.Destroy(go);
    }
}
