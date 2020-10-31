using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class DroneController : MonoBehaviour
    {
        [Tooltip("sets the drone's max forward speed")]
        public float forwardSpeed = 7f;
        [Tooltip("sets the drone's max backward speed")]
        public float backwardSpeed = 5f;
        [Tooltip("sets the drone's max left strafe speed")]
        public float rightSpeed = 5f;
        [Tooltip("sets the drone's max right strafe speed")]
        public float leftSpeed = 5f;
        [Tooltip("sets the drone's max rise speed")]
        public float riseSpeed = 5f;
        [Tooltip("sets the drone's max lower speed")]
        public float lowerSpeed = 5f;
        [Tooltip("how fast the drone speeds up")]
        public float acceleration = 0.5f;
        [Tooltip("how fast the drone slows down")]
        public float deceleration = 0.2f;
        [Tooltip("how eaisly the drone is affected by outside forces")]
        public float stability = 0.1f;
        [Tooltip("how fast the drone rotates")]
        public float turnSensitivty = 2f;
        [Tooltip("states whether or not the drone active on start")]
        public bool motorOn = true;
        [Tooltip("makes the drone move relative to an external compass")]
        public bool headless;
        [Tooltip("the external compass used to control the drone's flight direction")]
        public Transform compass;
        [Tooltip("assign drone's propellers to this array"), HideInInspector]
        public List<GameObject> propellers;
        [Tooltip("set propellers max spin speed"), HideInInspector]
        public float propSpinSpeed = 50f;
        [Tooltip("how fast the propellers slow down"), HideInInspector, Range(0f, 1f)]
        public float propStopSpeed = 1f;
        [Tooltip("the transform/location used to tilt the drone forward"), HideInInspector]
        public Transform frontTilt;
        [Tooltip("the transform/location used to tilt the drone backward"), HideInInspector]
        public Transform backTilt;
        [Tooltip("the transform/location used to tilt the drone right"), HideInInspector]
        public Transform rightTilt;
        [Tooltip("the transform/location used to tilt the drone left"), HideInInspector]
        public Transform leftTilt;
        [Tooltip("set whether or not the drone falls after a large impact"), HideInInspector]
        public bool fallAfterCollision = true;
        [Tooltip("sets the min. collision force used to drop the drone"), HideInInspector]
        public float fallMinimumForce = 6f;
        [Tooltip("sets the min. collision force used to create a spark"), HideInInspector]
        public float sparkMinimumForce = 1f;
        [Tooltip("the spark particle/object spawned on a collision"), HideInInspector]
        public GameObject sparkPrefab;
        [Tooltip("audio clip played during flight"), HideInInspector]
        public AudioSource flyingSound;
        [Tooltip("audio clip played on collision"), HideInInspector]
        public AudioSource sparkSound;
        [Tooltip("displays the collision force of the last impact"), HideInInspector]
        public float collisionMagnitude;
        [Tooltip("displays the current force lifting up the drone"), HideInInspector]
        public float liftForce;
        [Tooltip("displays the current force driving the drone"), HideInInspector]
        public float driveForce;
        [Tooltip("displays the current force strafing the drone"), HideInInspector]
        public float strafeForce;
        [Tooltip("displays the current force turning the drone"), HideInInspector]
        public float turnForce;
        [Tooltip("displays the drone's distance from the ground"), HideInInspector]
        public float groundDistance = float.PositiveInfinity;
        [Tooltip("displays the drones distance from being upright"), HideInInspector]
        public float uprightAngleDistance;
        [Tooltip("displays the current propeller speed"), HideInInspector]
        public float calPropSpeed;
        [Tooltip("displays drone's starting position"), HideInInspector]
        public Vector3 startPosition;
        [Tooltip("displays the drone's rotational position"), HideInInspector]
        public Quaternion startRotation;
        public Rigidbody rigidBody;
        private RaycastHit hit;
        private Collider coll;
        private float driveInput;
        private float strafeInput;
        private float liftInput;
        private float _drag;
        private float _angularDrag;
        private bool _gravity;

        public void AdjustAccel(float value)
        {
            this.acceleration = value;
        }

        public void AdjustDecel(float value)
        {
            this.deceleration = value;
        }

        public void AdjustLift(float value)
        {
            this.riseSpeed = value;
            this.lowerSpeed = value;
        }

        public void AdjustSpeed(float value)
        {
            this.forwardSpeed = value;
            this.backwardSpeed = value;
        }

        public void AdjustStable(float value)
        {
            this.stability = value;
        }

        public void AdjustStrafe(float value)
        {
            this.rightSpeed = value;
            this.leftSpeed = value;
        }

        public void AdjustTurn(float value)
        {
            this.turnSensitivty = value;
        }

        private void Awake()
        {
            this.coll = base.GetComponent<Collider>();
            this.rigidBody = base.GetComponent<Rigidbody>();
            this.startPosition = base.get_transform().get_position();
            this.startRotation = base.get_transform().get_rotation();
            this._gravity = this.rigidBody.get_useGravity();
            this._drag = this.rigidBody.get_drag();
            this._angularDrag = this.rigidBody.get_angularDrag();
        }

        public void ChangeFlightAudio(AudioClip newClip)
        {
            this.flyingSound.set_clip(newClip);
            this.flyingSound.set_enabled(false);
            this.flyingSound.set_enabled(true);
        }

        public void ChangeImpactAudio(AudioClip newClip)
        {
            this.sparkSound.set_clip(newClip);
            this.sparkSound.set_enabled(false);
            this.sparkSound.set_enabled(true);
        }

        public void DriveInput(float input)
        {
            if (input > 0f)
            {
                this.driveInput = input * this.forwardSpeed;
            }
            else if (input < 0f)
            {
                this.driveInput = input * this.backwardSpeed;
            }
            else
            {
                this.driveInput = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (!this.motorOn)
            {
                this.rigidBody.set_useGravity(this._gravity);
                this.rigidBody.set_drag(this._drag);
                this.rigidBody.set_angularDrag(this._angularDrag);
            }
            else
            {
                this.rigidBody.set_useGravity(false);
                this.rigidBody.set_drag(0f);
                this.rigidBody.set_angularDrag(0f);
                if (this.headless)
                {
                    if (!this.compass)
                    {
                        Debug.LogError("no headless compassed assinged! please asign a compass in order to use headless mode!");
                        this.headless = false;
                    }
                    if ((this.groundDistance > 0.2f) && ((this.driveInput != 0f) || (this.strafeInput != 0f)))
                    {
                        this.rigidBody.AddForceAtPosition(Vector3.get_down(), this.rigidBody.get_position() + (this.rigidBody.get_velocity().get_normalized() * 0.5f), 5);
                    }
                    Vector3 vector = this.compass.InverseTransformDirection(this.rigidBody.get_velocity());
                    vector.z = (this.driveInput != 0f) ? Mathf.Lerp(vector.z, this.driveInput, this.acceleration * 0.3f) : Mathf.Lerp(vector.z, this.driveInput, this.deceleration * 0.2f);
                    this.driveForce = (Mathf.Abs(vector.z) > 0.01f) ? vector.z : 0f;
                    vector.x = (this.strafeInput != 0f) ? Mathf.Lerp(vector.x, this.strafeInput, this.acceleration * 0.3f) : Mathf.Lerp(vector.x, this.strafeInput, this.deceleration * 0.2f);
                    this.strafeForce = (Mathf.Abs(vector.x) > 0.01f) ? vector.x : 0f;
                    this.rigidBody.set_velocity(this.compass.TransformDirection(vector));
                }
                else
                {
                    if (this.groundDistance > 0.2f)
                    {
                        if (this.driveInput > 0f)
                        {
                            this.rigidBody.AddForceAtPosition(Vector3.get_down() * (Mathf.Abs(this.driveInput) * 0.3f), this.frontTilt.get_position(), 5);
                        }
                        if (this.driveInput < 0f)
                        {
                            this.rigidBody.AddForceAtPosition(Vector3.get_down() * (Mathf.Abs(this.driveInput) * 0.3f), this.backTilt.get_position(), 5);
                        }
                        if (this.strafeInput > 0f)
                        {
                            this.rigidBody.AddForceAtPosition(Vector3.get_down() * (Mathf.Abs(this.strafeInput) * 0.3f), this.rightTilt.get_position(), 5);
                        }
                        if (this.strafeInput < 0f)
                        {
                            this.rigidBody.AddForceAtPosition(Vector3.get_down() * (Mathf.Abs(this.strafeInput) * 0.3f), this.leftTilt.get_position(), 5);
                        }
                    }
                    Vector3 vector3 = base.get_transform().InverseTransformDirection(this.rigidBody.get_velocity());
                    vector3.z = (this.driveInput != 0f) ? Mathf.Lerp(vector3.z, this.driveInput, this.acceleration * 0.3f) : Mathf.Lerp(vector3.z, this.driveInput, this.deceleration * 0.2f);
                    this.driveForce = (Mathf.Abs(vector3.z) > 0.01f) ? vector3.z : 0f;
                    vector3.x = (this.strafeInput != 0f) ? Mathf.Lerp(vector3.x, this.strafeInput, this.acceleration * 0.3f) : Mathf.Lerp(vector3.x, this.strafeInput, this.deceleration * 0.2f);
                    this.strafeForce = (Mathf.Abs(vector3.x) > 0.01f) ? vector3.x : 0f;
                    this.rigidBody.set_velocity(base.get_transform().TransformDirection(vector3));
                }
                this.liftForce = (this.liftInput != 0f) ? Mathf.Lerp(this.liftForce, this.liftInput, this.acceleration * 0.2f) : Mathf.Lerp(this.liftForce, this.liftInput, this.deceleration * 0.3f);
                this.liftForce = (Mathf.Abs(this.liftForce) > 0.01f) ? this.liftForce : 0f;
                this.rigidBody.set_velocity(new Vector3(this.rigidBody.get_velocity().x, this.liftForce, this.rigidBody.get_velocity().z));
                this.rigidBody.set_angularVelocity(this.rigidBody.get_angularVelocity() * (1f - (Mathf.Clamp(this.InputMagnitude(), 0.2f, 1f) * this.stability)));
                Quaternion quaternion = Quaternion.FromToRotation(base.get_transform().get_up(), Vector3.get_up());
                this.rigidBody.AddTorque(new Vector3(quaternion.x, 0f, quaternion.z) * 100f, 5);
                this.rigidBody.set_angularVelocity(new Vector3(this.rigidBody.get_angularVelocity().x, this.turnForce, this.rigidBody.get_angularVelocity().z));
            }
        }

        private float InputMagnitude() => 
            ((Mathf.Abs(this.driveInput) + Mathf.Abs(this.strafeInput)) + Mathf.Abs(this.liftInput)) / 3f;

        public void LiftInput(float input)
        {
            if (input > 0f)
            {
                this.liftInput = input * this.riseSpeed;
                this.motorOn = true;
            }
            else if (input < 0f)
            {
                this.liftInput = input * this.lowerSpeed;
            }
            else
            {
                this.liftInput = 0f;
            }
        }

        private void OnCollisionEnter(Collision newObject)
        {
            this.collisionMagnitude = newObject.get_relativeVelocity().get_magnitude();
            if (this.collisionMagnitude > this.sparkMinimumForce)
            {
                this.SpawnSparkPrefab(newObject.get_contacts()[0].get_point());
                if (this.sparkSound)
                {
                    this.sparkSound.set_pitch(this.collisionMagnitude * 0.1f);
                    this.sparkSound.PlayOneShot(this.sparkSound.get_clip(), this.collisionMagnitude * 0.05f);
                }
            }
            if ((this.collisionMagnitude > this.fallMinimumForce) && this.fallAfterCollision)
            {
                this.motorOn = false;
            }
        }

        private void OnCollisionStay(Collision newObject)
        {
            if (this.groundDistance < (this.coll.get_bounds().get_extents().y + 0.15f))
            {
                this.liftForce = Mathf.Clamp(this.liftForce, 0f, float.PositiveInfinity);
            }
        }

        public void ResetDronePosition()
        {
            this.rigidBody.set_position(this.startPosition);
            this.rigidBody.set_rotation(this.startRotation);
            this.rigidBody.set_velocity(Vector3.get_zero());
        }

        public void SpawnSparkPrefab(Vector3 position)
        {
            GameObject local1 = Object.Instantiate<GameObject>(this.sparkPrefab, position, Quaternion.get_identity());
            ParticleSystem.MainModule module = local1.GetComponent<ParticleSystem>().get_main();
            Object.Destroy(local1, module.get_duration() + module.get_startLifetime().get_constantMax());
        }

        private void Start()
        {
            MeshRenderer component = base.GetComponent<MeshRenderer>();
            if (component)
            {
                Object.Destroy(component);
            }
            Transform[] componentsInChildren = base.get_transform().GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if ((componentsInChildren[i] != base.get_transform()) && (!componentsInChildren[i].GetComponent<PA_DronePack.ObjectID>() || (componentsInChildren[i].GetComponent<PA_DronePack.ObjectID>().id != "uQ3mR7quTHTw2aAy")))
                {
                    base.set_enabled(false);
                    base.set_hideFlags(8);
                    Debug.LogError("ERROR: Editing " + base.get_name() + "'s prefab is NOT allowed!\n", base.get_gameObject());
                    Debug.LogWarning("Disabling PA_DroneController script...\n<color=#0057af>(Unlock the Full version of the drone pack to customize drones!)</color>", base.get_gameObject());
                    return;
                }
            }
            if (this.headless && !this.compass)
            {
                Debug.LogError("no headless compassed assinged! please asign a compass in order to use headless mode!");
                this.headless = false;
            }
        }

        public void StrafeInput(float input)
        {
            if (input > 0f)
            {
                this.strafeInput = input * this.rightSpeed;
            }
            else if (input < 0f)
            {
                this.strafeInput = input * this.leftSpeed;
            }
            else
            {
                this.strafeInput = 0f;
            }
        }

        private int TextureHash(Texture2D _texture)
        {
            int num = 0;
            try
            {
                foreach (Color color in _texture.GetPixels())
                {
                    num += Mathf.FloorToInt((color.r + color.g) + color.b);
                }
            }
            catch
            {
                Texture2D textured1 = new Texture2D(_texture.get_width(), _texture.get_height(), _texture.get_format(), _texture.get_mipmapCount() > 1);
                textured1.LoadRawTextureData(_texture.GetRawTextureData());
                textured1.Apply();
                foreach (Color color2 in textured1.GetPixels())
                {
                    num += Mathf.FloorToInt((color2.r + color2.g) + color2.b);
                }
            }
            return num;
        }

        public void ToggleFall(bool state)
        {
            this.fallAfterCollision = !this.fallAfterCollision;
        }

        public void ToggleHeadless()
        {
            this.headless = !this.headless;
        }

        public void ToggleMotor()
        {
            this.motorOn = !this.motorOn;
        }

        public void TurnInput(float input)
        {
            this.turnForce = input * this.turnSensitivty;
        }

        private void Update()
        {
            this.uprightAngleDistance = (1f - base.get_transform().get_up().y) * 0.5f;
            this.uprightAngleDistance = (this.uprightAngleDistance < 0.001) ? 0f : this.uprightAngleDistance;
            if (Physics.Raycast(base.get_transform().get_position(), Vector3.get_down(), ref this.hit, float.PositiveInfinity))
            {
                this.groundDistance = this.hit.get_distance();
            }
            this.calPropSpeed = this.motorOn ? this.propSpinSpeed : (this.calPropSpeed * (1f - (this.propStopSpeed / 2f)));
            using (List<GameObject>.Enumerator enumerator = this.propellers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.get_transform().Rotate(0f, 0f, this.calPropSpeed);
                }
            }
            if (this.flyingSound)
            {
                this.flyingSound.set_volume(this.calPropSpeed / this.propSpinSpeed);
                this.flyingSound.set_pitch(1f + (this.liftForce * 0.02f));
            }
        }
    }
}