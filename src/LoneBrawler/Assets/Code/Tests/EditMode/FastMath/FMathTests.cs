// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;

using NUnit.Framework;

namespace Code.Tests.EditMode.FastMath
{
  [TestFixture]
  public class FMathTests
  {
    private const float Tolerance = 0.01f; // tolerance for approximate calculations

    #region FastSqrt

    [Test]
    public void FastSqrt_OfFour_ReturnsApproximatelyTwo()
    {
      float result = FMath.FastSqrt(4f);
      Assert.That(result, Is.EqualTo(2f).Within(Tolerance));
    }

    [Test]
    public void FastSqrt_OfOne_ReturnsApproximatelyOne()
    {
      float result = FMath.FastSqrt(1f);
      Assert.That(result, Is.EqualTo(1f).Within(Tolerance));
    }

    [Test]
    public void FastSqrt_OfZero_ReturnsZero()
    {
      Assert.That(FMath.FastSqrt(0f), Is.EqualTo(0f));
    }

    [Test]
    public void FastSqrt_OfNegative_ReturnsZero()
    {
      Assert.That(FMath.FastSqrt(-1f), Is.EqualTo(0f));
    }

    [Test]
    public void FastSqrt_OfHundred_ReturnsApproximatelyTen()
    {
      Assert.That(FMath.FastSqrt(100f), Is.EqualTo(10f).Within(0.1f));
    }

    #endregion

    #region FastInvSqrt

    [Test]
    public void FastInvSqrt_OfFour_ReturnsApproximatelyHalf()
    {
      float result = FMath.FastInvSqrt(4f);
      Assert.That(result, Is.EqualTo(0.5f).Within(Tolerance));
    }

    [Test]
    public void FastInvSqrt_PresciseMode_IsMoreAccurate()
    {
      float rough = FMath.FastInvSqrt(100f, false);
      float precise = FMath.FastInvSqrt(100f, true);
      float expected = 0.1f;

      Assert.That(System.Math.Abs(precise - expected),
          Is.LessThan(System.Math.Abs(rough - expected)).Or.EqualTo(System.Math.Abs(rough - expected)));
      Assert.That(precise, Is.EqualTo(expected).Within(0.001f));
    }

    #endregion

    #region Clamp (float)

    [Test]
    public void ClampFloat_ValueBelowMin_ReturnsMin()
    {
      Assert.That(FMath.Clamp(-5f, 0f, 10f), Is.EqualTo(0f));
    }

    [Test]
    public void ClampFloat_ValueAboveMax_ReturnsMax()
    {
      Assert.That(FMath.Clamp(15f, 0f, 10f), Is.EqualTo(10f));
    }

    [Test]
    public void ClampFloat_ValueInRange_ReturnsSameValue()
    {
      Assert.That(FMath.Clamp(5f, 0f, 10f), Is.EqualTo(5f));
    }

    [Test]
    public void ClampFloat_ValueAtMin_ReturnsMin()
    {
      Assert.That(FMath.Clamp(0f, 0f, 10f), Is.EqualTo(0f));
    }

    [Test]
    public void ClampFloat_ValueAtMax_ReturnsMax()
    {
      Assert.That(FMath.Clamp(10f, 0f, 10f), Is.EqualTo(10f));
    }

    #endregion

    #region Clamp01

    [Test]
    public void Clamp01_Negative_ReturnsZero()
    {
      Assert.That(FMath.Clamp01(-0.5f), Is.EqualTo(0f));
    }

    [Test]
    public void Clamp01_GreaterThanOne_ReturnsOne()
    {
      Assert.That(FMath.Clamp01(1.5f), Is.EqualTo(1f));
    }

    [Test]
    public void Clamp01_HalfPoint_ReturnsSame()
    {
      Assert.That(FMath.Clamp01(0.5f), Is.EqualTo(0.5f));
    }

    #endregion

    #region Clamp (int)

    [Test]
    public void ClampInt_ValueBelowMin_ReturnsMin()
    {
      Assert.That(FMath.Clamp(-5, 0, 10), Is.EqualTo(0));
    }

    [Test]
    public void ClampInt_ValueAboveMax_ReturnsMax()
    {
      Assert.That(FMath.Clamp(20, 0, 10), Is.EqualTo(10));
    }

    [Test]
    public void ClampInt_ValueInRange_ReturnsSame()
    {
      Assert.That(FMath.Clamp(5, 0, 10), Is.EqualTo(5));
    }

    #endregion

    #region Lerp

    [Test]
    public void Lerp_AtZero_ReturnsA()
    {
      Assert.That(FMath.Lerp(0f, 10f, 0f), Is.EqualTo(0f));
    }

    [Test]
    public void Lerp_AtOne_ReturnsB()
    {
      Assert.That(FMath.Lerp(0f, 10f, 1f), Is.EqualTo(10f));
    }

