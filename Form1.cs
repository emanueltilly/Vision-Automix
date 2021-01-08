using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.IO;


namespace Vision_Automix
{
    public partial class Form1 : Form
    {

        ProjectData data = new ProjectData();       //Data-object for static project-settings
        RuntimeData runData = new RuntimeData();    //Data-object for realtime data

        MixOne mixone = new MixOne();

        

        /*Director director = new Director();             //Director
        CamerOperator cameraOp = new CamerOperator();   //Camera Operator
        Switcher switcher = new Switcher();             //Camera switcher*/

        string lastProjectFilePath = null;          //Filepath to current projectfile

        //Audio input devices
        private AudioInput audio1 = new AudioInput();
        private AudioInput audio2 = new AudioInput();
        private AudioInput audio3 = new AudioInput();
        private AudioInput audio4 = new AudioInput();
        private AudioInput audio5 = new AudioInput();
        private AudioInput audio6 = new AudioInput();
        private AudioInput audio7 = new AudioInput();
        private AudioInput audio8 = new AudioInput();


        public Form1()
        {
            InitializeComponent();
            data.Initialize();      //Initialize data object with default values
            //director.Initialize();
            UpdateAudioInterfacesList();       //Load available audio interfaces
            LoadGUIfromData();        //Load stored data into the GUI
            UpdateFormText();           //Set form text
            UpdateEnabledDevicesGUI();

            mixone.PushProjectdata(data);
            mixone.PushRuntimeData(runData);
            mixone.initialize();



        }

