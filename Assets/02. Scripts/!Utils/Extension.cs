using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    /// <summary>
    /// Ȯ���� ���� ���õ� Index�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="probs"></param>
    /// <returns></returns>
    public static int Choose(this IList<float> probs)
    {
        return Util.Ran.Choose(probs);
    }

    /// <summary>
    /// ����Ʈ�� ������ ������ �����մϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        Util.Ran.Shuffle(list);
    }

    /// <summary>
    /// ���ǿ� �ش��ϴ� ù ��° �ڽ� ������Ʈ�� �����մϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : Object
    {
        return Util.FindChild<T>(go, name, recursive);
    }

    /// <summary>
    /// ���ǿ� �ش��ϴ� ù ��° �ڽ� ���� ������Ʈ�� �����մϴ�.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static GameObject FindChild(this GameObject go, string name = null, bool recursive = false)
    {
        return Util.FindChild(go, name, recursive);
    }

    /// <summary>
    /// ���� ������Ʈ�� T ������Ʈ�� �߰��ϰų� ã���ϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Util.GetOrAddComponent<T>(go);
    }


    public static AnimationClip GetAnimationClip(this Animator animator, string name)
    {
        return Util.GetAnimationClip(animator, name);
    }

    public static void DestroyOrReturnToPool(this GameObject go)
    {
        Util.Destroy(go);
    }
}

