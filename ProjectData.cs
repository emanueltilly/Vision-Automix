using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Vision_Automix
{
    
    public class ProjectData
    {

        //ENABLED SPEAKERS AND CAMERAS

        public bool[] enabledSpeaker = new bool[8] { true, true, true, true, true, true, true, true };
        public bool[] enabledCamera = new bool[8] { true, true, true, true, true, true, true, true };

        //SPEAKER NAMES
        public string[] speakerNames = new string[8] { "Speaker 1", "Speaker 2", "Speaker 3", "Speaker 4", "Speaker 5", "Speaker 6", "Speaker 7", "Speaker 8" };

        //1 DEVICE SETUP
        public int speakers = 8;
        public int cameras = 8;
        public int[] companionCon = { 127, 0, 0, 1, 51235 }; //IP ADDR + PORT
        public int cameraMoveTime = 1;

        public int minimumShotTime = 2;
        public int maximumQuietTime = 2;
        public bool enableCutToWideOnQuiet = true;
        public bool enablePRWbusControl = true;

        public int voteLength = 40;


        //2 AUDIO SETUP
        //Speaker 1
        public int audioDevice1 = 0;
        public int audioChannel1 = 1;
        public int audioThreshold1 = 50;
        public int audioGain1 = 0;
        //Speaker 2
        public int audioDevice2 = 0;
        public int audioChannel2 = 1;
        public int audioThreshold2 = 50;
        public int audioGain2 = 0;
        //Speaker 3
        public int audioDevice3 = 0;
        public int audioChannel3 = 1;
        public int audioThreshold3 = 50;
        public int audioGain3 = 0;
        //Speaker 4
        public int audioDevice4 = 0;
        public int audioChannel4 = 1;
        public int audioThreshold4 = 50;
        public int audioGain4 = 0;
        //Speaker 5
        public int audioDevice5 = 0;
        public int audioChannel5 = 1;
        public int audioThreshold5 = 50;
        public int audioGain5 = 0;
        //Speaker 6
        public int audioDevice6 = 0;
        public int audioChannel6 = 1;
        public int audioThreshold6 = 50;
        public int audioGain6 = 0;
        //Speaker 7
        public int audioDevice7 = 0;
        public int audioChannel7 = 1;
        public int audioThreshold7 = 50;
        public int audioGain7 = 0;
        //Speaker 8
        public int audioDevice8 = 0;
        public int audioChannel8 = 1;
        public int audioThreshold8 = 50;
        public int audioGain8 = 0;


        //3 PTZ SETUP


        //Static settings
        public bool[] staticCameras = new bool[8] { false, false, false, false, false, false, false, false };
        public int[] staticCameraPositions = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        

        //Position-settings (Page / Bank / Enabled)
        public int[] c1p0 = new int[] { 1, 1, 0 };
        public int[] c2p0 = new int[] { 1, 1, 0 };
        public int[] c3p0 = new int[] { 1, 1, 0 };
        public int[] c4p0 = new int[] { 1, 1, 0 };
        public int[] c5p0 = new int[] { 1, 1, 0 };
        public int[] c6p0 = new int[] { 1, 1, 0 };
        public int[] c7p0 = new int[] { 1, 1, 0 };
        public int[] c8p0 = new int[] { 1, 1, 0 };

        public int[] c1p1 = new int[] { 1, 1, 0 };
        public int[] c2p1 = new int[] { 1, 1, 0 };
        public int[] c3p1 = new int[] { 1, 1, 0 };
        public int[] c4p1 = new int[] { 1, 1, 0 };
        public int[] c5p1 = new int[] { 1, 1, 0 };
        public int[] c6p1 = new int[] { 1, 1, 0 };
        public int[] c7p1 = new int[] { 1, 1, 0 };
        public int[] c8p1 = new int[] { 1, 1, 0 };

        public int[] c1p2 = new int[] { 1, 1, 0 };
        public int[] c2p2 = new int[] { 1, 1, 0 };
        public int[] c3p2 = new int[] { 1, 1, 0 };
        public int[] c4p2 = new int[] { 1, 1, 0 };
        public int[] c5p2 = new int[] { 1, 1, 0 };
        public int[] c6p2 = new int[] { 1, 1, 0 };
        public int[] c7p2 = new int[] { 1, 1, 0 };
        public int[] c8p2 = new int[] { 1, 1, 0 };

        public int[] c1p3 = new int[] { 1, 1, 0 };
        public int[] c2p3 = new int[] { 1, 1, 0 };
        public int[] c3p3 = new int[] { 1, 1, 0 };
        public int[] c4p3 = new int[] { 1, 1, 0 };
        public int[] c5p3 = new int[] { 1, 1, 0 };
        public int[] c6p3 = new int[] { 1, 1, 0 };
        public int[] c7p3 = new int[] { 1, 1, 0 };
        public int[] c8p3 = new int[] { 1, 1, 0 };

        public int[] c1p4 = new int[] { 1, 1, 0 };
        public int[] c2p4 = new int[] { 1, 1, 0 };
        public int[] c3p4 = new int[] { 1, 1, 0 };
        public int[] c4p4 = new int[] { 1, 1, 0 };
        public int[] c5p4 = new int[] { 1, 1, 0 };
        public int[] c6p4 = new int[] { 1, 1, 0 };
        public int[] c7p4 = new int[] { 1, 1, 0 };
        public int[] c8p4 = new int[] { 1, 1, 0 };

        public int[] c1p5 = new int[] { 1, 1, 0 };
        public int[] c2p5 = new int[] { 1, 1, 0 };
        public int[] c3p5 = new int[] { 1, 1, 0 };
        public int[] c4p5 = new int[] { 1, 1, 0 };
        public int[] c5p5 = new int[] { 1, 1, 0 };
        public int[] c6p5 = new int[] { 1, 1, 0 };
        public int[] c7p5 = new int[] { 1, 1, 0 };
        public int[] c8p5 = new int[] { 1, 1, 0 };

        public int[] c1p6 = new int[] { 1, 1, 0 };
        public int[] c2p6 = new int[] { 1, 1, 0 };
        public int[] c3p6 = new int[] { 1, 1, 0 };
        public int[] c4p6 = new int[] { 1, 1, 0 };
        public int[] c5p6 = new int[] { 1, 1, 0 };
        public int[] c6p6 = new int[] { 1, 1, 0 };
        public int[] c7p6 = new int[] { 1, 1, 0 };
        public int[] c8p6 = new int[] { 1, 1, 0 };

        public int[] c1p7 = new int[] { 1, 1, 0 };
        public int[] c2p7 = new int[] { 1, 1, 0 };
        public int[] c3p7 = new int[] { 1, 1, 0 };
        public int[] c4p7 = new int[] { 1, 1, 0 };
        public int[] c5p7 = new int[] { 1, 1, 0 };
        public int[] c6p7 = new int[] { 1, 1, 0 };
        public int[] c7p7 = new int[] { 1, 1, 0 };
        public int[] c8p7 = new int[] { 1, 1, 0 };

        public int[] c1p8 = new int[] { 1, 1, 0 };
        public int[] c2p8 = new int[] { 1, 1, 0 };
        public int[] c3p8 = new int[] { 1, 1, 0 };
        public int[] c4p8 = new int[] { 1, 1, 0 };
        public int[] c5p8 = new int[] { 1, 1, 0 };
        public int[] c6p8 = new int[] { 1, 1, 0 };
        public int[] c7p8 = new int[] { 1, 1, 0 };
        public int[] c8p8 = new int[] { 1, 1, 0 };

        //Program and Preview-buttons
        public int[] c1pgm = new int[] { 1, 1 };
        public int[] c2pgm = new int[] { 1, 1 };
        public int[] c3pgm = new int[] { 1, 1 };
        public int[] c4pgm = new int[] { 1, 1 };
        public int[] c5pgm = new int[] { 1, 1 };
        public int[] c6pgm = new int[] { 1, 1 };
        public int[] c7pgm = new int[] { 1, 1 };
        public int[] c8pgm = new int[] { 1, 1 };

        public int[] c1prw = new int[] { 1, 1 };
        public int[] c2prw = new int[] { 1, 1 };
        public int[] c3prw = new int[] { 1, 1 };
        public int[] c4prw = new int[] { 1, 1 };
        public int[] c5prw = new int[] { 1, 1 };
        public int[] c6prw = new int[] { 1, 1 };
        public int[] c7prw = new int[] { 1, 1 };
        public int[] c8prw = new int[] { 1, 1 };

        //Preferred Cameras
        public int[] prefPos = new int[] { 1,1,1,1,1,1,1,1,1 };


        //4 LIVE MONITOR


        public void Initialize()
        {
            //Populate arrays with default value
            UpdateEnabledDevices();
            
        }    

        public void UpdateEnabledDevices()
        {
            //Set enabled speakers
            enabledSpeaker[0] = (speakers >= 1);
            enabledSpeaker[1] = (speakers >= 2);
            enabledSpeaker[2] = (speakers >= 3);
            enabledSpeaker[3] = (speakers >= 4);
            enabledSpeaker[4] = (speakers >= 5);
            enabledSpeaker[5] = (speakers >= 6);
            enabledSpeaker[6] = (speakers >= 7);
            enabledSpeaker[7] = (speakers >= 8);
            
            //Set enabled cameras
            enabledCamera[0] = (cameras >= 1);
            enabledCamera[1] = (cameras >= 2);
            enabledCamera[2] = (cameras >= 3);
            enabledCamera[3] = (cameras >= 4);
            enabledCamera[4] = (cameras >= 5);
            enabledCamera[5] = (cameras >= 6);
            enabledCamera[6] = (cameras >= 7);
            enabledCamera[7] = (cameras >= 8);
        }

        public void SaveToFile(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer XML = new XmlSerializer(typeof(ProjectData));
                XML.Serialize(stream, this);
            }
        }

        public static ProjectData LoadFromFile(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                XmlSerializer XML = new XmlSerializer(typeof(ProjectData));
                return (ProjectData)XML.Deserialize(stream);
            }
        }



    }

 


}