        private void LoadGUIfromData()
        {


            //1 DEVICE SETUP
            numUpDownSpeakers.Value = data.speakers;
            numUpDownCameras.Value = data.cameras;
            numUpDownCameraMoveTime.Value = data.cameraMoveTime;
            numUpDwnMinShotTime.Value = data.minimumShotTime;
            numUpDwnQuietTime.Value = data.maximumQuietTime;
            checkBoxUseQuiet.Checked = data.enableCutToWideOnQuiet;
            checkBoxEnablePRWbus.Checked = data.enablePRWbusControl;

            txtBoxNameSpeaker1.Text = data.speakerNames[0];
            txtBoxNameSpeaker2.Text = data.speakerNames[1];
            txtBoxNameSpeaker3.Text = data.speakerNames[2];
            txtBoxNameSpeaker4.Text = data.speakerNames[3];
            txtBoxNameSpeaker5.Text = data.speakerNames[4];
            txtBoxNameSpeaker6.Text = data.speakerNames[5];
            txtBoxNameSpeaker7.Text = data.speakerNames[6];
            txtBoxNameSpeaker8.Text = data.speakerNames[7];

            compIP1.Text = data.companionCon[0].ToString();
            compIP2.Text = data.companionCon[1].ToString();
            compIP3.Text = data.companionCon[2].ToString();
            compIP4.Text = data.companionCon[3].ToString();
            compPort.Text = data.companionCon[4].ToString();

            votelengthTrackBar.Value = data.voteLength;
            votelengthLabel.Text = data.voteLength.ToString();


            //2 AUDIO SETUP

            labelAudioSetupSpeakerName1.Text = ("1 - " + data.speakerNames[0]);
            labelAudioSetupSpeakerName2.Text = ("2 - " + data.speakerNames[1]);
            labelAudioSetupSpeakerName3.Text = ("3 - " + data.speakerNames[2]);
            labelAudioSetupSpeakerName4.Text = ("4 - " + data.speakerNames[3]);
            labelAudioSetupSpeakerName5.Text = ("5 - " + data.speakerNames[4]);
            labelAudioSetupSpeakerName6.Text = ("6 - " + data.speakerNames[5]);
            labelAudioSetupSpeakerName7.Text = ("7 - " + data.speakerNames[6]);
            labelAudioSetupSpeakerName8.Text = ("8 - " + data.speakerNames[7]);

            try { comboBoxSpeaker1.SelectedIndex = data.audioDevice1; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 1."); }
            try { comboBoxSpeaker2.SelectedIndex = data.audioDevice2; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 2."); }
            try { comboBoxSpeaker3.SelectedIndex = data.audioDevice3; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 3."); }
            try { comboBoxSpeaker4.SelectedIndex = data.audioDevice4; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 4."); }
            try { comboBoxSpeaker5.SelectedIndex = data.audioDevice5; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 5."); }
            try { comboBoxSpeaker6.SelectedIndex = data.audioDevice6; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 6."); }
            try { comboBoxSpeaker7.SelectedIndex = data.audioDevice7; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 7."); }
            try { comboBoxSpeaker8.SelectedIndex = data.audioDevice8; } catch { ErrorMessage("A problem occured trying to select audiointerfaces for Speaker 8."); }

            numUpDwnChannelSpeaker1.Value = data.audioChannel1;
            numUpDwnChannelSpeaker2.Value = data.audioChannel2;
            numUpDwnChannelSpeaker3.Value = data.audioChannel3;
            numUpDwnChannelSpeaker4.Value = data.audioChannel4;
            numUpDwnChannelSpeaker5.Value = data.audioChannel5;
            numUpDwnChannelSpeaker6.Value = data.audioChannel6;
            numUpDwnChannelSpeaker7.Value = data.audioChannel7;
            numUpDwnChannelSpeaker8.Value = data.audioChannel8;

            thresholdBar1.Value = data.audioThreshold1;
            thresholdBar2.Value = data.audioThreshold2;
            thresholdBar3.Value = data.audioThreshold3;
            thresholdBar4.Value = data.audioThreshold4;
            thresholdBar5.Value = data.audioThreshold5;
            thresholdBar6.Value = data.audioThreshold6;
            thresholdBar7.Value = data.audioThreshold7;
            thresholdBar8.Value = data.audioThreshold8;

            numUpDwnGain1.Value = data.audioGain1;
            numUpDwnGain2.Value = data.audioGain2;
            numUpDwnGain3.Value = data.audioGain3;
            numUpDwnGain4.Value = data.audioGain4;
            numUpDwnGain5.Value = data.audioGain5;
            numUpDwnGain6.Value = data.audioGain6;
            numUpDwnGain7.Value = data.audioGain7;
            numUpDwnGain8.Value = data.audioGain8;

            //3 PTZ Setup

            //NAMES
            labelPTZsetupSpeaker1.Text = ("1 - " + data.speakerNames[0]);
            labelPTZsetupSpeaker2.Text = ("2 - " + data.speakerNames[1]);
            labelPTZsetupSpeaker3.Text = ("3 - " + data.speakerNames[2]);
            labelPTZsetupSpeaker4.Text = ("4 - " + data.speakerNames[3]);
            labelPTZsetupSpeaker5.Text = ("5 - " + data.speakerNames[4]);
            labelPTZsetupSpeaker6.Text = ("6 - " + data.speakerNames[5]);
            labelPTZsetupSpeaker7.Text = ("7 - " + data.speakerNames[6]);
            labelPTZsetupSpeaker8.Text = ("8 - " + data.speakerNames[7]);

            //PROGRAM
            a1pgm.Value = data.c1pgm[0]; b1pgm.Value = data.c1pgm[1];
            a2pgm.Value = data.c2pgm[0]; b2pgm.Value = data.c2pgm[1];
            a3pgm.Value = data.c3pgm[0]; b3pgm.Value = data.c3pgm[1];
            a4pgm.Value = data.c4pgm[0]; b4pgm.Value = data.c4pgm[1];
            a5pgm.Value = data.c5pgm[0]; b5pgm.Value = data.c5pgm[1];
            a6pgm.Value = data.c6pgm[0]; b6pgm.Value = data.c6pgm[1];
            a7pgm.Value = data.c7pgm[0]; b7pgm.Value = data.c7pgm[1];
            a8pgm.Value = data.c8pgm[0]; b8pgm.Value = data.c8pgm[1];

            //PREVIEW
            a1prw.Value = data.c1prw[0]; b1prw.Value = data.c1prw[1];
            a2prw.Value = data.c2prw[0]; b2prw.Value = data.c2prw[1];
            a3prw.Value = data.c3prw[0]; b3prw.Value = data.c3prw[1];
            a4prw.Value = data.c4prw[0]; b4prw.Value = data.c4prw[1];
            a5prw.Value = data.c5prw[0]; b5prw.Value = data.c5prw[1];
            a6prw.Value = data.c6prw[0]; b6prw.Value = data.c6prw[1];
            a7prw.Value = data.c7prw[0]; b7prw.Value = data.c7prw[1];
            a8prw.Value = data.c8prw[0]; b8prw.Value = data.c8prw[1];

            //STATIC CAMERA
            stat1.Checked = data.staticCameras[0];
            stat2.Checked = data.staticCameras[1];
            stat3.Checked = data.staticCameras[2];
            stat4.Checked = data.staticCameras[3];
            stat5.Checked = data.staticCameras[4];
            stat6.Checked = data.staticCameras[5];
            stat7.Checked = data.staticCameras[6];
            stat8.Checked = data.staticCameras[7];

            statPos1.Value = data.staticCameraPositions[0];
            statPos2.Value = data.staticCameraPositions[1];
            statPos3.Value = data.staticCameraPositions[2];
            statPos4.Value = data.staticCameraPositions[3];
            statPos5.Value = data.staticCameraPositions[4];
            statPos6.Value = data.staticCameraPositions[5];
            statPos7.Value = data.staticCameraPositions[6];
            statPos8.Value = data.staticCameraPositions[7];

            //PREFERRED CAM
            prfCam0.Value = data.prefPos[0];
            prfCam1.Value = data.prefPos[1];
            prfCam2.Value = data.prefPos[2];
            prfCam3.Value = data.prefPos[3];
            prfCam4.Value = data.prefPos[4];
            prfCam5.Value = data.prefPos[5];
            prfCam6.Value = data.prefPos[6];
            prfCam7.Value = data.prefPos[7];
            prfCam8.Value = data.prefPos[8];

            //CAMERA 1
            a10.Value = data.c1p0[0];
            b10.Value = data.c1p0[1];
            e10.Checked = Convert.ToBoolean(data.c1p0[2]);

            a11.Value = data.c1p1[0];
            b11.Value = data.c1p1[1];
            e11.Checked = Convert.ToBoolean(data.c1p1[2]);

            a12.Value = data.c1p2[0];
            b12.Value = data.c1p2[1];
            e12.Checked = Convert.ToBoolean(data.c1p2[2]);

            a13.Value = data.c1p3[0];
            b13.Value = data.c1p3[1];
            e13.Checked = Convert.ToBoolean(data.c1p3[2]);

            a14.Value = data.c1p4[0];
            b14.Value = data.c1p4[1];
            e14.Checked = Convert.ToBoolean(data.c1p4[2]);

            a15.Value = data.c1p5[0];
            b15.Value = data.c1p5[1];
            e15.Checked = Convert.ToBoolean(data.c1p5[2]);

            a16.Value = data.c1p6[0];
            b16.Value = data.c1p6[1];
            e16.Checked = Convert.ToBoolean(data.c1p6[2]);

            a17.Value = data.c1p7[0];
            b17.Value = data.c1p7[1];
            e17.Checked = Convert.ToBoolean(data.c1p7[2]);

            a18.Value = data.c1p8[0];
            b18.Value = data.c1p8[1];
            e18.Checked = Convert.ToBoolean(data.c1p8[2]);

            //CAMERA 2
            a20.Value = data.c2p0[0];
            b20.Value = data.c2p0[1];
            e20.Checked = Convert.ToBoolean(data.c2p0[2]);

            a21.Value = data.c2p1[0];
            b21.Value = data.c2p1[1];
            e21.Checked = Convert.ToBoolean(data.c2p1[2]);

            a22.Value = data.c2p2[0];
            b22.Value = data.c2p2[1];
            e22.Checked = Convert.ToBoolean(data.c2p2[2]);

            a23.Value = data.c2p3[0];
            b23.Value = data.c2p3[1];
            e23.Checked = Convert.ToBoolean(data.c2p3[2]);

            a24.Value = data.c2p4[0];
            b24.Value = data.c2p4[1];
            e24.Checked = Convert.ToBoolean(data.c2p4[2]);

            a25.Value = data.c2p5[0];
            b25.Value = data.c2p5[1];
            e25.Checked = Convert.ToBoolean(data.c2p5[2]);

            a26.Value = data.c2p6[0];
            b26.Value = data.c2p6[1];
            e26.Checked = Convert.ToBoolean(data.c2p6[2]);

            a27.Value = data.c2p7[0];
            b27.Value = data.c2p7[1];
            e27.Checked = Convert.ToBoolean(data.c2p7[2]);

            a28.Value = data.c2p8[0];
            b28.Value = data.c2p8[1];
            e28.Checked = Convert.ToBoolean(data.c2p8[2]);

            //CAMERA 3
            a30.Value = data.c3p0[0];
            b30.Value = data.c3p0[1];
            e30.Checked = Convert.ToBoolean(data.c3p0[2]);

            a31.Value = data.c3p1[0];
            b31.Value = data.c3p1[1];
            e31.Checked = Convert.ToBoolean(data.c3p1[2]);

            a32.Value = data.c3p2[0];
            b32.Value = data.c3p2[1];
            e32.Checked = Convert.ToBoolean(data.c3p2[2]);

            a33.Value = data.c3p3[0];
            b33.Value = data.c3p3[1];
            e33.Checked = Convert.ToBoolean(data.c3p3[2]);

            a34.Value = data.c3p4[0];
            b34.Value = data.c3p4[1];
            e34.Checked = Convert.ToBoolean(data.c3p4[2]);

            a35.Value = data.c3p5[0];
            b35.Value = data.c3p5[1];
            e35.Checked = Convert.ToBoolean(data.c3p5[2]);

            a36.Value = data.c3p6[0];
            b36.Value = data.c3p6[1];
            e36.Checked = Convert.ToBoolean(data.c3p6[2]);

            a37.Value = data.c3p7[0];
            b37.Value = data.c3p7[1];
            e37.Checked = Convert.ToBoolean(data.c3p7[2]);

            a38.Value = data.c3p8[0];
            b38.Value = data.c3p8[1];
            e38.Checked = Convert.ToBoolean(data.c3p8[2]);

            //CAMERA 4
            a40.Value = data.c4p0[0];
            b40.Value = data.c4p0[1];
            e40.Checked = Convert.ToBoolean(data.c4p0[2]);

            a41.Value = data.c4p1[0];
            b41.Value = data.c4p1[1];
            e41.Checked = Convert.ToBoolean(data.c4p1[2]);

            a42.Value = data.c4p2[0];
            b42.Value = data.c4p2[1];
            e42.Checked = Convert.ToBoolean(data.c4p2[2]);

            a43.Value = data.c4p3[0];
            b43.Value = data.c4p3[1];
            e43.Checked = Convert.ToBoolean(data.c4p3[2]);

            a44.Value = data.c4p4[0];
            b44.Value = data.c4p4[1];
            e44.Checked = Convert.ToBoolean(data.c4p4[2]);

            a45.Value = data.c4p5[0];
            b45.Value = data.c4p5[1];
            e45.Checked = Convert.ToBoolean(data.c4p5[2]);

            a46.Value = data.c4p6[0];
            b46.Value = data.c4p6[1];
            e46.Checked = Convert.ToBoolean(data.c4p6[2]);

            a47.Value = data.c4p7[0];
            b47.Value = data.c4p7[1];
            e47.Checked = Convert.ToBoolean(data.c4p7[2]);

            a48.Value = data.c4p8[0];
            b48.Value = data.c4p8[1];
            e48.Checked = Convert.ToBoolean(data.c4p8[2]);

            //CAMERA 5
            a50.Value = data.c5p0[0];
            b50.Value = data.c5p0[1];
            e50.Checked = Convert.ToBoolean(data.c5p0[2]);

            a51.Value = data.c5p1[0];
            b51.Value = data.c5p1[1];
            e51.Checked = Convert.ToBoolean(data.c5p1[2]);

            a52.Value = data.c5p2[0];
            b52.Value = data.c5p2[1];
            e52.Checked = Convert.ToBoolean(data.c5p2[2]);

            a53.Value = data.c5p3[0];
            b53.Value = data.c5p3[1];
            e53.Checked = Convert.ToBoolean(data.c5p3[2]);

            a54.Value = data.c5p4[0];
            b54.Value = data.c5p4[1];
            e54.Checked = Convert.ToBoolean(data.c5p4[2]);

            a55.Value = data.c5p5[0];
            b55.Value = data.c5p5[1];
            e55.Checked = Convert.ToBoolean(data.c5p5[2]);

            a56.Value = data.c5p6[0];
            b56.Value = data.c5p6[1];
            e56.Checked = Convert.ToBoolean(data.c5p6[2]);

            a57.Value = data.c5p7[0];
            b57.Value = data.c5p7[1];
            e57.Checked = Convert.ToBoolean(data.c5p7[2]);

            a58.Value = data.c5p8[0];
            b58.Value = data.c5p8[1];
            e58.Checked = Convert.ToBoolean(data.c5p8[2]);

            //CAMERA 6
            a60.Value = data.c6p0[0];
            b60.Value = data.c6p0[1];
            e60.Checked = Convert.ToBoolean(data.c6p0[2]);

            a61.Value = data.c6p1[0];
            b61.Value = data.c6p1[1];
            e61.Checked = Convert.ToBoolean(data.c6p1[2]);

            a62.Value = data.c6p2[0];
            b62.Value = data.c6p2[1];
            e62.Checked = Convert.ToBoolean(data.c6p2[2]);

            a63.Value = data.c6p3[0];
            b63.Value = data.c6p3[1];
            e63.Checked = Convert.ToBoolean(data.c6p3[2]);

            a64.Value = data.c6p4[0];
            b64.Value = data.c6p4[1];
            e64.Checked = Convert.ToBoolean(data.c6p4[2]);

            a65.Value = data.c6p5[0];
            b65.Value = data.c6p5[1];
            e65.Checked = Convert.ToBoolean(data.c6p5[2]);

            a66.Value = data.c6p6[0];
            b66.Value = data.c6p6[1];
            e66.Checked = Convert.ToBoolean(data.c6p6[2]);

            a67.Value = data.c6p7[0];
            b67.Value = data.c6p7[1];
            e67.Checked = Convert.ToBoolean(data.c6p7[2]);

            a68.Value = data.c6p8[0];
            b68.Value = data.c6p8[1];
            e68.Checked = Convert.ToBoolean(data.c6p8[2]);

            //CAMERA 7
            a70.Value = data.c7p0[0];
            b70.Value = data.c7p0[1];
            e70.Checked = Convert.ToBoolean(data.c7p0[2]);

            a71.Value = data.c7p1[0];
            b71.Value = data.c7p1[1];
            e71.Checked = Convert.ToBoolean(data.c7p1[2]);

            a72.Value = data.c7p2[0];
            b72.Value = data.c7p2[1];
            e72.Checked = Convert.ToBoolean(data.c7p2[2]);

            a73.Value = data.c7p3[0];
            b73.Value = data.c7p3[1];
            e73.Checked = Convert.ToBoolean(data.c7p3[2]);

            a74.Value = data.c7p4[0];
            b74.Value = data.c7p4[1];
            e74.Checked = Convert.ToBoolean(data.c7p4[2]);

            a75.Value = data.c7p5[0];
            b75.Value = data.c7p5[1];
            e75.Checked = Convert.ToBoolean(data.c7p5[2]);

            a76.Value = data.c7p6[0];
            b76.Value = data.c7p6[1];
            e76.Checked = Convert.ToBoolean(data.c7p6[2]);

            a77.Value = data.c7p7[0];
            b77.Value = data.c7p7[1];
            e77.Checked = Convert.ToBoolean(data.c7p7[2]);

            a78.Value = data.c7p8[0];
            b78.Value = data.c7p8[1];
            e78.Checked = Convert.ToBoolean(data.c7p8[2]);

            //CAMERA 8
            a80.Value = data.c8p0[0];
            b80.Value = data.c8p0[1];
            e80.Checked = Convert.ToBoolean(data.c8p0[2]);

            a81.Value = data.c8p1[0];
            b81.Value = data.c8p1[1];
            e81.Checked = Convert.ToBoolean(data.c8p1[2]);

            a82.Value = data.c8p2[0];
            b82.Value = data.c8p2[1];
            e82.Checked = Convert.ToBoolean(data.c8p2[2]);

            a83.Value = data.c8p3[0];
            b83.Value = data.c8p3[1];
            e83.Checked = Convert.ToBoolean(data.c8p3[2]);

            a84.Value = data.c8p4[0];
            b84.Value = data.c8p4[1];
            e84.Checked = Convert.ToBoolean(data.c8p4[2]);

            a85.Value = data.c8p5[0];
            b85.Value = data.c8p5[1];
            e85.Checked = Convert.ToBoolean(data.c8p5[2]);

            a86.Value = data.c8p6[0];
            b86.Value = data.c8p6[1];
            e86.Checked = Convert.ToBoolean(data.c8p6[2]);

            a87.Value = data.c8p7[0];
            b87.Value = data.c8p7[1];
            e87.Checked = Convert.ToBoolean(data.c8p7[2]);

            a88.Value = data.c8p8[0];
            b88.Value = data.c8p8[1];
            e88.Checked = Convert.ToBoolean(data.c8p8[2]);



            //5-NEW LIVE MONITOR
            labelSpeakerBoxSpeaker1.Text = data.speakerNames[0];
            labelSpeakerBoxSpeaker2.Text = data.speakerNames[1];
            labelSpeakerBoxSpeaker3.Text = data.speakerNames[2];
            labelSpeakerBoxSpeaker4.Text = data.speakerNames[3];
            labelSpeakerBoxSpeaker5.Text = data.speakerNames[4];
            labelSpeakerBoxSpeaker6.Text = data.speakerNames[5];
            labelSpeakerBoxSpeaker7.Text = data.speakerNames[6];
            labelSpeakerBoxSpeaker8.Text = data.speakerNames[7];


            //Update CamPosMatrix
            GenerateCamPosMatrix();

            unsavedChanges(false);


        }

        private void SaveGUItoData()
        {
            //1 Device Setup
            data.speakers = (int)numUpDownSpeakers.Value;
            data.cameras = (int)numUpDownCameras.Value;
            data.cameraMoveTime = (int)numUpDownCameraMoveTime.Value;
            data.minimumShotTime = (int)numUpDwnMinShotTime.Value;
            data.maximumQuietTime = (int)numUpDwnQuietTime.Value;
            if (data.maximumQuietTime < data.minimumShotTime) { data.maximumQuietTime = data.minimumShotTime; }
            data.enableCutToWideOnQuiet = checkBoxUseQuiet.Checked;
            data.enablePRWbusControl = checkBoxEnablePRWbus.Checked;

            if (InputValidation.FriendlyName("Speaker name 1", txtBoxNameSpeaker1.Text)) { data.speakerNames[0] = txtBoxNameSpeaker1.Text; }
            if (InputValidation.FriendlyName("Speaker name 2", txtBoxNameSpeaker2.Text)) { data.speakerNames[1] = txtBoxNameSpeaker2.Text; }
            if (InputValidation.FriendlyName("Speaker name 3", txtBoxNameSpeaker3.Text)) { data.speakerNames[2] = txtBoxNameSpeaker3.Text; }
            if (InputValidation.FriendlyName("Speaker name 4", txtBoxNameSpeaker4.Text)) { data.speakerNames[3] = txtBoxNameSpeaker4.Text; }
            if (InputValidation.FriendlyName("Speaker name 5", txtBoxNameSpeaker5.Text)) { data.speakerNames[4] = txtBoxNameSpeaker5.Text; }
            if (InputValidation.FriendlyName("Speaker name 6", txtBoxNameSpeaker6.Text)) { data.speakerNames[5] = txtBoxNameSpeaker6.Text; }
            if (InputValidation.FriendlyName("Speaker name 7", txtBoxNameSpeaker7.Text)) { data.speakerNames[6] = txtBoxNameSpeaker7.Text; }
            if (InputValidation.FriendlyName("Speaker name 8", txtBoxNameSpeaker8.Text)) { data.speakerNames[7] = txtBoxNameSpeaker8.Text; }


            if (InputValidation.IpAddress("Companion IP address", compIP1.Text, compIP2.Text, compIP3.Text, compIP4.Text) && InputValidation.Port("Companion port number", compPort.Text))
            {
                data.companionCon[0] = int.Parse(compIP1.Text);
                data.companionCon[1] = int.Parse(compIP2.Text);
                data.companionCon[2] = int.Parse(compIP3.Text);
                data.companionCon[3] = int.Parse(compIP4.Text);
                data.companionCon[4] = int.Parse(compPort.Text);
            }


            data.voteLength = votelengthTrackBar.Value;



            //2 Audio Setup
            data.audioDevice1 = comboBoxSpeaker1.SelectedIndex;
            data.audioDevice2 = comboBoxSpeaker2.SelectedIndex;
            data.audioDevice3 = comboBoxSpeaker3.SelectedIndex;
            data.audioDevice4 = comboBoxSpeaker4.SelectedIndex;
            data.audioDevice5 = comboBoxSpeaker5.SelectedIndex;
            data.audioDevice6 = comboBoxSpeaker6.SelectedIndex;
            data.audioDevice7 = comboBoxSpeaker7.SelectedIndex;
            data.audioDevice8 = comboBoxSpeaker8.SelectedIndex;

            data.audioChannel1 = (int)numUpDwnChannelSpeaker1.Value;
            data.audioChannel2 = (int)numUpDwnChannelSpeaker2.Value;
            data.audioChannel3 = (int)numUpDwnChannelSpeaker3.Value;
            data.audioChannel4 = (int)numUpDwnChannelSpeaker4.Value;
            data.audioChannel5 = (int)numUpDwnChannelSpeaker5.Value;
            data.audioChannel6 = (int)numUpDwnChannelSpeaker6.Value;
            data.audioChannel7 = (int)numUpDwnChannelSpeaker7.Value;
            data.audioChannel8 = (int)numUpDwnChannelSpeaker8.Value;

            data.audioThreshold1 = thresholdBar1.Value;
            data.audioThreshold2 = thresholdBar2.Value;
            data.audioThreshold3 = thresholdBar3.Value;
            data.audioThreshold4 = thresholdBar4.Value;
            data.audioThreshold5 = thresholdBar5.Value;
            data.audioThreshold6 = thresholdBar6.Value;
            data.audioThreshold7 = thresholdBar7.Value;
            data.audioThreshold8 = thresholdBar8.Value;

            data.audioGain1 = (int)numUpDwnGain1.Value;
            data.audioGain2 = (int)numUpDwnGain2.Value;
            data.audioGain3 = (int)numUpDwnGain3.Value;
            data.audioGain4 = (int)numUpDwnGain4.Value;
            data.audioGain5 = (int)numUpDwnGain5.Value;
            data.audioGain6 = (int)numUpDwnGain6.Value;
            data.audioGain7 = (int)numUpDwnGain7.Value;
            data.audioGain8 = (int)numUpDwnGain8.Value;

            audio1.SetDevice(data.audioDevice1, data.audioChannel1);
            audio2.SetDevice(data.audioDevice2, data.audioChannel2);
            audio3.SetDevice(data.audioDevice3, data.audioChannel3);
            audio4.SetDevice(data.audioDevice4, data.audioChannel4);
            audio5.SetDevice(data.audioDevice5, data.audioChannel5);
            audio6.SetDevice(data.audioDevice6, data.audioChannel6);
            audio7.SetDevice(data.audioDevice7, data.audioChannel7);
            audio8.SetDevice(data.audioDevice8, data.audioChannel8);

            audio1.SetThreshold(data.audioThreshold1);
            audio2.SetThreshold(data.audioThreshold2);
            audio3.SetThreshold(data.audioThreshold3);
            audio4.SetThreshold(data.audioThreshold4);
            audio5.SetThreshold(data.audioThreshold5);
            audio6.SetThreshold(data.audioThreshold6);
            audio7.SetThreshold(data.audioThreshold7);
            audio8.SetThreshold(data.audioThreshold8);

            audio1.SetGain(data.audioGain1);
            audio2.SetGain(data.audioGain2);
            audio3.SetGain(data.audioGain3);
            audio4.SetGain(data.audioGain4);
            audio5.SetGain(data.audioGain5);
            audio6.SetGain(data.audioGain6);
            audio7.SetGain(data.audioGain7);
            audio8.SetGain(data.audioGain8);


            //3 PTZ Setup

            //PROGRAM
            data.c1pgm[0] = (int)a1pgm.Value; data.c1pgm[1] = (int)b1pgm.Value;
            data.c2pgm[0] = (int)a2pgm.Value; data.c2pgm[1] = (int)b2pgm.Value;
            data.c3pgm[0] = (int)a3pgm.Value; data.c3pgm[1] = (int)b3pgm.Value;
            data.c4pgm[0] = (int)a4pgm.Value; data.c4pgm[1] = (int)b4pgm.Value;
            data.c5pgm[0] = (int)a5pgm.Value; data.c5pgm[1] = (int)b5pgm.Value;
            data.c6pgm[0] = (int)a6pgm.Value; data.c6pgm[1] = (int)b6pgm.Value;
            data.c7pgm[0] = (int)a7pgm.Value; data.c7pgm[1] = (int)b7pgm.Value;
            data.c8pgm[0] = (int)a8pgm.Value; data.c8pgm[1] = (int)b8pgm.Value;

            //PREVIEW
            data.c1prw[0] = (int)a1prw.Value; data.c1prw[1] = (int)b1prw.Value;
            data.c2prw[0] = (int)a2prw.Value; data.c2prw[1] = (int)b2prw.Value;
            data.c3prw[0] = (int)a3prw.Value; data.c3prw[1] = (int)b3prw.Value;
            data.c4prw[0] = (int)a4prw.Value; data.c4prw[1] = (int)b4prw.Value;
            data.c5prw[0] = (int)a5prw.Value; data.c5prw[1] = (int)b5prw.Value;
            data.c6prw[0] = (int)a6prw.Value; data.c6prw[1] = (int)b6prw.Value;
            data.c7prw[0] = (int)a7prw.Value; data.c7prw[1] = (int)b7prw.Value;
            data.c8prw[0] = (int)a8prw.Value; data.c8prw[1] = (int)b8prw.Value;

            //STATIC CAMERA
            data.staticCameras[0] = stat1.Checked;
            data.staticCameras[1] = stat2.Checked;
            data.staticCameras[2] = stat3.Checked;
            data.staticCameras[3] = stat4.Checked;
            data.staticCameras[4] = stat5.Checked;
            data.staticCameras[5] = stat6.Checked;
            data.staticCameras[6] = stat7.Checked;
            data.staticCameras[7] = stat8.Checked;

            data.staticCameraPositions[0] = (int)statPos1.Value;
            data.staticCameraPositions[1] = (int)statPos2.Value;
            data.staticCameraPositions[2] = (int)statPos3.Value;
            data.staticCameraPositions[3] = (int)statPos4.Value;
            data.staticCameraPositions[4] = (int)statPos5.Value;
            data.staticCameraPositions[5] = (int)statPos6.Value;
            data.staticCameraPositions[6] = (int)statPos7.Value;
            data.staticCameraPositions[7] = (int)statPos8.Value;

            //PREFERRED CAMERA
            data.prefPos[0] = (int)prfCam0.Value;
            data.prefPos[1] = (int)prfCam1.Value;
            data.prefPos[2] = (int)prfCam2.Value;
            data.prefPos[3] = (int)prfCam3.Value;
            data.prefPos[4] = (int)prfCam4.Value;
            data.prefPos[5] = (int)prfCam5.Value;
            data.prefPos[6] = (int)prfCam6.Value;
            data.prefPos[7] = (int)prfCam7.Value;
            data.prefPos[8] = (int)prfCam8.Value;

            //Camera 1
            data.c1p0[0] = (int)a10.Value;
            data.c1p0[1] = (int)b10.Value;
            data.c1p0[2] = Convert.ToInt32(e10.Checked);

            data.c1p1[0] = (int)a11.Value;
            data.c1p1[1] = (int)b11.Value;
            data.c1p1[2] = Convert.ToInt32(e11.Checked);

            data.c1p2[0] = (int)a12.Value;
            data.c1p2[1] = (int)b12.Value;
            data.c1p2[2] = Convert.ToInt32(e12.Checked);

            data.c1p3[0] = (int)a13.Value;
            data.c1p3[1] = (int)b13.Value;
            data.c1p3[2] = Convert.ToInt32(e13.Checked);

            data.c1p4[0] = (int)a14.Value;
            data.c1p4[1] = (int)b14.Value;
            data.c1p4[2] = Convert.ToInt32(e14.Checked);

            data.c1p5[0] = (int)a15.Value;
            data.c1p5[1] = (int)b15.Value;
            data.c1p5[2] = Convert.ToInt32(e15.Checked);

            data.c1p6[0] = (int)a16.Value;
            data.c1p6[1] = (int)b16.Value;
            data.c1p6[2] = Convert.ToInt32(e16.Checked);

            data.c1p7[0] = (int)a17.Value;
            data.c1p7[1] = (int)b17.Value;
            data.c1p7[2] = Convert.ToInt32(e17.Checked);

            data.c1p8[0] = (int)a18.Value;
            data.c1p8[1] = (int)b18.Value;
            data.c1p8[2] = Convert.ToInt32(e18.Checked);

            //Camera 2
            data.c2p0[0] = (int)a20.Value;
            data.c2p0[1] = (int)b20.Value;
            data.c2p0[2] = Convert.ToInt32(e20.Checked);

            data.c2p1[0] = (int)a21.Value;
            data.c2p1[1] = (int)b21.Value;
            data.c2p1[2] = Convert.ToInt32(e21.Checked);

            data.c2p2[0] = (int)a22.Value;
            data.c2p2[1] = (int)b22.Value;
            data.c2p2[2] = Convert.ToInt32(e22.Checked);

            data.c2p3[0] = (int)a23.Value;
            data.c2p3[1] = (int)b23.Value;
            data.c2p3[2] = Convert.ToInt32(e23.Checked);

            data.c2p4[0] = (int)a24.Value;
            data.c2p4[1] = (int)b24.Value;
            data.c2p4[2] = Convert.ToInt32(e24.Checked);

            data.c2p5[0] = (int)a25.Value;
            data.c2p5[1] = (int)b25.Value;
            data.c2p5[2] = Convert.ToInt32(e25.Checked);

            data.c2p6[0] = (int)a26.Value;
            data.c2p6[1] = (int)b26.Value;
            data.c2p6[2] = Convert.ToInt32(e26.Checked);

            data.c2p7[0] = (int)a27.Value;
            data.c2p7[1] = (int)b27.Value;
            data.c2p7[2] = Convert.ToInt32(e27.Checked);

            data.c2p8[0] = (int)a28.Value;
            data.c2p8[1] = (int)b28.Value;
            data.c2p8[2] = Convert.ToInt32(e28.Checked);

            //Camera 3
            data.c3p0[0] = (int)a30.Value;
            data.c3p0[1] = (int)b30.Value;
            data.c3p0[2] = Convert.ToInt32(e30.Checked);

            data.c3p1[0] = (int)a31.Value;
            data.c3p1[1] = (int)b31.Value;
            data.c3p1[2] = Convert.ToInt32(e31.Checked);

            data.c3p2[0] = (int)a32.Value;
            data.c3p2[1] = (int)b32.Value;
            data.c3p2[2] = Convert.ToInt32(e32.Checked);

            data.c3p3[0] = (int)a33.Value;
            data.c3p3[1] = (int)b33.Value;
            data.c3p3[2] = Convert.ToInt32(e33.Checked);

            data.c3p4[0] = (int)a34.Value;
            data.c3p4[1] = (int)b34.Value;
            data.c3p4[2] = Convert.ToInt32(e34.Checked);

            data.c3p5[0] = (int)a35.Value;
            data.c3p5[1] = (int)b35.Value;
            data.c3p5[2] = Convert.ToInt32(e35.Checked);

            data.c3p6[0] = (int)a36.Value;
            data.c3p6[1] = (int)b36.Value;
            data.c3p6[2] = Convert.ToInt32(e36.Checked);

            data.c3p7[0] = (int)a37.Value;
            data.c3p7[1] = (int)b37.Value;
            data.c3p7[2] = Convert.ToInt32(e37.Checked);

            data.c3p8[0] = (int)a38.Value;
            data.c3p8[1] = (int)b38.Value;
            data.c3p8[2] = Convert.ToInt32(e38.Checked);

            //Camera 4
            data.c4p0[0] = (int)a40.Value;
            data.c4p0[1] = (int)b40.Value;
            data.c4p0[2] = Convert.ToInt32(e40.Checked);

            data.c4p1[0] = (int)a41.Value;
            data.c4p1[1] = (int)b41.Value;
            data.c4p1[2] = Convert.ToInt32(e41.Checked);

            data.c4p2[0] = (int)a42.Value;
            data.c4p2[1] = (int)b42.Value;
            data.c4p2[2] = Convert.ToInt32(e42.Checked);

            data.c4p3[0] = (int)a43.Value;
            data.c4p3[1] = (int)b43.Value;
            data.c4p3[2] = Convert.ToInt32(e43.Checked);

            data.c4p4[0] = (int)a44.Value;
            data.c4p4[1] = (int)b44.Value;
            data.c4p4[2] = Convert.ToInt32(e44.Checked);

            data.c4p5[0] = (int)a45.Value;
            data.c4p5[1] = (int)b45.Value;
            data.c4p5[2] = Convert.ToInt32(e45.Checked);

            data.c4p6[0] = (int)a46.Value;
            data.c4p6[1] = (int)b46.Value;
            data.c4p6[2] = Convert.ToInt32(e46.Checked);

            data.c4p7[0] = (int)a47.Value;
            data.c4p7[1] = (int)b47.Value;
            data.c4p7[2] = Convert.ToInt32(e47.Checked);

            data.c4p8[0] = (int)a48.Value;
            data.c4p8[1] = (int)b48.Value;
            data.c4p8[2] = Convert.ToInt32(e48.Checked);

            //Camera 5
            data.c5p0[0] = (int)a50.Value;
            data.c5p0[1] = (int)b50.Value;
            data.c5p0[2] = Convert.ToInt32(e50.Checked);

            data.c5p1[0] = (int)a51.Value;
            data.c5p1[1] = (int)b51.Value;
            data.c5p1[2] = Convert.ToInt32(e51.Checked);

            data.c5p2[0] = (int)a52.Value;
            data.c5p2[1] = (int)b52.Value;
            data.c5p2[2] = Convert.ToInt32(e52.Checked);

            data.c5p3[0] = (int)a53.Value;
            data.c5p3[1] = (int)b53.Value;
            data.c5p3[2] = Convert.ToInt32(e53.Checked);

            data.c5p4[0] = (int)a54.Value;
            data.c5p4[1] = (int)b54.Value;
            data.c5p4[2] = Convert.ToInt32(e54.Checked);

            data.c5p5[0] = (int)a55.Value;
            data.c5p5[1] = (int)b55.Value;
            data.c5p5[2] = Convert.ToInt32(e55.Checked);

            data.c5p6[0] = (int)a56.Value;
            data.c5p6[1] = (int)b56.Value;
            data.c5p6[2] = Convert.ToInt32(e56.Checked);

            data.c5p7[0] = (int)a57.Value;
            data.c5p7[1] = (int)b57.Value;
            data.c5p7[2] = Convert.ToInt32(e57.Checked);

            data.c5p8[0] = (int)a58.Value;
            data.c5p8[1] = (int)b58.Value;
            data.c5p8[2] = Convert.ToInt32(e58.Checked);

            //Camera 6
            data.c6p0[0] = (int)a60.Value;
            data.c6p0[1] = (int)b60.Value;
            data.c6p0[2] = Convert.ToInt32(e60.Checked);

            data.c6p1[0] = (int)a61.Value;
            data.c6p1[1] = (int)b61.Value;
            data.c6p1[2] = Convert.ToInt32(e61.Checked);

            data.c6p2[0] = (int)a62.Value;
            data.c6p2[1] = (int)b62.Value;
            data.c6p2[2] = Convert.ToInt32(e62.Checked);

            data.c6p3[0] = (int)a63.Value;
            data.c6p3[1] = (int)b63.Value;
            data.c6p3[2] = Convert.ToInt32(e63.Checked);

            data.c6p4[0] = (int)a64.Value;
            data.c6p4[1] = (int)b64.Value;
            data.c6p4[2] = Convert.ToInt32(e64.Checked);

            data.c6p5[0] = (int)a65.Value;
            data.c6p5[1] = (int)b65.Value;
            data.c6p5[2] = Convert.ToInt32(e65.Checked);

            data.c6p6[0] = (int)a66.Value;
            data.c6p6[1] = (int)b66.Value;
            data.c6p6[2] = Convert.ToInt32(e66.Checked);

            data.c6p7[0] = (int)a67.Value;
            data.c6p7[1] = (int)b67.Value;
            data.c6p7[2] = Convert.ToInt32(e67.Checked);

            data.c6p8[0] = (int)a68.Value;
            data.c6p8[1] = (int)b68.Value;
            data.c6p8[2] = Convert.ToInt32(e68.Checked);

            //Camera 7
            data.c7p0[0] = (int)a70.Value;
            data.c7p0[1] = (int)b70.Value;
            data.c7p0[2] = Convert.ToInt32(e70.Checked);

            data.c7p1[0] = (int)a71.Value;
            data.c7p1[1] = (int)b71.Value;
            data.c7p1[2] = Convert.ToInt32(e71.Checked);

            data.c7p2[0] = (int)a72.Value;
            data.c7p2[1] = (int)b72.Value;
            data.c7p2[2] = Convert.ToInt32(e72.Checked);

            data.c7p3[0] = (int)a73.Value;
            data.c7p3[1] = (int)b73.Value;
            data.c7p3[2] = Convert.ToInt32(e73.Checked);

            data.c7p4[0] = (int)a74.Value;
            data.c7p4[1] = (int)b74.Value;
            data.c7p4[2] = Convert.ToInt32(e74.Checked);

            data.c7p5[0] = (int)a75.Value;
            data.c7p5[1] = (int)b75.Value;
            data.c7p5[2] = Convert.ToInt32(e75.Checked);

            data.c7p6[0] = (int)a76.Value;
            data.c7p6[1] = (int)b76.Value;
            data.c7p6[2] = Convert.ToInt32(e76.Checked);

            data.c7p7[0] = (int)a77.Value;
            data.c7p7[1] = (int)b77.Value;
            data.c7p7[2] = Convert.ToInt32(e77.Checked);

            data.c7p8[0] = (int)a78.Value;
            data.c7p8[1] = (int)b78.Value;
            data.c7p8[2] = Convert.ToInt32(e78.Checked);

            //Camera 8
            data.c8p0[0] = (int)a80.Value;
            data.c8p0[1] = (int)b80.Value;
            data.c8p0[2] = Convert.ToInt32(e80.Checked);

            data.c8p1[0] = (int)a81.Value;
            data.c8p1[1] = (int)b81.Value;
            data.c8p1[2] = Convert.ToInt32(e81.Checked);

            data.c8p2[0] = (int)a82.Value;
            data.c8p2[1] = (int)b82.Value;
            data.c8p2[2] = Convert.ToInt32(e82.Checked);

            data.c8p3[0] = (int)a83.Value;
            data.c8p3[1] = (int)b83.Value;
            data.c8p3[2] = Convert.ToInt32(e83.Checked);

            data.c8p4[0] = (int)a84.Value;
            data.c8p4[1] = (int)b84.Value;
            data.c8p4[2] = Convert.ToInt32(e84.Checked);

            data.c8p5[0] = (int)a85.Value;
            data.c8p5[1] = (int)b85.Value;
            data.c8p5[2] = Convert.ToInt32(e85.Checked);

            data.c8p6[0] = (int)a86.Value;
            data.c8p6[1] = (int)b86.Value;
            data.c8p6[2] = Convert.ToInt32(e86.Checked);

            data.c8p7[0] = (int)a87.Value;
            data.c8p7[1] = (int)b87.Value;
            data.c8p7[2] = Convert.ToInt32(e87.Checked);

            data.c8p8[0] = (int)a88.Value;
            data.c8p8[1] = (int)b88.Value;
            data.c8p8[2] = Convert.ToInt32(e88.Checked);

            //Update CamPosMatrix
            GenerateCamPosMatrix();


        }



        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void TickGUIupdate_Tick(object sender, EventArgs e)
        {
            //GLOBAL ACTIONS
            companionToggleButton.Text = (runData.companionOutputEnabled ? "Companion Enabled" : "Companion Disabled");
            companionToggleButton.ForeColor = (runData.companionOutputEnabled ? Color.DarkGreen : Color.DarkRed);

            labelPeopleTalking.Text = (" |   People talking: " + mixone.GuiGetPeopleTalking());


            if (tabControl1.SelectedIndex == 1)
            {
                //AUDIO SETUP PAGE
                volumeBar1.Value = runData.speaker1Volume;
                volumeBar2.Value = runData.speaker2Volume;
                volumeBar3.Value = runData.speaker3Volume;
                volumeBar4.Value = runData.speaker4Volume;
                volumeBar5.Value = runData.speaker5Volume;
                volumeBar6.Value = runData.speaker6Volume;
                volumeBar7.Value = runData.speaker7Volume;
                volumeBar8.Value = runData.speaker8Volume;
            }

            if (tabControl1.SelectedIndex == 2)
            {
                //PTZ SETUP PAGE
            }



            if (tabControl1.SelectedIndex == 3)
            {
                //LIVE MONITOR PAGE NICE ONE
                int cameraInPGM = mixone.GuiGetProgram();
                int cameraInPRW = mixone.GuiGetPreview();


                //PREVIEW BOX
                if (runData.cameraPRW != 0)
                {
                    labelPreviewCamera.Text = ("CAM " + cameraInPRW.ToString());

                    int previewSpeakerId = mixone.GuiGetSpeakerIDForCamera(cameraInPRW);
                    if (previewSpeakerId >= 0 && previewSpeakerId < 9)
                    {
                        labelPreviewSpeaker.Text = ((previewSpeakerId == 0) ? "Group shot" : mixone.GuiGetSpeakerNameForCamera(cameraInPRW));
                    }
                    else { labelPreviewSpeaker.Text = "Unknown position"; }
                }
                else
                {
                    labelPreviewCamera.Text = "N/A";
                    labelPreviewSpeaker.Text = "---";
                }


                //PROGRAM BOX
                if (runData.cameraPGM != 0)
                {
                    labelProgramCamera.Text = ("CAM " + cameraInPGM.ToString());

                    int programSpeakerId = mixone.GuiGetSpeakerIDForCamera(cameraInPGM);
                    if (programSpeakerId >= 0 && programSpeakerId < 9)
                    {
                        labelProgramSpeaker.Text = ((programSpeakerId == 0) ? "Group shot" : mixone.GuiGetSpeakerNameForCamera(cameraInPGM));
                    }
                    else { labelProgramSpeaker.Text = "Unknown position"; }
                }
                else
                {
                    labelProgramCamera.Text = "N/A";
                    labelProgramSpeaker.Text = "---";
                }
                labelCurrentShotTime.Text = mixone.GuiGetCurrentShotDuration().ToString();

                bool[] speakersOpen = mixone.GuiGetSpeakersOpen();
                //SPEAKER BOX
                labelSpeakerBoxSpeaker1.ForeColor = (speakersOpen[0] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));
                labelSpeakerBoxSpeaker2.ForeColor = (speakersOpen[1] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));
                labelSpeakerBoxSpeaker3.ForeColor = (speakersOpen[2] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));
                labelSpeakerBoxSpeaker4.ForeColor = (speakersOpen[3] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));
                labelSpeakerBoxSpeaker5.ForeColor = (speakersOpen[4] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));
                labelSpeakerBoxSpeaker6.ForeColor = (speakersOpen[5] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));
                labelSpeakerBoxSpeaker7.ForeColor = (speakersOpen[6] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));
                labelSpeakerBoxSpeaker8.ForeColor = (speakersOpen[7] ? Color.FromArgb(255, 255, 214, 5) : Color.FromArgb(255, 45, 45, 45));

                //CAMERAS BOX
                //Status
                labelCameraBoxStatus1.Text = (mixone.GetCameraBusy(data, 1) ? "MOVING" : ((mixone.GuiGetProgram() == 1) ? "ON AIR" : ""));
                labelCameraBoxStatus2.Text = (mixone.GetCameraBusy(data, 2) ? "MOVING" : ((mixone.GuiGetProgram() == 2) ? "ON AIR" : ""));
                labelCameraBoxStatus3.Text = (mixone.GetCameraBusy(data, 3) ? "MOVING" : ((mixone.GuiGetProgram() == 3) ? "ON AIR" : ""));
                labelCameraBoxStatus4.Text = (mixone.GetCameraBusy(data, 4) ? "MOVING" : ((mixone.GuiGetProgram() == 4) ? "ON AIR" : ""));
                labelCameraBoxStatus5.Text = (mixone.GetCameraBusy(data, 5) ? "MOVING" : ((mixone.GuiGetProgram() == 5) ? "ON AIR" : ""));
                labelCameraBoxStatus6.Text = (mixone.GetCameraBusy(data, 6) ? "MOVING" : ((mixone.GuiGetProgram() == 6) ? "ON AIR" : ""));
                labelCameraBoxStatus7.Text = (mixone.GetCameraBusy(data, 7) ? "MOVING" : ((mixone.GuiGetProgram() == 7) ? "ON AIR" : ""));
                labelCameraBoxStatus8.Text = (mixone.GetCameraBusy(data, 8) ? "MOVING" : ((mixone.GuiGetProgram() == 8) ? "ON AIR" : ""));
                //Position
                labelCameraBoxPosition1.Text = (mixone.GuiGetSpeakerNameForCamera(1));
                labelCameraBoxPosition2.Text = (mixone.GuiGetSpeakerNameForCamera(2));
                labelCameraBoxPosition3.Text = (mixone.GuiGetSpeakerNameForCamera(3));
                labelCameraBoxPosition4.Text = (mixone.GuiGetSpeakerNameForCamera(4));
                labelCameraBoxPosition5.Text = (mixone.GuiGetSpeakerNameForCamera(5));
                labelCameraBoxPosition6.Text = (mixone.GuiGetSpeakerNameForCamera(6));
                labelCameraBoxPosition7.Text = (mixone.GuiGetSpeakerNameForCamera(7));
                labelCameraBoxPosition8.Text = (mixone.GuiGetSpeakerNameForCamera(8));

                

                //Colors
                Color colorNORMAL = Color.WhiteSmoke;
                Color colorPGM = Color.FromArgb(255, 233, 69, 69);
                Color colorPRW = Color.FromArgb(255, 69, 233, 69);
                if (mixone.GuiGetProgram() == 1) { camBoxLabelCam1.ForeColor = colorPGM; labelCameraBoxPosition1.ForeColor = colorPGM; labelCameraBoxStatus1.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 1) { camBoxLabelCam1.ForeColor = colorPRW; labelCameraBoxPosition1.ForeColor = colorPRW; labelCameraBoxStatus1.ForeColor = colorPRW; }
                else { camBoxLabelCam1.ForeColor = colorNORMAL; labelCameraBoxPosition1.ForeColor = colorNORMAL; labelCameraBoxStatus1.ForeColor = colorNORMAL; }

                if (mixone.GuiGetProgram() == 2) { camBoxLabelCam2.ForeColor = colorPGM; labelCameraBoxPosition2.ForeColor = colorPGM; labelCameraBoxStatus2.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 2) { camBoxLabelCam2.ForeColor = colorPRW; labelCameraBoxPosition2.ForeColor = colorPRW; labelCameraBoxStatus2.ForeColor = colorPRW; }
                else { camBoxLabelCam2.ForeColor = colorNORMAL; labelCameraBoxPosition2.ForeColor = colorNORMAL; labelCameraBoxStatus2.ForeColor = colorNORMAL; }

                if (mixone.GuiGetProgram() == 3) { camBoxLabelCam3.ForeColor = colorPGM; labelCameraBoxPosition3.ForeColor = colorPGM; labelCameraBoxStatus3.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 3) { camBoxLabelCam3.ForeColor = colorPRW; labelCameraBoxPosition3.ForeColor = colorPRW; labelCameraBoxStatus3.ForeColor = colorPRW; }
                else { camBoxLabelCam3.ForeColor = colorNORMAL; labelCameraBoxPosition3.ForeColor = colorNORMAL; labelCameraBoxStatus3.ForeColor = colorNORMAL; }

                if (mixone.GuiGetProgram() == 4) { camBoxLabelCam4.ForeColor = colorPGM; labelCameraBoxPosition4.ForeColor = colorPGM; labelCameraBoxStatus4.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 4) { camBoxLabelCam4.ForeColor = colorPRW; labelCameraBoxPosition4.ForeColor = colorPRW; labelCameraBoxStatus4.ForeColor = colorPRW; }
                else { camBoxLabelCam4.ForeColor = colorNORMAL; labelCameraBoxPosition4.ForeColor = colorNORMAL; labelCameraBoxStatus4.ForeColor = colorNORMAL; }

                if (mixone.GuiGetProgram() == 5) { camBoxLabelCam5.ForeColor = colorPGM; labelCameraBoxPosition5.ForeColor = colorPGM; labelCameraBoxStatus5.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 5) { camBoxLabelCam5.ForeColor = colorPRW; labelCameraBoxPosition5.ForeColor = colorPRW; labelCameraBoxStatus5.ForeColor = colorPRW; }
                else { camBoxLabelCam5.ForeColor = colorNORMAL; labelCameraBoxPosition5.ForeColor = colorNORMAL; labelCameraBoxStatus5.ForeColor = colorNORMAL; }

                if (mixone.GuiGetProgram() == 6) { camBoxLabelCam6.ForeColor = colorPGM; labelCameraBoxPosition6.ForeColor = colorPGM; labelCameraBoxStatus6.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 6) { camBoxLabelCam6.ForeColor = colorPRW; labelCameraBoxPosition6.ForeColor = colorPRW; labelCameraBoxStatus6.ForeColor = colorPRW; }
                else { camBoxLabelCam6.ForeColor = colorNORMAL; labelCameraBoxPosition6.ForeColor = colorNORMAL; labelCameraBoxStatus6.ForeColor = colorNORMAL; }

                if (mixone.GuiGetProgram() == 7) { camBoxLabelCam7.ForeColor = colorPGM; labelCameraBoxPosition7.ForeColor = colorPGM; labelCameraBoxStatus7.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 7) { camBoxLabelCam7.ForeColor = colorPRW; labelCameraBoxPosition7.ForeColor = colorPRW; labelCameraBoxStatus7.ForeColor = colorPRW; }
                else { camBoxLabelCam7.ForeColor = colorNORMAL; labelCameraBoxPosition7.ForeColor = colorNORMAL; labelCameraBoxStatus7.ForeColor = colorNORMAL; }

                if (mixone.GuiGetProgram() == 8) { camBoxLabelCam8.ForeColor = colorPGM; labelCameraBoxPosition8.ForeColor = colorPGM; labelCameraBoxStatus8.ForeColor = colorPGM; }
                else if (mixone.GuiGetPreview() == 8) { camBoxLabelCam8.ForeColor = colorPRW; labelCameraBoxPosition8.ForeColor = colorPRW; labelCameraBoxStatus8.ForeColor = colorPRW; }
                else { camBoxLabelCam8.ForeColor = colorNORMAL; labelCameraBoxPosition8.ForeColor = colorNORMAL; labelCameraBoxStatus8.ForeColor = colorNORMAL; }
            }
        }
        //CamPos Matrix is used to identify what cameras are enabled for what positions
        private void GenerateCamPosMatrix()
        {
            Console.WriteLine("CamPos Matrix update has started...");

            bool[,] camposMatrix = new bool[8, 9];

            //CAM1
            camposMatrix[0, 0] = Convert.ToBoolean(data.c1p0[2]);
            camposMatrix[0, 1] = Convert.ToBoolean(data.c1p1[2]);
            camposMatrix[0, 2] = Convert.ToBoolean(data.c1p2[2]);
            camposMatrix[0, 3] = Convert.ToBoolean(data.c1p3[2]);
            camposMatrix[0, 4] = Convert.ToBoolean(data.c1p4[2]);
            camposMatrix[0, 5] = Convert.ToBoolean(data.c1p5[2]);
            camposMatrix[0, 6] = Convert.ToBoolean(data.c1p6[2]);
            camposMatrix[0, 7] = Convert.ToBoolean(data.c1p7[2]);
            camposMatrix[0, 8] = Convert.ToBoolean(data.c1p8[2]);

            Console.WriteLine("Cam1 OK");
            //CAM2
            camposMatrix[1, 0] = Convert.ToBoolean(data.c2p0[2]);
            camposMatrix[1, 1] = Convert.ToBoolean(data.c2p1[2]);
            camposMatrix[1, 2] = Convert.ToBoolean(data.c2p2[2]);
            camposMatrix[1, 3] = Convert.ToBoolean(data.c2p3[2]);
            camposMatrix[1, 4] = Convert.ToBoolean(data.c2p4[2]);
            camposMatrix[1, 5] = Convert.ToBoolean(data.c2p5[2]);
            camposMatrix[1, 6] = Convert.ToBoolean(data.c2p6[2]);
            camposMatrix[1, 7] = Convert.ToBoolean(data.c2p7[2]);
            camposMatrix[1, 8] = Convert.ToBoolean(data.c2p8[2]);

            Console.WriteLine("Cam2 OK");
            //CAM3
            camposMatrix[2, 0] = Convert.ToBoolean(data.c3p0[2]);
            camposMatrix[2, 1] = Convert.ToBoolean(data.c3p1[2]);
            camposMatrix[2, 2] = Convert.ToBoolean(data.c3p2[2]);
            camposMatrix[2, 3] = Convert.ToBoolean(data.c3p3[2]);
            camposMatrix[2, 4] = Convert.ToBoolean(data.c3p4[2]);
            camposMatrix[2, 5] = Convert.ToBoolean(data.c3p5[2]);
            camposMatrix[2, 6] = Convert.ToBoolean(data.c3p6[2]);
            camposMatrix[2, 7] = Convert.ToBoolean(data.c3p7[2]);
            camposMatrix[2, 8] = Convert.ToBoolean(data.c3p8[2]);

            Console.WriteLine("Cam3 OK");
            //CAM4
            camposMatrix[3, 0] = Convert.ToBoolean(data.c4p0[2]);
            camposMatrix[3, 1] = Convert.ToBoolean(data.c4p1[2]);
            camposMatrix[3, 2] = Convert.ToBoolean(data.c4p2[2]);
            camposMatrix[3, 3] = Convert.ToBoolean(data.c4p3[2]);
            camposMatrix[3, 4] = Convert.ToBoolean(data.c4p4[2]);
            camposMatrix[3, 5] = Convert.ToBoolean(data.c4p5[2]);
            camposMatrix[3, 6] = Convert.ToBoolean(data.c4p6[2]);
            camposMatrix[3, 7] = Convert.ToBoolean(data.c4p7[2]);
            camposMatrix[3, 8] = Convert.ToBoolean(data.c4p8[2]);

            Console.WriteLine("Cam4 OK");
            //CAM5
            camposMatrix[4, 0] = Convert.ToBoolean(data.c5p0[2]);
            camposMatrix[4, 1] = Convert.ToBoolean(data.c5p1[2]);
            camposMatrix[4, 2] = Convert.ToBoolean(data.c5p2[2]);
            camposMatrix[4, 3] = Convert.ToBoolean(data.c5p3[2]);
            camposMatrix[4, 4] = Convert.ToBoolean(data.c5p4[2]);
            camposMatrix[4, 5] = Convert.ToBoolean(data.c5p5[2]);
            camposMatrix[4, 6] = Convert.ToBoolean(data.c5p6[2]);
            camposMatrix[4, 7] = Convert.ToBoolean(data.c5p7[2]);
            camposMatrix[4, 8] = Convert.ToBoolean(data.c5p8[2]);

            Console.WriteLine("Cam5 OK");
            //CAM6
            camposMatrix[5, 0] = Convert.ToBoolean(data.c6p0[2]);
            camposMatrix[5, 1] = Convert.ToBoolean(data.c6p1[2]);
            camposMatrix[5, 2] = Convert.ToBoolean(data.c6p2[2]);
            camposMatrix[5, 3] = Convert.ToBoolean(data.c6p3[2]);
            camposMatrix[5, 4] = Convert.ToBoolean(data.c6p4[2]);
            camposMatrix[5, 5] = Convert.ToBoolean(data.c6p5[2]);
            camposMatrix[5, 6] = Convert.ToBoolean(data.c6p6[2]);
            camposMatrix[5, 7] = Convert.ToBoolean(data.c6p7[2]);
            camposMatrix[5, 8] = Convert.ToBoolean(data.c6p8[2]);

            Console.WriteLine("Cam6 OK");
            //CAM7
            camposMatrix[6, 0] = Convert.ToBoolean(data.c7p0[2]);
            camposMatrix[6, 1] = Convert.ToBoolean(data.c7p1[2]);
            camposMatrix[6, 2] = Convert.ToBoolean(data.c7p2[2]);
            camposMatrix[6, 3] = Convert.ToBoolean(data.c7p3[2]);
            camposMatrix[6, 4] = Convert.ToBoolean(data.c7p4[2]);
            camposMatrix[6, 5] = Convert.ToBoolean(data.c7p5[2]);
            camposMatrix[6, 6] = Convert.ToBoolean(data.c7p6[2]);
            camposMatrix[6, 7] = Convert.ToBoolean(data.c7p7[2]);
            camposMatrix[6, 8] = Convert.ToBoolean(data.c7p8[2]);

            Console.WriteLine("Cam7 OK");
            //CAM8
            camposMatrix[7, 0] = Convert.ToBoolean(data.c8p0[2]);
            camposMatrix[7, 1] = Convert.ToBoolean(data.c8p1[2]);
            camposMatrix[7, 2] = Convert.ToBoolean(data.c8p2[2]);
            camposMatrix[7, 3] = Convert.ToBoolean(data.c8p3[2]);
            camposMatrix[7, 4] = Convert.ToBoolean(data.c8p4[2]);
            camposMatrix[7, 5] = Convert.ToBoolean(data.c8p5[2]);
            camposMatrix[7, 6] = Convert.ToBoolean(data.c8p6[2]);
            camposMatrix[7, 7] = Convert.ToBoolean(data.c8p7[2]);
            camposMatrix[7, 8] = Convert.ToBoolean(data.c8p8[2]);

            Console.WriteLine("Cam8 OK");

            runData.camposMatrix = camposMatrix;


            Console.WriteLine("CamPos Matrix update OK.");

        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            ApplySettings();

        }

