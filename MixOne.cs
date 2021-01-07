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
        int[] peopleTalkingHistory = new int[historyLength];




        int[] cameraPositions = new int[8];
        Int64[] cameraLastMoveTime = new Int64[8];
        
        int switcherPGM;
        int switcherPRW;


        public void initialize()
        {
            //Fill variables with default data
            speakersOpenOverTime = ArrayTools.Fill2DArray(speakersOpenOverTime);
            speakersOpenOverTimeSum = ArrayTools.FillArray(speakersOpenOverTimeSum);
            peopleTalkingHistory = ArrayTools.FillArray(peopleTalkingHistory);
            currentSpeaker = 0;

            cameraPositions = new int[8] { 99, 99, 99, 99, 99, 99, 99, 99 };
            cameraLastMoveTime = new Int64[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            int switcherPGM = 99;
            int switcherPRW = 99;


            //Setup timers
            tickTimer = new System.Timers.Timer(50);
            tickTimer.AutoReset = true;
            tickTimer.Enabled = true;
            tickTimer.Elapsed += TickTimer_Elapsed;
        }

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
            

            ///*
            ///AUDIO AND SPEAKERS 
            ///

            # region Load current audio values into history

            if (historyWriteLoopCounter >= speakersOpenOverTime.Length) { historyWriteLoopCounter = 0; }
            genericLoopCounter1 = 0;
            foreach (int openInt in runData.speakersOpen)
            {
                speakersOpenOverTime[genericLoopCounter1, historyWriteLoopCounter] = openInt;
            }
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
                genericLoopCounter1++;
            }
            #endregion

            # region Decide how many people are talking
            int tempPeopleTalking = 0;
            foreach (int talking in peopleTalkingHistory)
            {
                if (talking > (historyLength / 2)) { tempPeopleTalking++; }
            }
            #endregion

            # region Check this result against the last X results
            if (peopleTalkingHistoryWriteLoopCounter >= historyLength) { peopleTalkingHistoryWriteLoopCounter = 0; }
            peopleTalkingHistory[peopleTalkingHistoryWriteLoopCounter] = tempPeopleTalking;

            double avgPeopleTalk = Queryable.Average(peopleTalkingHistory.AsQueryable());
            if (avgPeopleTalk < 1) { peopleTalking = 0; }
            else if (avgPeopleTalk > 1 && avgPeopleTalk < 2) { peopleTalking = 1; }
            else { avgPeopleTalk = 2; }

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
                }
                
            }
            #endregion

            ///*
            ///MOVE CAMERAS 
            ///

            //Only move cameras is Current Speaker is changed && Current Speaker is not live on Program
            if (currentSpeakerChanged == true && (cameraPositions[(switcherPGM - 1)] != currentSpeaker))
            {
                currentSpeakerChanged = false;

                //Check if any cameras are pointed at the current speaker
                bool[] camerasAvailable = GetCamerasPointingAtSpeaker(currentSpeaker);

                //If there is one or more cameras available
                if (camerasAvailable.Contains(true)){
                    
                    //There is already a camera pointed at the speaker, doing nothing

                } else
                {
                    //NO - No camera is pointed at current speaker
                    //Get available cameras (Cameras that have a position enabled for the speaker, but is NOT in PGM)
                    int cameraIDFound = 0;

                    genericLoopCounter1 = 0;
                    foreach (bool cam in camerasAvailable)
                    {
                        if (cameraIDFound != 0)
                        {
                            if (camerasAvailable[genericLoopCounter1] == true && switcherPGM == (genericLoopCounter1 + 1)) { cameraIDFound = (genericLoopCounter1 + 1);  }
                        }
                        genericLoopCounter1++;
                    }

                    //Call position on camera if a camera is found
                    if (cameraIDFound != 0 && cameraIDFound >= 1 && cameraIDFound <= 8)
                    {
                        CallCameraPosition(data, cameraIDFound, currentSpeaker);
                    }
                }








            }



            ///*
            ///SWITCH MIXER
            ///

            //Check if camera currently in PGM is showing the speaker currently talking

            //YES
            //Do nothing

            //NO
            //Is there any camera pointed at the current speaker?
            //Yes
            //Is minimum shot time reached?
            //Yes
            //Put camera in PROGRAM
            //Reset current shottime
            //No
            //Put camera in PREVIEW


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

        private bool GetCameraBusy(ProjectData data, int cameraID)
        {
            if (TimeManager.GetElapsedFromTimestamp(cameraLastMoveTime[(cameraID - 1)]) < data.cameraMoveTime) { return true; }
            else { return false; }
        }

        private bool GetPositionEnabledForCamera (ProjectData data, int cameraID, int speakerID)
        {
            if (data.enabledCamera[(cameraID - 1)] && data.enabledSpeaker[(speakerID - 1)])
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
                        }break;
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
            } else
            {
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
            runData.cameraPosition[(cam - 1)] = pos;

            //Tell Companion to push the button
            companion.sendPush(runData, companion.getIPstringFromCon(data.companionCon), data.companionCon[4], page, bank);
        }
        
    }
}
