using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class DroneCamera : MonoBehaviour
    {
        public CameraMode cameraMode;
        public FollowMode followMode;
        public PA_DronePack.DroneController target;
        public Transform fpsPosition;
        [Range(0.03f, 0.5f)]
        public float followSmoothing = 0.1f;
        [Range(0f, 7f)]
        public float xSensivity = 2f;
        [Range(0f, 7f)]
        public float ySensivity;
        public float height = 0.5f;
        public float distance = 1.5f;
        public float angle = 19f;
        public bool findTarget = true;
        public bool autoPosition = true;
        public bool freeLook;
        public bool findFPS = true;
        public bool gyroscopeEnabled;
        public bool invertYAxis;
        public List<Rigidbody> jitterRigidBodies;
        private Vector3 velocity;
        private float fpsMinAngle = -20f;
        private float fpsMaxAngle = 70f;
        private float angleV;
        private float scrollForce;
        private float turnForce;
        private float liftForce;
        private float targetRot;

        public void CanFreeLook(bool state)
        {
            this.freeLook = state;
        }

        public void ChangeCameraMode()
        {
            this.cameraMode = (this.cameraMode == CameraMode.firstPerson) ? CameraMode.thirdPerson : CameraMode.firstPerson;
        }

        public void ChangeFollowMode()
        {
            this.followMode = (this.followMode == FollowMode.smooth) ? FollowMode.firm : FollowMode.smooth;
        }

        public void ChangeGyroscope()
        {
            this.gyroscopeEnabled = !this.gyroscopeEnabled;
        }

        private void FixedUpdate()
        {
            if (this.freeLook && (this.cameraMode == CameraMode.thirdPerson))
            {
                this.target.TurnInput(0f);
            }
            if (this.target && ((this.followMode == FollowMode.smooth) && (this.cameraMode == CameraMode.thirdPerson)))
            {
                if ((this.target.rigidBody.get_interpolation() != null) && !base.GetComponent<Camera>().get_targetTexture())
                {
                    this.target.rigidBody.set_interpolation(0);
                }
                this.height += ((this.angle <= -60f) || (this.angle >= 60f)) ? 0f : (this.liftForce * 0.03f);
                this.angle = Mathf.Clamp(this.angle + this.liftForce, -60f, 60f);
                this.targetRot = this.freeLook ? (this.targetRot + (this.turnForce * this.xSensivity)) : this.target.get_transform().get_eulerAngles().y;
                float num = Mathf.SmoothDampAngle(base.get_transform().get_eulerAngles().y, this.targetRot, ref this.angleV, (this.followSmoothing * Time.get_fixedDeltaTime()) * 60f);
                Vector3 vector = (this.target.get_transform().get_position() - ((Quaternion.Euler(0f, num, 0f) * Vector3.get_forward()) * this.distance)) + new Vector3(0f, this.height, 0f);
                base.get_transform().set_position(Vector3.SmoothDamp(base.get_transform().get_position(), vector, ref this.velocity, (this.followSmoothing * Time.get_fixedDeltaTime()) * 60f));
                base.get_transform().set_rotation(Quaternion.Lerp(base.get_transform().get_rotation(), Quaternion.Euler(this.angle, this.targetRot, 0f), (this.followSmoothing * Time.get_fixedDeltaTime()) * 60f));
                foreach (Rigidbody rigidbody in this.jitterRigidBodies)
                {
                    if ((rigidbody.get_interpolation() != null) && !base.GetComponent<Camera>().get_targetTexture())
                    {
                        rigidbody.set_interpolation(0);
                    }
                }
            }
        }

        public void InvertYAxis(bool state)
        {
            this.invertYAxis = state;
        }

        private void LateUpdate()
        {
            if (this.freeLook && (this.cameraMode == CameraMode.thirdPerson))
            {
                this.target.TurnInput(0f);
            }
            if (this.target && ((this.followMode == FollowMode.firm) && (this.cameraMode == CameraMode.thirdPerson)))
            {
                if ((this.target.rigidBody.get_interpolation() != 1) && !base.GetComponent<Camera>().get_targetTexture())
                {
                    this.target.rigidBody.set_interpolation(1);
                }
                this.height += ((this.angle <= -60f) || (this.angle >= 60f)) ? 0f : (this.liftForce * 0.03f);
                this.angle = Mathf.Clamp(this.angle + this.liftForce, -60f, 60f);
                this.targetRot = this.freeLook ? (this.targetRot + (this.turnForce * this.xSensivity)) : this.target.get_transform().get_eulerAngles().y;
                base.get_transform().set_position((this.target.get_transform().get_position() - ((Quaternion.Euler(0f, this.targetRot, 0f) * Vector3.get_forward()) * this.distance)) + new Vector3(0f, this.height, 0f));
                base.get_transform().set_rotation(Quaternion.Euler(this.angle, this.targetRot, 0f));
                foreach (Rigidbody rigidbody in this.jitterRigidBodies)
                {
                    if ((rigidbody.get_interpolation() != 1) && !base.GetComponent<Camera>().get_targetTexture())
                    {
                        rigidbody.set_interpolation(1);
                    }
                }
            }
            if (this.target && (this.fpsPosition && ((this.cameraMode == CameraMode.firstPerson) && !base.GetComponent<Camera>().get_targetTexture())))
            {
                this.target.rigidBody.set_interpolation(1);
                base.get_transform().set_position(this.fpsPosition.get_position());
                base.get_transform().set_rotation(!this.gyroscopeEnabled ? Quaternion.Euler(this.target.get_transform().get_rotation().get_eulerAngles().x + this.scrollForce, this.fpsPosition.get_rotation().get_eulerAngles().y, this.fpsPosition.get_rotation().get_eulerAngles().z) : Quaternion.Euler(this.scrollForce, this.fpsPosition.get_rotation().get_eulerAngles().y, 0f));
                this.fpsPosition.set_rotation(Quaternion.Euler(base.get_transform().get_rotation().get_eulerAngles().x, this.target.get_transform().get_rotation().get_eulerAngles().y, this.target.get_transform().get_rotation().get_eulerAngles().z));
                using (List<Rigidbody>.Enumerator enumerator = this.jitterRigidBodies.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.set_interpolation(1);
                    }
                }
            }
        }

        public void LiftInput(float input)
        {
            this.liftForce = !this.invertYAxis ? (input * this.ySensivity) : (-input * this.ySensivity);
        }

        private void Start()
        {
            if (this.findTarget)
            {
                this.target = Object.FindObjectOfType<PA_DronePack.DroneController>();
                if (this.target == null)
                {
                    Debug.LogWarning("PA_DroneCamera could not find a drone target", base.get_gameObject());
                }
            }
            if (this.findFPS)
            {
                this.fpsPosition = GameObject.Find("FPSView").get_transform();
                if (this.fpsPosition == null)
                {
                    Debug.LogWarning("PA_DroneCamera could not find an FPS position", base.get_gameObject());
                }
            }
            if (!this.autoPosition || !this.target)
            {
                if (!this.target)
                {
                    Debug.LogError("ERROR: PA_DroneCamera is missing a target");
                }
            }
            else
            {
                float num = Mathf.Abs(this.target.get_transform().get_position().x - base.get_transform().get_position().x);
                float num2 = Mathf.Abs(this.target.get_transform().get_position().z - base.get_transform().get_position().z);
                this.distance = (num > num2) ? num : num2;
                this.height = Mathf.Abs(this.target.get_transform().get_position().y - base.get_transform().get_position().y);
                this.angle = base.get_transform().get_eulerAngles().x;
            }
        }

        public void TurnInput(float input)
        {
            this.turnForce = input * this.xSensivity;
        }

        private void Update()
        {
            if (this.freeLook && (this.cameraMode == CameraMode.thirdPerson))
            {
                this.target.TurnInput(0f);
            }
            this.scrollForce = Mathf.Clamp(this.scrollForce + (-Input.GetAxis("Mouse ScrollWheel") * 10f), this.fpsMinAngle, this.fpsMaxAngle);
        }

        public void XSensitivity(float value)
        {
            this.xSensivity = value;
        }

        public void YSensitivity(float value)
        {
            this.ySensivity = value;
        }

        public enum CameraMode
        {
            thirdPerson,
            firstPerson
        }

        public enum FollowMode
        {
            firm,
            smooth
        }
    }
}