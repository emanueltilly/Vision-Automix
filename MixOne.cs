using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Vision_Automix
{
    class MixOne
    {
        //RUNTIME VARIABLES

        Timer tickTimer;

        ProjectData data;
        RuntimeData runData;

        Companion companion = new Companion();

        int pgmSpeakerHistory = 99;

        int genericLoopCounter1 = 0;
        int genericLoopCounter2 = 0;


        readonly static int historyLength = 50;

        int historyWriteLoopCounter = 0;
        int peopleTalkingHistoryWriteLoopCounter = 0;
        int[,] speakersOpenOverTime = new int[8, historyLength];
        int[] speakersOpenOverTimeSum = new int[8];
        int currentSpeaker;
        bool currentSpeakerChanged = false;
        int peopleTalking = 0;
        int[] peopleTalkingHistory = new int[historyLength * 2];



        int[] cameraPositions = new int[8];
        Int64[] cameraLastMoveTime = new Int64[8];


        Int64 lastCutTime = 0;
        int switcherPGM = 1;
        int switcherPRW = 1;


        public void initialize()
        {
            Console.WriteLine("Initializing MixOne...");
            //Fill variables with default data
            speakersOpenOverTime = ArrayTools.Fill2DArray(speakersOpenOverTime);
            speakersOpenOverTimeSum = ArrayTools.FillArray(speakersOpenOverTimeSum);
            peopleTalkingHistory = ArrayTools.FillArray(peopleTalkingHistory);
            currentSpeaker = 0;

            cameraPositions = new int[8] { 99, 99, 99, 99, 99, 99, 99, 99 };
            cameraLastMoveTime = new Int64[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            int switcherPGM = 1;
            int switcherPRW = 1;


            //Setup timers
            tickTimer = new System.Timers.Timer(50);
            tickTimer.AutoReset = true;
            tickTimer.Enabled = true;
            tickTimer.Elapsed += TickTimer_Elapsed;
        }


        #region GUI PULL DATA
        public bool[] GuiGetSpeakersOpen()
        {
            try
            {
                bool[] result = new bool[8] { false, false, false, false, false, false, false, false };

                int localGuiSpeakerOpenCounter = 0;
                foreach (int vol in speakersOpenOverTimeSum)
                {
                    result[localGuiSpeakerOpenCounter] = ((speakersOpenOverTimeSum[localGuiSpeakerOpenCounter] >= ((double)historyLength * 0.5)) ? true : false);
                    localGuiSpeakerOpenCounter++;
                }
                return result;


            } catch
            {
                return new bool[8] { false, false, false, false, false, false, false, false };

            }

        }

        public int GuiGetPeopleTalking()
        {
            return peopleTalking;
        }

        public int GuiGetProgram()
        {
            return switcherPGM;
        }
        public int GuiGetPreview()
        {
            return switcherPRW;
        }

        public int GuiGetSpeakerIDForCamera(int cameraID)
        {
            return cameraPositions[(cameraID - 1)];
        }

        public string GuiGetSpeakerNameForCamera(int cameraID)
        {

            if (cameraID >= 1 && cameraID <= 8 && cameraPositions[(cameraID - 1)] != 99)
            {
                int positionOfCam = cameraPositions[(cameraID - 1)];
                if (positionOfCam > 0) { return data.speakerNames[positionOfCam - 1]; }
                else { return "Wide Shot"; }



            }
            else if (cameraID == 0)
            {
                return "Wide Shot";
            }
            else
            {
                return "Unknown";
            }


        }

        public Int64 GuiGetCurrentShotDuration()
        {
            Int64 result = TimeManager.GetElapsedFromTimestamp(lastCutTime);
            return ((result > 9999) ? 0 : result);
        }

        #endregion




        private void TickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            tick();
        }

        public void PushProjectdata(ProjectData newData)
        {
            data = newData;
        }

        public void PushRuntimeData(RuntimeData newRunData)
        {
            runData = newRunData;
        }

        private void tick()
        {
            //Console.WriteLine("...---...");
            ///*
            ///AUDIO AND SPEAKERS 
            ///

            # region Load current audio values into history

            if (historyWriteLoopCounter >= historyLength) { historyWriteLoopCounter = 0; }
            genericLoopCounter1 = 0;
            foreach (int openInt in runData.speakersOpen)
            {
                speakersOpenOverTime[genericLoopCounter1, historyWriteLoopCounter] = openInt;

                genericLoopCounter1++;

            }
            historyWriteLoopCounter++;
            #endregion

            # region Create sum of history
            genericLoopCounter1 = 0;
            genericLoopCounter2 = 0;
            speakersOpenOverTimeSum = ArrayTools.FillArray(speakersOpenOverTimeSum);

            while (genericLoopCounter1 < 8)
            {
                while (genericLoopCounter2 < historyLength)
                {
                    if (speakersOpenOverTime[genericLoopCounter1, genericLoopCounter2] == 1)
                    {
                        speakersOpenOverTimeSum[genericLoopCounter1]++;

                    }
                    genericLoopCounter2++;
                }
                genericLoopCounter2 = 0;
                genericLoopCounter1++;
            }

            #endregion

            # region Decide how many people are talking
            int tempPeopleTalking = 0;
            foreach (int talking in speakersOpenOverTimeSum)
            {
                if (talking > (historyLength / 2)) { tempPeopleTalking++; }
            }

            #endregion

            # region Check this result against the last X results
            if (peopleTalkingHistoryWriteLoopCounter >= (historyLength * 2)) { peopleTalkingHistoryWriteLoopCounter = 0; }
            peopleTalkingHistory[peopleTalkingHistoryWriteLoopCounter] = tempPeopleTalking;

            double avgPeopleTalk = Queryable.Average(peopleTalkingHistory.AsQueryable());
            if (avgPeopleTalk < 0.1) { peopleTalking = 0; }
            else if (avgPeopleTalk > 0.1 && avgPeopleTalk < 1) { peopleTalking = 1; }
            else { peopleTalking = 2; }


            peopleTalkingHistoryWriteLoopCounter++;
            #endregion

            # region Set speakerID accordingly
            //If only one person is talking, set current speaker to Speaker ID
            if (peopleTalking == 1)
            {
                //Get speaker talking most in history
                int tempIndexOfSpeaker = (ArrayTools.GetIndexOfHighestValue(speakersOpenOverTimeSum) + 1);
                if (tempIndexOfSpeaker >= 1 && tempIndexOfSpeaker <= 8)
                {
                    if (currentSpeaker != tempIndexOfSpeaker)
                    {
                        currentSpeaker = tempIndexOfSpeaker;
                        currentSpeakerChanged = true;
                        Console.WriteLine("Setting speaker ID to " + currentSpeaker);
                    }
                }
            }
            //If more than one or no speaker is talking, set Speaker ID to 0
            else
            {
                if (currentSpeaker != 0)
                {
                    currentSpeaker = 0;
                    currentSpeakerChanged = true;
                    Console.WriteLine("Setting speaker ID to " + currentSpeaker);
                }

            }
            #endregion

            ///*
            ///MOVE CAMERAS 
            ///
            #region MOVE CAMERAS
            //Only move cameras is Current Speaker is changed && Current Speaker is not live on Program
            if (currentSpeakerChanged == true && (cameraPositions[(switcherPGM - 1)] != currentSpeaker))
            {
                currentSpeakerChanged = false;
                /*Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Staring Move Camera routine...");*/

                //Check if any cameras are pointed at the current speaker
                bool[] camerasAvailable = GetCamerasPointingAtSpeaker(currentSpeaker);

                //If there is one or more cameras available
                
                if (camerasAvailable.Contains(true))
                {

                    //There is already a camera pointed at the speaker, doing nothing
                    //Console.WriteLine("Camera already pointed at Speaker " + currentSpeaker + ". Doing nothing.");

                }
                else
                {
                    //Console.WriteLine("No camera is pointed at Speaker " + currentSpeaker + ". Searching for camera available...");
                    //NO - No camera is pointed at current speaker
                    //Get available cameras (Cameras that have a position enabled for the speaker, but is NOT in PGM)


                    int cameraIDFound = 0;

                    genericLoopCounter1 = 0;
                    foreach (bool cam in camerasAvailable)
                    {
                        if (cameraIDFound == 0)
                        {
                            if (GetPositionEnabledForCamera(data, (genericLoopCounter1 + 1), currentSpeaker) && switcherPGM != (genericLoopCounter1 + 1)) { cameraIDFound = (genericLoopCounter1 + 1);  }
                            
                        }
                        genericLoopCounter1++;
                    }

                    //Call position on camera if a camera is found
                    if (cameraIDFound != 0 && cameraIDFound >= 1 && cameraIDFound <= 8)
                    {
                        //Console.WriteLine("Found Camera " + cameraIDFound + ". Pointing at Speaker " + currentSpeaker);
                        CallCameraPosition(data, cameraIDFound, currentSpeaker);

                    }
                    else
                    {
                       // Console.WriteLine("No camera found for Speaker " + currentSpeaker + ". Found cam ID: " + cameraIDFound);
                    }
                }
                

            }
            #endregion

           
            ///*
            ///SWITCH MIXER
            ///

            //Check if camera currently in PGM is showing the speaker currently talking
            if (pgmSpeakerHistory != currentSpeaker)
            {
                
                /*Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Staring Switcher routine...");
                Console.WriteLine("Current speaker: Speaker" + currentSpeaker);
                Console.WriteLine("Current on air: Speaker " + cameraPositions[(switcherPGM - 1)]);*/

                //Is there any camera pointed at the current speaker?
                bool[] camerasAvailable = GetCamerasPointingAtSpeaker(currentSpeaker);

                //Filter out cameras that are busy moving
                genericLoopCounter1 = 0;
                foreach (bool cam in camerasAvailable)
                {

                    try { if (GetCameraBusy(data, (genericLoopCounter1 + 1)) == true) { camerasAvailable[genericLoopCounter1] = false; } } catch { Console.WriteLine("Error filtering out cameras that are busy moving..."); }
                    genericLoopCounter1++;
                }
                Console.WriteLine("Cameras available:");
                Console.WriteLine("C1: " + camerasAvailable[0]);
                Console.WriteLine("C2: " + camerasAvailable[1]);
                Console.WriteLine("C3: " + camerasAvailable[2]);
                Console.WriteLine("C4: " + camerasAvailable[3]);
                Console.WriteLine("C5: " + camerasAvailable[4]);
                Console.WriteLine("C6: " + camerasAvailable[5]);
                Console.WriteLine("C7: " + camerasAvailable[6]);
                Console.WriteLine("C8: " + camerasAvailable[7]);
                if (camerasAvailable.Contains(true))
                {

                    //Get first available camera with speaker that is not busy
                    int switchID = (Array.IndexOf(camerasAvailable, true) + 1);
                    Console.WriteLine("Camera " + switchID + " is availabe for speaker " + currentSpeaker);
                    //Is minimum shot time reached?
                    if (TimeManager.GetElapsedFromTimestamp(lastCutTime) >= data.minimumShotTime)
                    {
                        if (switcherPGM != switchID)
                        {
                            Console.WriteLine("Minimum shottime reached. Switching on PGM...");
                            //Put camera in PROGRAM
                            TellMixer(data, runData, true, switchID);
                            //TellMixer(data,runData,true,)
                            lastCutTime = TimeManager.GetTimestamp();
                            //Set bus info
                            switcherPGM = switchID;
                            pgmSpeakerHistory = currentSpeaker;
                        }

                    }
                    else
                    {
                        if (switcherPRW != switchID)
                        {
                            Console.WriteLine("Minimum shottime NOT reached. Switching on PRW...");
                            //Put camera in PREVIEW
                            TellMixer(data, runData, false, switchID);
                            //Set bus info
                            switcherPRW = switchID;
                        }

                    }


                }
                else
                {
                    Console.WriteLine("NO CAMERA IS AVAILABLE");
                }

            }

        }
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
            if ((bus == false && data.enablePRWbusControl == false) != true)      //Dont send command for PRW is PRW control is disabled
            {
                companion.sendPush(runData, companion.getIPstringFromCon(data.companionCon), data.companionCon[4], page, bank);
            }





            //Set GUI
            if (bus == true) { runData.cameraPGM = camera; runData.lastCutTime = 0; }
            else { runData.cameraPRW = camera; }
        }

        private bool[] GetCamerasPointingAtSpeaker(int speakerID)
        {
            bool[] result = new bool[] { false, false, false, false, false, false, false, false };

            int gcpasLoopCounter = 0;
            foreach (int pos in cameraPositions)
            {
                result[gcpasLoopCounter] = (cameraPositions[gcpasLoopCounter] == speakerID);
                gcpasLoopCounter++;
            }
            
            return result;
        }

        public bool GetCameraBusy(ProjectData data, int cameraID)
        {
            try
            {
                if (TimeManager.GetElapsedFromTimestamp(cameraLastMoveTime[(cameraID - 1)]) < data.cameraMoveTime) { return true; }
                else { return false; }
            } catch  (Exception ex)
            {
                Console.WriteLine("ERROR getting Camera Busy status for Camera " + cameraID);
                Console.WriteLine(ex);
                return true;
            }
            
        }

        private bool GetPositionEnabledForCamera (ProjectData data, int cameraID, int speakerID)
        {
            bool allow;
            if (speakerID == 0) { allow = true; }
            else { allow = data.enabledSpeaker[(speakerID - 1)]; }

            try
            {
                if (data.enabledCamera[(cameraID - 1)] && allow)
                {

                    switch (cameraID)
                    {
                        case 1:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c1p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c1p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c1p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c1p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c1p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c1p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c1p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c1p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c1p8[2]);
                            }
                            break;
                        case 2:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c2p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c2p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c2p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c2p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c2p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c2p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c2p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c2p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c2p8[2]);
                            }
                            break;
                        case 3:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c3p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c3p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c3p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c3p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c3p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c3p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c3p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c3p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c3p8[2]);
                            }
                            break;
                        case 4:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c4p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c4p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c4p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c4p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c4p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c4p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c4p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c4p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c4p8[2]);
                            }
                            break;
                        case 5:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c5p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c5p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c5p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c5p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c5p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c5p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c5p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c5p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c5p8[2]);
                            }
                            break;
                        case 6:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c6p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c6p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c6p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c6p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c6p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c6p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c6p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c6p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c6p8[2]);
                            }
                            break;
                        case 7:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c7p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c7p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c7p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c7p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c7p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c7p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c7p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c7p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c7p8[2]);
                            }
                            break;
                        case 8:
                            switch (speakerID)
                            {
                                case 0:
                                    return Convert.ToBoolean(data.c8p0[2]);
                                case 1:
                                    return Convert.ToBoolean(data.c8p1[2]);
                                case 2:
                                    return Convert.ToBoolean(data.c8p2[2]);
                                case 3:
                                    return Convert.ToBoolean(data.c8p3[2]);
                                case 4:
                                    return Convert.ToBoolean(data.c8p4[2]);
                                case 5:
                                    return Convert.ToBoolean(data.c8p5[2]);
                                case 6:
                                    return Convert.ToBoolean(data.c8p6[2]);
                                case 7:
                                    return Convert.ToBoolean(data.c8p7[2]);
                                case 8:
                                    return Convert.ToBoolean(data.c8p8[2]);
                            }
                            break;
                    }

                    return false;
                }
                else
                {
                    Console.WriteLine("Speaker " + speakerID + " is not enabled, returning false.");
                    return false;
                }
            } catch
            {
                Console.WriteLine("Error getting Position Enabled for Camera.");
                return false;
            }
            
        }

        private void CallCameraPosition(ProjectData data, int cam, int pos)
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

            //Update cameras latest move time
            cameraLastMoveTime[(cam - 1)] = TimeManager.GetTimestamp();

            //Set cameras position in data
            cameraPositions[(cam - 1)] = pos;
            Console.WriteLine("Setting cameraPosition field " + (cam - 1) + " at " + pos);

            //Tell Companion to push the button
            companion.sendPush(runData, companion.getIPstringFromCon(data.companionCon), data.companionCon[4], page, bank);
        }
        
    }
}
