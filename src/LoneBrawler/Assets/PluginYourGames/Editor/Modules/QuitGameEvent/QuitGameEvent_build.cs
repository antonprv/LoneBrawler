// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if YandexGamesPlatform_yg
namespace YG.EditorScr.BuildModify
{
  public partial class ModifyBuild
  {
    public static void QuitGameEvent()
    {
      if (infoYG.QuitGameEvent.enable)
      {
        string copyCode = FileTextCopy("QuitGameEvent_js.js");

        copyCode = copyCode.Replace("{{{ObjectName}}}", infoYG.QuitGameEvent.objectName);
        copyCode = copyCode.Replace("{{{MethodName}}}", infoYG.QuitGameEvent.methodName);

        AddIndexCode(copyCode, CodeType.JS);
      }
    }
  }
}
#endif