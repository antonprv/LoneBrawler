// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace YG
{
  public static partial class YG2
  {
    [AttributeUsage(AttributeTargets.Method)]
    private class InitYG_0Attribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    private class InitYG_1Attribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    private class InitYG_2Attribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    private class InitYGAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    private class StartYGAttribute : Attribute { }
  }
}