    [Test]
    public void Lerp_AtHalf_ReturnsMidpoint()
    {
      Assert.That(FMath.Lerp(0f, 10f, 0.5f), Is.EqualTo(5f).Within(0.001f));
    }

    [Test]
    public void Lerp_BelowZero_ClampsToA()
    {
      Assert.That(FMath.Lerp(0f, 10f, -1f), Is.EqualTo(0f));
    }

    [Test]
    public void Lerp_AboveOne_ClampsToB()
    {
      Assert.That(FMath.Lerp(0f, 10f, 2f), Is.EqualTo(10f));
    }

    #endregion

    #region LerpUnclamped

    [Test]
    public void LerpUnclamped_AboveOne_Extrapolates()
    {
      Assert.That(FMath.LerpUnclamped(0f, 10f, 2f), Is.EqualTo(20f).Within(0.001f));
    }

    #endregion

    #region InverseLerp

    [Test]
    public void InverseLerp_AtA_ReturnsZero()
    {
      Assert.That(FMath.InverseLerp(0f, 10f, 0f), Is.EqualTo(0f));
    }

    [Test]
    public void InverseLerp_AtB_ReturnsOne()
    {
      Assert.That(FMath.InverseLerp(0f, 10f, 10f), Is.EqualTo(1f));
    }

    [Test]
    public void InverseLerp_AtMidpoint_ReturnsHalf()
    {
      Assert.That(FMath.InverseLerp(0f, 10f, 5f), Is.EqualTo(0.5f).Within(0.001f));
    }

    [Test]
    public void InverseLerp_WhenAEqualsB_ReturnsZero()
    {
      Assert.That(FMath.InverseLerp(5f, 5f, 5f), Is.EqualTo(0f));
    }

    #endregion

    #region Abs

    [Test]
    public void Abs_Negative_ReturnsPositive()
    {
      Assert.That(FMath.Abs(-3.5f), Is.EqualTo(3.5f));
    }

    [Test]
    public void Abs_Positive_ReturnsSame()
    {
      Assert.That(FMath.Abs(3.5f), Is.EqualTo(3.5f));
    }

    [Test]
    public void Abs_Zero_ReturnsZero()
    {
      Assert.That(FMath.Abs(0f), Is.EqualTo(0f));
    }

    #endregion

    #region IsNearlyEqual

    [Test]
    public void IsNearlyEqual_SameValues_ReturnsTrue()
    {
      Assert.That(FMath.IsNearlyEqual(1f, 1f), Is.True);
    }

    [Test]
    public void IsNearlyEqual_TinyDifference_ReturnsTrue()
    {
      Assert.That(FMath.IsNearlyEqual(1f, 1f + FMath.KINDA_SMALL_NUMBER * 0.5f), Is.True);
    }

    [Test]
    public void IsNearlyEqual_LargeDifference_ReturnsFalse()
    {
      Assert.That(FMath.IsNearlyEqual(1f, 2f), Is.False);
    }

    #endregion

    #region IsNearlyZero

    [Test]
    public void IsNearlyZero_Zero_ReturnsTrue()
    {
      Assert.That(FMath.IsNearlyZero(0f), Is.True);
    }

    [Test]
    public void IsNearlyZero_LargeValue_ReturnsFalse()
    {
      Assert.That(FMath.IsNearlyZero(1f), Is.False);
    }

    #endregion

    #region Floor / Ceil / Round

    [Test]
    public void Floor_PositiveDecimal_RoundsDown()
    {
      Assert.That(FMath.Floor(2.9f), Is.EqualTo(2f));
    }

    [Test]
    public void Floor_NegativeDecimal_RoundsDown()
    {
      Assert.That(FMath.Floor(-2.1f), Is.EqualTo(-3f));
    }

    [Test]
    public void Ceil_PositiveDecimal_RoundsUp()
    {
      Assert.That(FMath.Ceil(2.1f), Is.EqualTo(3f));
    }

    [Test]
    public void Ceil_ExactInteger_ReturnsSame()
    {
      Assert.That(FMath.Ceil(3f), Is.EqualTo(3f));
    }

    [Test]
    public void Round_HalfPoint_RoundsUp()
    {
      Assert.That(FMath.Round(2.5f), Is.EqualTo(3f));
    }

    [Test]
    public void Round_BelowHalf_RoundsDown()
    {
      Assert.That(FMath.Round(2.4f), Is.EqualTo(2f));
    }

    #endregion

    #region Sign

    [Test]
    public void Sign_Positive_ReturnsOne()
    {
      Assert.That(FMath.Sign(5f), Is.EqualTo(1f));
    }

    [Test]
    public void Sign_Negative_ReturnsMinusOne()
    {
      Assert.That(FMath.Sign(-5f), Is.EqualTo(-1f));
    }

    [Test]
    public void Sign_Zero_ReturnsOne()
    {
      Assert.That(FMath.Sign(0f), Is.EqualTo(1f));
    }

