using System.Collections.Generic;
using UnityEngine;

public class HologramLevelMirror : MonoBehaviour
{
    [Header("Source Roots")]
    [SerializeField] private Transform levelSourceRoot;
    [SerializeField] private Transform ballSourceRoot;

    [Tooltip("建議指定實際遊戲場景的共同根節點，例如 ArenaRoot / LevelRuntimeRoot。若留空，會直接使用世界座標。")]
    [SerializeField] private Transform sourceReferenceRoot;

    [Header("Hologram Settings")]
    [SerializeField] private Material hologramMaterial;

    [Tooltip("整個 Hologram 的縮放倍率，例如 0.1 代表縮小成 1/10。")]
    [SerializeField] private float hologramScale = 0.1f;

    [SerializeField] private float cubeProxySize = 0.2f;
    [SerializeField] private float sphereProxySize = 0.18f;

    [Header("Sync Settings")]
    [SerializeField] private bool includeInactiveObjects = false;
    [SerializeField] private bool syncRotation = true;

    [Tooltip("若來源物件大小不同，是否讓 Hologram 代理物件反映來源物件的大概比例。")]
    [SerializeField] private bool syncApproximateScale = false;

    private Transform contentRoot;

    private readonly Dictionary<Transform, GameObject> levelProxies = new();
    private readonly Dictionary<Transform, GameObject> ballProxies = new();

    private readonly List<Transform> tempSourceList = new();
    private readonly List<Transform> staleList = new();

    private void Awake()
    {
        CreateContentRootIfNeeded();
    }

    private void LateUpdate()
    {
        SyncGroup(
            levelSourceRoot,
            levelProxies,
            PrimitiveType.Cube,
            cubeProxySize,
            "Hologram_Block_"
        );

        SyncGroup(
            ballSourceRoot,
            ballProxies,
            PrimitiveType.Sphere,
            sphereProxySize,
            "Hologram_Ball_"
        );
    }

    private void CreateContentRootIfNeeded()
    {
        if (contentRoot != null) return;

        GameObject content = new GameObject("Hologram Content");
        contentRoot = content.transform;
        contentRoot.SetParent(transform, false);
        contentRoot.localPosition = Vector3.zero;
        contentRoot.localRotation = Quaternion.identity;
        contentRoot.localScale = Vector3.one * hologramScale;
    }

    private void SyncGroup(
        Transform sourceRoot,
        Dictionary<Transform, GameObject> proxies,
        PrimitiveType primitiveType,
        float proxySize,
        string proxyNamePrefix)
    {
        if (sourceRoot == null) return;

        CreateContentRootIfNeeded();

        contentRoot.localScale = Vector3.one * hologramScale;

        CollectSources(sourceRoot);

        // 新增或更新代理物件
        for (int i = 0; i < tempSourceList.Count; i++)
        {
            Transform source = tempSourceList[i];

            if (source == sourceRoot) continue;

            if (!proxies.TryGetValue(source, out GameObject proxy) || proxy == null)
            {
                proxy = CreateProxy(source, primitiveType, proxyNamePrefix);
                proxies[source] = proxy;
            }

            UpdateProxyTransform(source, proxy.transform, proxySize);
        }

        // 移除已不存在或不應顯示的代理物件
        staleList.Clear();

        foreach (var pair in proxies)
        {
            Transform source = pair.Key;

            if (source == null)
            {
                staleList.Add(source);
                continue;
            }

            if (!includeInactiveObjects && !source.gameObject.activeInHierarchy)
            {
                staleList.Add(source);
                continue;
            }

            if (!source.IsChildOf(sourceRoot))
            {
                staleList.Add(source);
            }
        }

        for (int i = 0; i < staleList.Count; i++)
        {
            Transform staleSource = staleList[i];

            if (proxies.TryGetValue(staleSource, out GameObject proxy) && proxy != null)
            {
                Destroy(proxy);
            }

            proxies.Remove(staleSource);
        }
    }

    private void CollectSources(Transform root)
    {
        tempSourceList.Clear();

        Transform[] children = root.GetComponentsInChildren<Transform>(includeInactiveObjects);

        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];

            if (child == root) continue;

            if (!includeInactiveObjects && !child.gameObject.activeInHierarchy) continue;

            tempSourceList.Add(child);
        }
    }

    private GameObject CreateProxy(Transform source, PrimitiveType primitiveType, string namePrefix)
    {
        GameObject proxy = GameObject.CreatePrimitive(primitiveType);
        proxy.name = namePrefix + source.name;
        proxy.transform.SetParent(contentRoot, false);

        Collider collider = proxy.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer renderer = proxy.GetComponent<Renderer>();
        if (renderer != null && hologramMaterial != null)
        {
            renderer.sharedMaterial = hologramMaterial;
        }

        return proxy;
    }

    private void UpdateProxyTransform(Transform source, Transform proxy, float baseSize)
    {
        proxy.localPosition = GetMappedLocalPosition(source.position);

        if (syncRotation)
        {
            proxy.localRotation = GetMappedLocalRotation(source.rotation);
        }
        else
        {
            proxy.localRotation = Quaternion.identity;
        }

        if (syncApproximateScale)
        {
            Vector3 sourceScale = GetApproximateLocalScale(source);
            proxy.localScale = sourceScale * baseSize;
        }
        else
        {
            proxy.localScale = Vector3.one * baseSize;
        }
    }

    private Vector3 GetMappedLocalPosition(Vector3 sourceWorldPosition)
    {
        if (sourceReferenceRoot != null)
        {
            return sourceReferenceRoot.InverseTransformPoint(sourceWorldPosition);
        }

        return sourceWorldPosition;
    }

    private Quaternion GetMappedLocalRotation(Quaternion sourceWorldRotation)
    {
        if (sourceReferenceRoot != null)
        {
            return Quaternion.Inverse(sourceReferenceRoot.rotation) * sourceWorldRotation;
        }

        return sourceWorldRotation;
    }

    private Vector3 GetApproximateLocalScale(Transform source)
    {
        if (sourceReferenceRoot == null)
        {
            return source.lossyScale;
        }

        Vector3 refScale = sourceReferenceRoot.lossyScale;
        Vector3 sourceScale = source.lossyScale;

        return new Vector3(
            SafeDivide(sourceScale.x, refScale.x),
            SafeDivide(sourceScale.y, refScale.y),
            SafeDivide(sourceScale.z, refScale.z)
        );
    }

    private float SafeDivide(float value, float divisor)
    {
        if (Mathf.Approximately(divisor, 0f)) return value;
        return value / divisor;
    }

    public void ClearAllProxies()
    {
        ClearProxyGroup(levelProxies);
        ClearProxyGroup(ballProxies);
    }

    private void ClearProxyGroup(Dictionary<Transform, GameObject> proxies)
    {
        foreach (var pair in proxies)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value);
            }
        }

        proxies.Clear();
    }
}