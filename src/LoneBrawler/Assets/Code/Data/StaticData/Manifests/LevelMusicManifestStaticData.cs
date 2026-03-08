// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

using Code.Common.CustomTypes.Domain.Collections;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Manifests
{
  [CreateAssetMenu(fileName = "LevelMusicManifest",
    menuName = "StaticData/Manifests/LevelMusicManifest")]
  public class LevelMusicManifestStaticData : ScriptableObject
  {
    [Tooltip("Key: exact Unity scene name (case-sensitive). " +
             "Value: the MusicPlaylist that plays while that level is active.")]
    public DictionaryData<string, AssetReferenceT<MusicPlaylist>> PlaylistsByLevel = new();
  }
}
