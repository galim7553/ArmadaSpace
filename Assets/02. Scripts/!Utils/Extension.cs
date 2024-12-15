using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    /// <summary>
    /// 확률에 따라 선택된 Index를 반환합니다.
    /// </summary>
    /// <param name="probs"></param>
    /// <returns></returns>
    public static int Choose(this IList<float> probs)
    {
        return Util.Ran.Choose(probs);
    }

    /// <summary>
    /// 리스트를 랜덤한 순서로 셔플합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        Util.Ran.Shuffle(list);
    }

    /// <summary>
    /// 조건에 해당하는 첫 번째 자식 오브젝트를 리턴합니다.
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
    /// 조건에 해당하는 첫 번째 자식 게임 오브젝트를 리턴합니다.
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
    /// 게임 오브젝트에 T 컴포넌트를 추가하거나 찾습니다.
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

