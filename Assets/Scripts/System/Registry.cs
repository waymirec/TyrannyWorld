using UnityEngine;

namespace System
{
    public class Registry : MonoBehaviour
    {

        public static T Get<T>() where T : Singleton<T>
        {
            return Singleton<T>.Instance;
        }
    }
}