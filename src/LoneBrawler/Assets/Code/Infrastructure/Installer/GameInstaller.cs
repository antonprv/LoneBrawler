// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Reflex.Core;
using System;

public class GameInstaller : ProjectRootInstaller
{
    public override void InstallBindings(ContainerBuilder builder)
    {
        BindInputService(builder);
    }

    private void BindInputService(ContainerBuilder builder)
    {
        throw new NotImplementedException();
    }
}
