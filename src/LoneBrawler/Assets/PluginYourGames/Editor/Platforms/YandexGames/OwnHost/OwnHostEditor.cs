// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if YandexGamesPlatform_yg
namespace YG.EditorScr.BuildModify
{
  public partial class ModifyBuild
  {
    public static void OwnHost()
    {
      if (YG2.infoYG.platformInfo.ownHost)
      {
        AddIndexCode("<script src=\"https://sdk.games.s3.yandex.net/sdk.js\"></script>", CodeType.Head);
      }
    }
  }
}
#endif