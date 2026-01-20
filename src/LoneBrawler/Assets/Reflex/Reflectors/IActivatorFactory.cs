// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Reflection;

using Reflex.Delegates;

namespace Reflex.Reflectors
{
  internal interface IActivatorFactory
  {
    ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters);
    ObjectActivator GenerateDefaultActivator(Type type);
  }
}