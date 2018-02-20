﻿using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
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

            if (gameObject.gameObject.scene.name == null && gameObject.scene.name == null)
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

        /// TODO DELETE
        public static T GetComponentInChildrenAndSelf<T>(this GameObject gameObject)
        {
            return gameObject.GetComponentInChildren<T>();
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
                copy = Undo.AddComponent(destination, type);
            }

            if (copy == null)
            {
                Debug.Log(@"TinyExtensions.CopyComponent failed to copy component of type " + type + @". Sometimes this happens when 
                    Unity automatically adds components. Instead we will try to copy over the Unity added component.");

                copy = destination.GetComponent(type);
            }
            
            List<string> propertiesToIgnore = new List<string> { };

            //propertiesToIgnore.Add("UnityEngine.Transform.parent");

            if (type == typeof(Renderer))
            {
                Renderer rendererCopy = copy as Renderer;
                Renderer rendererOriginal = original as Renderer;
                rendererCopy.sharedMaterial = rendererOriginal.sharedMaterial;
                rendererCopy.sharedMaterials = rendererOriginal.sharedMaterials;

                propertiesToIgnore.Add("UnityEngine.Renderer.material");
                propertiesToIgnore.Add("UnityEngine.Renderer.materials");
            }

            if (type == typeof(MeshFilter))
            {
                MeshFilter meshFilterCopy = copy as MeshFilter;
                MeshFilter meshFilterOriginal = original as MeshFilter;
                meshFilterCopy.sharedMesh = meshFilterOriginal.sharedMesh;
                
                propertiesToIgnore.Add("UnityEngine.MeshFilter.mesh");
            }

            if (type == typeof(MeshRenderer))
            {
                MeshRenderer meshRendererCopy = copy as MeshRenderer;
                MeshRenderer meshRendererOriginal = original as MeshRenderer;
                meshRendererCopy.sharedMaterial = meshRendererOriginal.sharedMaterial;
                meshRendererCopy.sharedMaterials = meshRendererOriginal.sharedMaterials;
                
                propertiesToIgnore.Add("UnityEngine.MeshRenderer.material");
                propertiesToIgnore.Add("UnityEngine.MeshRenderer.materials");
            }

            if (type == typeof(SpriteRenderer))
            {
                SpriteRenderer spriteRendererCopy = copy as SpriteRenderer;
                SpriteRenderer spriteRendererOriginal = original as SpriteRenderer;
                spriteRendererCopy.sharedMaterial = spriteRendererOriginal.sharedMaterial;
                spriteRendererCopy.sharedMaterials = spriteRendererOriginal.sharedMaterials;
                
                propertiesToIgnore.Add("UnityEngine.SpriteRenderer.material");
                propertiesToIgnore.Add("UnityEngine.SpriteRenderer.materials");
            }

            if (type == typeof(ParticleSystemRenderer))
            {
                ParticleSystemRenderer particleRendererCopy = copy as ParticleSystemRenderer;
                ParticleSystemRenderer particleRendererOriginal = original as ParticleSystemRenderer;
                particleRendererCopy.sharedMaterial = particleRendererOriginal.sharedMaterial;
                particleRendererCopy.sharedMaterials = particleRendererOriginal.sharedMaterials;
                
                propertiesToIgnore.Add("UnityEngine.ParticleSystemRenderer.material");
                propertiesToIgnore.Add("UnityEngine.ParticleSystemRenderer.materials");
            }

            if (type == typeof(Transform))
            {
                Transform copyTransform = copy as Transform;
                Transform originalTransform = original as Transform;
                
                copyTransform.localScale = originalTransform.localScale;
                copyTransform.localRotation = originalTransform.localRotation;
                copyTransform.localPosition = originalTransform.localPosition;

                copy = copyTransform;
            }
            else
            {
                System.Reflection.FieldInfo[] fields = type.GetFields();
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    try
                    {
                        field.SetValue(copy, field.GetValue(original));
                    }
                    catch (Exception exception)
                    {
                        if (verbose)
                        {
                            Debug.Log("Couldnt set field " + type.ToString() + "." + field.Name + ". " + exception.Message);
                        }
                    }
                }

                System.Reflection.PropertyInfo[] properties = type.GetProperties();
                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    string propertyName = string.Concat(type.ToString(), ".", property.Name);
                    bool ignoreProperty = false;

                    foreach (string propertyNameToIgnore in propertiesToIgnore)
                    {
                        if (string.Equals(propertyName, propertyNameToIgnore))
                        {
                            ignoreProperty = true;
                            break;
                        }
                    }

                    if (!ignoreProperty)
                    {
                        try
                        {
                            object value = property.GetValue(original, null);

                            if (value != null)
                            {
                                property.SetValue(copy, value, null);
                            }
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
            }

            return copy;
        }
    }
}