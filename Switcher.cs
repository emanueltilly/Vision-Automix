using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision_Automix
{
    class Switcher
    {
        private Companion companion = new Companion();

        private int localCurrentSpeaker = 99;

        private bool wideShotIsLive = false;

        public void Initialize()
        {
            localCurrentSpeaker = 99;
        }

        public void Tick(ProjectData data, RuntimeData runData)
        {
            ///*
            ///PGM
            ///
            ///
            ///
            ///

            int localQuietTime = (int)(TimeManager.GetTimestamp() - runData.lastTalkingTime);
            if (localQuietTime < 0) { localQuietTime = 0; }

            runData.currentShotTime = TimeManager.GetElapsedFromTimestamp(runData.lastCutTime); //Update current shot time
            
            //Check if current shot is long enough new cut
            if ((int)runData.currentShotTime > data.minimumShotTime)
            {
                int cameraID;

                //GET CUT TYPE
                // 0 - No cut
                // 1 - Normal Cut
                // 2 - Multiple speakers
                // 3 - Quiet / No speakers
                
                int switchType;
                bool currentPgmShowsSpeakerTalkning = (runData.cameraPosition[(runData.cameraPGM - 1)] == runData.currentSpeaker);

                
                // Wide is live, but only one speaker is talking
                if (wideShotIsLive == true && runData.currentSpeaker != 0) { switchType = 1; /*runData.forceResetSpeakerHistory = true; Console.WriteLine(" Wide is live, but only one speaker is talking");*/}
                // Closeup is live, but on the incorrect speaker
                else if (wideShotIsLive != true && currentPgmShowsSpeakerTalkning != true) {
                    if(GetCamera(data, runData, false, runData.currentSpeaker) != runData.cameraPGM)
                    {
                        switchType = 4;
                        
                        
                    } else
                    {
                        switchType = 2;
                        runData.forceResetSpeakerHistory = true;
                        
                    }
                }
                // Multple speakers are talking and wide camera is not live
                else if (wideShotIsLive != true && runData.currentSpeaker == 0 && runData.multipleSpeakers == true) { switchType = 2;}
                // No speaker is talking and wide camera is not live
                else if (wideShotIsLive != true && runData.currentSpeaker == 0 && runData.noSpeakers == true) { switchType = 3;}
                // Catch
                else { switchType = 0; }

                /*
                if ((runData.cameraPosition[(runData.cameraPGM - 1)] != runData.currentSpeaker) && runData.currentSpeaker != 0) { switchType = 1; }
                else if ((runData.cameraPosition[(runData.cameraPGM - 1)] == 0) && runData.currentSpeaker != 0) { switchType = 2; }
                else if (runData.currentSpeaker != localCurrentSpeaker && runData.currentSpeaker != 0) { switchType = 1; }
                else if (runData.multipleSpeakers == true && runData.currentSpeaker == 0 && wideShotIsLive == false) { switchType = 2; }
                else if (runData.noSpeakers == true && data.enableCutToWideOnQuiet == true && wideShotIsLive == false && localQuietTime > data.minimumShotTime) { switchType = 3; }
                else { switchType = 0; }
                */

                //PREFORM CUT
                switch (switchType)
                {
                    case 1:
                        cameraID = GetCamera(data, runData, false, runData.currentSpeaker);
                        
                        if (cameraID != 0)                                                  //Catch no camera is available
                        {
                            localCurrentSpeaker = runData.currentSpeaker;
                            
                            TellMixer(data, runData, true, cameraID);
                            runData.lastCutTime = TimeManager.GetTimestamp();
                            wideShotIsLive = false;                                         //Set wideshot flag
                            Console.WriteLine("---CASE 1");
                        }
                        break;
                    case 2:
                        cameraID = GetCamera(data, runData, true, 0);
                        if (cameraID != 0)                                                  //Catch no camera is available
                        {
                            localCurrentSpeaker = runData.currentSpeaker;
                            
                            TellMixer(data, runData, true, cameraID);
                            runData.lastCutTime = TimeManager.GetTimestamp();
                            wideShotIsLive = true;                                         //Set wideshot flag
                            Console.WriteLine("---CASE 2");
                        }
                        break;
                    case 3:
                        cameraID = GetCamera(data, runData, true, 0);
                        if (cameraID != 0)                                                  //Catch no camera is available
                        {
                            
                            TellMixer(data, runData, true, cameraID);
                            runData.lastCutTime = TimeManager.GetTimestamp();
                            wideShotIsLive = true;                                         //Set wideshot flag
                            Console.WriteLine("---CASE 3");
                        }
                        break;
                    case 4:
                        cameraID = GetCamera(data, runData, true, runData.currentSpeaker);
                        if (cameraID != 0)                                                  //Catch no camera is available
                        {
                            
                            TellMixer(data, runData, true, cameraID);
                            runData.lastCutTime = TimeManager.GetTimestamp();
                            wideShotIsLive = ((runData.cameraPosition[(cameraID-1)] == 0)?true:false);                                         //Set wideshot flag
                            Console.WriteLine("---CASE 4");
                        }
                        break;


                }
                
   
            }

            ///*
            ///PREVIEW
            ///
            ///
            ///
            ///
            if (runData.changedNextSpeaker == true && (runData.cameraPRW != runData.changePRWcam))
            {
                TellMixer(data, runData, false, runData.changePRWcam);
                runData.changePRW = false;
                runData.changedNextSpeaker = false;
            }


        }

        //Set camera on PGM or PRW bus
        private void TellMixer(ProjectData data, RuntimeData runData, bool bus, int camera)
        {
            ///*
            ///BUS
            ///TRUE = PROGRAM
            ///FALSE = PREVIEW
            ///

            

            //Log
            if (bus == true) { Console.WriteLine("Setting PGM to Camera " + camera); }

            int page = 1;
            int bank = 1;

            //Get page/bank data
            switch (camera)
            {
                case 1:
                    page = (bus ? data.c1pgm[0] : data.c1prw[0]);
                    bank = (bus ? data.c1pgm[1] : data.c1prw[1]);
                    break;
                case 2:
                    page = (bus ? data.c2pgm[0] : data.c2prw[0]);
                    bank = (bus ? data.c2pgm[1] : data.c2prw[1]);
                    break;
                case 3:
                    page = (bus ? data.c3pgm[0] : data.c3prw[0]);
                    bank = (bus ? data.c3pgm[1] : data.c3prw[1]);
                    break;
                case 4:
                    page = (bus ? data.c4pgm[0] : data.c4prw[0]);
                    bank = (bus ? data.c4pgm[1] : data.c4prw[1]);
                    break;
                case 5:
                    page = (bus ? data.c5pgm[0] : data.c5prw[0]);
                    bank = (bus ? data.c5pgm[1] : data.c5prw[1]);
                    break;
                case 6:
                    page = (bus ? data.c6pgm[0] : data.c6prw[0]);
                    bank = (bus ? data.c6pgm[1] : data.c6prw[1]);
                    break;
                case 7:
                    page = (bus ? data.c7pgm[0] : data.c7prw[0]);
                    bank = (bus ? data.c7pgm[1] : data.c7prw[1]);
                    break;
                case 8:
                    page = (bus ? data.c8pgm[0] : data.c8prw[0]);
                    bank = (bus ? data.c8pgm[1] : data.c8prw[1]);
                    break;

            }

            //Send button press to companion
            if ((bus == false && data.enablePRWbusControl == false) != true )      //Dont send command for PRW is PRW control is disabled
            {
                companion.sendPush(runData, companion.getIPstringFromCon(data.companionCon), data.companionCon[4], page, bank);
            }
                

            
            

            //Set GUI
            if (bus == true) { runData.cameraPGM = camera; runData.lastCutTime = 0; }
            else { runData.cameraPRW = camera; }
        }

        //Get array of available cameras for position
        private bool[] CamerasAvailableForPosition(RuntimeData runData, int speakerID)
        {
            bool[] result = new bool[] { false, false, false, false, false, false, false, false };
            int loopCounter = 0;

            //Check what cameras are pointing at requested speaker
            foreach (int i in runData.cameraPosition)
            {
                result[loopCounter] = (runData.cameraPosition[loopCounter] == speakerID);
                loopCounter++;
            }

            //Check if the cameras found are currently busy
            loopCounter = 0;
            foreach(bool b in runData.cameraBusy)
            {
                if (result[loopCounter] == true && runData.cameraBusy[loopCounter] == true) { result[loopCounter] = false; }
                
            }




            return result;


        }

        //Select a camera for switching
        //Will return 0 if no camera available
        private int GetCamera(ProjectData data, RuntimeData runData, bool wideShot, int speakerID)
        {


            if(wideShot == true) { speakerID = 0; } //Set correct speaker ID for wideshots

            //Get available cameras
            bool[] availableCameras = CamerasAvailableForPosition(runData, speakerID);

            //Return if not camera is available
            if (CountTrueInBoolArray(availableCameras) < 1) { return 0; }

            //If one or more cameras is available
            else
            {

                int loopcounter = 0;
                bool foundCamera = false;
                int cameraIDfound = 0;
                

                //Check if preferred camera is available
                if (availableCameras[((data.prefPos[speakerID]) - 1)] == true)
                {
                    foundCamera = true;
                    cameraIDfound = data.prefPos[speakerID];
                    return cameraIDfound;
                }

                //If not search for another camera
                foreach (bool b in availableCameras)
                {
                    if (availableCameras[loopcounter] == true && foundCamera == false)
                    {
                        foundCamera = true;                 //Set camera found
                        cameraIDfound = (loopcounter + 1);  //Set camera ID for found camera
                        loopcounter++;
                    } else
                    {
                        loopcounter++;
                    }
                }
                //Console.WriteLine("CAMERA ID FOUND: " + cameraIDfound);
                return cameraIDfound;
               
            }
            
        }

        private int CountTrueInBoolArray(bool[] sourceArray)
        {
            int loopCounter = 0;
            int result = 0;
            foreach (bool b in sourceArray)
            {
                if (sourceArray[loopCounter] == true) { result++; }
                loopCounter++;
            }
            


            return result;

        }
    }
}
