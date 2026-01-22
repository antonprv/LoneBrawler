// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Common.DebugUtils
{
  public static class DrawDebug
  {
    public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
    {
      GL.PushMatrix();
      GL.Begin(GL.LINES);

      // Устанавливаем цвет линии
      GL.Color(color);

      // Вычисляем половину размера для удобства
      Vector3 halfSize = size * 0.5f;

      // Нижняя грань
      DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
      DrawLine(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, -halfSize.y, halfSize.z));
      DrawLine(center + new Vector3(halfSize.x, -halfSize.y, halfSize.z), center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
      DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));

      // Верхняя грань
      DrawLine(center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, halfSize.y, -halfSize.z));
      DrawLine(center + new Vector3(halfSize.x, halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, halfSize.y, halfSize.z));
      DrawLine(center + new Vector3(halfSize.x, halfSize.y, halfSize.z), center + new Vector3(-halfSize.x, halfSize.y, halfSize.z));
      DrawLine(center + new Vector3(-halfSize.x, halfSize.y, halfSize.z), center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));

      // Соединительные ребра
      DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
      DrawLine(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, halfSize.y, -halfSize.z));
      DrawLine(center + new Vector3(halfSize.x, -halfSize.y, halfSize.z), center + new Vector3(halfSize.x, halfSize.y, halfSize.z));
      DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), center + new Vector3(-halfSize.x, halfSize.y, halfSize.z));

      GL.End();
      GL.PopMatrix();
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
      GL.Vertex(start);
      GL.Vertex(end);
    }
  }
}
