using System;
using UnityEngine;

namespace HonkAPI.Props
{
    public class HonkPropCallbacks : MonoBehaviour
    {
        public Action<Goose> onGrabbed;
        public Action<Goose> onDropped;
    }

    public class PropBuilder
    {
        private string propName = "CustomProp";
        private PrimitiveType primitiveType = PrimitiveType.Cube;
        private Color propColor = Color.white;
        private Vector3 spawnPosition = Vector3.zero;
        private Quaternion spawnRotation = Quaternion.identity;
        private Vector3 propScale = Vector3.one;
        private float propMass = 1.0f;
        private float bounciness = 0.0f;
        private Vector3 handleOffset = Vector3.zero;
        private bool needsToBeDragged = false;
        private bool isWettable = false;
        private Mesh customMesh = null;
        private Material customMaterial = null;
        private Action<Goose> onGrabbedAction = null;
        private Action<Goose> onDroppedAction = null;

        public static PropBuilder Create(string name)
        {
            PropBuilder b = new PropBuilder();
            b.propName = name;
            return b;
        }

        public PropBuilder FromPrimitive(PrimitiveType type)
        {
            primitiveType = type;
            return this;
        }

        public PropBuilder WithMesh(Mesh mesh, Material material = null)
        {
            customMesh = mesh;
            customMaterial = material;
            return this;
        }

        public PropBuilder WithColor(Color color)
        {
            propColor = color;
            return this;
        }

        public PropBuilder WithPosition(Vector3 position)
        {
            spawnPosition = position;
            return this;
        }

        public PropBuilder WithRotation(Quaternion rotation)
        {
            spawnRotation = rotation;
            return this;
        }

        public PropBuilder WithScale(Vector3 scale)
        {
            propScale = scale;
            return this;
        }

        public PropBuilder WithMass(float mass)
        {
            propMass = mass;
            return this;
        }

        public PropBuilder WithBounciness(float bounce)
        {
            bounciness = Mathf.Clamp01(bounce);
            return this;
        }

        public PropBuilder WithHandleOffset(Vector3 offset)
        {
            handleOffset = offset;
            return this;
        }

        public PropBuilder WithNeedsToBeDragged(bool dragged)
        {
            needsToBeDragged = dragged;
            return this;
        }

        public PropBuilder MakeWettable(bool wet = true)
        {
            isWettable = wet;
            return this;
        }

        public PropBuilder OnGrabbed(Action<Goose> action)
        {
            onGrabbedAction = action;
            return this;
        }

        public PropBuilder OnDropped(Action<Goose> action)
        {
            onDroppedAction = action;
            return this;
        }

        public Prop Register()
        {
            GameObject go;
            if (customMesh != null)
            {
                go = new GameObject(propName);
                MeshFilter mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = customMesh;
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = customMaterial != null ? customMaterial : new Material(Shader.Find("Standard"));
                MeshCollider mc = go.AddComponent<MeshCollider>();
                mc.convex = true;
            }
            else
            {
                go = GameObject.CreatePrimitive(primitiveType);
                go.name = propName;
                Renderer rend = go.GetComponent<Renderer>();
                if (rend != null)
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.color = propColor;
                    rend.material = mat;
                }
            }

            go.transform.position = spawnPosition;
            go.transform.rotation = spawnRotation;
            go.transform.localScale = propScale;

            if (bounciness > 0f)
            {
                Collider col = go.GetComponent<Collider>();
                if (col != null)
                {
                    PhysicMaterial pm = new PhysicMaterial();
                    pm.bounciness = bounciness;
                    pm.bounceCombine = PhysicMaterialCombine.Maximum;
                    col.material = pm;
                }
            }

            if (isWettable)
            {
                go.AddComponent<Wettable>();
            }

            if (onGrabbedAction != null || onDroppedAction != null)
            {
                HonkPropCallbacks cb = go.AddComponent<HonkPropCallbacks>();
                cb.onGrabbed = onGrabbedAction;
                cb.onDropped = onDroppedAction;
            }

            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = go.AddComponent<Rigidbody>();
            }
            rb.mass = propMass;

            int propLayer = LayerMask.NameToLayer("Prop");
            if (propLayer != -1)
            {
                go.layer = propLayer;
            }

            Ideal ideal = go.AddComponent<Ideal>();
            ideal.model = go;

            Prop prop = go.AddComponent<Prop>();
            prop.rb = rb;
            prop.needsToBeDragged = needsToBeDragged;
            prop.propXray = new PropXray();
            prop.propXray.dontShowXray = true;
            prop.holdSiblings = new Rigidbody[0];
            prop.annoyingSwitches = new SwitchReference[0];
            prop.extraOwners = new System.Collections.Generic.List<Folk>();
            prop.nonCleaningOwners = new System.Collections.Generic.List<Folk>();
            prop.secondaryPropGroups = new System.Collections.Generic.List<PropGroup>();

            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(go.transform, false);
            handleObj.transform.localPosition = handleOffset;

            BeakHandle handle = handleObj.AddComponent<BeakHandle>();
            handle.prop = prop;
            handle.rb = rb;
            handle.tugOfWarJob = new JobReference();

            prop.beakHandles = new BeakHandle[] { handle };

            if (UnityEngine.SceneManagement.SceneManager.sceneCount > 1)
            {
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, UnityEngine.SceneManagement.SceneManager.GetSceneAt(1));
            }

            if (GameCollections.Instance != null)
            {
                if (GameCollections.Instance.allProps != null && !GameCollections.Instance.allProps.Contains(prop))
                {
                    GameCollections.Instance.allProps.Add(prop);
                }
                if (GameCollections.Instance.allBeakHandles != null && !GameCollections.Instance.allBeakHandles.Contains(handle))
                {
                    GameCollections.Instance.allBeakHandles.Add(handle);
                }
            }

            return prop;
        }
    }
}
