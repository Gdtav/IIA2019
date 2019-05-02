using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public int player;


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {

            // pass the input to the car!
            float h = (player == 0 ? CrossPlatformInputManager.GetAxis("Horizontal") : CrossPlatformInputManager.GetAxis("Horizontal1"));
            float v = (player == 0 ? CrossPlatformInputManager.GetAxis("Vertical") : CrossPlatformInputManager.GetAxis("Vertical1"));
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
