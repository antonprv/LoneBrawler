// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Code.Common.DebugUtils
{
  public struct DebugShapeName
  {
    public const string Sphere = "DebugWireSphere";
    public const string Cube = "DebugWireCube";

    public const string SphereTemp = "DebugWireSphereTemp";
    public const string CubeTemp = "DebugWireCubeTemp";
  }

  public static class DrawDebugRuntime
  {
    private const int DefaultSphereSegments = 24;
    private const string ParentSuffix = "_Parent";
    private const string ChildSuffix = "_Part";

    private static readonly Queue<LineRenderer> _pool = new Queue<LineRenderer>();
    private static readonly Material _lineMaterial = new Material(Shader.Find("Sprites/Default"));

    // Parents for hierarchy organization
    private static GameObject _cubeParent;
    private static GameObject _cubeTempParent;
    private static GameObject _sphereParent;
    private static GameObject _sphereTempParent;

    private static void EnsureParents(string name)
    {
      switch (name)
      {
        case DebugShapeName.Sphere:
          {
            _sphereParent = _sphereParent ?? new GameObject(DebugShapeName.Sphere + ParentSuffix);
            break;
          }
        case DebugShapeName.Cube:
          {
            _cubeParent = _cubeParent ?? new GameObject(DebugShapeName.Cube + ParentSuffix);
            break;
          }
        case DebugShapeName.SphereTemp:
          {
            _sphereTempParent = _sphereTempParent ?? new GameObject(DebugShapeName.SphereTemp + ParentSuffix);
            break;
          }
        case DebugShapeName.CubeTemp:
          {
            _cubeTempParent = _cubeTempParent ?? new GameObject(DebugShapeName.CubeTemp + ParentSuffix);
            break;
          }
        default:
          break;
      }
    }

    public static void DestroyByName(string name)
    {
      if (name == DebugShapeName.SphereTemp || name == DebugShapeName.CubeTemp)
        throw new System.InvalidOperationException("Destruction of temporary shapes is not allowed");

      foreach (var lr in Object.FindObjectsByType<LineRenderer>(FindObjectsSortMode.None))
      {
        if (lr.gameObject.name == name + ChildSuffix)
          Object.Destroy(lr.gameObject);
      }
      DestroyNonTempParents();
    }

    public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
    {
      EnsureParents(DebugShapeName.Cube);

      GameObject go = new GameObject(DebugShapeName.Cube + ChildSuffix);
      go.transform.SetParent(_cubeParent.transform, true);
      var lr = go.AddComponent<LineRenderer>();
      lr.material = _lineMaterial;
      lr.startColor = lr.endColor = color;
      lr.startWidth = lr.endWidth = 0.02f;
      lr.loop = false;

      Vector3 h = size * 0.5f;
      Vector3[] v =
      {
                center + new Vector3(-h.x, -h.y, -h.z),
                center + new Vector3(h.x, -h.y, -h.z),
                center + new Vector3(h.x, -h.y, h.z),
                center + new Vector3(-h.x, -h.y, h.z),
                center + new Vector3(-h.x, h.y, -h.z),
                center + new Vector3(h.x, h.y, -h.z),
                center + new Vector3(h.x, h.y, h.z),
                center + new Vector3(-h.x, h.y, h.z),
            };

      Vector3[] positions =
      {
                v[0], v[1], v[1], v[2], v[2], v[3], v[3], v[0],
                v[4], v[5], v[5], v[6], v[6], v[7], v[7], v[4],
                v[0], v[4], v[1], v[5], v[2], v[6], v[3], v[7]
            };

      lr.positionCount = positions.Length;
      lr.SetPositions(positions);
    }

    public static void DrawWireSphere(Vector3 center, float radius, Color color, int segments = DefaultSphereSegments)
    {
      EnsureParents(DebugShapeName.Sphere);

      GameObject go = new GameObject(DebugShapeName.Sphere + ChildSuffix);
      go.transform.SetParent(_sphereParent.transform, true);
      var lr = go.AddComponent<LineRenderer>();
      lr.material = _lineMaterial;
      lr.startColor = lr.endColor = color;
      lr.startWidth = lr.endWidth = 0.02f;
      lr.loop = false;

      List<Vector3> positions = new List<Vector3>();

      void DrawCircle(Vector3 axisA, Vector3 axisB)
      {
        float step = Mathf.PI * 2f / segments;
        for (int i = 0; i <= segments; i++)
        {
          float a = step * i;
          positions.Add(center + (axisA * Mathf.Cos(a) + axisB * Mathf.Sin(a)) * radius);
        }
      }

      DrawCircle(Vector3.right, Vector3.forward);
      DrawCircle(Vector3.up, Vector3.forward);
      DrawCircle(Vector3.up, Vector3.right);

      lr.positionCount = positions.Count;
      lr.SetPositions(positions.ToArray());
    }

    public static void DrawTempWireCube(Vector3 center, Vector3 size, Color color, float duration = 1f)
    {
      EnsureCoroutineRunner();
      EnsureParents(DebugShapeName.CubeTemp);

      LineRenderer lr = GetPooledLineRenderer(DebugShapeName.CubeTemp + ChildSuffix, color);
      lr.transform.SetParent(_cubeTempParent.transform, true);

      Vector3 h = size * 0.5f;
      Vector3[] v =
      {
                center + new Vector3(-h.x, -h.y, -h.z),
                center + new Vector3(h.x, -h.y, -h.z),
                center + new Vector3(h.x, -h.y, h.z),
                center + new Vector3(-h.x, -h.y, h.z),
                center + new Vector3(-h.x, h.y, -h.z),
                center + new Vector3(h.x, h.y, -h.z),
                center + new Vector3(h.x, h.y, h.z),
                center + new Vector3(-h.x, h.y, h.z),
            };

      Vector3[] positions =
      {
                v[0], v[1], v[1], v[2], v[2], v[3], v[3], v[0],
                v[4], v[5], v[5], v[6], v[6], v[7], v[7], v[4],
                v[0], v[4], v[1], v[5], v[2], v[6], v[3], v[7]
            };

      lr.positionCount = positions.Length;
      lr.SetPositions(positions);

      CoroutineRunner.Instance.StartCoroutine(ReleaseAfter(lr, duration));
    }

    public static void DrawTempWireSphere(Vector3 center, float radius, Color color, int segments = DefaultSphereSegments, float duration = 1f)
    {
      EnsureCoroutineRunner();
      EnsureParents(DebugShapeName.SphereTemp);

      LineRenderer lr = GetPooledLineRenderer(DebugShapeName.SphereTemp + ChildSuffix, color);
      lr.transform.SetParent(_sphereTempParent.transform, true);

      List<Vector3> positions = new List<Vector3>();

      void DrawCircle(Vector3 axisA, Vector3 axisB)
      {
        float step = Mathf.PI * 2f / segments;
        for (int i = 0; i <= segments; i++)
        {
          float a = step * i;
          positions.Add(center + (axisA * Mathf.Cos(a) + axisB * Mathf.Sin(a)) * radius);
        }
      }

      DrawCircle(Vector3.right, Vector3.forward);
      DrawCircle(Vector3.up, Vector3.forward);
      DrawCircle(Vector3.up, Vector3.right);

      lr.positionCount = positions.Count;
      lr.SetPositions(positions.ToArray());

      CoroutineRunner.Instance.StartCoroutine(ReleaseAfter(lr, duration));
    }

    // ---------- INTERNAL ----------

    private static void DestroyNonTempParents()
    {
      if (_sphereParent != null)
      {
        Object.Destroy(_sphereParent);
      }
      if (_cubeParent != null)
      {
        Object.Destroy(_cubeParent);
      }
    }

    private static LineRenderer GetPooledLineRenderer(string name, Color color, float width = 0.02f)
    {
      LineRenderer lr;
      if (_pool.Count > 0)
      {
        lr = _pool.Dequeue();
        lr.gameObject.SetActive(true);
      }
      else
      {
        GameObject go = new GameObject(name);
        lr = go.AddComponent<LineRenderer>();
        lr.material = _lineMaterial;
      }

      lr.startColor = lr.endColor = color;
      lr.startWidth = lr.endWidth = width;
      lr.loop = false;
      lr.positionCount = 0;

      return lr;
    }

    private static void Release(LineRenderer lr)
    {
      if (lr == null) return;
      lr.gameObject.SetActive(false);
      _pool.Enqueue(lr);
    }

    private static IEnumerator ReleaseAfter(LineRenderer lr, float delay)
    {
      yield return new WaitForSeconds(delay);
      Release(lr);
    }

    private static void EnsureCoroutineRunner()
    {
      if (CoroutineRunner.Instance == null)
      {
        var go = new GameObject("DebugCoroutineRunner");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<CoroutineRunner>();
      }
    }

    private class CoroutineRunner : MonoBehaviour
    {
      public static CoroutineRunner Instance { get; private set; }

      private void Awake()
      {
        if (Instance == null)
          Instance = this;
        else
          Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
      }
    }
  }
}
