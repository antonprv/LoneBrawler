// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes.Collections
{
  [TestFixture]
  public class HashSetDataTests
  {
    private HashSetData<string> _set;

    [SetUp]
    public void SetUp() => _set = new HashSetData<string>();

    [Test]
    public void Default_IsEmpty()
    {
      Assert.That(_set.Count, Is.EqualTo(0));
    }

    [Test]
    public void Add_NewItem_Succeeds()
    {
      _set.Add("item1");
      Assert.That(_set.Contains("item1"), Is.True);
    }

    [Test]
    public void Add_Duplicate_NotDuplicated()
    {
      _set.Add("same");
      _set.Add("same");
      Assert.That(_set.Count, Is.EqualTo(1));
    }

    [Test]
    public void Remove_ExistingItem_Succeeds()
    {
      _set.Add("item");
      _set.Remove("item");
      Assert.That(_set.Contains("item"), Is.False);
    }

    [Test]
    public void OnBeforeSerialize_ThenOnAfterDeserialize_RestoresItems()
    {
      _set.Add("alpha");
      _set.Add("beta");
      _set.OnBeforeSerialize();
      _set.Clear();
      _set.OnAfterDeserialize();
      Assert.That(_set.Contains("alpha"), Is.True);
      Assert.That(_set.Contains("beta"), Is.True);
    }

    [Test]
    public void Serialization_RoundtripPreservesCount()
    {
      _set.Add("x");
      _set.Add("y");
      _set.Add("z");
      _set.OnBeforeSerialize();
      _set.Clear();
      _set.OnAfterDeserialize();
      Assert.That(_set.Count, Is.EqualTo(3));
    }

    [Test]
    public void OnAfterDeserialize_EmptyData_ClearsSet()
    {
      _set.Add("something");
      _set.OnBeforeSerialize();
      _set.Clear();
      // manually clear serialized data before deserialization
      _set = new HashSetData<string>(); // fresh, empty internal list
      _set.OnAfterDeserialize();
      Assert.That(_set.Count, Is.EqualTo(0));
    }
  }
}
