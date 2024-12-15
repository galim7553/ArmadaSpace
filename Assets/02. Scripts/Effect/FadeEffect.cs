using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    const string FADE_AMOUNT_NAME = "_FadeAmount";
    const float MIN_FADE_AMOUNT = -0.1F;
    const float MAX_FADE_AMOUNT = 1.0F;

    [SerializeField] Material FadeEffectMaterial;

    [SerializeField] Image[] Images;
    [SerializeField] TextMeshProUGUI[] TMPs;

    Material[] _originMaterials;
    Material _fadeEffectMaterial;

    private void Awake()
    {
        _originMaterials = new Material[Images.Length];
        for(int i = 0; i < _originMaterials.Length; i++)
            _originMaterials[i] = Images[i].material;
        _fadeEffectMaterial = new Material(FadeEffectMaterial);
    }

    public void PlayFadeOutAnim(float animTime)
    {
        StartCoroutine(PlayFadeOutAnimCo(animTime));
    }

    IEnumerator PlayFadeOutAnimCo(float animTime)
    {
        // FadeEffectMaterial의 FadeAmount 초기화
        _fadeEffectMaterial.SetFloat(FADE_AMOUNT_NAME, MIN_FADE_AMOUNT);

        // 먼저 Images의 Material들을 전부 교체
        foreach(var img in Images)
            img.material = _fadeEffectMaterial;

        // 수정될 값들 초기화
        float fadeAmount = MIN_FADE_AMOUNT;
        Color color = Color.white;
        float alpha = 1.0f;

        // 애니메이션
        float elapsedTime = 0;
        while (elapsedTime < animTime)
        {
            elapsedTime += Time.deltaTime;
            fadeAmount = Mathf.Lerp(MIN_FADE_AMOUNT, MAX_FADE_AMOUNT, elapsedTime / animTime);
            _fadeEffectMaterial.SetFloat(FADE_AMOUNT_NAME, fadeAmount);

            alpha = Mathf.Lerp(1, 0, elapsedTime / animTime);
            if(TMPs != null)
            {
                foreach (var tmp in TMPs)
                {
                    color = tmp.color;
                    color.a = alpha;
                    tmp.color = color;
                }
            }
            yield return null;
        }
    }

    public void PlayFadeInAnim(float animTime)
    {
        StartCoroutine(PlayFadeInAnimCo(animTime));
    }

    IEnumerator PlayFadeInAnimCo(float animTime)
    {
        _fadeEffectMaterial.SetFloat(FADE_AMOUNT_NAME, MAX_FADE_AMOUNT);

        // Images Material들 전부 교체
        foreach (var img in Images)
            img.material = _fadeEffectMaterial;

        float fadeAmount = MAX_FADE_AMOUNT;
        Color color = Color.white;
        float alpha = 0.0f;

        // 애니메이션
        float elapsedTime = 0;
        while (elapsedTime < animTime)
        {
            elapsedTime += Time.deltaTime;
            fadeAmount = Mathf.Lerp(MAX_FADE_AMOUNT, MIN_FADE_AMOUNT, elapsedTime / animTime);
            _fadeEffectMaterial.SetFloat(FADE_AMOUNT_NAME, fadeAmount);

            alpha = Mathf.Lerp(0, 1, elapsedTime / animTime);
            if (TMPs != null)
            {
                foreach (var tmp in TMPs)
                {
                    color = tmp.color;
                    color.a = alpha;
                    tmp.color = color;
                }
            }
            yield return null;
        }

        ResetMaterials();
    }

    private void OnDisable()
    {
        ResetMaterials();
        ResetColors();
    }

    void ResetMaterials()
    {
        for (int i = 0; i < Images.Length; i++)
            Images[i].material = _originMaterials[i];
    }
    void ResetColors()
    {
        if (TMPs != null)
        {
            Color color = Color.white;
            foreach (var tmp in TMPs)
            {
                color = tmp.color;
                color.a = 1.0f;
                tmp.color = color;
            }
        }
    }
}
