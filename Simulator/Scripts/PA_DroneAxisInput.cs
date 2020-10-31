using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class DroneAxisInput : MonoBehaviour
    {
        public InputType inputType;
        private InputType? _inputType;
        public string forwardBackward;
        public string _forwardBackward;
        public string strafeLeftRight;
        public string _strafeLeftRight;
        public string riseLower;
        public string _riseLower;
        public string turn;
        public string _turn;
        public string toggleMotor;
        public string _toggleMotor;
        public string toggleCameraMode;
        public string _toggleCameraMode;
        public string toggleCameraGyro;
        public string _toggleCameraGyro;
        public string toggleFollowMode;
        public string _toggleFollowMode;
        public string toggleHeadless;
        public string _toggleHeadless;
        public string cameraRiseLower;
        public string _cameraRiseLower;
        public string cameraTurn;
        public string _cameraTurn;
        public string cameraFreeLook;
        public string _cameraFreeLook;
        public PA_DronePack.DroneController dcoScript;
        public PA_DronePack.DroneCamera dcScript;
        private bool toggleMotorIsKey;
        private bool toggleCameraModeIsKey;
        private bool toggleCameraGyroIsKey;
        private bool toggleFollowModeIsKey;
        private bool cameraFreeLookIsKey;
        private bool toggleHeadlessLookIsKey;
        private string[] keys;

        public DroneAxisInput()
        {
            string[] textArray1 = new string[0x4d];
            textArray1[0] = "f1";
            textArray1[1] = "f2";
            textArray1[2] = "f3";
            textArray1[3] = "f4";
            textArray1[4] = "f5";
            textArray1[5] = "f6";
            textArray1[6] = "f7";
            textArray1[7] = "f8";
            textArray1[8] = "f9";
            textArray1[9] = "f10";
            textArray1[10] = "f11";
            textArray1[11] = "f12";
            textArray1[12] = "f13";
            textArray1[13] = "f14";
            textArray1[14] = "f15";
            textArray1[15] = "0";
            textArray1[0x10] = "1";
            textArray1[0x11] = "2";
            textArray1[0x12] = "3";
            textArray1[0x13] = "4";
            textArray1[20] = "5";
            textArray1[0x15] = "6";
            textArray1[0x16] = "7";
            textArray1[0x17] = "8";
            textArray1[0x18] = "9";
            textArray1[0x19] = "a";
            textArray1[0x1a] = "b";
            textArray1[0x1b] = "c";
            textArray1[0x1c] = "d";
            textArray1[0x1d] = "e";
            textArray1[30] = "f";
            textArray1[0x1f] = "g";
            textArray1[0x20] = "h";
            textArray1[0x21] = "i";
            textArray1[0x22] = "j";
            textArray1[0x23] = "k";
            textArray1[0x24] = "l";
            textArray1[0x25] = "m";
            textArray1[0x26] = "n";
            textArray1[0x27] = "o";
            textArray1[40] = "p";
            textArray1[0x29] = "q";
            textArray1[0x2a] = "r";
            textArray1[0x2b] = "s";
            textArray1[0x2c] = "t";
            textArray1[0x2d] = "u";
            textArray1[0x2e] = "v";
            textArray1[0x2f] = "w";
            textArray1[0x30] = "x";
            textArray1[0x31] = "y";
            textArray1[50] = "z";
            textArray1[0x33] = "backspace";
            textArray1[0x34] = "delete";
            textArray1[0x35] = "tab";
            textArray1[0x36] = "clear";
            textArray1[0x37] = "return";
            textArray1[0x38] = "pause";
            textArray1[0x39] = "escape";
            textArray1[0x3a] = "space";
            textArray1[0x3b] = "up";
            textArray1[60] = "down";
            textArray1[0x3d] = "right";
            textArray1[0x3e] = "left";
            textArray1[0x3f] = "insert";
            textArray1[0x40] = "home";
            textArray1[0x41] = "end";
            textArray1[0x42] = "pageup";
            textArray1[0x43] = "pagedown";
            textArray1[0x44] = "numlock";
            textArray1[0x45] = "capslock";
            textArray1[70] = "scroll ock";
            textArray1[0x47] = "rightshift";
            textArray1[0x48] = "leftshift";
            textArray1[0x49] = "rightctrl";
            textArray1[0x4a] = "leftctrl";
            textArray1[0x4b] = "rightalt";
            textArray1[0x4c] = "leftalt";
            this.keys = textArray1;
        }

        private void Awake()
        {
            this.dcoScript = base.GetComponent<PA_DronePack.DroneController>();
            this.dcScript = Object.FindObjectOfType<PA_DronePack.DroneCamera>();
            this.UpdateInput();
        }

        private void ParseKeys()
        {
            this.toggleMotorIsKey = this.keys.Contains<string>(this.toggleMotor.ToLower());
            this.toggleCameraModeIsKey = this.keys.Contains<string>(this.toggleCameraMode.ToLower());
            this.toggleCameraGyroIsKey = this.keys.Contains<string>(this.toggleCameraGyro.ToLower());
            this.toggleFollowModeIsKey = this.keys.Contains<string>(this.toggleFollowMode.ToLower());
            this.cameraFreeLookIsKey = this.keys.Contains<string>(this.cameraFreeLook.ToLower());
            this.toggleHeadlessLookIsKey = this.keys.Contains<string>(this.toggleHeadless.ToLower());
        }

        private void Update()
        {
            InputType? nullable = this._inputType;
            InputType inputType = this.inputType;
            if (!((((InputType) nullable.GetValueOrDefault()) == inputType) & (nullable != null)))
            {
                this.UpdateInput();
                this._inputType = new InputType?(this.inputType);
            }
            if (this.forwardBackward != "")
            {
                this.dcoScript.DriveInput(Input.GetAxisRaw(this.forwardBackward));
            }
            if (this.strafeLeftRight != "")
            {
                this.dcoScript.StrafeInput(Input.GetAxisRaw(this.strafeLeftRight));
            }
            if (this.riseLower != "")
            {
                this.dcoScript.LiftInput(Input.GetAxisRaw(this.riseLower));
            }
            if (this.turn != "")
            {
                this.dcoScript.TurnInput(Input.GetAxisRaw(this.turn));
            }
            if ((this.cameraRiseLower != "") && this.dcScript)
            {
                this.dcScript.LiftInput(Input.GetAxisRaw(this.cameraRiseLower));
            }
            if ((this.cameraTurn != "") && this.dcScript)
            {
                this.dcScript.TurnInput(Input.GetAxisRaw(this.cameraTurn));
            }
            if (this.toggleMotor != "")
            {
                if (this.toggleMotorIsKey)
                {
                    if (Input.GetKeyDown((KeyCode) Enum.Parse(typeof(KeyCode), this.toggleMotor)))
                    {
                        this.dcoScript.ToggleMotor();
                    }
                }
                else if (Input.GetButtonDown(this.toggleMotor))
                {
                    this.dcoScript.ToggleMotor();
                }
            }
            if (this.toggleHeadless != "")
            {
                if (this.toggleHeadlessLookIsKey)
                {
                    if (Input.GetKeyDown((KeyCode) Enum.Parse(typeof(KeyCode), this.toggleHeadless)))
                    {
                        this.dcoScript.ToggleHeadless();
                    }
                }
                else if (Input.GetButtonDown(this.toggleHeadless))
                {
                    this.dcoScript.ToggleHeadless();
                }
            }
            if ((this.toggleCameraMode != "") && this.dcScript)
            {
                if (this.toggleCameraModeIsKey)
                {
                    if (Input.GetKeyDown((KeyCode) Enum.Parse(typeof(KeyCode), this.toggleCameraMode)))
                    {
                        this.dcScript.ChangeCameraMode();
                    }
                }
                else if (Input.GetButtonDown(this.toggleCameraMode))
                {
                    this.dcScript.ChangeCameraMode();
                }
            }
            if ((this.toggleCameraGyro != "") && this.dcScript)
            {
                if (this.toggleCameraGyroIsKey)
                {
                    if (Input.GetKeyDown((KeyCode) Enum.Parse(typeof(KeyCode), this.toggleCameraGyro)))
                    {
                        this.dcScript.ChangeGyroscope();
                    }
                }
                else if (Input.GetButtonDown(this.toggleCameraGyro))
                {
                    this.dcScript.ChangeGyroscope();
                }
            }
            if ((this.toggleFollowMode != "") && this.dcScript)
            {
                if (this.toggleFollowModeIsKey)
                {
                    if (Input.GetKeyDown((KeyCode) Enum.Parse(typeof(KeyCode), this.toggleFollowMode)))
                    {
                        this.dcScript.ChangeFollowMode();
                    }
                }
                else if (Input.GetButtonDown(this.toggleFollowMode))
                {
                    this.dcScript.ChangeFollowMode();
                }
            }
            if ((this.cameraFreeLook != "") && this.dcScript)
            {
                if (!this.cameraFreeLookIsKey)
                {
                    if (Input.GetButtonDown(this.cameraFreeLook))
                    {
                        this.dcScript.CanFreeLook(true);
                    }
                    if (Input.GetButtonUp(this.cameraFreeLook))
                    {
                        this.dcScript.CanFreeLook(false);
                    }
                }
                else
                {
                    if (Input.GetKeyDown((KeyCode) Enum.Parse(typeof(KeyCode), this.cameraFreeLook)))
                    {
                        this.dcScript.CanFreeLook(true);
                    }
                    if (Input.GetKeyUp((KeyCode) Enum.Parse(typeof(KeyCode), this.cameraFreeLook)))
                    {
                        this.dcScript.CanFreeLook(false);
                    }
                }
            }
        }

        public void UpdateInput()
        {
            if (this.inputType == InputType.Desktop)
            {
                this.forwardBackward = "Vertical";
                this.strafeLeftRight = "Horizontal";
                this.riseLower = "Lift";
                this.turn = "Mouse X";
                this.cameraRiseLower = "Mouse Y";
                this.cameraTurn = "Mouse X";
                this.toggleMotor = "Z";
                this.toggleCameraMode = "C";
                this.toggleCameraGyro = "G";
                this.toggleFollowMode = "F";
                this.cameraFreeLook = "LeftAlt";
                this.toggleHeadless = "H";
            }
            if (this.inputType == InputType.Gamepad)
            {
                this.forwardBackward = "GP SecondaryJoystick Y";
                this.strafeLeftRight = "GP SecondaryJoystick X";
                this.riseLower = "GP PrimaryJoystick Y";
                this.turn = "GP PrimaryJoystick X";
                this.cameraRiseLower = "GP DPad Y";
                this.cameraTurn = "GP PrimaryJoystick X";
                this.toggleMotor = "GP Button 0";
                this.toggleCameraMode = "GP Button 1";
                this.toggleCameraGyro = "GP Button 2";
                this.toggleFollowMode = "GP Button 3";
                this.cameraFreeLook = "GP Button 5";
                this.toggleHeadless = "GP Button 6";
            }
            if (this.inputType == InputType.OpenVR)
            {
                this.forwardBackward = "OVR RightJoystick Y";
                this.strafeLeftRight = "OVR RightJoystick X";
                this.riseLower = "OVR LeftJoystick Y";
                this.turn = "OVR LeftJoystick X";
                this.cameraRiseLower = "";
                this.cameraTurn = "OVR LeftJoystick X";
                this.toggleMotor = "OVR RightButton 0";
                this.toggleCameraMode = "C";
                this.toggleCameraGyro = "G";
                this.toggleFollowMode = "F";
                this.cameraFreeLook = "OVR RightTrigger";
                this.toggleHeadless = "OVR RightGrip";
            }
            if (this.inputType == InputType.Custom)
            {
                this.forwardBackward = this._forwardBackward;
                this.strafeLeftRight = this._strafeLeftRight;
                this.riseLower = this._riseLower;
                this.turn = this._turn;
                this.toggleMotor = this._toggleMotor;
                this.toggleCameraMode = this._toggleCameraMode;
                this.toggleCameraGyro = this._toggleCameraGyro;
                this.toggleFollowMode = this._toggleFollowMode;
                this.cameraRiseLower = this._cameraRiseLower;
                this.cameraTurn = this._cameraTurn;
                this.cameraFreeLook = this._cameraFreeLook;
                this.toggleHeadless = this._toggleHeadless;
            }
            this.ParseKeys();
        }

        public enum InputType
        {
            Desktop,
            Gamepad,
            OpenVR,
            Custom
        }
    }
}
