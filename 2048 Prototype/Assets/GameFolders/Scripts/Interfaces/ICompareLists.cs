using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Scripts.Interfaces
{
    public interface ICompareLists
    {
        /// <summary>
        /// Compare two list if they are contain speceficly epual/same objects no matter order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aListA"></param>
        /// <param name="aListB"></param>
        /// <returns></returns>
        public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
        {
            if (aListA == null || aListB == null || aListA.Count != aListB.Count)
                return false;
            if (aListA.Count == 0)
                return true;
            Dictionary<T, int> lookUp = new Dictionary<T, int>();
            // create index for the first list
            for (int i = 0; i < aListA.Count; i++)
            {
                int count = 0;
                if (!lookUp.TryGetValue(aListA[i], out count))
                {
                    lookUp.Add(aListA[i], 1);
                    continue;
                }
                lookUp[aListA[i]] = count + 1;
            }
            for (int i = 0; i < aListB.Count; i++)
            {
                int count = 0;
                if (!lookUp.TryGetValue(aListB[i], out count))
                {
                    // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                    return false;
                }
                count--;
                if (count <= 0)
                    lookUp.Remove(aListB[i]);
                else
                    lookUp[aListB[i]] = count;
            }
            // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
            return lookUp.Count == 0;

            /// <summary>
            /// Example how to use.
            /// </summary>
            //List<int> A = new List<int>(new int[] { 1, 5, 6, 7, 3, 1 });
            //List<int> B = new List<int>(new int[] { 6, 3, 5, 1, 1, 7 });
            //if (CompareLists(A, B))
            //    Debug.Log("Equal");
            //else
            //    Debug.Log("Not equal");
        }
    }
}
