using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MQOD
{
    public class UnityHelper
    {
        public static GameObject FindRootGameObjectInSceneByName(string scene, string name)
        {
            
            Scene s = SceneManager.GetSceneByName(scene);
            IEnumerator<GameObject> i = s.GetRootGameObjects().Where(o => o.name == name).GetEnumerator();
            GameObject gameObject = i.Current;
            if (gameObject == null)throw new NullReferenceException($"Could not find GameObject with name {name} at root of {scene}");
            if (i.MoveNext()) MelonLogger.Warning($"Multiple GameObjects with name {name} exist at root of {scene}");
            return gameObject;
        }
    }
}