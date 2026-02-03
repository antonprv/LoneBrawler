// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;
using System.Collections.Generic;

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Time;

using Unity.Mathematics;

using UnityEngine;

namespace Code.Gameplay.Common.Visuals.Particles
{
  public sealed class ParticleSmoothFade : MonoBehaviour, IParticleSmoothFade
  {
    [SerializeField]
    private ParticleSystem[] _particleSystems;

    [SerializeField]
    private float _fadeDuration = 5f;

    private bool _triggered;
    private ITimeService _timeService;

    public event Action OnStopped;

    private void Awake()
    {
      _timeService = RootContext.Resolve<ITimeService>();

      if (_particleSystems == null || _particleSystems.Length == 0)
        _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    public void TriggerStop()
    {
      if (_triggered) return;
      _triggered = true;

      foreach (var ps in _particleSystems)
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

      StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
      var systems = CollectAllSystems(_particleSystems);

      var mains = new ParticleSystem.MainModule[systems.Count];
      var trails = new ParticleSystem.TrailModule[systems.Count];
      var initialColors = new ParticleSystem.MinMaxGradient[systems.Count];
      var initialTrailColors = new ParticleSystem.MinMaxGradient[systems.Count];

      for (int i = 0; i < systems.Count; i++)
      {
        mains[i] = systems[i].main;
        trails[i] = systems[i].trails;

        initialColors[i] = mains[i].startColor;
        initialTrailColors[i] = trails[i].enabled
          ? trails[i].colorOverLifetime
          : default;
      }

      float time = 0f;

      while (time < _fadeDuration)
      {
        float t = time / _fadeDuration;
        float alpha = math.lerp(1f, 0f, t);

        for (int i = 0; i < systems.Count; i++)
        {
          mains[i].startColor = FadeMinMaxGradient(initialColors[i], alpha);

          if (trails[i].enabled)
            trails[i].colorOverLifetime = FadeMinMaxGradient(initialTrailColors[i], alpha);
        }

        time += _timeService.DeltaAt60FPS;
        yield return null;
      }

      for (int i = 0; i < systems.Count; i++)
      {
        mains[i].startColor = FadeMinMaxGradient(initialColors[i], 0f);

        if (trails[i].enabled)
          trails[i].colorOverLifetime = FadeMinMaxGradient(initialTrailColors[i], 0f);
      }

      OnStopped?.Invoke();
    }

    private static List<ParticleSystem> CollectAllSystems(
      ParticleSystem[] roots)
    {
      var result = new List<ParticleSystem>(roots);

      for (int i = 0; i < result.Count; i++)
      {
        var ps = result[i];
        var sub = ps.subEmitters;

        if (!sub.enabled) continue;

        int count = sub.subEmittersCount;
        for (int j = 0; j < count; j++)
        {
          var child = sub.GetSubEmitterSystem(j);
          if (child != null && !result.Contains(child))
            result.Add(child);
        }
      }

      return result;
    }

    private static ParticleSystem.MinMaxGradient FadeMinMaxGradient(
      ParticleSystem.MinMaxGradient src,
      float alpha)
    {
      switch (src.mode)
      {
        case ParticleSystemGradientMode.Color:
          {
            var c = src.color;
            c.a *= alpha;
            return new ParticleSystem.MinMaxGradient(c);
          }

        case ParticleSystemGradientMode.TwoColors:
          {
            var c1 = src.colorMin;
            var c2 = src.colorMax;
            c1.a *= alpha;
            c2.a *= alpha;
            return new ParticleSystem.MinMaxGradient(c1, c2);
          }

        case ParticleSystemGradientMode.Gradient:
          return new ParticleSystem.MinMaxGradient(
            CloneAndFadeGradient(src.gradient, alpha));

        case ParticleSystemGradientMode.TwoGradients:
          return new ParticleSystem.MinMaxGradient(
            CloneAndFadeGradient(src.gradientMin, alpha),
            CloneAndFadeGradient(src.gradientMax, alpha));

        default:
          return src;
      }
    }

    private static Gradient CloneAndFadeGradient(
      Gradient src,
      float alpha)
    {
      var g = new Gradient();

      var colorKeys = src.colorKeys;
      var alphaKeys = src.alphaKeys;

      for (int i = 0; i < alphaKeys.Length; i++)
        alphaKeys[i].alpha *= alpha;

      g.SetKeys(colorKeys, alphaKeys);
      return g;
    }
  }
}
