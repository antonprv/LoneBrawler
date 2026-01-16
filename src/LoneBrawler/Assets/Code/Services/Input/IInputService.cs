// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace CodeBase.Services.Input
{
    public interface IInputService
    {
        Vector2 Axis { get; }

        bool IsAttackButtonUp();
    }
}