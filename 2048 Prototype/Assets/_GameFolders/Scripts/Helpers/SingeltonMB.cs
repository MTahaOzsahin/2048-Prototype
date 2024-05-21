using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMB<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        private static readonly object Lock = new object();
        
        private static bool isQuitting = false;

        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null)
                    {
                        instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return instance;
                        }

                        if (instance == null && !isQuitting) return null;
                    }

                    return instance;
                }
            }
        }
    
        public void OnApplicationQuit()
        {
            isQuitting = true;
        }
    }