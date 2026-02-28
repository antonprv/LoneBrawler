// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions;

using NUnit.Framework;

namespace Code.Tests.EditMode.Extensions
{
  [TestFixture]
  public class ArrayExtensionsTests
  {
    [Test]
    public void Empty_NullArray_DoesNotThrow()
    {
      string[] arr = null;
      Assert.DoesNotThrow(() => arr.Empty());
    }

    [Test]
    public void Empty_NonNullArray_SetsAllToNull()
    {
      var arr = new string[] { "a", "b", "c" };
      arr.Empty();
      Assert.That(arr[0], Is.Null);
      Assert.That(arr[1], Is.Null);
      Assert.That(arr[2], Is.Null);
    }

    [Test]
    public void Empty_EmptyArray_DoesNotThrow()
    {
      var arr = new string[0];
      Assert.DoesNotThrow(() => arr.Empty());
    }

    [Test]
    public void Empty_ArrayLengthUnchanged()
    {
      var arr = new string[] { "x", "y" };
      arr.Empty();
      Assert.That(arr.Length, Is.EqualTo(2));
    }
  }
}
