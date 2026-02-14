// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService
{
  public class StaticDataAddresses
  {
    private const string _rootFolder = "StaticData";

    private static readonly string _uiFolder = $"{_rootFolder}/UI";

    public static readonly string BuildConfigAddress = "BuildConfig";
    public static readonly string GameConfigAddress = "GameConfig";

    public static readonly string EnemyDataPath = $"{_rootFolder}/Enemies";
    public static readonly string PlayerDataPath = $"{_rootFolder}/PlayerStaticData";

    public static readonly string LevelDataPath = $"{_rootFolder}/Levels";

    public static readonly string WindowDataPath = $"{_uiFolder}/WindowStaticData";
  }
}
