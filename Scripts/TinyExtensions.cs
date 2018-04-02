using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TP.ExtensionMethods
{
    /// <summary>
    /// Usefull extenstions methods intened to increase productivity and readablity.
    /// </summary>
    public static class ExtensionMethods
    {
        public static bool masterVerbose = false;

        /// <summary>
        /// Selects an element from a list at random. Each
        /// Item has a weight value and higher weights
        /// increase their chance of being chosen.
        /// </summary>
        /// <param name="weights">A list of weights in an Item list</param>
        /// <returns>The index of the Item in the original list to be picked</returns>
        public static int GetWeightedIndex(this List<float> weights)
        {
            int index = -1;
            float maxChoice = 0;
            foreach (float weight in weights)
            {
                maxChoice += weight;
            }
            float randChoice = Random.Range(0, maxChoice);
            float weightSum = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                weightSum += weights[i];
                if (randChoice <= weightSum)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static Bounds Encapsulate(this Bounds boundsA, Bounds boundsB, bool ignoreEmpty)
        {
            if (!ignoreEmpty)
            {
                boundsA.Encapsulate(boundsB);
            }
            else
            {
                if (boundsB.extents != Vector3.zero)
                {
                    if (boundsA.extents == Vector3.zero)
                    {
                        boundsA = boundsB;
                    }
                    else
                    {
                        boundsA.Encapsulate(boundsB);
                    }
                }
            }

            return boundsA;
        }

        public static bool IsPrefab(this GameObject gameObject)
        {
            bool isPrefab = false;

            if (gameObject != null && gameObject.scene.name == null ||
                gameObject != null && gameObject.gameObject != null && gameObject.gameObject.scene.name == null)
            {
                isPrefab = true;
            }

            return isPrefab;
        }

        /// <summary>
        /// An overload for Kill that has no delay.
        /// </summary>
        /// <param name="gameObject">A Self GameObject reference</param>
        //public static void Kill(this GameObject gameObject)
        //{
        //    Kill(gameObject, 0);
        //}

        /// <summary>
        /// An overload for KillDelayed that references this GameObject.
        /// </summary>
        /// <param name="gameObject">A Self GameObject reference</param>
        /// <param name="delay">The duration of the delay before killing gameObject</param>
        //public static void Kill(this GameObject gameObject, float delay)
        //{
        //    KillDelayed(gameObject, delay);
        //}

        /// <summary>
        /// Sets off all CreateOnKill's on target GameObject before
        /// blowing it up.
        /// </summary>
        /// <param name="gameObject">The GameObject to kill</param>
        /// <param name="delay">How long to delay the BlowUp</param>
        //public static void KillDelayed(GameObject gameObject, float delay)
        //{
        //    CreateOnKill[] onKills = gameObject.GetComponents<CreateOnKill>();
        //    foreach (CreateOnKill onKill in onKills)
        //    {
        //        onKill.Trigger();
        //    }

        //    gameObject.BlowUp(delay);
        //}

        /// <summary>
        /// Destroys every GameObject within a list.
        /// </summary>
        /// <param name="gameObjectList">The list of GameObjects to be destroyed</param>
        public static void BlowUp(this List<GameObject> gameObjectList)
        {
            for (int i = gameObjectList.Count - 1; i >= 0; i--)
            {
                gameObjectList[i].BlowUp();
            }
            gameObjectList.Clear();
        }

        /// <summary>
        /// Overload for BlowUp to blow self up with no delay.
        /// </summary>
        /// <param name="objectToBlowUp">A Self GameObject reference</param>
        public static void BlowUp(this Object objectToBlowUp)
        {
            BlowUp(objectToBlowUp, 0);
        }

        /// <summary>
        /// Overload for BlowUpDelayed to blow self up with a specified delay.
        /// </summary>
        /// <param name="gameObject">A Self GameObject reference</param>
        /// <param name="delay">How long to delay the BlowUp</param>
        public static void BlowUp(this Object objectToBlowUp, float delay)
        {
            BlowUpDelayed(objectToBlowUp, delay);
        }

        /// <summary>
        /// Destroys the provided GameObject. Has a delay if the application
        /// is playing, otherwise it is destroyed immediately.

        /// The reason for this is that destroy doesnt work in editor mode. 
        /// This allows us to use the same destroy logic whether the game is
        /// playing or not.
        /// </summary>
        /// <param name="gameObject">The GameObject to destroy</param>
        /// <param name="delay">How long to delay if the application is playing</param>
        private static void BlowUpDelayed(Object objectToBlowUp, float delay)
        {
            try // There is a chance the GameObject has already been destroyed
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(objectToBlowUp, delay);
                }
                else
                {
                    GameObject.DestroyImmediate(objectToBlowUp);
                }
            }
            catch (NullReferenceException exception)
            {
                // Print debug message and include the exception message in order to remove unused variable warning
                Debug.Log("Tried destroying " + objectToBlowUp + " but value is null: " + exception.Message);
            }
        }

        /// <summary>
        /// Gets the closes active GameObject to the origin GameObject.
        /// </summary>
        /// <param name="origin">A Self GameObject reference</param>
        /// <param name="gameObjectList">An IEnumerable List of gameObjects to pick the closest one from</param>
        /// <returns>A reference to the nearest GameObject from the provided list</returns>
        public static GameObject Nearest(this GameObject origin, IEnumerable<MonoBehaviour> gameObjectList)
        {
            GameObject nearest = null;
            float nearestDistance = Mathf.Infinity;

            for (int i = 0; i < gameObjectList.Count(); i++)
            {
                GameObject tempGameObject = gameObjectList.ElementAt(i).gameObject;

                if (tempGameObject.activeInHierarchy)
                {
                    float gameObjectDistance = Vector3.Distance(origin.transform.position, tempGameObject.transform.position);

                    if (gameObjectDistance < nearestDistance)
                    {
                        nearest = tempGameObject;
                        nearestDistance = gameObjectDistance;
                    }
                }
            }

            return nearest;
        }

        /// <summary>
        /// Removes an element at a given index from an array in place.
        /// </summary>
        /// <typeparam name="T">The type of array we are working with</typeparam>
        /// <param name="arr">The array to have an element removed</param>
        /// <param name="index">The index of the element to remove</param>
        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        }

        /// <summary>
        /// Add one new element to the end of an array.
        /// </summary>
        /// <typeparam name="T">The type of array we are working with</typeparam>
        /// <param name="array">The array to be appened; self referential</param>
        /// <param name="itemToAppend">The element to be added to the end of the array</param>
        /// <returns></returns>
        public static T[] Prepend<T>(this Array array, T itemToPrepend)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, 0, newArray, 1, array.Length);
            newArray[0] = itemToPrepend;
            return newArray;
        }

        /// <summary>
        /// Add one new element to the end of an array.
        /// </summary>
        /// <typeparam name="T">The type of array we are working with</typeparam>
        /// <param name="array">The array to be appened; self referential</param>
        /// <param name="itemToAppend">The element to be added to the end of the array</param>
        /// <returns></returns>
        public static T[] Append<T>(this Array array, T itemToAppend)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[newArray.Length - 1] = itemToAppend;
            return newArray;
        }

        /// <summary>
        /// Removes the last element from an array
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="array">The array to have the element removed; self referential</param>
        /// <returns></returns>
        public static T[] RemoveLast<T>(this Array array)
        {
            T[] newArray = new T[array.Length - 1];
            Array.Copy(array, newArray, newArray.Length);
            return newArray;
        }

        /// <summary>
        /// An array overload for GetComponentInChildrenAndSelf
        /// </summary>
        /// <typeparam name="T">The component type to be found</typeparam>
        /// <param name="gameObject">The GaneObject to get these components from</param>
        /// <returns>An array of the components of type T found on the given GameObject or its children</returns>
        public static T[] GetComponentsInChildrenAndSelf<T>(this GameObject gameObject)
        {
            return gameObject.GetComponentsInChildren<T>();
        }

        /// <summary>
        /// Converts a 3D point to level space.
        /// </summary>
        /// <param name="vector3">The point to convert; self referential</param>
        /// <returns>The converted point</returns>
        //public static Vector3 ToLevelSpace(this Vector3 vector3)
        //{
        //    return LevelTransform.Rotation * vector3;
        //}

        /// <summary>
        /// Converts a 2D point to level space.
        /// </summary>
        /// <param name="vector2">The point to convert; self referential</param>
        /// <returns>The converted point</returns>
        //public static Vector3 ToLevelSpace(this Vector2 vector2)
        //{
        //    Vector3 newVector = new Vector3(vector2.x, 0, vector2.y);
        //    return LevelTransform.Rotation * newVector;
        //}

        /// <summary>
        /// Gets the y value of the vector in level space.
        /// </summary>
        /// <param name="vector3">The vector to get the value from; self referential</param>
        /// <returns>The y value of the vector</returns>
        //public static float Up(this Vector3 vector3)
        //{
        //    return vector3.ToLevelSpace().y;
        //}

        /// <summary>
        /// Gets the z value of the vector in level space.
        /// </summary>
        /// <param name="vector3">The vector to get the value from; self referential</param>
        /// <returns>The z value of the vector</returns>
        //public static float Forward(this Vector3 vector3)
        //{
        //    return vector3.ToLevelSpace().z;
        //}

        /// <summary>
        /// Convert a 2D point to 3D. INCOMPLETE
        /// </summary>
        /// <param name="vector2">The point to be converted; self referential</param>
        /// <returns>The 3D point</returns>
        public static Vector3 ToVector3(this Vector2 vector2)
        {
            //  TODO: Update this to use the current up direction for the game rather than
            //  assuming that z is up. or take a second param "axis" of type int.
            return new Vector3(vector2.x, vector2.y, 0);
        }

        public static Vector3 ToVector3(this float number)
        {
            return new Vector3(number, number, number);
        }

        public static int ParentDepth(this Transform transform)
        {
            int parentCount = 0;

            Transform root = transform.root;
            Transform current = transform;

            int maxParents = transform.hierarchyCount;

            for (; current != root; parentCount++)
            {
                current = current.parent;

                if (parentCount > maxParents)
                {
                    Debug.Log(@"TinyExtensions.ParentDepth: Something went wrong while attepting to calculate parent depth.
                        Root transform was not detected properly.");

                    break;
                }
            }

            return parentCount;
        }

        public static void AddUnique<T>(this List<T> list, T newItem)
        {
            if (newItem != null && !list.Contains<T>(newItem))
            {
                list.Add(newItem);
            }
        }

        public static void AddUniqueRange<T>(this List<T> list, List<T> range)
        {
            foreach (T item in range)
            {
                if (!list.Contains<T>(item))
                {
                    list.Add(item);
                }
            }
        }

        public static void RemoveRange<T>(this List<T> list, List<T> range)
        {
            foreach (T item in range)
            {
                list.Remove(item);
            }
        }

        public static List<Type> RequiredComponents (this Component component)
        {
            List<Type> requiredComponentTypes = new List<Type> { };

            Type componentType = component.GetType();
            RequireComponent[] requireComponentAttrs = Attribute.GetCustomAttributes(componentType, typeof(RequireComponent), true) as RequireComponent[];

            foreach (RequireComponent requireComponentAttr in requireComponentAttrs)
            {                    
                requiredComponentTypes.AddUnique(requireComponentAttr.m_Type0);
                requiredComponentTypes.AddUnique(requireComponentAttr.m_Type1);
                requiredComponentTypes.AddUnique(requireComponentAttr.m_Type2);
            }

            return requiredComponentTypes;
        }

        public static List<Component> RequiredByComponents (this Component component, List<Component> componentsToCheck)
        {
            List<Component> requiredByComponents = new List<Component> { };
            
            Type componentType = component.GetType();

            foreach (Component componentToCheck in componentsToCheck)
            {
                List<Type> requiredComponents = RequiredComponents(componentToCheck);

                if (requiredComponents.Contains(componentType))
                {
                    requiredByComponents.Add(componentToCheck);
                }
            }

            return requiredByComponents;
        }

        public static bool IsOnNavMesh(this Vector3 position)
        {
            NavMeshHit hit = new NavMeshHit();
            return NavMesh.SamplePosition(position, out hit, 1, NavMesh.AllAreas);
        }
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static bool LongerThan(this Vector3 vectorA, float lengthB)
        {
            bool isLongerThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = lengthB * lengthB;
            if (squareLengthA > squareLengthB)
            {
                isLongerThan = true;
            }

            return isLongerThan;
        }
        
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static bool ShorterThan(this Vector3 vectorA, float lengthB)
        {
            bool isShorterThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = lengthB * lengthB;
            if (squareLengthA < squareLengthB)
            {
                isShorterThan = true;
            }

            return isShorterThan;
        }
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static bool LongerThan(this Vector3 vectorA, Vector3 vectorB)
        {
            bool isLongerThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = vectorB.sqrMagnitude;
            if (squareLengthA > squareLengthB)
            {
                isLongerThan = true;
            }

            return isLongerThan;
        }
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static bool ShorterThan(this Vector3 vectorA, Vector3 vectorB)
        {
            bool isShorterThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = vectorB.sqrMagnitude;
            if (squareLengthA < squareLengthB)
            {
                isShorterThan = true;
            }

            return isShorterThan;
        }
        
        /// <summary>
        /// Accurate way of move towards something and snapping exactly to the final value. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static Vector3 MoveTowardsSnap(this Vector3 vectorA, Vector3 vectorB, float maxDistanceDelta)
        {
            if ((vectorA - vectorB).ShorterThan(maxDistanceDelta))
            {
                vectorA = vectorB;
            }
            else
            {
                vectorA = Vector3.MoveTowards(vectorA, vectorB, maxDistanceDelta);
            }

            return vectorA;
        }

        /// <summary>
        /// Convert a 3D point to 2D. INCOMPLETE
        /// </summary>
        /// <param name="vector3">The point to be converted; self referential</param>
        /// <returns>The 2D point</returns>
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            //  TODO: Update this to use the current up direction for the game rather than
            //  assuming that z is up. or take a second param "axis" of type int.
            return new Vector2(vector3.x, vector3.z);
        }
        
        /// <summary>
        /// Accurate way of rotate towards something and snapping exactly to the final value. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static Quaternion RotateTowardsSnap(this Quaternion rotationA, Quaternion rotationB, float maxDegreesDelta)
        {
            if (Quaternion.Angle(rotationA, rotationB) < maxDegreesDelta)
            {
                rotationA = rotationB;
            }
            else
            {
                rotationA = Quaternion.RotateTowards(rotationA, rotationB, maxDegreesDelta);
            }

            return rotationA;
        }
                
        /// <summary>
        /// Check to see if a layermask contains an individual layer.
        /// </summary>
        /// <param name="vector3">The point to be converted; self referential</param>
        /// <returns>The 2D point</returns>
        public static bool Contains(this LayerMask layerMask, int layer)
         {
             return layerMask == (layerMask | (1 << layer));
         }

        /// <summary>
        /// Scales a rect by a given factor
        /// </summary>
        /// <param name="rect">The rect to scale; self referential</param>
        /// <param name="scale">The factor to scale by</param>
        /// <returns>The scaled rect</returns>
        public static Rect ScaledCopy(this Rect rect, float scale)
        {
            float widthChange = rect.width * scale - rect.width;
            float heightChange = rect.height * scale - rect.height;

            rect.position = new Vector2(
                rect.x - widthChange / 2,
                rect.y - heightChange / 2);

            rect.width += widthChange;
            rect.height += heightChange;

            return rect;
        }

        #region Enumerable Management

        /// <summary>
        /// Picks an element at random from an array
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="array">The array to pick from; self referential</param>
        /// <returns>The randomly selected element</returns>
        public static T PickRandom<T>(this Array array)
        {
            int randIndex = UnityEngine.Random.Range(0, array.Length);
            T randomPick = (T)array.GetValue(randIndex);

            return randomPick;
        }

        /// <summary>
        /// Picks an element at random from an IEnumerable list.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type</typeparam>
        /// <param name="list">The list to pick an element from; self referential</param>
        /// <returns>The randomly selected element</returns>
        public static T PickRandom<T>(this IEnumerable<T> list)
        {
            int randIndex = UnityEngine.Random.Range(0, list.Count());
            T randomPick = list.ElementAt(randIndex);

            return randomPick;
        }

        /// <summary>
        /// Picks a random number of elements at random from within an IEnumerable list.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type</typeparam>
        /// <param name="list">The list to pick from; self referential</param>
        /// <param name="minNuberToPick">The min number of items to pick [inclusive]</param>
        /// <param name="maxNuberToPick">The max number of items to pick [exclusive]</param>
        /// <returns>The randomly chosen elements</returns>
        public static List<T> PickRandom<T>(this IEnumerable<T> list, int minNuberToPick, int maxNuberToPick)
        {
            return PickRandom<T>(list, Random.Range(minNuberToPick, maxNuberToPick));
        }

        /// <summary>
        /// Picks a given number of random elements from an IEnumerable list.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type</typeparam>
        /// <param name="list">The list to pick from; self referential</param>
        /// <param name="numberToPick">The number of elements that will be chosen at random</param>
        /// <returns>The randomly chosen elements</returns>
        public static List<T> PickRandom<T>(this IEnumerable<T> list, int numberToPick)
        {
            if (numberToPick > list.Count())
            {
                // This does not cause any error other than the list being shorter than expected,
                // but still should not occur so it is logged for debugging
                Debug.LogWarning("PickRandom: Attempted to pick more elements than exist in list");
            }

            List<T> randomPicks = new List<T> { };
            List<T> notPicked = new List<T>(list);

            for (int i = 0; i < numberToPick && notPicked.Count > 0; i++)
            {
                int randIndex = Random.Range(0, notPicked.Count);
                T randT = notPicked[randIndex];
                randomPicks.Add(randT);
                notPicked.Remove(randT);
            }

            return randomPicks;
        }

        public static bool AddUnique<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            bool added = false;
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                added = true;
            }
            else if (!Equals(dictionary[key], value))
            {
                Debug.LogWarning("You are trying assing a new value to a key that already exists in a dictionary.");
            }

            return added;
        }

        #endregion

        #region Deep Equality Checking

        public enum ValueEqualsReportMatchType {
            NONE = 0,
            NAMES_EQUAL = 1,
            TARGET_EQUAL = 2,
            VALUE_EQUAL = 3,
            EQUAL = 4,
            REFERENCE_EQUAL = 5
        }

        public struct ValueEqualsReportMatch
        {
            object match;
            ValueEqualsReportMatchType type; 

            public ValueEqualsReportMatch(object match, ValueEqualsReportMatchType type=ValueEqualsReportMatchType.NONE)
            {
                this.match = match;
                this.type = type;
            }

            public object Match { get { return match; } }
            public ValueEqualsReportMatchType Type { get { return type; } }
        }
        
        public class ValueEqualsReport
        {
            private bool equal = true;
            private Dictionary<object, ValueEqualsReportMatch> allMatches = new Dictionary<object, ValueEqualsReportMatch> { };
            private List<object> badMatches = new List<object> {};
            private List<object> goodMatches = new List<object> {};
            private int addCount = 0;
            private int addLimit = 0;
            private Type[] ignoreComponents;
            public List<string> activeNotes = new List<string> { };
            private Dictionary<object, List<string>> notes = new Dictionary<object, List<string>> { };

            public void AddNonMatch(object nonMatchedObject)
            {
                equal = false;

                if (nonMatchedObject == null)
                {
                    return;
                }

                ValueEqualsReportMatch matchA = new ValueEqualsReportMatch(null);
                bool matchFoundA = allMatches.TryGetValue(nonMatchedObject, out matchA);
                if (!matchFoundA)
                {
                    allMatches.AddUnique(nonMatchedObject, new ValueEqualsReportMatch(null));
                    badMatches.AddUnique(nonMatchedObject);
                    
                    GameObject nonMatchedGameObject =  nonMatchedObject as GameObject;
                    if (nonMatchedGameObject != null)
                    {
                        foreach (GameObject child in nonMatchedGameObject.Children())
                        {
                            AddNonMatch(child);
                        }

                        foreach (Component component in nonMatchedGameObject.GetComponents<Component>())
                        {
                            if (!ShouldIgnore(component))
                            {
                                AddNonMatch(component);
                            }
                        }
                    }
                }
            }

            public bool ShouldIgnore(object objectToCheck)
            {
                if (IgnoreComponents != null)
                {
                    if (IgnoreComponents.Contains(objectToCheck.GetType()))
                    {
                        return true;
                    }
                }

                return false;
            }

            public void AddMatch(object objectA, object objectB, ValueEqualsReportMatchType type)
            {
                if (objectA == null || objectB == null)
                {
                    return;
                }

                if (addLimit > 0 && addCount >= addLimit)
                {
                    return;
                }
                addCount++;

                ValueEqualsReportMatch matchA = new ValueEqualsReportMatch(null);
                ValueEqualsReportMatch matchB = new ValueEqualsReportMatch(null);
                bool matchAFound = allMatches.TryGetValue(objectA, out matchA);
                bool matchBFound = allMatches.TryGetValue(objectB, out matchB);
                
                bool betterMatchExists = false;
                if ((matchAFound && matchA.Type >= type) || (matchBFound && matchB.Type >= type))
                {
                    betterMatchExists = true;
                }

                if (!betterMatchExists)
                {
                    ReplaceMatch(
                        objectA: objectA, 
                        newMatch: new ValueEqualsReportMatch(objectB, type),
                        recursive: matchA.Type != ValueEqualsReportMatchType.NONE && matchA.Match != objectB
                    );
                }
            }

            private void AddMatch(object objectA, ValueEqualsReportMatch match)
            {       
                object objectB = match.Match;
                if (allMatches.AddUnique(objectA, new ValueEqualsReportMatch(objectB, match.Type)))
                {
                    if (objectA != objectB)
                    {
                        RemoveMatch(objectB);
                        allMatches.AddUnique(objectB, new ValueEqualsReportMatch(objectA, match.Type));
                    }
                    goodMatches.AddUnique(objectA);
                }
            }

            private void ReplaceMatch(object objectA, ValueEqualsReportMatch newMatch=default(ValueEqualsReportMatch), bool recursive=false)
            {
                ValueEqualsReportMatch matchA = new ValueEqualsReportMatch();
                bool matchAFound = allMatches.TryGetValue(objectA, out matchA);

                if (ShouldIgnore(objectA))
                {
                    return;
                }
                
                RemoveMatch(objectA);

                if (matchAFound && matchA.Type != ValueEqualsReportMatchType.NONE)
                {
                    object objectB = matchA.Match;
                    ValueEqualsReportMatch matchB = new ValueEqualsReportMatch();
                    bool matchBFound = allMatches.TryGetValue(objectB, out matchB);

                    if (objectB != objectA && matchBFound && matchB.Type != ValueEqualsReportMatchType.NONE)
                    {
                        ReplaceMatch(objectB, recursive:recursive);
                    }
                }

                if (newMatch.Type == ValueEqualsReportMatchType.NONE)
                {
                    AddNonMatch(objectA);
                }
                else
                {
                    AddMatch(objectA, newMatch);
                }

                if (newMatch.Type >= ValueEqualsReportMatchType.VALUE_EQUAL)
                {
                    ClearNotes(objectA);
                }
                
                if (recursive)
                {
                    if (objectA is GameObject)
                    {
                        foreach (GameObject child in (objectA as GameObject).Children())
                        {
                            ReplaceMatch(child, recursive:recursive);
                        }

                        foreach (Component component in (objectA as GameObject).GetComponents<Component>())
                        {
                            ReplaceMatch(component, recursive:recursive);
                        }
                    }
                }
            }

            //private void RemoveMatch(object objectToRemove)
            //{
            //    allMatches.Remove(objectToRemove);
            //    badMatches.Remove(objectToRemove);
            //    goodMatches.Remove(objectToRemove);
            //}

            private bool RemoveMatch(object objectToRemove)
            {
                bool found = allMatches.Remove(objectToRemove);
                badMatches.Remove(objectToRemove);
                goodMatches.Remove(objectToRemove);

                return found;
            }

            private void RemoveGoodMatch(object objectToRemove)
            {
                if (goodMatches.Remove(objectToRemove))
                {
                    allMatches.Remove(objectToRemove);
                }
            }

            private void RemoveBadMatch(object objectToRemove)
            {
                if (badMatches.Remove(objectToRemove))
                {
                    allMatches.Remove(objectToRemove);
                }
            }

            public object GetMatch(object objectToFindMatchOf)
            {
                return allMatches[objectToFindMatchOf].Match;
            }

            public ValueEqualsReportMatch GetMatchDetails(object objectToFindMatchOf)
            {
                ValueEqualsReportMatch details = new ValueEqualsReportMatch(null);
                allMatches.TryGetValue(objectToFindMatchOf, out details);

                return details;
            }

            public bool Matched(object objectToCheck)
            {
                ValueEqualsReportMatch match = new ValueEqualsReportMatch();

                if (!allMatches.TryGetValue(objectToCheck, out match))
                {
                    Debug.Log("Object " + objectToCheck + " not checked for match.");
                }

                return match.Type != ValueEqualsReportMatchType.NONE;
            }

            public void PushNotes(object objectKey)
            {
                if (activeNotes.Count > 0)
                {
                    if (objectKey != null)
                    {
                        List<string> objectNotes;
                        if (!notes.TryGetValue(objectKey, out objectNotes))
                        {
                            objectNotes = new List<string> { };
                        }
                        else
                        {
                            notes.Remove(objectKey);
                        }

                        objectNotes.AddRange(activeNotes);
                        notes.Add(objectKey, objectNotes);
                    }

                    activeNotes.Clear();
                }
            }

            public List<string> GetNotes(object objectKey)
            {
                List<string> objectNotes;
                notes.TryGetValue(objectKey, out objectNotes);
                return objectNotes;
            }

            public void ClearNotes(object objectKey) {
                if (notes.ContainsKey(notes))
                {
                    notes[notes].Clear();
                }
            }

            public bool IsEqual
            {
                get { return equal; }
            }

            public List<object> AllMatches
            {
                get { return allMatches.Keys.ToList<object>(); } 
            }

            public List<object> BadMatches
            {
                get { return badMatches; } 
            }

            public List<object> GoodMatches
            {
                get { return goodMatches; }
            }

            public int Length {
                get { return badMatches.Count + goodMatches.Count; }
            }

            public int AddLimit
            {
                get { return addLimit; }
                set { addLimit = value; }
            }

            public Type[] IgnoreComponents
            {
                get { return ignoreComponents; }
                set { ignoreComponents = value; }
            }
        }

        public delegate bool ValueEqualsFunc<T>(T itemA, T itemB, ValueEqualsReport valueEqualsReport=null);

        public static bool ValueEquals(this GameObject gameObjectA, GameObject gameObjectB, ValueEqualsReport valueEqualsReport=null)
        {
            return DeepEqualsGameObj(gameObjectA, gameObjectB, valueEqualsReport);
        }
        
        public static bool ValueEquals(this object objectA, object objectB, ValueEqualsReport valueEqualsReport=null)
        {
            return DeepEquals(objectA, objectB, valueEqualsReport);
        }

        private static bool DeepEqualsGameObj(GameObject gameObjectA, GameObject gameObjectB, ValueEqualsReport valueEqualsReport=null)
        {
            bool equal = gameObjectA == gameObjectB;
            bool childrenMatch = gameObjectA.Children().ValueScrambledEquals(gameObjectB.Children(), DeepEqualsGameObj, valueEqualsReport:valueEqualsReport);
            bool componentsMatch = gameObjectA.GetComponents<Component>().ValueScrambledEquals(gameObjectB.GetComponents<Component>(), DeepEqualsComponent, valueEqualsReport:valueEqualsReport);
            bool namesMatch = gameObjectA.name.Equals(gameObjectB.name);

            bool deepEquals = childrenMatch && componentsMatch;

            if (valueEqualsReport != null)
            {
                if (equal)
                {
                    valueEqualsReport.AddMatch(gameObjectA, gameObjectB, ValueEqualsReportMatchType.EQUAL);
                }
                else if (deepEquals)
                {
                    valueEqualsReport.AddMatch(gameObjectA, gameObjectB, ValueEqualsReportMatchType.VALUE_EQUAL);
                }
                else if (namesMatch)
                {
                    valueEqualsReport.AddMatch(gameObjectA, gameObjectB, ValueEqualsReportMatchType.NAMES_EQUAL);
                }
            }

            return equal || deepEquals || namesMatch;
        }

        private static bool DeepEqualsComponent(Component componentA, Component componentB, ValueEqualsReport valueEqualsReport=null)
        {
            bool equal = componentA == componentB;
            bool deepEquals = DeepEquals(componentA, componentB, valueEqualsReport);
            bool typesMatch = componentA.GetType() == componentB.GetType();
            bool gameObjectNamesMatch = componentA.name.Equals(componentB.name);

            bool namesMatch = typesMatch && gameObjectNamesMatch;
            bool anyEqual = equal || deepEquals || namesMatch;

            if (equal)
            {
                valueEqualsReport.AddMatch(componentA, componentB, ValueEqualsReportMatchType.EQUAL);
            }
            if (deepEquals)
            {
                //Do nothing, match assinged in DeepEquals function call.
            }
            else if (namesMatch)
            {
                valueEqualsReport.AddMatch(componentA, componentB, ValueEqualsReportMatchType.NAMES_EQUAL);
            }

            if (anyEqual && !deepEquals)
            {
                valueEqualsReport.PushNotes(componentA);
            }
            else
            {
                valueEqualsReport.PushNotes(null);
            }

            return anyEqual;
        }

        private static bool DeepEquals(object objectA, object objectB, ValueEqualsReport valueEqualsReport=null)
        {
            bool equal = true;

            if (objectA != null && objectB != null)
            {
                Type type = objectA.GetType();

                if (objectA.GetType() != objectB.GetType())
                {
                    equal = false;
                }
                else if (type != typeof(Transform))
                {
                    equal = false;
                    
                    if (objectA.Equals(objectB))
                    {
                        equal = true;

                        //valueEqualsReport.AddMatch(objectA, objectB, ValueEqualsReportMatchType.EQUAL);
                    }
                    else if (objectA == objectB) //Approximate equals for floats
                    {
                        equal = true;

                        //valueEqualsReport.AddMatch(objectA, objectB, ValueEqualsReportMatchType.EQUAL);
                    }
                    else if (type.IsSubclassOf(typeof(Component)))
                    {   
                        FieldInfo[] fields = objectA.GetType().GetFieldsComparable().ToArray();
                        PropertyInfo[] properties = objectA.GetType().GetPropertiesComparable().ToArray();

                        bool verbose = false;
                        string[] fieldNames = null;
                        string[] propertyNames = null;
                        if (verbose || masterVerbose || valueEqualsReport != null)
                        {
                            fieldNames = Array.ConvertAll(fields, fieldInfo => fieldInfo.Name);
                            propertyNames = Array.ConvertAll(properties, fieldInfo => fieldInfo.Name);
                        }

                        object[] fieldsValueA = Array.ConvertAll(fields, fieldInfo => fieldInfo.GetValueNoError(objectA, true));
                        object[] fieldsValueB = Array.ConvertAll(fields, fieldInfo => fieldInfo.GetValueNoError(objectB, true));
                        bool fieldsEqual = fieldsValueA.ValueOrderedEquals(fieldsValueB, DeepEquals, verbose, fieldNames, valueEqualsReport);
                        
                        object[] propertiesValueA = Array.ConvertAll(properties, propertyInfo => propertyInfo.GetValueNoError(objectA));
                        object[] propertiesValueB = Array.ConvertAll(properties, propertyInfo => propertyInfo.GetValueNoError(objectB));
                        bool propertiesEqual = propertiesValueA.ValueOrderedEquals(propertiesValueB, DeepEquals, verbose, propertyNames, valueEqualsReport);

                        equal = fieldsEqual && propertiesEqual;

                        if (equal)
                        {
                            valueEqualsReport.AddMatch(objectA, objectB, ValueEqualsReportMatchType.VALUE_EQUAL);
                        }
                    }
                }
            }
            
            return equal;
        }
        
        public static bool ValueOrderedEquals<T>(this T[] setA, T[] setB, ValueEqualsFunc<T> valueEquals, bool verbose=false, string[] names=null, ValueEqualsReport valueEqualsReport=null)
        {
            bool equals = true;
            int length = setA.Length;
            bool iterateThoughAll = verbose || masterVerbose || valueEqualsReport != null;

            if (setA.Length != setB.Length)
            {
                equals = false;

                if (!iterateThoughAll)
                {
                    return equals;
                }
            }
            
            for (int i = 0; i < length; i++)
            {
                T itemA = setA[i];
                T itemB = setB[i];

                if (!valueEquals(itemA, itemB, valueEqualsReport))
                {
                    equals = false;

                    if (verbose || masterVerbose)
                    {
                        string itemNameA = names[i];
                        string itemValueA = itemA != null ? "null" : itemA.ToString();
                        string itemNameB = names[i];
                        string itemValueB = itemB != null ? "null" : itemB.ToString();

                        Debug.Log(String.Concat("Non-Match: (", itemNameA, ") ", itemA.ToString(), " != ", itemB.ToString()));
                    }

                    if (names != null && names.Length > i)
                    {
                        string itemNameA = names[i];
                        string itemValueA = itemA == null ? "null" : itemA.ToString();
                        string itemNameB = names[i];
                        string itemValueB = itemB == null ? "null" : itemB.ToString();

                        string note = String.Concat(itemNameA, ": ", itemValueA, " != ", itemValueB);

                        valueEqualsReport.activeNotes.Add(note);
                    }

                    if (!iterateThoughAll)
                    {
                        break;
                    }
                }
                else
                {
                    //if ((verbose || masterVerbose) && itemA != null)
                    //{
                    //    string itemNameA = names[i];
                    //    string itemValueA = itemA != null ? "null" : itemA.ToString();

                    //    Debug.Log(String.Concat("Match: (", itemNameA, ") ", itemValueA));
                    //}
                }

                //if (valueEqualsReport != null)
                //{
                //    if (!equals)
                //    {
                //        valueEqualsReport.AddNonMatch(itemA);
                //        valueEqualsReport.AddNonMatch(itemB);
                //    }
                //}
            }

            return equals;
        }
        
        public static bool ValueScrambledEquals<T>(this T[] setA, T[] setB, ValueEqualsFunc<T> valueEquals, bool verbose=false, string[] names=null, ValueEqualsReport valueEqualsReport=null) 
        {
            bool equals = true;
            int lengthA = setA.Length;
            int lengthB = setB.Length;
            bool iterateThoughAll = verbose || masterVerbose || valueEqualsReport != null;

            if (lengthA != lengthB)
            {
                equals = false;

                if (!iterateThoughAll)
                {
                    return equals;
                }
            }
            
            bool[] indexMatchedA = new bool[lengthA];
            bool[] indexMatchedB = new bool[lengthB];

            for (int ia = 0; ia < lengthA; ia++)
            {
                if (indexMatchedA[ia]) { continue; }

                bool matchFound = false;

                T itemA = setA[ia];
                for (int ib = 0; ib < lengthB; ib++)
                {
                    if (indexMatchedB[ib]) { continue; }

                    T itemB = setB[ib];
                    if (valueEquals(itemA, itemB, valueEqualsReport))
                    {
                        indexMatchedA[ia] = true;
                        indexMatchedB[ib] = true;
                        matchFound = true;
                            
                        //if (valueEqualsReport != null)
                        //{
                        //    valueEqualsReport.AddMatch(itemA, itemB, ValueEqualsReportMatchType.VALUE_EQUAL);
                        //}
                            
                        break;
                    }
                }

                if (!matchFound)
                {
                    equals = false;
                        
                    if (!iterateThoughAll)
                    {
                        break;
                    }
                }
            }

            //if (valueEqualsReport != null)
            //{
            //    if (equals)
            //    {
            //        for (int i = 0; i < indexMatchedA.Length; i++)
            //        {
            //            if (!indexMatchedA[i])
            //            {
            //                valueEqualsReport.AddNonMatch(setA[i]);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < indexMatchedB.Length; i++)
            //        {
            //            if (!indexMatchedB[i])
            //            {
            //                valueEqualsReport.AddNonMatch(setB[i]);
            //            }
            //        }
            //    }
            //}

            return equals;
        }

        #endregion

        public static GameObject[] Children(this GameObject gameObject)
        {
            Transform transform  = gameObject.transform;
            int childCount = transform.childCount;
            GameObject[] children = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = transform.GetChild(i).gameObject;
            }

            return children;
        }

        public static bool includeNames = true;

        public static List<string> CantCopy(this Type type)
        {
            List<string> types = new List<string> { };

            if (type.IsSubclassOf(typeof(Component)))
            {
                //if (!includeNames)
                //{
                //types.Add(string.Concat(type, ".name"));
                //}
                types.Add(string.Concat(type, ".mesh"));
                types.Add(string.Concat(type, ".material"));
                types.Add(string.Concat(type, ".materials"));   
            }

            return types;
        }

        public static List<string> CantCompare(this Type type)
        {
            List<string> types = new List<string> { };
            
            if (type.IsSubclassOf(typeof(Component)))
            {
                ///if (!includeNames)
                //{
                types.Add(string.Concat(type, ".name"));
                //}
                types.Add(string.Concat(type, ".gameObject"));
                types.Add(string.Concat(type, ".hideFlags"));
                types.Add(string.Concat(type, ".mesh"));
                types.Add(string.Concat(type, ".material"));
                types.Add(string.Concat(type, ".materials"));
                types.Add(string.Concat(type, ".sharedMesh"));
                types.Add(string.Concat(type, ".sharedMaterial"));
                types.Add(string.Concat(type, ".sharedMaterials"));
                types.Add(string.Concat(type, ".isActiveAndEnabled"));

                if (type.IsSubclassOf(typeof(Renderer)))
                {
                    types.Add(string.Concat(type, ".isVisible"));
                    types.Add(string.Concat(type, ".bounds"));
                    types.Add(string.Concat(type, ".worldToLocalMatrix"));
                    types.Add(string.Concat(type, ".localToWorldMatrix"));
                }

                //Debug.Log("type.ToString(): " + type.ToString());
                //if (type.ToString().Equals("PrefabLink"))
                //{
                //    types.Add(string.Concat(type, ".target"));
                //    types.Add(string.Concat(type, ".Target"));
                //}

                //if (type.IsSubclassOf(typeof(Rigidbody)))
                //{
                    types.Add(string.Concat(type, ".worldCenterOfMass"));
                    types.Add(string.Concat(type, ".position"));
                    types.Add(string.Concat(type, ".centerOfMass"));
                    types.Add(string.Concat(type, ".inertiaTensor"));
                //}

                if (type.IsSubclassOf(typeof(Collider)))
                {
                    types.Add(string.Concat(type, ".bounds"));
                }
            }

            return types;
        }

        public static object GetValueNoError(this FieldInfo fieldInfo, object obj, bool verbose=false)
        {
            object value = default(object);

            try
            {
                value = fieldInfo.GetValue(obj);
            }
            catch (Exception exception)
            {
                if (verbose)
                {
                    Debug.Log("Couldnt access field " + obj.ToString() + "." + fieldInfo.Name + ". " + exception.Message);
                }
            }

            return value;
        }

        public static object GetValueNoError(this PropertyInfo propertyInfo, object obj, bool verbose=false)
        {
            object value = default(object);

            try
            {
                value = propertyInfo.GetValue(obj, null);
            }
            catch (Exception exception)
            {
                if (verbose)
                {
                    Debug.Log("Couldnt access field " + obj.ToString() + "." + propertyInfo.Name + ". " + exception.Message);
                }
            }

            return value;
        }

        public static List<PropertyInfo> GetPropertiesComparable(this Type type)
        {
            return GetPropertiesFiltered(type, type.CantCompare());
        }

        public static List<PropertyInfo> GetPropertiesCopyable(this Type type)
        {
            return GetPropertiesFiltered(type, type.CantCopy());
        }

        public static List<PropertyInfo> GetPropertiesFiltered(this Type type, List<string> typesToIgnore)
        {
            PropertyInfo[] propertiesUnfiltered = type.GetProperties();
            List<PropertyInfo> propertiesFiltered = new  List<PropertyInfo>{ };

            foreach (PropertyInfo property in propertiesUnfiltered)
            {
                string fullName = string.Concat(type.ToString(), ".", property.Name);

                if (!typesToIgnore.Contains(fullName))
                {
                    propertiesFiltered.Add(property);
                }
            }

            return propertiesFiltered;
        }
        

        public static List<FieldInfo> GetFieldsComparable(this Type type)
        {
            return GetFieldsFiltered(type, type.CantCompare());
        }

        public static List<FieldInfo> GetFieldsCopyable(this Type type)
        {
            return GetFieldsFiltered(type, type.CantCopy());
        }

        public static List<FieldInfo> GetFieldsFiltered(this Type type, List<string> typesToIgnore)
        {
            FieldInfo[] fieldsUnfiltered = type.GetFields();
            List<FieldInfo> fieldsFiltered = new  List<FieldInfo>{ };

            foreach (FieldInfo property in fieldsUnfiltered)
            {
                string fullName = string.Concat(type.ToString(), ".", property.Name);

                if (!typesToIgnore.Contains(fullName))
                {
                    fieldsFiltered.Add(property);
                }
            }

            return fieldsFiltered;
        }

        public static Component CopyComponent<T>(this GameObject destination, T original, bool verbose = false) where T : Component
        {
            System.Type type = original.GetType();

            Component copy;

            if (Application.isPlaying)
            {
                copy = destination.AddComponent(type);
            }
            else
            {
                #if UNITY_EDITOR
                    copy = Undo.AddComponent(destination, type);
                #endif
            }

            if (copy == null)
            {
                Debug.Log(@"TinyExtensions.CopyComponent failed to copy component of type " + type + @". Sometimes this happens when 
                    Unity automatically adds components. Instead we will try to copy over the Unity added component.");

                copy = destination.GetComponent(type);
            }

            return copy.CopyComponent(original);
        }
        
        public static Component CopyComponent<T>(this Component destination, T original, bool verbose = false) where T : Component
        {
            System.Type type = original.GetType();

            if (type == typeof(Transform))
            {
                Transform copyTransform = destination as Transform;
                Transform originalTransform = original as Transform;
                
                copyTransform.localScale = originalTransform.localScale;
                copyTransform.localRotation = originalTransform.localRotation;
                copyTransform.localPosition = originalTransform.localPosition;

                destination = copyTransform;
            }
            else
            {
                List<FieldInfo> fields = type.GetFieldsCopyable();
                foreach (FieldInfo field in fields)
                {
                    try
                    {
                        field.SetValue(destination, field.GetValue(original));
                    }
                    catch (Exception exception)
                    {
                        if (verbose)
                        {
                            Debug.Log("Couldnt set field " + type.ToString() + "." + field.Name + ". " + exception.Message);
                        }
                    }
                }

                List<PropertyInfo> properties = type.GetPropertiesCopyable();
                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        property.SetValue(destination, property.GetValue(original, null), null);
                    }
                    catch (Exception exception)
                    {
                        if (verbose)
                        {
                            Debug.Log("Couldnt access property " + type.ToString() + "." + property.Name + ". " + exception.Message);
                        }
                    }
                }
            }

            return destination;
        }
    }
}
