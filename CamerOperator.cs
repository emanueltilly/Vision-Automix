using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Vision_Automix
{
    class CamerOperator
    {
        Companion companion = new Companion();

        private int speakerHistory = 99; //99 only to trigger change on init

        public void Initialize()
        {
            speakerHistory = 99;
        }

        public void Tick(ProjectData data, RuntimeData runData)
        {
            if (runData.nextSpeaker != speakerHistory)
            {
                speakerHistory = runData.nextSpeaker;       //Update local history

                int prefCam = data.prefPos[runData.nextSpeaker];       //Get preferred camer for next speaker

                //Check if preferred camera is available
                bool prefCAMposEnabled = GetCameraPositionEnabled(runData, prefCam, runData.nextSpeaker);
                if (runData.cameraBusy[(prefCam-1)] != true && runData.cameraPGM != prefCam && GetCameraEnabled(data, prefCam) == true && prefCAMposEnabled == true)
                {
                    CallPosition(data, runData, prefCam, runData.nextSpeaker);      //Move preferred camera to new position
                    //Flag for switcher to load camera on Preview bus
                    runData.changePRWcam = prefCam;
                    runData.changePRW = true;
                    
                }
                else //If preferred camera is unavailable
                {
                    //Gather available cameras for speaker
                    bool[] availableCameras;
                    availableCameras = GetCamerasWithPresetForPosition(data, runData.nextSpeaker);

                    //Check if available cameras are busy, if not call position
                    int loopcounter = 0;
                    bool breakLoop = false;
                    while (loopcounter < 8 && breakLoop == false)
                    {
                        if (availableCameras[loopcounter] == true && runData.cameraBusy[loopcounter] == false && GetCameraEnabled(data, (loopcounter + 1)) == true)
                        {
                            CallPosition(data, runData, (loopcounter + 1), runData.nextSpeaker);
                            //Flag for switcher to load camera on Preview bus
                            runData.changePRWcam = (loopcounter + 1);
                            runData.changePRW = true;
                            breakLoop = true;
                        }
                        
                        loopcounter++;
                    }
                    


                }
            }
        }


 


        //Full sequence for moving a camera to a new position
        private void CallPosition(ProjectData data, RuntimeData runData, int cam, int pos)
        {
            int cameraCurrentPosition = runData.cameraPosition[(cam - 1)];      //Get the current position of the camera

            if (cameraCurrentPosition != pos)
            {
                Console.WriteLine("Calling position " + pos + " on camera " + cam);
                //Init variables
                int page = 1;
                int bank = 1;

                //Switch gets correct pages and banks from data-object
                switch (cam)
                {
                    case 1:
                        switch (pos)
                        {
                            case 0:
                                page = data.c1p0[0];
                                bank = data.c1p0[1];
                                break;
                            case 1:
                                page = data.c1p1[0];
                                bank = data.c1p1[1];
                                break;
                            case 2:
                                page = data.c1p2[0];
                                bank = data.c1p2[1];
                                break;
                            case 3:
                                page = data.c1p3[0];
                                bank = data.c1p3[1];
                                break;
                            case 4:
                                page = data.c1p4[0];
                                bank = data.c1p4[1];
                                break;
                            case 5:
                                page = data.c1p5[0];
                                bank = data.c1p5[1];
                                break;
                            case 6:
                                page = data.c1p6[0];
                                bank = data.c1p6[1];
                                break;
                            case 7:
                                page = data.c1p7[0];
                                bank = data.c1p7[1];
                                break;
                            case 8:
                                page = data.c1p8[0];
                                bank = data.c1p8[1];
                                break;
                        }
                        break;
                    case 2:
                        switch (pos)
                        {
                            case 0:
                                page = data.c2p0[0];
                                bank = data.c2p0[1];
                                break;
                            case 1:
                                page = data.c2p1[0];
                                bank = data.c2p1[1];
                                break;
                            case 2:
                                page = data.c2p2[0];
                                bank = data.c2p2[1];
                                break;
                            case 3:
                                page = data.c2p3[0];
                                bank = data.c2p3[1];
                                break;
                            case 4:
                                page = data.c2p4[0];
                                bank = data.c2p4[1];
                                break;
                            case 5:
                                page = data.c2p5[0];
                                bank = data.c2p5[1];
                                break;
                            case 6:
                                page = data.c2p6[0];
                                bank = data.c2p6[1];
                                break;
                            case 7:
                                page = data.c2p7[0];
                                bank = data.c2p7[1];
                                break;
                            case 8:
                                page = data.c2p8[0];
                                bank = data.c2p8[1];
                                break;
                        }
                        break;
                    case 3:
                        switch (pos)
                        {
                            case 0:
                                page = data.c3p0[0];
                                bank = data.c3p0[1];
                                break;
                            case 1:
                                page = data.c3p1[0];
                                bank = data.c3p1[1];
                                break;
                            case 2:
                                page = data.c3p2[0];
                                bank = data.c3p2[1];
                                break;
                            case 3:
                                page = data.c3p3[0];
                                bank = data.c3p3[1];
                                break;
                            case 4:
                                page = data.c3p4[0];
                                bank = data.c3p4[1];
                                break;
                            case 5:
                                page = data.c3p5[0];
                                bank = data.c3p5[1];
                                break;
                            case 6:
                                page = data.c3p6[0];
                                bank = data.c3p6[1];
                                break;
                            case 7:
                                page = data.c3p7[0];
                                bank = data.c3p7[1];
                                break;
                            case 8:
                                page = data.c3p8[0];
                                bank = data.c3p8[1];
                                break;
                        }
                        break;
                    case 4:
                        switch (pos)
                        {
                            case 0:
                                page = data.c4p0[0];
                                bank = data.c4p0[1];
                                break;
                            case 1:
                                page = data.c4p1[0];
                                bank = data.c4p1[1];
                                break;
                            case 2:
                                page = data.c4p2[0];
                                bank = data.c4p2[1];
                                break;
                            case 3:
                                page = data.c4p3[0];
                                bank = data.c4p3[1];
                                break;
                            case 4:
                                page = data.c4p4[0];
                                bank = data.c4p4[1];
                                break;
                            case 5:
                                page = data.c4p5[0];
                                bank = data.c4p5[1];
                                break;
                            case 6:
                                page = data.c4p6[0];
                                bank = data.c4p6[1];
                                break;
                            case 7:
                                page = data.c4p7[0];
                                bank = data.c4p7[1];
                                break;
                            case 8:
                                page = data.c4p8[0];
                                bank = data.c4p8[1];
                                break;
                        }
                        break;
                    case 5:
                        switch (pos)
                        {
                            case 0:
                                page = data.c5p0[0];
                                bank = data.c5p0[1];
                                break;
                            case 1:
                                page = data.c5p1[0];
                                bank = data.c5p1[1];
                                break;
                            case 2:
                                page = data.c5p2[0];
                                bank = data.c5p2[1];
                                break;
                            case 3:
                                page = data.c5p3[0];
                                bank = data.c5p3[1];
                                break;
                            case 4:
                                page = data.c5p4[0];
                                bank = data.c5p4[1];
                                break;
                            case 5:
                                page = data.c5p5[0];
                                bank = data.c5p5[1];
                                break;
                            case 6:
                                page = data.c5p6[0];
                                bank = data.c5p6[1];
                                break;
                            case 7:
                                page = data.c5p7[0];
                                bank = data.c5p7[1];
                                break;
                            case 8:
                                page = data.c5p8[0];
                                bank = data.c5p8[1];
                                break;
                        }
                        break;
                    case 6:
                        switch (pos)
                        {
                            case 0:
                                page = data.c6p0[0];
                                bank = data.c6p0[1];
                                break;
                            case 1:
                                page = data.c6p1[0];
                                bank = data.c6p1[1];
                                break;
                            case 2:
                                page = data.c6p2[0];
                                bank = data.c6p2[1];
                                break;
                            case 3:
                                page = data.c6p3[0];
                                bank = data.c6p3[1];
                                break;
                            case 4:
                                page = data.c6p4[0];
                                bank = data.c6p4[1];
                                break;
                            case 5:
                                page = data.c6p5[0];
                                bank = data.c6p5[1];
                                break;
                            case 6:
                                page = data.c6p6[0];
                                bank = data.c6p6[1];
                                break;
                            case 7:
                                page = data.c6p7[0];
                                bank = data.c6p7[1];
                                break;
                            case 8:
                                page = data.c6p8[0];
                                bank = data.c6p8[1];
                                break;
                        }
                        break;
                    case 7:
                        switch (pos)
                        {
                            case 0:
                                page = data.c7p0[0];
                                bank = data.c7p0[1];
                                break;
                            case 1:
                                page = data.c7p1[0];
                                bank = data.c7p1[1];
                                break;
                            case 2:
                                page = data.c7p2[0];
                                bank = data.c7p2[1];
                                break;
                            case 3:
                                page = data.c7p3[0];
                                bank = data.c7p3[1];
                                break;
                            case 4:
                                page = data.c7p4[0];
                                bank = data.c7p4[1];
                                break;
                            case 5:
                                page = data.c7p5[0];
                                bank = data.c7p5[1];
                                break;
                            case 6:
                                page = data.c7p6[0];
                                bank = data.c7p6[1];
                                break;
                            case 7:
                                page = data.c7p7[0];
                                bank = data.c7p7[1];
                                break;
                            case 8:
                                page = data.c7p8[0];
                                bank = data.c7p8[1];
                                break;
                        }
                        break;
                    case 8:
                        switch (pos)
                        {
                            case 0:
                                page = data.c8p0[0];
                                bank = data.c8p0[1];
                                break;
                            case 1:
                                page = data.c8p1[0];
                                bank = data.c8p1[1];
                                break;
                            case 2:
                                page = data.c8p2[0];
                                bank = data.c8p2[1];
                                break;
                            case 3:
                                page = data.c8p3[0];
                                bank = data.c8p3[1];
                                break;
                            case 4:
                                page = data.c8p4[0];
                                bank = data.c8p4[1];
                                break;
                            case 5:
                                page = data.c8p5[0];
                                bank = data.c8p5[1];
                                break;
                            case 6:
                                page = data.c8p6[0];
                                bank = data.c8p6[1];
                                break;
                            case 7:
                                page = data.c8p7[0];
                                bank = data.c8p7[1];
                                break;
                            case 8:
                                page = data.c8p8[0];
                                bank = data.c8p8[1];
                                break;
                        }
                        break;

                }

                //Set camera status as busy
                SetCameraBusy(runData, cam);

                //Set cameras position in data
                runData.cameraPosition[(cam - 1)] = pos;

                //Tell Companion to push the button
                companion.sendPush(runData, companion.getIPstringFromCon(data.companionCon), data.companionCon[4], page, bank);

                
            }
            
        }

        private void SetCameraBusy(RuntimeData runData, int cameraID)
        {
            runData.cameraBusy[(cameraID - 1)] = true;                  //Set camera to busy
            runData.camerBusyTime[(cameraID - 1)] = TimeManager.GetTimestamp();     //Set timestamp of operation start
            
        }
        
        public void UpdateAllCamerasBusyStatus(ProjectData data, RuntimeData runData)
        {
            Int64 currentTime = TimeManager.GetTimestamp();
            Int64 moveTime = data.cameraMoveTime;

            int loopcounter = 0;
            foreach (bool b in runData.cameraBusy)
            {
                if (runData.cameraBusy[loopcounter] == true)
                {
                    if (currentTime >= (runData.camerBusyTime[loopcounter] + (Int64)moveTime))
                    {
                        runData.cameraBusy[loopcounter] = false;
                        

                    }
                }
                loopcounter++;
            }
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        

        private bool[] GetCamerasWithPresetForPosition(ProjectData data, int position)
        {
            bool[] returnArray = new bool[] { false, false, false, false, false, false, false, false };

            switch (position) //Update array with enabled cameras for requested position
            {
                case 0:
                    returnArray[0] = Convert.ToBoolean(data.c1p0[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p0[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p0[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p0[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p0[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p0[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p0[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p0[2]);
                    break;
                case 1:
                    returnArray[0] = Convert.ToBoolean(data.c1p1[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p1[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p1[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p1[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p1[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p1[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p1[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p1[2]);
                    break;
                case 2:
                    returnArray[0] = Convert.ToBoolean(data.c1p2[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p2[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p2[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p2[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p2[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p2[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p2[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p2[2]);
                    break;
                case 3:
                    returnArray[0] = Convert.ToBoolean(data.c1p3[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p3[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p3[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p3[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p3[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p3[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p3[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p3[2]);
                    break;
                case 4:
                    returnArray[0] = Convert.ToBoolean(data.c1p4[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p4[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p4[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p4[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p4[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p4[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p4[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p4[2]);
                    break;
                case 5:
                    returnArray[0] = Convert.ToBoolean(data.c1p5[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p5[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p5[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p5[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p5[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p5[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p5[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p5[2]);
                    break;
                case 6:
                    returnArray[0] = Convert.ToBoolean(data.c1p6[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p6[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p6[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p6[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p6[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p6[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p6[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p6[2]);
                    break;
                case 7:
                    returnArray[0] = Convert.ToBoolean(data.c1p7[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p7[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p7[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p7[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p7[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p7[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p7[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p7[2]);
                    break;
                case 8:
                    returnArray[0] = Convert.ToBoolean(data.c1p8[2]);
                    returnArray[1] = Convert.ToBoolean(data.c2p8[2]);
                    returnArray[2] = Convert.ToBoolean(data.c3p8[2]);
                    returnArray[3] = Convert.ToBoolean(data.c4p8[2]);
                    returnArray[4] = Convert.ToBoolean(data.c5p8[2]);
                    returnArray[5] = Convert.ToBoolean(data.c6p8[2]);
                    returnArray[6] = Convert.ToBoolean(data.c7p8[2]);
                    returnArray[7] = Convert.ToBoolean(data.c8p8[2]);
                    break;
            }

            return returnArray;
        }

        private bool GetCameraEnabled(ProjectData data, int camera)
        {
            bool returnValue = false;

            switch (camera)
            {
                case 1:
                    returnValue = data.enabledCamera1;
                    break;
                case 2:
                    returnValue = data.enabledCamera2;
                    break;
                case 3:
                    returnValue = data.enabledCamera3;
                    break;
                case 4:
                    returnValue = data.enabledCamera4;
                    break;
                case 5:
                    returnValue = data.enabledCamera5;
                    break;
                case 6:
                    returnValue = data.enabledCamera6;
                    break;
                case 7:
                    returnValue = data.enabledCamera7;
                    break;
                case 8:
                    returnValue = data.enabledCamera8;
                    break;
            }


            return returnValue;
        }

        private bool GetCameraPositionEnabled(RuntimeData runData, int cam, int pos)
        {
            try
            {
                if (runData.camposMatrix[(cam - 1), pos] == true) { return true; } else { return false; }
            }
            catch
            {
                Console.WriteLine("ERROR fetching from CamPosMatrix from CameraOperator / getCameraPositionEnabled");
                return false;
            }
            
        }

        

    }
}