        private void ApplySettings()
        {
            //ORDER IS IMPORTANT
            //Saving GUI data before restarting director will result in CamposMatrix not working.
            //RestartDirector();
            SaveGUItoData();
            LoadGUIfromData();
            UpdateEnabledDevicesGUI();

            unsavedChanges(false);
        }

        private void UpdateAudioInterfacesList()
        {
            //Get list of devices from object
            List<String> listedDevices1 = audio1.Initialize(1);
            List<String> listedDevices2 = audio2.Initialize(2);
            List<String> listedDevices3 = audio3.Initialize(3);
            List<String> listedDevices4 = audio4.Initialize(4);
            List<String> listedDevices5 = audio5.Initialize(5);
            List<String> listedDevices6 = audio6.Initialize(6);
            List<String> listedDevices7 = audio7.Initialize(7);
            List<String> listedDevices8 = audio8.Initialize(8);

            //Populate comboboxes on page 2
            foreach (String n in listedDevices1) { comboBoxSpeaker1.Items.Add(n); }
            foreach (String n in listedDevices2) { comboBoxSpeaker2.Items.Add(n); }
            foreach (String n in listedDevices3) { comboBoxSpeaker3.Items.Add(n); }
            foreach (String n in listedDevices4) { comboBoxSpeaker4.Items.Add(n); }
            foreach (String n in listedDevices5) { comboBoxSpeaker5.Items.Add(n); }
            foreach (String n in listedDevices6) { comboBoxSpeaker6.Items.Add(n); }
            foreach (String n in listedDevices7) { comboBoxSpeaker7.Items.Add(n); }
            foreach (String n in listedDevices8) { comboBoxSpeaker8.Items.Add(n); }
        }

