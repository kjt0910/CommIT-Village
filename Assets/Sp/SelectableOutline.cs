// SelectableOutline.cs
using UnityEngine;

public class SelectableOutline : MonoBehaviour
{
    public Material outlineMaterial;
    public float outlineWidth = 0.02f;
    public Color outlineColor = Color.cyan;
    public bool includeChildren = true;

    private Renderer[] outlineRenderers;

    void Awake()
    {
        BuildOutlines();
        SetOutlined(false);
    }

    public void SetOutlined(bool on)
    {
        if (outlineRenderers == null) return;
        foreach (var r in outlineRenderers) if (r) r.enabled = on;
    }

    public void SetOutlineParams(float width, Color color)
    {
        if (outlineRenderers == null) return;
        foreach (var r in outlineRenderers)
            foreach (var m in r.sharedMaterials)
            { if (m) { m.SetFloat("_OutlineWidth", width); m.SetColor("_OutlineColor", color); } }
    }

    void BuildOutlines()
    {
        var rs = includeChildren ? GetComponentsInChildren<Renderer>(true) : GetComponents<Renderer>();
        var list = new System.Collections.Generic.List<Renderer>();

        foreach (var r in rs)
        {
            if (!r || r is ParticleSystemRenderer) continue;

            var holder = new GameObject("__Outline");
            holder.transform.SetParent(r.transform, false);
            holder.layer = r.gameObject.layer;

            Renderer oR = null;
            Mesh mesh = null;

            if (r is SkinnedMeshRenderer sk)
            {
                var o = holder.AddComponent<SkinnedMeshRenderer>();
                o.sharedMesh = sk.sharedMesh;
                o.bones = sk.bones; o.rootBone = sk.rootBone;
                o.updateWhenOffscreen = sk.updateWhenOffscreen;
                oR = o; mesh = sk.sharedMesh;
            }
            else if (r is MeshRenderer)
            {
                var mf = r.GetComponent<MeshFilter>(); if (!mf || !mf.sharedMesh) { Destroy(holder); continue; }
                holder.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
                oR = holder.AddComponent<MeshRenderer>(); mesh = mf.sharedMesh;
            }
            else { Destroy(holder); continue; }

            int sub = mesh ? Mathf.Max(1, mesh.subMeshCount) : 1;
            var mats = new Material[sub];
            for (int i = 0; i < sub; i++)
            {
                mats[i] = new Material(outlineMaterial);
                mats[i].SetFloat("_OutlineWidth", outlineWidth);
                mats[i].SetColor("_OutlineColor", outlineColor);
            }
            oR.sharedMaterials = mats;

            oR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            oR.receiveShadows = false;
            oR.enabled = false;

            list.Add(oR);
        }
        outlineRenderers = list.ToArray();
    }
}
