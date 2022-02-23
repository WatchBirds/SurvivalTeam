using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Narrate
{
      public class Test : MonoBehaviour
      {
            public TextAsset text;
            // Start is called before the first frame update
            public Text text2;
            public Button button;
            private void Start()
            {
                  text2.text = string.Empty;
                  NarrateCore.Instance.OnWordAction += NarrateCore_OnWordAction;
                  NarrateCore.Instance.OnAction += Instance_OnAction;
                  Actor[] actor = new Actor[] { new Actor() { id ="A"},new Actor() { id="B"} };
                  button.onClick.AddListener(()=> { NarrateCore.Instance.StartAction(actor, text.text); });
            }

            private void Instance_OnAction(string arg1, string arg2)
            {
                  text2.text = arg1;
            }

            private void NarrateCore_OnWordAction(char obj,string name,bool b)
            {
                  if(b)
                  { text2.text = string.Empty; }
                  text2.text += obj;
            }
      }
}