        private void OpenProject()
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Title = "Open Project",
                Filter = "Xml Files (*.xml)|*.xml" + "|" +
                                "All Files (*.*)|*.*"
            };
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                lastProjectFilePath = openDialog.FileName;

                data = ProjectData.LoadFromFile(lastProjectFilePath);

                LoadGUIfromData();
                UpdateFormText();
                SaveGUItoData();
                UpdateEnabledDevicesGUI();
            }



        }

        private void SaveNewProject()
        {
            SaveGUItoData();

            SaveFileDialog SaveFileDialog1 = new SaveFileDialog
            {
                RestoreDirectory = true,
                Title = "Save Project",
                DefaultExt = "xml",
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
            };
            //SaveFileDialog1.ShowDialog();

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lastProjectFilePath = SaveFileDialog1.FileName;
                //SAVE TO FILE
                data.SaveToFile(lastProjectFilePath);
                UpdateFormText();

            }

        }

        private void SaveCurrentProject()
        {

            if (lastProjectFilePath != null)
            {
                SaveGUItoData();
                //SAVE TO FILE
                data.SaveToFile(lastProjectFilePath);
                UpdateFormText();
            }
            else
            {
                SaveNewProject();
            }

        }

        private void UpdateFormText()
        {
            if (lastProjectFilePath != null)
            {
                this.Text = ("Vision Automix - " + Path.GetFileNameWithoutExtension(lastProjectFilePath));
            }
            else
            {
                this.Text = ("Vision Automix - New project");
            }
        }

        private void UpdateEnabledDevicesGUI()
        {
            data.UpdateEnabledDevices(); //Sync the data-object first

            //1 Device setup
            txtBoxNameSpeaker1.Enabled = (data.enabledSpeaker[0]);
            txtBoxNameSpeaker2.Enabled = (data.enabledSpeaker[1]);
            txtBoxNameSpeaker3.Enabled = (data.enabledSpeaker[2]);
            txtBoxNameSpeaker4.Enabled = (data.enabledSpeaker[3]);
            txtBoxNameSpeaker5.Enabled = (data.enabledSpeaker[4]);
            txtBoxNameSpeaker6.Enabled = (data.enabledSpeaker[5]);
            txtBoxNameSpeaker7.Enabled = (data.enabledSpeaker[6]);
            txtBoxNameSpeaker8.Enabled = (data.enabledSpeaker[7]);

            //2 Audio Setup
            comboBoxSpeaker1.Enabled = (data.enabledSpeaker[0]);
            numUpDwnChannelSpeaker1.Enabled = (data.enabledSpeaker[0]);
            volumeBar1.Enabled = (data.enabledSpeaker[0]);
            thresholdBar1.Enabled = (data.enabledSpeaker[0]);
            numUpDwnGain1.Enabled = (data.enabledSpeaker[0]);

            comboBoxSpeaker2.Enabled = (data.enabledSpeaker[1]);
            numUpDwnChannelSpeaker2.Enabled = (data.enabledSpeaker[1]);
            volumeBar2.Enabled = (data.enabledSpeaker[1]);
            thresholdBar2.Enabled = (data.enabledSpeaker[1]);
            numUpDwnGain2.Enabled = (data.enabledSpeaker[0]);

            comboBoxSpeaker3.Enabled = (data.enabledSpeaker[2]);
            numUpDwnChannelSpeaker3.Enabled = (data.enabledSpeaker[2]);
            volumeBar3.Enabled = (data.enabledSpeaker[2]);
            thresholdBar3.Enabled = (data.enabledSpeaker[2]);
            numUpDwnGain3.Enabled = (data.enabledSpeaker[0]);

            comboBoxSpeaker4.Enabled = (data.enabledSpeaker[3]);
            numUpDwnChannelSpeaker4.Enabled = (data.enabledSpeaker[3]);
            volumeBar4.Enabled = (data.enabledSpeaker[3]);
            thresholdBar4.Enabled = (data.enabledSpeaker[3]);
            numUpDwnGain4.Enabled = (data.enabledSpeaker[0]);

            comboBoxSpeaker5.Enabled = (data.enabledSpeaker[4]);
            numUpDwnChannelSpeaker5.Enabled = (data.enabledSpeaker[4]);
            volumeBar5.Enabled = (data.enabledSpeaker[4]);
            thresholdBar5.Enabled = (data.enabledSpeaker[4]);
            numUpDwnGain5.Enabled = (data.enabledSpeaker[0]);

            comboBoxSpeaker6.Enabled = (data.enabledSpeaker[5]);
            numUpDwnChannelSpeaker6.Enabled = (data.enabledSpeaker[5]);
            volumeBar6.Enabled = (data.enabledSpeaker[5]);
            thresholdBar6.Enabled = (data.enabledSpeaker[5]);
            numUpDwnGain6.Enabled = (data.enabledSpeaker[0]);

            comboBoxSpeaker7.Enabled = (data.enabledSpeaker[6]);
            numUpDwnChannelSpeaker7.Enabled = (data.enabledSpeaker[6]);
            volumeBar7.Enabled = (data.enabledSpeaker[6]);
            thresholdBar7.Enabled = (data.enabledSpeaker[6]);
            numUpDwnGain7.Enabled = (data.enabledSpeaker[0]);

            comboBoxSpeaker8.Enabled = (data.enabledSpeaker[7]);
            numUpDwnChannelSpeaker8.Enabled = (data.enabledSpeaker[7]);
            volumeBar8.Enabled = (data.enabledSpeaker[7]);
            thresholdBar8.Enabled = (data.enabledSpeaker[7]);
            numUpDwnGain8.Enabled = (data.enabledSpeaker[0]);

            //Audio values reset
            if (data.enabledSpeaker[0] != true) { runData.speaker1Volume = 0; runData.speakersOpen[0] = 0; }
            if (data.enabledSpeaker[1] != true) { runData.speaker2Volume = 0; runData.speakersOpen[1] = 0; }
            if (data.enabledSpeaker[2] != true) { runData.speaker3Volume = 0; runData.speakersOpen[2] = 0; }
            if (data.enabledSpeaker[3] != true) { runData.speaker4Volume = 0; runData.speakersOpen[3] = 0; }
            if (data.enabledSpeaker[4] != true) { runData.speaker5Volume = 0; runData.speakersOpen[4] = 0; }
            if (data.enabledSpeaker[5] != true) { runData.speaker6Volume = 0; runData.speakersOpen[5] = 0; }
            if (data.enabledSpeaker[6] != true) { runData.speaker7Volume = 0; runData.speakersOpen[6] = 0; }
            if (data.enabledSpeaker[7] != true) { runData.speaker8Volume = 0; runData.speakersOpen[7] = 0; }


            //2 PTZ Setup

            prfCam1.Enabled = data.enabledSpeaker[0];
            prfCam2.Enabled = data.enabledSpeaker[1];
            prfCam3.Enabled = data.enabledSpeaker[2];
            prfCam4.Enabled = data.enabledSpeaker[3];
            prfCam5.Enabled = data.enabledSpeaker[4];
            prfCam6.Enabled = data.enabledSpeaker[5];
            prfCam7.Enabled = data.enabledSpeaker[6];
            prfCam8.Enabled = data.enabledSpeaker[7];

            //Static Pos fields
            statPos1.Enabled = data.staticCameras[0];
            statPos2.Enabled = data.staticCameras[1];
            statPos3.Enabled = data.staticCameras[2];
            statPos4.Enabled = data.staticCameras[3];
            statPos5.Enabled = data.staticCameras[4];
            statPos6.Enabled = data.staticCameras[5];
            statPos7.Enabled = data.staticCameras[6];
            statPos8.Enabled = data.staticCameras[7];

            //CAMERA1
            a1pgm.Enabled = data.enabledCamera[0];
            b1pgm.Enabled = data.enabledCamera[0];
            a1prw.Enabled = data.enabledCamera[0];
            b1prw.Enabled = data.enabledCamera[0];

            a10.Enabled = (data.enabledCamera[0] && (data.staticCameras[0] != true));
            b10.Enabled = (data.enabledCamera[0] && (data.staticCameras[0] != true));
            e10.Enabled = (data.enabledCamera[0] && (data.staticCameras[0] != true));

            a11.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[0] && (data.staticCameras[0] != true));
            b11.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[0] && (data.staticCameras[0] != true));
            e11.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[0] && (data.staticCameras[0] != true));

            a12.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[1] && (data.staticCameras[0] != true));
            b12.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[1] && (data.staticCameras[0] != true));
            e12.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[1] && (data.staticCameras[0] != true));

            a13.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[2] && (data.staticCameras[0] != true));
            b13.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[2] && (data.staticCameras[0] != true));
            e13.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[2] && (data.staticCameras[0] != true));

            a14.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[3] && (data.staticCameras[0] != true));
            b14.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[3] && (data.staticCameras[0] != true));
            e14.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[3] && (data.staticCameras[0] != true));

            a15.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[4] && (data.staticCameras[0] != true));
            b15.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[4] && (data.staticCameras[0] != true));
            e15.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[4] && (data.staticCameras[0] != true));

            a16.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[5] && (data.staticCameras[0] != true));
            b16.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[5] && (data.staticCameras[0] != true));
            e16.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[5] && (data.staticCameras[0] != true));

            a17.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[6] && (data.staticCameras[0] != true));
            b17.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[6] && (data.staticCameras[0] != true));
            e17.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[6] && (data.staticCameras[0] != true));

            a18.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[7] && (data.staticCameras[0] != true));
            b18.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[7] && (data.staticCameras[0] != true));
            e18.Enabled = (data.enabledCamera[0] && data.enabledSpeaker[7] && (data.staticCameras[0] != true));

            //CAMERa2
            a2pgm.Enabled = data.enabledCamera[1];
            b2pgm.Enabled = data.enabledCamera[1];
            a2prw.Enabled = data.enabledCamera[1];
            b2prw.Enabled = data.enabledCamera[1];

            prfCam1.Enabled = data.enabledSpeaker[0];

            a20.Enabled = (data.enabledCamera[1] && (data.staticCameras[1] != true));
            b20.Enabled = (data.enabledCamera[1] && (data.staticCameras[1] != true));
            e20.Enabled = (data.enabledCamera[1] && (data.staticCameras[1] != true));

            a21.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[0] && (data.staticCameras[1] != true));
            b21.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[0] && (data.staticCameras[1] != true));
            e21.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[0] && (data.staticCameras[1] != true));

            a22.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[1] && (data.staticCameras[1] != true));
            b22.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[1] && (data.staticCameras[1] != true));
            e22.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[1] && (data.staticCameras[1] != true));

            a23.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[2] && (data.staticCameras[1] != true));
            b23.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[2] && (data.staticCameras[1] != true));
            e23.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[2] && (data.staticCameras[1] != true));

            a24.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[3] && (data.staticCameras[1] != true));
            b24.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[3] && (data.staticCameras[1] != true));
            e24.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[3] && (data.staticCameras[1] != true));

            a25.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[4] && (data.staticCameras[1] != true));
            b25.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[4] && (data.staticCameras[1] != true));
            e25.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[4] && (data.staticCameras[1] != true));

            a26.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[5] && (data.staticCameras[1] != true));
            b26.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[5] && (data.staticCameras[1] != true));
            e26.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[5] && (data.staticCameras[1] != true));

            a27.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[6] && (data.staticCameras[1] != true));
            b27.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[6] && (data.staticCameras[1] != true));
            e27.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[6] && (data.staticCameras[1] != true));

            a28.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[7] && (data.staticCameras[1] != true));
            b28.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[7] && (data.staticCameras[1] != true));
            e28.Enabled = (data.enabledCamera[1] && data.enabledSpeaker[7] && (data.staticCameras[1] != true));

            //CAMERa3
            a3pgm.Enabled = data.enabledCamera[2];
            b3pgm.Enabled = data.enabledCamera[2];
            a3prw.Enabled = data.enabledCamera[2];
            b3prw.Enabled = data.enabledCamera[2];

            a30.Enabled = (data.enabledCamera[2] && (data.staticCameras[2] != true));
            b30.Enabled = (data.enabledCamera[2] && (data.staticCameras[2] != true));
            e30.Enabled = (data.enabledCamera[2] && (data.staticCameras[2] != true));

            a31.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[0] && (data.staticCameras[2] != true));
            b31.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[0] && (data.staticCameras[2] != true));
            e31.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[0] && (data.staticCameras[2] != true));

            a32.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[1] && (data.staticCameras[2] != true));
            b32.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[1] && (data.staticCameras[2] != true));
            e32.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[1] && (data.staticCameras[2] != true));

            a33.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[2] && (data.staticCameras[2] != true));
            b33.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[2] && (data.staticCameras[2] != true));
            e33.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[2] && (data.staticCameras[2] != true));

            a34.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[3] && (data.staticCameras[2] != true));
            b34.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[3] && (data.staticCameras[2] != true));
            e34.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[3] && (data.staticCameras[2] != true));

            a35.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[4] && (data.staticCameras[2] != true));
            b35.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[4] && (data.staticCameras[2] != true));
            e35.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[4] && (data.staticCameras[2] != true));

            a36.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[5] && (data.staticCameras[2] != true));
            b36.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[5] && (data.staticCameras[2] != true));
            e36.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[5] && (data.staticCameras[2] != true));

            a37.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[6] && (data.staticCameras[2] != true));
            b37.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[6] && (data.staticCameras[2] != true));
            e37.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[6] && (data.staticCameras[2] != true));

            a38.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[7] && (data.staticCameras[2] != true));
            b38.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[7] && (data.staticCameras[2] != true));
            e38.Enabled = (data.enabledCamera[2] && data.enabledSpeaker[7] && (data.staticCameras[2] != true));

            //CAMERa4
            a4pgm.Enabled = data.enabledCamera[3];
            b4pgm.Enabled = data.enabledCamera[3];
            a4prw.Enabled = data.enabledCamera[3];
            b4prw.Enabled = data.enabledCamera[3];

            a40.Enabled = (data.enabledCamera[3] && (data.staticCameras[3] != true));
            b40.Enabled = (data.enabledCamera[3] && (data.staticCameras[3] != true));
            e40.Enabled = (data.enabledCamera[3] && (data.staticCameras[3] != true));

            a41.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[0] && (data.staticCameras[3] != true));
            b41.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[0] && (data.staticCameras[3] != true));
            e41.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[0] && (data.staticCameras[3] != true));

            a42.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[1] && (data.staticCameras[3] != true));
            b42.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[1] && (data.staticCameras[3] != true));
            e42.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[1] && (data.staticCameras[3] != true));

            a43.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[2] && (data.staticCameras[3] != true));
            b43.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[2] && (data.staticCameras[3] != true));
            e43.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[2] && (data.staticCameras[3] != true));

            a44.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[3] && (data.staticCameras[3] != true));
            b44.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[3] && (data.staticCameras[3] != true));
            e44.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[3] && (data.staticCameras[3] != true));

            a45.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[4] && (data.staticCameras[3] != true));
            b45.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[4] && (data.staticCameras[3] != true));
            e45.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[4] && (data.staticCameras[3] != true));

            a46.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[5] && (data.staticCameras[3] != true));
            b46.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[5] && (data.staticCameras[3] != true));
            e46.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[5] && (data.staticCameras[3] != true));

            a47.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[6] && (data.staticCameras[3] != true));
            b47.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[6] && (data.staticCameras[3] != true));
            e47.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[6] && (data.staticCameras[3] != true));

            a48.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[7] && (data.staticCameras[3] != true));
            b48.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[7] && (data.staticCameras[3] != true));
            e48.Enabled = (data.enabledCamera[3] && data.enabledSpeaker[7] && (data.staticCameras[3] != true));

            //CAMERa5
            a5pgm.Enabled = data.enabledCamera[4];
            b5pgm.Enabled = data.enabledCamera[4];
            a5prw.Enabled = data.enabledCamera[4];
            b5prw.Enabled = data.enabledCamera[4];

            a50.Enabled = (data.enabledCamera[4] && (data.staticCameras[4] != true));
            b50.Enabled = (data.enabledCamera[4] && (data.staticCameras[4] != true));
            e50.Enabled = (data.enabledCamera[4] && (data.staticCameras[4] != true));

            a51.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[0] && (data.staticCameras[4] != true));
            b51.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[0] && (data.staticCameras[4] != true));
            e51.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[0] && (data.staticCameras[4] != true));

            a52.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[1] && (data.staticCameras[4] != true));
            b52.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[1] && (data.staticCameras[4] != true));
            e52.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[1] && (data.staticCameras[4] != true));

            a53.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[2] && (data.staticCameras[4] != true));
            b53.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[2] && (data.staticCameras[4] != true));
            e53.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[2] && (data.staticCameras[4] != true));

            a54.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[3] && (data.staticCameras[4] != true));
            b54.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[3] && (data.staticCameras[4] != true));
            e54.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[3] && (data.staticCameras[4] != true));

            a55.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[4] && (data.staticCameras[4] != true));
            b55.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[4] && (data.staticCameras[4] != true));
            e55.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[4] && (data.staticCameras[4] != true));

            a56.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[5] && (data.staticCameras[4] != true));
            b56.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[5] && (data.staticCameras[4] != true));
            e56.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[5] && (data.staticCameras[4] != true));

            a57.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[6] && (data.staticCameras[4] != true));
            b57.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[6] && (data.staticCameras[4] != true));
            e57.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[6] && (data.staticCameras[4] != true));

            a58.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[7] && (data.staticCameras[4] != true));
            b58.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[7] && (data.staticCameras[4] != true));
            e58.Enabled = (data.enabledCamera[4] && data.enabledSpeaker[7] && (data.staticCameras[4] != true));

            //CAMERa6
            a6pgm.Enabled = data.enabledCamera[5];
            b6pgm.Enabled = data.enabledCamera[5];
            a6prw.Enabled = data.enabledCamera[5];
            b6prw.Enabled = data.enabledCamera[5];

            a60.Enabled = (data.enabledCamera[5] && (data.staticCameras[5] != true));
            b60.Enabled = (data.enabledCamera[5] && (data.staticCameras[5] != true));
            e60.Enabled = (data.enabledCamera[5] && (data.staticCameras[5] != true));

            a61.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[0] && (data.staticCameras[5] != true));
            b61.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[0] && (data.staticCameras[5] != true));
            e61.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[0] && (data.staticCameras[5] != true));

            a62.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[1] && (data.staticCameras[5] != true));
            b62.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[1] && (data.staticCameras[5] != true));
            e62.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[1] && (data.staticCameras[5] != true));

            a63.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[2] && (data.staticCameras[5] != true));
            b63.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[2] && (data.staticCameras[5] != true));
            e63.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[2] && (data.staticCameras[5] != true));

            a64.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[3] && (data.staticCameras[5] != true));
            b64.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[3] && (data.staticCameras[5] != true));
            e64.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[3] && (data.staticCameras[5] != true));

            a65.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[4] && (data.staticCameras[5] != true));
            b65.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[4] && (data.staticCameras[5] != true));
            e65.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[4] && (data.staticCameras[5] != true));

            a66.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[5] && (data.staticCameras[5] != true));
            b66.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[5] && (data.staticCameras[5] != true));
            e66.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[5] && (data.staticCameras[5] != true));

            a67.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[6] && (data.staticCameras[5] != true));
            b67.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[6] && (data.staticCameras[5] != true));
            e67.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[6] && (data.staticCameras[5] != true));

            a68.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[7] && (data.staticCameras[5] != true));
            b68.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[7] && (data.staticCameras[5] != true));
            e68.Enabled = (data.enabledCamera[5] && data.enabledSpeaker[7] && (data.staticCameras[5] != true));

            //CAMERa7
            a7pgm.Enabled = data.enabledCamera[6];
            b7pgm.Enabled = data.enabledCamera[6];
            a7prw.Enabled = data.enabledCamera[6];
            b7prw.Enabled = data.enabledCamera[6];

            a70.Enabled = (data.enabledCamera[6] && (data.staticCameras[6] != true));
            b70.Enabled = (data.enabledCamera[6] && (data.staticCameras[6] != true));
            e70.Enabled = (data.enabledCamera[6] && (data.staticCameras[6] != true));

            a71.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[0] && (data.staticCameras[6] != true));
            b71.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[0] && (data.staticCameras[6] != true));
            e71.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[0] && (data.staticCameras[6] != true));

            a72.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[1] && (data.staticCameras[6] != true));
            b72.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[1] && (data.staticCameras[6] != true));
            e72.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[1] && (data.staticCameras[6] != true));

            a73.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[2] && (data.staticCameras[6] != true));
            b73.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[2] && (data.staticCameras[6] != true));
            e73.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[2] && (data.staticCameras[6] != true));

            a74.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[3] && (data.staticCameras[6] != true));
            b74.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[3] && (data.staticCameras[6] != true));
            e74.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[3] && (data.staticCameras[6] != true));

            a75.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[4] && (data.staticCameras[6] != true));
            b75.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[4] && (data.staticCameras[6] != true));
            e75.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[4] && (data.staticCameras[6] != true));

            a76.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[5] && (data.staticCameras[6] != true));
            b76.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[5] && (data.staticCameras[6] != true));
            e76.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[5] && (data.staticCameras[6] != true));

            a77.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[6] && (data.staticCameras[6] != true));
            b77.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[6] && (data.staticCameras[6] != true));
            e77.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[6] && (data.staticCameras[6] != true));

            a78.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[7] && (data.staticCameras[6] != true));
            b78.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[7] && (data.staticCameras[6] != true));
            e78.Enabled = (data.enabledCamera[6] && data.enabledSpeaker[7] && (data.staticCameras[6] != true));

            //CAMERa8
            a8pgm.Enabled = data.enabledCamera[7];
            b8pgm.Enabled = data.enabledCamera[7];
            a8prw.Enabled = data.enabledCamera[7];
            b8prw.Enabled = data.enabledCamera[7];

            a80.Enabled = (data.enabledCamera[7] && (data.staticCameras[7] != true));
            b80.Enabled = (data.enabledCamera[7] && (data.staticCameras[7] != true));
            e80.Enabled = (data.enabledCamera[7] && (data.staticCameras[7] != true));

            a81.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[0] && (data.staticCameras[7] != true));
            b81.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[0] && (data.staticCameras[7] != true));
            e81.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[0] && (data.staticCameras[7] != true));

            a82.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[1] && (data.staticCameras[7] != true));
            b82.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[1] && (data.staticCameras[7] != true));
            e82.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[1] && (data.staticCameras[7] != true));

            a83.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[2] && (data.staticCameras[7] != true));
            b83.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[2] && (data.staticCameras[7] != true));
            e83.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[2] && (data.staticCameras[7] != true));

            a84.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[3] && (data.staticCameras[7] != true));
            b84.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[3] && (data.staticCameras[7] != true));
            e84.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[3] && (data.staticCameras[7] != true));

            a85.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[4] && (data.staticCameras[7] != true));
            b85.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[4] && (data.staticCameras[7] != true));
            e85.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[4] && (data.staticCameras[7] != true));

            a86.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[5] && (data.staticCameras[7] != true));
            b86.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[5] && (data.staticCameras[7] != true));
            e86.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[5] && (data.staticCameras[7] != true));

            a87.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[6] && (data.staticCameras[7] != true));
            b87.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[6] && (data.staticCameras[7] != true));
            e87.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[6] && (data.staticCameras[7] != true));

            a88.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[7] && (data.staticCameras[7] != true));
            b88.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[7] && (data.staticCameras[7] != true));
            e88.Enabled = (data.enabledCamera[7] && data.enabledSpeaker[7] && (data.staticCameras[7] != true));


            // 4 - Live monitor
            labelSpeakerBoxSpeaker1.Visible = (data.enabledSpeaker[0]);
            labelSpeakerBoxSpeaker2.Visible = (data.enabledSpeaker[1]);
            labelSpeakerBoxSpeaker3.Visible = (data.enabledSpeaker[2]);
            labelSpeakerBoxSpeaker4.Visible = (data.enabledSpeaker[3]);
            labelSpeakerBoxSpeaker5.Visible = (data.enabledSpeaker[4]);
            labelSpeakerBoxSpeaker6.Visible = (data.enabledSpeaker[5]);
            labelSpeakerBoxSpeaker7.Visible = (data.enabledSpeaker[6]);
            labelSpeakerBoxSpeaker8.Visible = (data.enabledSpeaker[7]);

            camBoxLabelCam1.Visible = (data.enabledCamera[0]);
            camBoxLabelCam2.Visible = (data.enabledCamera[1]);
            camBoxLabelCam3.Visible = (data.enabledCamera[2]);
            camBoxLabelCam4.Visible = (data.enabledCamera[3]);
            camBoxLabelCam5.Visible = (data.enabledCamera[4]);
            camBoxLabelCam6.Visible = (data.enabledCamera[5]);
            camBoxLabelCam7.Visible = (data.enabledCamera[6]);
            camBoxLabelCam8.Visible = (data.enabledCamera[7]);

            labelCameraBoxStatus1.Visible = (data.enabledCamera[0]);
            labelCameraBoxStatus2.Visible = (data.enabledCamera[1]);
            labelCameraBoxStatus3.Visible = (data.enabledCamera[2]);
            labelCameraBoxStatus4.Visible = (data.enabledCamera[3]);
            labelCameraBoxStatus5.Visible = (data.enabledCamera[4]);
            labelCameraBoxStatus6.Visible = (data.enabledCamera[5]);
            labelCameraBoxStatus7.Visible = (data.enabledCamera[6]);
            labelCameraBoxStatus8.Visible = (data.enabledCamera[7]);

            labelCameraBoxPosition1.Visible = (data.enabledCamera[0]);
            labelCameraBoxPosition2.Visible = (data.enabledCamera[1]);
            labelCameraBoxPosition3.Visible = (data.enabledCamera[2]);
            labelCameraBoxPosition4.Visible = (data.enabledCamera[3]);
            labelCameraBoxPosition5.Visible = (data.enabledCamera[4]);
            labelCameraBoxPosition6.Visible = (data.enabledCamera[5]);
            labelCameraBoxPosition7.Visible = (data.enabledCamera[6]);
            labelCameraBoxPosition8.Visible = (data.enabledCamera[7]);
        }

        private void SaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentProject();
        }

        private void SaveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveNewProject();
        }

        private void OpenProToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void ErrorMessage(string message)
        {
            Console.WriteLine(message);
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void TickAudio_Tick(object sender, EventArgs e)
        {
            //GET AUDIO

            if (data.enabledSpeaker[0]) { runData.speaker1Volume = audio1.GetVolume(); }
            if (data.enabledSpeaker[1]) { runData.speaker2Volume = audio2.GetVolume(); }
            if (data.enabledSpeaker[2]) { runData.speaker3Volume = audio3.GetVolume(); }
            if (data.enabledSpeaker[3]) { runData.speaker4Volume = audio4.GetVolume(); }
            if (data.enabledSpeaker[4]) { runData.speaker5Volume = audio5.GetVolume(); }
            if (data.enabledSpeaker[5]) { runData.speaker6Volume = audio6.GetVolume(); }
            if (data.enabledSpeaker[6]) { runData.speaker7Volume = audio7.GetVolume(); }
            if (data.enabledSpeaker[7]) { runData.speaker8Volume = audio8.GetVolume(); }

            if (data.enabledSpeaker[0]) { runData.speakersOpen[0] = Convert.ToInt32(audio1.GetOpen()); }
            if (data.enabledSpeaker[1]) { runData.speakersOpen[1] = Convert.ToInt32(audio2.GetOpen()); }
            if (data.enabledSpeaker[2]) { runData.speakersOpen[2] = Convert.ToInt32(audio3.GetOpen()); }
            if (data.enabledSpeaker[3]) { runData.speakersOpen[3] = Convert.ToInt32(audio4.GetOpen()); }
            if (data.enabledSpeaker[4]) { runData.speakersOpen[4] = Convert.ToInt32(audio5.GetOpen()); }
            if (data.enabledSpeaker[5]) { runData.speakersOpen[5] = Convert.ToInt32(audio6.GetOpen()); }
            if (data.enabledSpeaker[6]) { runData.speakersOpen[6] = Convert.ToInt32(audio7.GetOpen()); }
            if (data.enabledSpeaker[7]) { runData.speakersOpen[7] = Convert.ToInt32(audio8.GetOpen()); }

            //Console.WriteLine(runData.speakersOpen[0] + " " + runData.speakersOpen[1] + " " + runData.speakersOpen[2] + " " + runData.speakersOpen[3] + " " + runData.speakersOpen[4] + " " + runData.speakersOpen[5] + " " + runData.speakersOpen[6] + " " + runData.speakersOpen[7]);
        }




        private void VotelengthTrackBar_Scroll(object sender, EventArgs e)
        {
            votelengthLabel.Text = votelengthTrackBar.Value.ToString();
        }

        private void CallMixPreset(string presetName)
        {
            ///*
            ///This is used to call presets from the presets-class.
            ///This requires the GUI to do some functions aswell after calling the preset, wich this function handles

            bool presetLoadedSucessfully = Presets.LoadPreset(data, presetName);
            if (presetLoadedSucessfully == true) { LoadGUIfromData(); }

        }

        private void InstantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallMixPreset("instant");
        }

        private void FastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallMixPreset("fast");
        }

        private void NormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallMixPreset("normal");
        }

        private void CarefulToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallMixPreset("careful");
        }

        private void SlowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallMixPreset("slow");
        }

        private void CompanionToggleButton_Click(object sender, EventArgs e)
        {
            runData.companionOutputEnabled = (runData.companionOutputEnabled ? false : true);
            Console.WriteLine(runData.companionOutputEnabled ? "Companion output enabled" : "Companion output disabled");
        }

        private void ResetNamesToDefaultBtn_Click(object sender, EventArgs e)
        {
            txtBoxNameSpeaker1.Text = "Speaker 1";
            txtBoxNameSpeaker2.Text = "Speaker 2";
            txtBoxNameSpeaker3.Text = "Speaker 3";
            txtBoxNameSpeaker4.Text = "Speaker 4";
            txtBoxNameSpeaker5.Text = "Speaker 5";
            txtBoxNameSpeaker6.Text = "Speaker 6";
            txtBoxNameSpeaker7.Text = "Speaker 7";
            txtBoxNameSpeaker8.Text = "Speaker 8";
            unsavedChanges(true);
        }




        private void unsavedChanges(bool exists)
        {
            if (exists == true) { unchangedSettingsTimer.Enabled = true; applyButton.Enabled = true; }
            else { unchangedSettingsTimer.Enabled = false; applyButton.Enabled = false; }
        }

        private void unchangedSettingsTimer_Tick(object sender, EventArgs e)
        {
            applyButton.Enabled = true;
            applyButton.ForeColor = ((applyButton.ForeColor == Color.LightGray) ? Color.Black : Color.LightGray);
        }

        private void stat1_MouseUp_1(object sender, MouseEventArgs e)
        {
            unsavedChanges(true);
        }

        private void b8prw_ValueChanged(object sender, EventArgs e)
        {
            unsavedChanges(true);
        }

        private void comboBoxSpeaker2_SelectedIndexChanged(object sender, EventArgs e)
        {
            unsavedChanges(true);
        }



        private void Form1_Move(object sender, EventArgs e)
        {
            //Pause drawing gui
            tickGUIupdate.Enabled = false;
            formMovingTimer.Enabled = true;

        }

        private void formMovingTimer_Tick(object sender, EventArgs e)
        {
            //Start drawing gui again
            tickGUIupdate.Enabled = true;
            formMovingTimer.Enabled = false;
        }

        private void mixonePushData_Tick(object sender, EventArgs e)
        {
            mixone.PushProjectdata(data);
            mixone.PushRuntimeData(runData);
        }
    }
}
