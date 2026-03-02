// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.AssetManagement.Addresses
{
  public static class AssetAddresses
  {
    // Player
    public static readonly string PlayerAddress = "PP_Player";

    // UI
    public static readonly string HudAddress = "PUI_Hud";
    public static readonly string UIRootAddress = "Included/UI/PUI_Root.prefab";
    public static readonly string MainMenuAddress = "PUI_MainMenu";
    public static readonly string InventorySlotAddress = "Inventory/PUI_InventorySlot.prefab";
    public static readonly string HotbarSlotAddress = "Inventory/PUI_HotBarSlot.prefab";

    // Level logic
    public static readonly string LevelTeleportAddress =
      "Infrastructure/LevelTransition/PA_LevelTeleportTrigger.prefab";

    // Enemies
    public static readonly string EnemySpawnerAddress =
      "SpawnLogic/PA_EnemySpawnPoint.prefab";
  }
}
