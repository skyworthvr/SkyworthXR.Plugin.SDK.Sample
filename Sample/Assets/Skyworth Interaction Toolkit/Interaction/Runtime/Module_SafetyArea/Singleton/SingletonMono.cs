using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SC.XR.Unity
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        private static object lockObj = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        lock (lockObj)
                        {
                            if (instance == null)
                            {
                                GameObject singleton = new GameObject();
                                instance = singleton.AddComponent<T>();
                                singleton.name = "(Singleton) " + typeof(T).Name;
                                DontDestroyOnLoad(singleton);
                            }
                        }
                    }
                }
                return instance;
            }
        }
    }
}
