using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.Props
{
    public static class PropRegistry
    {
        public static Prop[] GetAllProps()
        {
            if (GameCollections.Instance != null && GameCollections.Instance.allProps != null)
            {
                return GameCollections.Instance.allProps.ToArray();
            }
            return UnityEngine.Object.FindObjectsOfType<Prop>();
        }

        public static List<Prop> FindAllProps(string nameFilter = null)
        {
            List<Prop> result = new List<Prop>();
            Prop[] props = GetAllProps();
            foreach (Prop p in props)
            {
                if (p == null) continue;
                if (string.IsNullOrEmpty(nameFilter) || p.name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public static Prop FindProp(string name)
        {
            Prop[] props = GetAllProps();
            foreach (Prop p in props)
            {
                if (p != null && p.name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return p;
                }
            }
            return null;
        }

        public static Prop CloneProp(string templateName, Vector3 position)
        {
            Prop template = FindProp(templateName);
            if (template == null)
            {
                return null;
            }

            GameObject cloneObj = UnityEngine.Object.Instantiate(template.gameObject, position, template.transform.rotation);
            cloneObj.name = template.name + "_Clone";

            Prop cloneProp = cloneObj.GetComponent<Prop>();
            if (cloneProp != null)
            {
                RegisterProp(cloneProp);
            }

            return cloneProp;
        }

        public static void RegisterProp(Prop prop)
        {
            if (prop == null) return;

            if (GameCollections.Instance != null)
            {
                if (GameCollections.Instance.allProps != null && !GameCollections.Instance.allProps.Contains(prop))
                {
                    GameCollections.Instance.allProps.Add(prop);
                }

                BeakHandle[] handles = prop.GetComponentsInChildren<BeakHandle>();
                if (GameCollections.Instance.allBeakHandles != null && handles != null)
                {
                    foreach (BeakHandle h in handles)
                    {
                        if (!GameCollections.Instance.allBeakHandles.Contains(h))
                        {
                            GameCollections.Instance.allBeakHandles.Add(h);
                        }
                    }
                }
            }
        }

        public static void MakeWet(Prop prop)
        {
            if (prop == null) return;
            Wettable wettable = prop.GetComponent<Wettable>();
            if (wettable == null)
            {
                wettable = prop.gameObject.AddComponent<Wettable>();
            }
            wettable.kindsOfWet.wet = true;
            wettable.kindsOfWet.causeDrips = true;
            wettable.kindsOfWet.causeSplashes = true;
        }

        public static void LaunchProp(Prop prop, Vector3 force)
        {
            if (prop == null || prop.rb == null) return;
            prop.rb.AddForce(force, ForceMode.Impulse);
        }

        public static void SetBounciness(Prop prop, float bounciness)
        {
            if (prop == null) return;
            Collider col = prop.GetComponent<Collider>();
            if (col != null)
            {
                PhysicMaterial pm = new PhysicMaterial();
                pm.bounciness = Mathf.Clamp01(bounciness);
                pm.bounceCombine = PhysicMaterialCombine.Maximum;
                col.material = pm;
            }
        }
    }
}
