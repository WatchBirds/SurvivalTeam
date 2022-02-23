using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Narrate
{

      public class NarrateCore : MonoBehaviour
      {
            public float narrateSpeed;
            public float wordSpeed;
            public event Action<char,string,bool> OnWordAction;
            public event Action<string, string> OnAction;
            public static NarrateCore Instance { get => narrate; }
            private static NarrateCore narrate;
            private void Awake()
            {
                  narrate = this;
            }
            public void StartAction(Actor[] actors,string text)
            {
                  SetActorLines(actors,text);
                  StartCoroutine(Action(actors));
            }
            public bool word;
            private Coroutine coroutine;
            private IEnumerator Action(Actor[] actors)
            {
                  while (true)
                  {
                        if (Order.Count <= 0)
                        {
                              Debug.Log("¶Ô»°½áÊø");
                              break;
                        }
                        string actorId = Order.Dequeue();
                        Debug.Log(actorId);
                        Actor actor = Array.Find(actors, a => a.id == actorId);
                        if (word)
                        {
                              yield return coroutine = StartCoroutine(actor.SpeakWord((char c, string name, bool b) => { OnWordAction?.Invoke(c, name, b); }, wordSpeed));
                        }
                        else
                        {
                              if(coroutine!=null)
                              {
                                    StopCoroutine(coroutine);
                              }
                              actor.SpeakLine((string c, string name) => { OnAction?.Invoke(c, name); });
                        }
                        yield return new WaitForSeconds(narrateSpeed);
                  }
            }
            
            private Queue<string> Order = new Queue<string>();
            private void SetActorLines(Actor[] actors, string text)
            {
                  string[] lines = text.Split('\n');
                  for (int i = 0; i < lines.Length; i++)
                  {
                        string[] actorIdAndLine = lines[i].Split(':');
                        string actorID = actorIdAndLine[0];
                        string line = actorIdAndLine[1];
                        Actor actor = Array.Find(actors, a => a.id == actorID);
                        actor.lines.Enqueue(line);
                        RecordOrder(actorID);
                  }
            }
            private void RecordOrder(string actorID)
            {
                  Order.Enqueue(actorID);
            }
      }
}