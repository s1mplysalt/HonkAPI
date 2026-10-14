using System;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.Props
{
    public static class PropAPI
    {
        public static PropBuilder Create(string name)
        {
            return PropBuilder.Create(name);
        }

        public static List<Prop> GetAll()
        {
            return new List<Prop>(PropRegistry.GetAllProps());
        }

        public static Prop GetClosest(Vector3 position)
        {
            List<Prop> all = GetAll();
            Prop closest = null;
            float minDist = float.MaxValue;
            foreach (Prop p in all)
            {
                if (p == null) continue;
                float d = Vector3.Distance(p.transform.position, position);
                if (d < minDist)
                {
                    minDist = d;
                    closest = p;
                }
            }
            return closest;
        }

        public static Prop FindByName(string name)
        {
            return PropRegistry.FindProp(name);
        }

        public static void Launch(Prop prop, Vector3 force)
        {
            if (prop != null && prop.rb != null && !prop.rb.isKinematic)
            {
                prop.rb.AddForce(force, ForceMode.Impulse);
            }
        }

        public static void Teleport(Prop prop, Vector3 position)
        {
            if (prop != null)
            {
                prop.transform.position = position;
            }
        }

        public static void Destroy(Prop prop)
        {
            if (prop != null)
            {
                UnityEngine.Object.Destroy(prop.gameObject);
            }
        }

        public static List<Prop> FindAll(string nameFilter)
        {
            return PropRegistry.FindAllProps(nameFilter);
        }

        public static void MakeWet(Prop prop)
        {
            PropRegistry.MakeWet(prop);
        }

        public static void SetMass(Prop prop, float mass)
        {
            if (prop != null && prop.rb != null) prop.rb.mass = Mathf.Max(0.001f, mass);
        }

        public static void Freeze(Prop prop, bool frozen)
        {
            if (prop == null || prop.rb == null) return;
            if (frozen)
            {
                prop.rb.velocity = Vector3.zero;
                prop.rb.angularVelocity = Vector3.zero;
            }
            prop.rb.isKinematic = frozen;
        }

        public static void SetScale(Prop prop, float scale)
        {
            if (prop != null) prop.transform.localScale = Vector3.one * Mathf.Max(0.01f, scale);
        }

        /// <summary>Colors every renderer on the prop with a tint.</summary>
        public static void Tint(Prop prop, Color color)
        {
            if (prop == null) return;
            foreach (Renderer r in prop.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null) continue;
                foreach (Material m in r.materials)
                {
                    if (m != null && m.HasProperty("_Color")) m.color = color;
                }
            }
        }

        public static void SetVisible(Prop prop, bool visible)
        {
            if (prop == null) return;
            foreach (Renderer r in prop.GetComponentsInChildren<Renderer>(true))
            {
                if (r != null) r.enabled = visible;
            }
        }

        /// <summary>Returns the prop to the place the game considers its starting position.</summary>
        public static void Respawn(Prop prop)
        {
            if (prop != null) prop.ResetPosition();
        }

        public static bool IsHeld(Prop prop)
        {
            return prop != null && prop.heldBy != null;
        }

        public static List<Prop> GetHeldProps()
        {
            List<Prop> result = new List<Prop>();
            foreach (Prop p in GetAll())
            {
                if (p != null && p.heldBy != null) result.Add(p);
            }
            return result;
        }

        public static void SetBounciness(Prop prop, float bounciness)
        {
            PropRegistry.SetBounciness(prop, bounciness);
        }
    }
}
