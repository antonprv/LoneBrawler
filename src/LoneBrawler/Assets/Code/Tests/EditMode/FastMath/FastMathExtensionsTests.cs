// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;

using NUnit.Framework;

namespace Code.Tests.EditMode.FastMath
{
  [TestFixture]
  public class FastMathExtensionsTests
  {
    private const float Tol = 0.01f;

    [Test]
    public void FastSqrt_ExtensionMethod_ReturnsCorrectApproximation()
    {
      float result = 9f.FastSqrt();
      Assert.That(result, Is.EqualTo(3f).Within(Tol));
    }

    [Test]
    public void FastInvSqrt_ExtensionMethod_ReturnsApproximation()
    {
      float result = 4f.FastInvSqrt();
      Assert.That(result, Is.EqualTo(0.5f).Within(Tol));
    }

    [Test]
    public void IsNearlyEqual_ExtensionMethod_ReturnsTrue_ForSameValues()
    {
      Assert.That(1f.IsNearlyEqual(1f), Is.True);
    }

    [Test]
    public void IsNearlyEqual_ExtensionMethod_ReturnsFalse_ForLargeGap()
    {
      Assert.That(0f.IsNearlyEqual(1f), Is.False);
    }

    [Test]
    public void IsNearlyZero_ExtensionMethod_Zero_ReturnsTrue()
    {
      Assert.That(0f.IsNearlyZero(), Is.True);
    }

    [Test]
    public void IsNearlyZero_ExtensionMethod_Large_ReturnsFalse()
    {
      Assert.That(5f.IsNearlyZero(), Is.False);
    }
  }
}
