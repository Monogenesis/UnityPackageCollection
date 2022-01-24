using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestScript : MonoBehaviour
{
   public  void Method1(ChangeEvent<bool> evt)
   {
      VisualElement target = (VisualElement) evt.target;
      target.style.backgroundColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
   }

   public void Method2()
   {
      Debug.Log("whoww");
   }

   public void QuitApplication()
   {
      Application.Quit();
   }
}
