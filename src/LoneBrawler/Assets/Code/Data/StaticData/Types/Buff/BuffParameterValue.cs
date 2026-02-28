// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Types.Buff
{
  public enum BuffParameterType
  {
    Int,
    Float,
    Bool,
    String,
    AssetReference
  }

  /// <summary>
  /// Union-style value container for buff dynamic parameters.
  /// Stores one value of a chosen type; irrelevant fields are ignored.
  /// Supports: int, float, bool, string, AssetReference (Addressables).
  /// </summary>
  [Serializable]
  public class BuffParameterValue
  {
    public BuffParameterType Type = BuffParameterType.Float;

    [SerializeField] private int _intValue;
    [SerializeField] private float _floatValue;
    [SerializeField] private bool _boolValue;
    [SerializeField] private string _stringValue;
    [SerializeField] private AssetReference _assetReferenceValue;

    // ── Typed accessors ──────────────────────────────────────────

    public int IntValue
    {
      get => _intValue;
      set { _intValue = value; Type = BuffParameterType.Int; }
    }

    public float FloatValue
    {
      get => _floatValue;
      set { _floatValue = value; Type = BuffParameterType.Float; }
    }

    public bool BoolValue
    {
      get => _boolValue;
      set { _boolValue = value; Type = BuffParameterType.Bool; }
    }

    public string StringValue
    {
      get => _stringValue;
      set { _stringValue = value; Type = BuffParameterType.String; }
    }

    public AssetReference AssetReferenceValue
    {
      get => _assetReferenceValue;
      set { _assetReferenceValue = value; Type = BuffParameterType.AssetReference; }
    }

    // ── Generic read ─────────────────────────────────────────────

    /// <summary>
    /// Returns the stored value cast to T.
    /// Throws InvalidCastException if the stored type doesn't match T.
    /// </summary>
    public T Get<T>()
    {
      object boxed = Type switch
      {
        BuffParameterType.Int => _intValue,
        BuffParameterType.Float => _floatValue,
        BuffParameterType.Bool => _boolValue,
        BuffParameterType.String => _stringValue,
        BuffParameterType.AssetReference => _assetReferenceValue,
        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
      };

      return (T)boxed;
    }

    public override string ToString() => Type switch
    {
      BuffParameterType.Int => _intValue.ToString(),
      BuffParameterType.Float => _floatValue.ToString("G"),
      BuffParameterType.Bool => _boolValue.ToString(),
      BuffParameterType.String => _stringValue ?? string.Empty,
      BuffParameterType.AssetReference => _assetReferenceValue?.AssetGUID ?? "null",
      _ => base.ToString()
    };
  }
}
