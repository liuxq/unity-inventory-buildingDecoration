using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player")]
public class Player : MonoBehaviour {

    // 组件
    public Transform m_transform;
    CharacterController m_ch;


    // 角色移动速度
    float m_movSpeed = 3.0f;

    // 重力
    float m_gravity = 2.0f;


    // 摄像机
    Transform m_camTransform;

    // 摄像机旋转角度
    Vector3 m_camRot;

    // 摄像机高度
    float m_camHeight = 1.4f;

    // 射击音效
    public AudioClip m_audio;

	// Use this for initialization
	void Start () {

        // 获取组件
        m_transform = this.transform;
        m_ch = this.GetComponent<CharacterController>();

        // 获取摄像机
        m_camTransform = Camera.main.transform;

        // 设置摄像机初始位置
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;
        m_camTransform.rotation = m_transform.rotation;

        m_camRot = m_camTransform.eulerAngles;

        //Screen.lockCursor = true;

        Joystick.On_JoystickHolding += Joystick_On_JoystickHolding;
	
	}
	
	// Update is called once per frame
	void Update () {

        Control();
	}

    void Control()
    {
       
        ////获取鼠标移动距离
        //float rh = Input.GetAxis("Mouse X");
        //float rv = Input.GetAxis("Mouse Y");

        //// 旋转摄像机
        //m_camRot.x -= rv*3;
        //m_camRot.y += rh*3;
        //m_camTransform.eulerAngles = m_camRot;

        //// 使主角的面向方向与摄像机一致
        //Vector3 camrot = m_camTransform.eulerAngles;
        //camrot.x = 0; camrot.z = 0;
        //m_transform.eulerAngles = camrot;

        
        float xm = 0, ym = 0, zm = 0;

        // 重力运动
        ym -= m_gravity*Time.deltaTime;

        // 上下左右运动
        if (Input.GetKey(KeyCode.W)){
            zm += m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S)){
            zm -= m_movSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A)){
            xm -= m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D)){
            xm += m_movSpeed * Time.deltaTime;
        }

        //移动
        m_ch.Move( m_transform.TransformDirection(new Vector3(xm, ym, zm)) );

        // 使摄像机的位置与主角一致
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;


        
    }

    private void Joystick_On_JoystickHolding(Joystick joystick)
    {
        if (joystick.JoystickName == "NguiJoystick")
        {
            //transform.rotation = Quaternion.LookRotation(new Vector3(joystick.JoystickAxis.x, 0f, joystick.JoystickAxis.y));
            float xm = 0, ym = 0, zm = 0;
            xm += joystick.JoystickAxis.x * m_movSpeed * Time.deltaTime;
            zm += joystick.JoystickAxis.y * m_movSpeed * Time.deltaTime;
            //移动
            m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));

            // 使摄像机的位置与主角一致
            Vector3 pos = m_transform.position;
            pos.y += m_camHeight;
            m_camTransform.position = pos;
        }
        if (joystick.JoystickName == "NguiJoystickRotate")
        {
            //获取鼠标移动距离
            float rh = joystick.JoystickAxis.x * 25 * Time.deltaTime;
            float rv = joystick.JoystickAxis.y * 25 * Time.deltaTime;

            // 旋转摄像机
            m_camRot.x -= rv * 3;
            m_camRot.y += rh * 3;
            m_camTransform.eulerAngles = m_camRot;

            // 使主角的面向方向与摄像机一致
            Vector3 camrot = m_camTransform.eulerAngles;
            camrot.x = 0; camrot.z = 0;
            m_transform.eulerAngles = camrot;

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
    }

}
