// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Reflex.Delegates;

namespace Reflex.Caching
{
  internal sealed class TypeConstructionInfo
  {
    public readonly ObjectActivator ObjectActivator;
    public readonly Type[] ConstructorParameters;

    public TypeConstructionInfo(ObjectActivator objectActivator, Type[] constructorParameters)
    {
      ObjectActivator = objectActivator;
      ConstructorParameters = constructorParameters;
    }
  }
}