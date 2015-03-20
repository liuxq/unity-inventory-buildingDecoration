/*
 * 描术：
 * 
 * 作者：AnYuanLzh
 * 公司：lexun
 * 时间：2014-xx-xx
 */
using UnityEngine;
using System.Collections;

public class JoystickTest : MonoBehaviour
{
    public float speed = 10f;
	
	void OnEnable()
    {
        Joystick.On_JoystickHolding += Joystick_On_JoystickHolding;
    }

    void OnDisable()
    {
        Joystick.On_JoystickHolding -= Joystick_On_JoystickHolding;
    }

    private void Joystick_On_JoystickHolding(Joystick joystick)
    {
        if(joystick.JoystickName == "NguiJoystick")
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(joystick.JoystickAxis.x, 0f, joystick.JoystickAxis.y));
            transform.Translate(new Vector3(joystick.JoystickAxis.x, 0f, joystick.JoystickAxis.y) * speed * Time.deltaTime, Space.World);
        }
    }

   
}
