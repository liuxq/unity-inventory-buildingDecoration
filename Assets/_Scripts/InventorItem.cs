//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

[AddComponentMenu("lxq/InventorItem")]
public class InventorItem : UIDragDropItem
{
    /// <summary>
    /// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
    /// </summary>

    public GameObject prefab;

    /// <summary>
    /// Drop a 3D game object onto the surface.
    /// </summary>

    protected override void OnDragDropRelease(GameObject surface)
    {
        UIDragDropContainer container = surface ? NGUITools.FindInParents<UIDragDropContainer>(surface) : null;

        if (container == null)
        {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100);

            if (hit)
            {
                Vector3 buildPos = hitInfo.point;
                buildPos.y += 1;
                Instantiate(prefab, buildPos, Quaternion.identity);

                // Destroy this icon as it's no longer needed
                NGUITools.Destroy(gameObject);
                return;
            }
        }
        
        base.OnDragDropRelease(surface);
    }
}
