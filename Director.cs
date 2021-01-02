using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision_Automix
{
    class Director
    {

        //Declare variables
        private int historyRowCounter = 0;          //Counter for what row to overwright with data on next tick
        private int[,] history = new int[15, 8];
        private int[] historyWeighted = new int[] {0,0,0,0,0,0,0,0};

        private int voteCounter = 0;
        
        private int voteLength = 40; //Default before data read
        private int voteLengthPrev = 40; //This is used to store current vote length for comparison. If missmatch with new value array is filled with 0 to not crash.

        private int[] voteArray;


    public void Initialize()
        {
            voteArray = new int[voteLength];
            historyRowCounter = 0;
            //Fill history with 0
            history = Fill2DArray(history);

            //Fill vote array with 0
            voteArray = FillArray(voteArray);

            voteLength = 40;
            voteLengthPrev = 40;
            voteCounter = 0;
            
            history = new int[15, 8];
            historyWeighted = new int[] { 0,0,0,0,0,0,0,0};


        }

        public void Tick(ProjectData data, RuntimeData runData)
        {
            //Load values
            voteLength = data.voteLength;
            if (voteLength != voteLengthPrev) { voteArray = new int[voteLength]; voteArray = FillArray(voteArray); Console.WriteLine("Votelength changed from " + voteLengthPrev + " to " + voteLength); voteLengthPrev = voteLength;  }   //Fix crash when updating votelength on the fly

            ///*
            ///DIRECTOR
            ///

            //1-Update histor and create weighted history
            DirectorHistory(runData);

            //2-Decide if there is No, One or Multiple speakers talking
            int switchStatement = DirectorMultipleSpeakersCheck(runData);

            //3-If only one speaker, decide who it is
            int m = historyWeighted.Max();
            runData.longestSpeaker = (Array.IndexOf(historyWeighted, m))+1;  //This returns index of one talking speaker. +1 to compensate for array starting at 0

            //4-Take action
            switch (switchStatement)
            {
                case 1:             //No speaker is talkning
                    if (runData.currentShotTime < data.maximumQuietTime && data.enableCutToWideOnQuiet == true)    //Check that it has been quiet for long enough && Quiet cut is enabled
                    {
                        CueSpeaker(data, runData, 0); AddShotTime(runData); //Cue wideshot
                    }
                    
                    //runData.multipleSpeakers = false;
                    //runData.noSpeakers = true;
                    break;                 
                    //if (getCurrentShotTimeStatus(runData, data) < 2) { addShotTime(runData); break; }     //Do nothing if current shot is below maximum shot time
                    //else { cueSpeaker(runData,0); addShotTime(runData); break; }                   //Cue wideshot
                    

                case 2:             //One speaker is talking
                    if (runData.longestSpeaker == runData.currentSpeaker) { AddShotTime(runData); break; }         //Do nothing if same speaker
                    else
                    {
                        CueSpeaker(data, runData, runData.longestSpeaker);                                             //Cue speaker
                    }
                    //runData.multipleSpeakers = false;
                    //runData.noSpeakers = false;
                    break;

                case 3:             //Multiple speakers are talking
                    CueSpeaker(data, runData, 0); AddShotTime(runData);                         //Cue wideshot
                    //runData.multipleSpeakers = true;
                    //runData.noSpeakers = false;
                    break;                   
                    //if (getCurrentShotTimeStatus(runData, data) < 1) { addShotTime(runData); break; }     //Do nothing if current shot is below minimum shot time
                    //else { cueSpeaker(runData,0); addShotTime(runData); break; }                   //Cue wideshot
                    
            }

            //5-Get Next Speakers vote status for GUI
            runData.nextSpeakerVotePercent = GetNextSpeakerVoteStatus(data, runData);
            
            



        }

        private void DirectorHistory(RuntimeData runData)
        {
            //PREPARE HISTORY
            historyRowCounter++;
            if (historyRowCounter >= 15) { historyRowCounter = 0; } //Cycle the counter when reaching the array length

            //Overwright new data to oldest slot in history
            int loopCounter = 0;
            foreach (int i in runData.speakersOpen)
            {
                history[historyRowCounter, loopCounter] = runData.speakersOpen[loopCounter];
                loopCounter++;
            }

            //Update weighted history
            historyWeighted = FillArray(historyWeighted);   //Clear array with 0

            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 15; row++)
                {
                    historyWeighted[col] = (historyWeighted[col] + history[row, col]);
                }
            }
        }

        private int DirectorMultipleSpeakersCheck(RuntimeData runData)
        {
            

            //Check for no speaker
            if (historyWeighted.Sum() == 0) {  runData.noSpeakers = true; runData.multipleSpeakers = false;     }
            else
            {
                runData.lastTalkingTime = TimeManager.GetTimestamp();

                //Check for multiple speakers
                int speakingCounter = 0;    //Sum of currently talking speakers
                for (int i = 0; i < 8; i++)
                {
                    if (historyWeighted[i] >= 3) { speakingCounter++; }
                }
                runData.noSpeakers = false;
                runData.multipleSpeakers = (speakingCounter >= 2);
                
            }

            //Create return statement
            if (runData.noSpeakers == true) { return  1; }
            else if (runData.multipleSpeakers == true) { return 3; }
            else { return 2; }


            /*
            if (runData.noSpeakers == true) { Console.WriteLine("No speaker"); }
            if (runData.multipleSpeakers == true) { Console.WriteLine("Multiple speakers"); }
            if (runData.multipleSpeakers == false && runData.noSpeakers == false) { Console.WriteLine("ONE speakers"); }*/
        }

        private int[,] Fill2DArray(int[,] arr)
        {
            int numRows = arr.GetLength(0);
            int numCols = arr.GetLength(1);

            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numCols; ++j)
                {
                    arr[i, j] = 1;
                }
            }

            return arr;
        }

        private int[] FillArray(int[] arr)
        {
            int counter = 0;
            foreach (int i in arr)
            {
                arr[counter] = 0;
                counter++;
            }
            return arr;
        }


        private void CueSpeaker(ProjectData data, RuntimeData runData, int speakerNumber)
        {
            if ( runData.nextSpeaker != speakerNumber)
            {
                runData.nextSpeaker = speakerNumber;                    //Cue this speaker as next speaker
                runData.changedNextSpeaker = true;
                

            }


            bool voteAllowed = VoteSpeaker(speakerNumber, data, runData);          //Vote on speaker as current speaker

            if (voteAllowed == true && runData.nextSpeaker != runData.currentSpeaker)        //If elected set as current speaker
            {
                runData.currentSpeaker = speakerNumber;
                runData.changedCurrentSpeaker = true;
                //Console.WriteLine("Elected next speaker: " + speakerNumber);
                //resetShotTime(runData);                                     //ONLY FOR DEV

            }
        }

        private bool VoteSpeaker(int speakerNumber, ProjectData data, RuntimeData runData)
        {
            //Counter
            voteCounter++;
            if (voteCounter >= voteLength) { voteCounter = 1; }

            //Cast vote
            voteArray[voteCounter-1] = speakerNumber;

            //Check if all votes match
            if (GetNextSpeakerVoteStatus(data, runData) == 100) { return true; }
            else { return false; }
            
            //if (voteArray[0] == speakerNumber && voteArray[1] == speakerNumber && voteArray[2] == speakerNumber && voteArray[3] == speakerNumber) { return true; }
            //else { return false; }
        }





        private void AddShotTime(RuntimeData runData)
        {
            runData.currentShotTime++;
        }



        public int GetNextSpeakerVoteStatus(ProjectData data, RuntimeData runData)
        {
            int found = 0;
            int loopCounter = 0;

            //Count how many times next speaker is found in the vote length
            foreach (int i in voteArray)
            {
                if (voteArray[loopCounter] == runData.nextSpeaker) { found++;  }
                loopCounter++;
            }
            
            //Calculate percent
            double percent = (((double)found / (double)data.voteLength) * 100);


            //Cleanup
            //Cleanup for GUI
            percent *= 1.1;
            if (percent > 99) { percent = 100; }
            if (percent < 1) { percent = 1; }

            return (int)percent;

        }
    }
}
