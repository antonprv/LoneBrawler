// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;

namespace YG
{
  public partial class InfoYG
  {
    public QuitGameEventSettings QuitGameEvent = new QuitGameEventSettings();

    [Serializable]
    public partial class QuitGameEventSettings
    {
#if RU_YG2
            [Tooltip("Выполнять определённый метод при закрытии или обновлении страницы игры.")]
#else
      [Tooltip("Perform a specific method when closing or refreshing the game page.")]
#endif
      public bool enable = true;
#if RU_YG2
            [Tooltip("Имя объекта, который содержит нужный метод для выполнения после закрытия игры.")]
#else
      [Tooltip("The name of the object that contains the desired method to execute after the game is closed.")]
#endif
      [NestedYG(nameof(enable))]
      public string objectName = "LiveProgressSync";
#if RU_YG2
            [Tooltip("Имя метода. Подходит публичный метод без перегрузок.")]
#else
      [Tooltip("The name of the method. A public method without overloads is suitable.")]
#endif
      [NestedYG(nameof(enable))]
      public string methodName = "OnQuitGame";
    }
  }
}
