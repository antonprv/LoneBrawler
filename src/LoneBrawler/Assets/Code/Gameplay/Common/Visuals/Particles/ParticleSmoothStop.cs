// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Time;

using UnityEngine;

namespace Code.Gameplay.Common.Visuals.Particles
{
  public sealed class ParticleSmoothStop : MonoBehaviour, IParticleSmoothStop
  {
    [SerializeField]
    private ParticleSystem[] _particleSystems;

    [SerializeField]
    private float _fadeDuration = 0.5f;

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
      if (_triggered)
        return;

      _triggered = true;

      foreach (var system in _particleSystems)
        system.Stop(true, ParticleSystemStopBehavior.StopEmitting);

      StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
      var renderers = new ParticleSystemRenderer[_particleSystems.Length];
      var materials = new Material[_particleSystems.Length];
      var initialColors = new Color[_particleSystems.Length];

      for (int i = 0; i < _particleSystems.Length; i++)
      {
        var renderer = _particleSystems[i].GetComponent<ParticleSystemRenderer>();
        renderers[i] = renderer;

        var mat = renderer.material;
        materials[i] = mat;

        initialColors[i] = GetColor(mat);
      }

      float time = 0f;

      while (time < _fadeDuration)
      {
        float t = time / _fadeDuration;

        for (int i = 0; i < materials.Length; i++)
        {
          var c = initialColors[i];
          c.a = Mathf.Lerp(initialColors[i].a, 0f, t);
          SetColor(materials[i], c);
        }

        time += _timeService.DeltaAt60FPS;
        yield return null;
      }

      for (int i = 0; i < materials.Length; i++)
      {
        var c = initialColors[i];
        c.a = 0f;
        SetColor(materials[i], c);
      }

      OnStopped?.Invoke();
    }

    private static Color GetColor(Material mat)
    {
      if (mat.HasProperty("_BaseColor"))
        return mat.GetColor("_BaseColor");

      return mat.GetColor("_Color");
    }

    private static void SetColor(Material mat, Color color)
    {
      if (mat.HasProperty("_BaseColor"))
        mat.SetColor("_BaseColor", color);
      else
        mat.SetColor("_Color", color);
    }
  }
}
