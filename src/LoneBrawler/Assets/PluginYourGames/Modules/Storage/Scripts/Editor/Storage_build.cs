// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if YandexGamesPlatform_yg
namespace YG.EditorScr.BuildModify
{
  public partial class ModifyBuild
  {
    public static void Storage()
    {
      if (YG2.infoYG.Storage.saveCloud)
      {
        InitFunction("LoadCloud");

        string copyCode = FileTextCopy("CloudStorage_js.js");
        AddIndexCode(copyCode, CodeType.JS);
      }
    }
  }
}
#endif