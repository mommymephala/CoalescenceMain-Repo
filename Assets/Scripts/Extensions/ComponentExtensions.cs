using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Extensions
{
    public static class ComponentExtensions
    {
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            var pinfos = type.GetProperties(flags);
            foreach (PropertyInfo pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {
                        // ignored
                    } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        // --------------------------------------------------------------------

        public static void GetComponentsInChildrenStopAt<T, TStopT>(this Component comp, List<T> components)
        {
            var searchedComponents = comp.GetComponents<T>();
            components.AddRange(searchedComponents);

            for (var i = 0; i < comp.transform.childCount; ++i)
            {
                Transform child = comp.transform.GetChild(i);
                if (child.GetComponent<TStopT>() == null)
                {
                    child.GetComponentsInChildrenStopAt<T, TStopT>(components);
                }
            }
        }
    }
}