using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirwayView : MonoBehaviour
{
    static float s_PointOffset = 0.2f;

    const float PARTICLE_LIFETIME = 1.0f;

    ParticleSystem _emitterA;
    ParticleSystem _subEmitterA;
    ParticleSystem _emitterB;
    ParticleSystem _subEmitterB;

    Image _phaseCountBox;
    TextMeshProUGUI _phaseCountText;
    private void Awake()
    {
        _emitterA = gameObject.FindChild<ParticleSystem>("EmitterA");
        _subEmitterA = gameObject.FindChild<ParticleSystem>("SubEmitterA", true);
        _emitterB = gameObject.FindChild<ParticleSystem>("EmitterB");
        _subEmitterB = gameObject.FindChild<ParticleSystem>("SubEmitterB", true);
        _phaseCountBox = gameObject.FindChild<Image>("PhaseCountBox", true);
        _phaseCountText = gameObject.FindChild<TextMeshProUGUI>("PhaseCountText", true);
    }

    public void SetWayPoint(float originPosX, float originPosY, float destPosX, float destPosY)
    {
        SetWayPoint(_emitterA, new Vector2(originPosX, originPosY), new Vector2(destPosX, destPosY));
        SetWayPoint(_emitterB, new Vector2(destPosX, destPosY), new Vector2(originPosX, originPosY));

        Vector3 pos = _phaseCountBox.transform.position;
        pos.x = (originPosX + destPosX) / 2;
        pos.y = (originPosY + destPosY) / 2;
        _phaseCountBox.transform.position = pos;
    }
    void SetWayPoint(ParticleSystem targetEmitter, Vector2 originPoint, Vector2 destPoint)
    {
        // 출발지점, 도착지점 설정
        Vector2 dir = (destPoint - originPoint).normalized;
        Vector2 normalDir = new Vector2(-dir.y, dir.x);
        originPoint += normalDir * s_PointOffset;
        destPoint += normalDir * s_PointOffset;

        targetEmitter.transform.position = originPoint;
        ParticleSystem.VelocityOverLifetimeModule module = targetEmitter.velocityOverLifetime;
        module.x = new ParticleSystem.MinMaxCurve((destPoint.x - originPoint.x) / PARTICLE_LIFETIME);
        module.y = new ParticleSystem.MinMaxCurve((destPoint.y - originPoint.y) / PARTICLE_LIFETIME);
    }

    public void Play()
    {
        _emitterA.Play();
        _emitterB.Play();
    }
    public void Stop()
    {
        _emitterA.Stop();
        _emitterB.Stop();
    }

    public void SetColor(Color color)
    {
        ParticleSystem.MainModule mainModule = _emitterA.main;
        mainModule.startColor = color;
        mainModule = _subEmitterA.main;
        mainModule.startColor = color;

        mainModule = _emitterB.main;
        mainModule.startColor = color;
        mainModule = _subEmitterB.main;
        mainModule.startColor = color;

        _phaseCountBox.color = color;
    }
    public void SetPhaseCountText(int value)
    {
        _phaseCountText.text = value.ToString();
    }

    private void OnDisable()
    {
        Stop();
    }
}
