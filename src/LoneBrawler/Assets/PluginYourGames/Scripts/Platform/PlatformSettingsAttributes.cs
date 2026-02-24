// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;

namespace YG.Insides
{
  [AttributeUsage(AttributeTargets.Method)]
  public class ApplySettingsAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Method)]
  public class SelectPlatformAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Method)]
  public class DeletePlatformAttribute : Attribute { }

  public class PlatformAttribute : PropertyAttribute
  {
    public string name { get; private set; }

    public PlatformAttribute(string platformName)
    {
      name = platformName;
    }
  }
}