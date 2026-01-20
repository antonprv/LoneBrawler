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
