using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Narrate
{
      public class LinesDistribution 
      {
            public static void SetActorLines(Actor[] actors,string text)
            {
                  string[] lines = text.Split('\n');
                  for(int i = 0;i<lines.Length;i++)
                  {
                        string[] actorIdAndLine = lines[0].Split(':');
                        string actorID = actorIdAndLine[0];
                        string line = actorIdAndLine[1];
                        Actor actor = Array.Find(actors,a=>a.id== actorID);
                        actor.lines.Enqueue(line) ;
                  }
            }
      }
}