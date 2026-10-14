using System;
using System.Collections.Generic;
using UnityEngine;
using HonkAPI.Props;

namespace HonkAPI.PhysicsApi
{
    public static class PhysicsAPI
    {
        private static readonly Vector3 DefaultGravity = new Vector3(0f, -9.81f, 0f);

        public static void Explosion(Vector3 origin, float force, float radius, float upwardModifier = 1f)
        {
            Collider[] colliders = Physics.OverlapSphere(origin, radius);
            List<Rigidbody> affected = new List<Rigidbody>();

            foreach (Collider col in colliders)
            {
                if (col == null || col.attachedRigidbody == null) continue;
                Rigidbody rb = col.attachedRigidbody;
                if (!affected.Contains(rb) && !rb.isKinematic)
                {
                    affected.Add(rb);
                    rb.AddExplosionForce(force, origin, radius, upwardModifier, ForceMode.Impulse);
                }
            }
        }

        public static void Attract(Vector3 center, float force, float radius)
        {
            Collider[] colliders = Physics.OverlapSphere(center, radius);
            List<Rigidbody> affected = new List<Rigidbody>();

            foreach (Collider col in colliders)
            {
                if (col == null || col.attachedRigidbody == null) continue;
                Rigidbody rb = col.attachedRigidbody;
                if (!affected.Contains(rb) && !rb.isKinematic)
                {
                    affected.Add(rb);
                    Vector3 dir = (center - rb.transform.position).normalized;
                    float dist = Vector3.Distance(center, rb.transform.position);
                    float factor = 1f - Mathf.Clamp01(dist / radius);
                    rb.AddForce(dir * force * factor, ForceMode.Force);
                }
            }
        }

        public static void SetGravity(Vector3 gravity)
        {
            Physics.gravity = gravity;
        }

        public static void SetGravity(float x, float y, float z)
        {
            Physics.gravity = new Vector3(x, y, z);
        }

        public static void ResetGravity()
        {
            Physics.gravity = DefaultGravity;
        }

        public static void ZeroGravity()
        {
            Physics.gravity = Vector3.zero;
        }

        public static void LaunchAllProps(Vector3 force)
        {
            List<Prop> props = PropAPI.GetAll();
            foreach (Prop p in props)
            {
                if (p != null && p.rb != null && !p.rb.isKinematic)
                {
                    p.rb.AddForce(force, ForceMode.Impulse);
                }
            }
        }

        public static void FreezeAllProps(bool freeze)
        {
            List<Prop> props = PropAPI.GetAll();
            foreach (Prop p in props)
            {
                if (p != null && p.rb != null)
                {
                    p.rb.isKinematic = freeze;
                }
            }
        }

        public static List<Prop> GetPropsInRadius(Vector3 center, float radius)
        {
            List<Prop> result = new List<Prop>();
            foreach (Prop p in PropAPI.GetAll())
            {
                if (p != null && Vector3.Distance(p.transform.position, center) <= radius) result.Add(p);
            }
            return result;
        }

        public static List<Brain> GetNPCsInRadius(Vector3 center, float radius)
        {
            List<Brain> result = new List<Brain>();
            foreach (Brain b in HonkAPI.NPC.NPCAPI.GetAll())
            {
                if (b != null && Vector3.Distance(b.transform.position, center) <= radius) result.Add(b);
            }
            return result;
        }

        /// <summary>
        /// A radial blast: pushes props and every goose in range, and optionally frightens NPCs in range.
        /// </summary>
        public static void Shockwave(Vector3 origin, float force, float radius, bool affectGeese = true, bool scareNPCs = true)
        {
            Explosion(origin, force, radius, 0.5f);
            if (affectGeese)
            {
                foreach (Goose g in HonkAPI.GooseApi.GooseAPI.AllGeese)
                {
                    if (g == null || g.rb == null) continue;
                    float d = Vector3.Distance(g.transform.position, origin);
                    if (d > radius) continue;
                    Vector3 dir = (g.transform.position - origin).normalized + Vector3.up * 0.5f;
                    g.rb.AddForce(dir.normalized * force * (1f - d / radius), ForceMode.Impulse);
                }
            }
            if (scareNPCs)
            {
                foreach (Brain b in GetNPCsInRadius(origin, radius)) HonkAPI.NPC.NPCAPI.Scare(b);
            }
        }
    }
}
