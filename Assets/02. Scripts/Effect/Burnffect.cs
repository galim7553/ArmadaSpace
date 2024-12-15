using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Burnffect : MonoBehaviour
{
    [SerializeField] Material BurnEffectMaterial;

    [SerializeField] TextMeshProUGUI[] TMPs;

    Material _originMaterial;
    Material _burnEffectMaterial;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _originMaterial = GetComponent<Image>().material;
        _burnEffectMaterial = new Material(BurnEffectMaterial);
    }

    void PlayBurnAnim()
    {

    }
}
