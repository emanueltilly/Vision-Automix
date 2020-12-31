using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Vision_Automix
{
    class AudioInput
    {
        MMDeviceEnumerator masterEnumerator = new MMDeviceEnumerator();


        private WaveIn recorder;

        private int globalInterfaceIndex = 0;

        private int liveLevel = 0;
        private int liveThreshold = 50;
        private double gain = 1.0;


        private List<String> listedDevices;
        private List<MMDevice> deviceList;
        private int selectedDevice = 0;
        private int selectedChannel = 0;

        private bool messageFlag = false; //Flag for showing error message to stop flooding


        public List<String> Initialize(int interfaceIndex)
        {
            globalInterfaceIndex = interfaceIndex;
            Console.WriteLine("");
            Console.WriteLine("Initializing Audio Input " + globalInterfaceIndex);

            recorder = new WaveIn();
            recorder.StartRecording();

            var devices = masterEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);


            //Return list of devices to add to GUI list
            List<String> tempList = new List<String>();
            List<MMDevice> tempMMlist = new List<MMDevice>();

            foreach (MMDevice n in devices)
            {
                tempList.Add(n.ToString());
                tempMMlist.Add(n);
                Console.WriteLine("Adding to devicelist: " + n.ToString());
            }
            listedDevices = tempList;   //Add to object list
            deviceList = tempMMlist;


            return tempList;


        }


        //SET
        public void SetDevice(int deviceIndex, int channelIndex)
        {
            selectedDevice = deviceIndex;
            selectedChannel = channelIndex - 1;
            Console.WriteLine("Audio Device " + globalInterfaceIndex + " updated.");
            messageFlag = false;    //Reset error-message flag

        }
        public void SetThreshold(int requestedThreshold)
        {
            liveThreshold = requestedThreshold;
            messageFlag = false;    //Reset error-message flag
        }
        public void SetGain (double newGain)
        {
            gain = (1+(newGain*0.04));
            Console.WriteLine("New gain: " + gain);
        }

        //GET
        public int GetVolume()
        {
            try
            {
                //Get peak level from audio device
                var singledevice = (MMDevice)deviceList[selectedDevice];
                liveLevel = (int)(singledevice.AudioMeterInformation.PeakValues[selectedChannel] * 100);

                //Gain and clipping
                liveLevel = (int)((double)liveLevel * gain);
                if (liveLevel > 100 ) { liveLevel = 100; }

                return liveLevel;
            }
            catch
            {

                ShowErrorMessage(("AUDIO IN for Speaker " + globalInterfaceIndex), "Error getting volume from device for speaker" + globalInterfaceIndex + ". Channel out of range?");
                
                return 0;
            }
        }
        public bool GetOpen()
        {
            
            return (liveLevel >= liveThreshold);
        }
        public double GetGain()
        {
            return gain;
        }

        private void ShowErrorMessage(string title, string message)
        {
            if (messageFlag == false)
            {
                Console.WriteLine(message);
                messageFlag = true;
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


    }
}
