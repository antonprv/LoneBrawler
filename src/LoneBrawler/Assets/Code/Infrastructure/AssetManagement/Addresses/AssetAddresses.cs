// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.AssetManagement.Addresses
{
  public static class AssetAddresses
  {
    public static readonly string PlayerAddress = "PP_Player";
    public static readonly string HudAddress = "PUI_Hud";

    public static readonly string UIRootAddress = "Included/UI/PUI_Root.prefab";

    public static readonly string LevelTeleportAddress =
      "Infrastructure/LevelTransition/PA_LevelTeleportTrigger.prefab";

    public static readonly string EnemySpawnerAddress =
      "SpawnLogic/PA_EnemySpawnPoint.prefab";
  }
}
