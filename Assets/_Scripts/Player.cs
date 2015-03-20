using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player")]
public class Player : MonoBehaviour {

    // ���
    public Transform m_transform;
    CharacterController m_ch;


    // ��ɫ�ƶ��ٶ�
    float m_movSpeed = 3.0f;

    // ����
    float m_gravity = 2.0f;


    // �����
    Transform m_camTransform;

    // �������ת�Ƕ�
    Vector3 m_camRot;

    // ������߶�
    float m_camHeight = 1.4f;

    // �����Ч
    public AudioClip m_audio;

	// Use this for initialization
	void Start () {

        // ��ȡ���
        m_transform = this.transform;
        m_ch = this.GetComponent<CharacterController>();

        // ��ȡ�����
        m_camTransform = Camera.main.transform;

        // �����������ʼλ��
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
       
        ////��ȡ����ƶ�����
        //float rh = Input.GetAxis("Mouse X");
        //float rv = Input.GetAxis("Mouse Y");

        //// ��ת�����
        //m_camRot.x -= rv*3;
        //m_camRot.y += rh*3;
        //m_camTransform.eulerAngles = m_camRot;

        //// ʹ���ǵ��������������һ��
        //Vector3 camrot = m_camTransform.eulerAngles;
        //camrot.x = 0; camrot.z = 0;
        //m_transform.eulerAngles = camrot;

        
        float xm = 0, ym = 0, zm = 0;

        // �����˶�
        ym -= m_gravity*Time.deltaTime;

        // ���������˶�
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

        //�ƶ�
        m_ch.Move( m_transform.TransformDirection(new Vector3(xm, ym, zm)) );

        // ʹ�������λ��������һ��
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
            //�ƶ�
            m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));

            // ʹ�������λ��������һ��
            Vector3 pos = m_transform.position;
            pos.y += m_camHeight;
            m_camTransform.position = pos;
        }
        if (joystick.JoystickName == "NguiJoystickRotate")
        {
            //��ȡ����ƶ�����
            float rh = joystick.JoystickAxis.x * 25 * Time.deltaTime;
            float rv = joystick.JoystickAxis.y * 25 * Time.deltaTime;

            // ��ת�����
            m_camRot.x -= rv * 3;
            m_camRot.y += rh * 3;
            m_camTransform.eulerAngles = m_camRot;

            // ʹ���ǵ��������������һ��
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
