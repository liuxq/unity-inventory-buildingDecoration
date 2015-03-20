using UnityEngine;
using System.Collections;

public class DragRigidbodyShadow : MonoBehaviour {

    public float spring = 50.0f;
    public float damper = 5.0f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;
    public float distance = 0.2f;
    public float pushForce = 0.2f;
    public bool  attachToCenterOfMass = false;

    public Material highlightMaterial;
    private GameObject highlightObject;

    private SpringJoint springJoint;

    private bool isJoystickHold = false;

    void Start()
    {
        Joystick.On_JoystickMoveStart += Joystick_On_JoystickMoveStart;
        Joystick.On_JoystickMoveEnd += Joystick_On_JoystickMoveEnd;
    }
    private void Joystick_On_JoystickMoveStart(Joystick j)
    {
        isJoystickHold = true;
    }
    private void Joystick_On_JoystickMoveEnd(Joystick j)
    {
        isJoystickHold = false;
    }
    void Update()
    {
        Camera mainCamera = FindCamera();

        highlightObject = null;
        if (springJoint != null && springJoint.connectedBody != null)
        {
            highlightObject = springJoint.connectedBody.gameObject;
        }
        else
        {
            // We need to actually hit an object
            RaycastHit hitt;
            if (!isJoystickHold && Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitt, 100))
            {
                if (hitt.rigidbody && !hitt.rigidbody.isKinematic)
                {
                    highlightObject = hitt.rigidbody.gameObject;
                }
            }
        }


        // Make sure the user pressed the mouse down
        if (!Input.GetMouseButtonDown(0))
            return;


        // We need to actually hit an object
        RaycastHit hit;
        if (isJoystickHold || !Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            return;
        }
        // We need to hit a rigidbody that is not kinematic
        if (!hit.rigidbody || hit.rigidbody.isKinematic)
        {
            return;
        }

        if (!springJoint)
        {
            GameObject go = new GameObject("Rigidbody dragger");
            Rigidbody body = go.AddComponent<Rigidbody>();
            springJoint = go.AddComponent<SpringJoint>();
            body.isKinematic = true;
        }

        springJoint.transform.position = hit.point;
        if (attachToCenterOfMass)
        {
            Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
            anchor = springJoint.transform.InverseTransformPoint(anchor);
            springJoint.anchor = anchor;
        }
        else
        {
            springJoint.anchor = Vector3.zero;
        }

        springJoint.spring = spring;
        springJoint.damper = damper;
        springJoint.maxDistance = distance;
        springJoint.connectedBody = hit.rigidbody;

        StartCoroutine(DragObject(hit.distance, hit.point, mainCamera.ScreenPointToRay(Input.mousePosition).direction));
    }

    IEnumerator DragObject(float distance, Vector3 hitpoint, Vector3 dir)
    {
        float startTime = Time.time;
        Vector3 mousePos = Input.mousePosition;


        float oldDrag = springJoint.connectedBody.drag;
        float oldAngularDrag = springJoint.connectedBody.angularDrag;
        springJoint.connectedBody.drag = drag;
        springJoint.connectedBody.angularDrag = angularDrag;
        Camera mainCamera = FindCamera();
        while (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            springJoint.transform.position = ray.GetPoint(distance);
            yield return null;
        }

        if (Mathf.Abs(mousePos.x - Input.mousePosition.x) <= 2 && Mathf.Abs(mousePos.y - Input.mousePosition.y) <= 2 && Time.time - startTime < .2 && springJoint.connectedBody)
        {
            dir.y = 0;
            dir.Normalize();
            springJoint.connectedBody.AddForceAtPosition(dir * pushForce, hitpoint, ForceMode.VelocityChange);
            ToggleLight(springJoint.connectedBody.gameObject);
        }

        //是否要删除模型
        UIToggle tog = GameObject.Find("UI Root (2D)/Chk_delete").GetComponent<UIToggle>();

        if (tog && tog.value && springJoint.connectedBody.gameObject.layer != LayerMask.NameToLayer("DisableDelete"))
        {
           
            UIGrid grid = GameObject.Find("UI Root (2D)/Container - Left/Left Scroll View/Grid").GetComponent<UIGrid>();

            GameObject prototype = Resources.Load("inventorItem") as GameObject;
            GameObject go = Instantiate(prototype) as GameObject;
            Transform trans = go.transform;
            //添加模型prefab
            InventorItem ii = go.GetComponent<InventorItem>();
            string prefabName = springJoint.connectedBody.gameObject.name;
            int strSize = prefabName.IndexOf("(Clone)");
            if (strSize>0)
            {
                prefabName = prefabName.Substring(0, strSize);
            }
            ii.prefab = Resources.Load(prefabName) as GameObject;

            UISprite sp = trans.FindChild("Icon").GetComponent<UISprite>();
            sp.spriteName = prefabName;
           
            trans.parent = grid.transform;
            Vector3 pos = trans.localPosition;
            pos.z = 0f;
            trans.localPosition = pos;
            trans.localScale = new Vector3(100.0f, 100.0f, 100.0f);
            trans.gameObject.name = springJoint.connectedBody.gameObject.name;
            NGUITools.MarkParentAsChanged(trans.gameObject);
            grid.repositionNow = true;

            Destroy(springJoint.connectedBody.gameObject);
        }

        if (springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }
    }

    void ToggleLight(GameObject go)
    {
        Light theLight = go.GetComponentInChildren<Light>();
        if (!theLight)
            return;

        theLight.enabled = !theLight.enabled;
        bool illumOn = theLight.enabled;
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            if (r.gameObject.layer == 1)
            {
                r.material.shader = Shader.Find(illumOn ? "Self-Illumin/Diffuse" : "Diffuse");
            }
        }
    }

    Camera FindCamera()
    {
        if (camera)
            return camera;
        else
            return Camera.main;
    }

    void OnPostRender()
    {
        if (highlightObject == null)
            return;

        GameObject go = highlightObject;
        highlightMaterial.SetPass(0);
        MeshFilter[] meshes = go.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter m in meshes)
        {
            Graphics.DrawMeshNow(m.sharedMesh, m.transform.position, m.transform.rotation);
        }
    }
}
