// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

public class Test : MonoBehaviour
{
  private void Start()
  {
    var test = Resources.Load<GameObject>("Player/Player");
    Debug.Log(test);
  }


  // Update is called once per frame
  void Update()
  {

  }
}