    [Test]
    public void SignInt_Negative_ReturnsMinusOne()
    {
      Assert.That(FMath.SignInt(-3f), Is.EqualTo(-1));
    }

    #endregion

    #region DeltaAngle

    [Test]
    public void DeltaAngle_NoChange_ReturnsZero()
    {
      Assert.That(FMath.DeltaAngle(90f, 90f), Is.EqualTo(0f).Within(0.001f));
    }

    [Test]
    public void DeltaAngle_WrapsAround_ReturnsShortestPath()
    {
      // from 350° to 10° → +20°
      Assert.That(FMath.DeltaAngle(350f, 10f), Is.EqualTo(20f).Within(0.001f));
    }

    [Test]
    public void DeltaAngle_OppositeDirection_ReturnsNegative()
    {
      // from 10° to 350° → -20°
      Assert.That(FMath.DeltaAngle(10f, 350f), Is.EqualTo(-20f).Within(0.001f));
    }

    #endregion

    #region Max / Min (Span)

    [Test]
    public void Max_Span_ReturnsMaximum()
    {
      float[] values = { 1f, 5f, 3f, 2f };
      Assert.That(FMath.Max(values), Is.EqualTo(5f));
    }

    [Test]
    public void Min_Span_ReturnsMinimum()
    {
      float[] values = { 1f, 5f, 3f, 2f };
      Assert.That(FMath.Min(values), Is.EqualTo(1f));
    }

    [Test]
    public void Max_SingleElement_ReturnsThatElement()
    {
      float[] values = { 42f };
      Assert.That(FMath.Max(values), Is.EqualTo(42f));
    }

    [Test]
    public void Max_EmptySpan_ThrowsArgumentException()
    {
      Assert.Throws<System.ArgumentException>(() => FMath.Max(System.Array.Empty<float>()));
    }

    [Test]
    public void Min_EmptySpan_ThrowsArgumentException()
    {
      Assert.Throws<System.ArgumentException>(() => FMath.Min(System.Array.Empty<float>()));
    }

    #endregion

    #region Map

    [Test]
    public void Map_MidpointInput_ReturnsMidpointOutput()
    {
      // 5 from range [0..10] → 50 from [0..100]
      Assert.That(FMath.Map(5f, 0f, 10f, 0f, 100f), Is.EqualTo(50f).Within(0.001f));
    }

    [Test]
    public void Map_MinInput_ReturnsToMin()
    {
      Assert.That(FMath.Map(0f, 0f, 10f, 0f, 100f), Is.EqualTo(0f).Within(0.001f));
    }

    [Test]
    public void Map_MaxInput_ReturnsToMax()
    {
      Assert.That(FMath.Map(10f, 0f, 10f, 0f, 100f), Is.EqualTo(100f).Within(0.001f));
    }

    #endregion

    #region DistanceSquared / FastDistance / FastLength

    [Test]
    public void DistanceSquared_KnownPoints_ReturnsCorrectValue()
    {
      // From (0,0,0) to (3,4,0) → 25
      Assert.That(FMath.DistanceSquared(0f, 0f, 0f, 3f, 4f, 0f), Is.EqualTo(25f).Within(0.001f));
    }

    [Test]
    public void DistanceSquared_SamePoint_ReturnsZero()
    {
      Assert.That(FMath.DistanceSquared(1f, 2f, 3f, 1f, 2f, 3f), Is.EqualTo(0f));
    }

    [Test]
    public void FastDistance_KnownPoints_ReturnsApproximatelyCorrect()
    {
      // From (0,0,0) to (3,4,0) → 5
      Assert.That(FMath.FastDistance(0f, 0f, 0f, 3f, 4f, 0f), Is.EqualTo(5f).Within(0.05f));
    }

    [Test]
    public void FastLength_ZeroVector_ReturnsZero()
    {
      Assert.That(FMath.FastLength(0f, 0f, 0f), Is.EqualTo(0f));
    }

    #endregion

    #region FastNormalize

    [Test]
    public void FastNormalize_ZeroVector_SetsToZero()
    {
      float x = 0f, y = 0f, z = 0f;
      FMath.FastNormalize(ref x, ref y, ref z);
      Assert.That(x, Is.EqualTo(0f));
      Assert.That(y, Is.EqualTo(0f));
      Assert.That(z, Is.EqualTo(0f));
    }

    [Test]
    public void FastNormalize_UnitX_RemainsUnitX()
    {
      float x = 1f, y = 0f, z = 0f;
      FMath.FastNormalize(ref x, ref y, ref z);
      Assert.That(x, Is.EqualTo(1f).Within(Tolerance));
    }

    [Test]
    public void FastNormalize_LargeVector_ResultHasApproxLengthOne()
    {
      float x = 10f, y = 0f, z = 0f;
      FMath.FastNormalize(ref x, ref y, ref z);
      float length = x * x + y * y + z * z;
      Assert.That(length, Is.EqualTo(1f).Within(Tolerance));
    }

    #endregion
  }
}
