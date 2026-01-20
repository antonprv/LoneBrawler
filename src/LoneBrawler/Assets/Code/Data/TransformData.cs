// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Data
{
  public class TransformData
  {
    public Vector3Data Position;
    public QuatData Rotation;
    public Vector3Data Scale;

    public TransformData(Vector3Data position, QuatData rotation, Vector3Data scale)
    {
      Position = position;
      Rotation = rotation;
      Scale = scale;
    }
  }
}
