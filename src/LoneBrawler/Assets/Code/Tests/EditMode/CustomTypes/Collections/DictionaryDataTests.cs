// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;

using NUnit.Framework;

namespace Code.Tests.EditMode.CustomTypes.Collections
{
  [TestFixture]
  public class DictionaryDataTests
  {
    private DictionaryData<string, int> _dict;

    [SetUp]
    public void SetUp() => _dict = new DictionaryData<string, int>();

    [Test]
    public void Default_IsEmpty()
    {
      Assert.That(_dict.Count, Is.EqualTo(0));
    }

    [Test]
    public void Add_ThenContainsKey()
    {
      _dict.Add("alpha", 1);
      Assert.That(_dict.ContainsKey("alpha"), Is.True);
    }

    [Test]
    public void Indexer_Set_CanReadBack()
    {
      _dict["key"] = 42;
      Assert.That(_dict["key"], Is.EqualTo(42));
    }

    [Test]
    public void Remove_ExistingKey_ReturnsTrueAndRemoves()
    {
      _dict.Add("x", 10);
      bool result = _dict.Remove("x");
      Assert.That(result, Is.True);
      Assert.That(_dict.ContainsKey("x"), Is.False);
    }

    [Test]
    public void Remove_NonExistingKey_ReturnsFalse()
    {
      bool result = _dict.Remove("nonexistent");
      Assert.That(result, Is.False);
    }

    [Test]
    public void Clear_RemovesAllEntries()
    {
      _dict.Add("a", 1);
      _dict.Add("b", 2);
      _dict.Clear();
      Assert.That(_dict.Count, Is.EqualTo(0));
    }

    [Test]
    public void TryAdd_NewKey_ReturnsTrueAndAdds()
    {
      bool result = _dict.TryAdd("newkey", 99);
      Assert.That(result, Is.True);
      Assert.That(_dict["newkey"], Is.EqualTo(99));
    }

    [Test]
    public void TryAdd_DuplicateKey_ReturnsFalse()
    {
      _dict.Add("dup", 1);
      bool result = _dict.TryAdd("dup", 2);
      Assert.That(result, Is.False);
      Assert.That(_dict["dup"], Is.EqualTo(1)); // unchanged
    }

    [Test]
    public void ForceSerialization_DoesNotThrow()
    {
      _dict.Add("test", 1);
      Assert.DoesNotThrow(() => _dict.ForceSerialization());
    }

    [Test]
    public void OnBeforeSerialize_DoesNotThrow()
    {
      _dict.Add("key", 5);
      Assert.DoesNotThrow(() => _dict.OnBeforeSerialize());
    }

    [Test]
    public void OnAfterDeserialize_AfterOnBeforeSerialize_RebuildsDictionary()
    {
      _dict.Add("a", 1);
      _dict.Add("b", 2);
      _dict.OnBeforeSerialize();  // save to lists
      _dict.Clear();              // destroy dictionary
      _dict.OnAfterDeserialize(); // rebuild from lists
      Assert.That(_dict.ContainsKey("a"), Is.True);
      Assert.That(_dict.ContainsKey("b"), Is.True);
    }

    [Test]
    public void TryGetValue_ExistingKey_ReturnsValue()
    {
      _dict.Add("hello", 123);
      bool found = _dict.TryGetValue("hello", out int val);
      Assert.That(found, Is.True);
      Assert.That(val, Is.EqualTo(123));
    }

    [Test]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
      bool found = _dict.TryGetValue("missing", out int _);
      Assert.That(found, Is.False);
    }

    [Test]
    public void MultipleEntries_Survive_SerializeRoundtrip()
    {
      for (int i = 0; i < 5; i++)
        _dict.Add($"key{i}", i * 10);

      _dict.OnBeforeSerialize();
      _dict.Clear();
      _dict.OnAfterDeserialize();

      Assert.That(_dict.Count, Is.EqualTo(5));
      for (int i = 0; i < 5; i++)
        Assert.That(_dict[$"key{i}"], Is.EqualTo(i * 10));
    }
  }
}
