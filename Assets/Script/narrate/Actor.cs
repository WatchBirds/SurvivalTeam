using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace Narrate
{
      public class Actor
      {
            public string id;
            public string name;
            public Queue<string> lines = new Queue<string>();
            public Transform traget;
            public Image image;

            public void SpeakLine(Action<string, string> action)
            {
                  if(lines.Count<=0)
                  {
                        Debug.Log("Actor:"+ id+":SpeakOver");
                        return;
                  }
                  string line = lines.Dequeue();
                  action.Invoke(line,name);
            }
           public IEnumerator SpeakWord(Action<char,string,bool> action,float speed)
            {
                  string line = lines.Peek();
                  Debug.Log(line);
                  for(int i = 0;i<line.Length;i++)
                  {
                        action?.Invoke(line[i], name, i==0);
                        yield return new WaitForSeconds(speed);
                  }
                  lines.Dequeue();
            }
            
      }
}