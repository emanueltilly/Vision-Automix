using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision_Automix
{
    class MixOne
    {
        //RUNTIME VARIABLES

        int[,] speakersOpenOverTime = new int[8, 50];
        int currentSpeaker;

        int[] cameraPositions = new int[8] {99,99,99,99,99,99,99,99};
        Int64[] cameraLastMoveTime = new long[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        
        int switcherPGM;
        int switcherPRW;


        public void initialize()
        {

        }


        public void tick()
        {
            ///*
            ///AUDIO AND SPEAKERS 
            ///

            //Load current audio values into history

            //Decide how many people are talking

            //Check this result against the last X results

            //If match - change current speaker


            ///*
            ///MOVE CAMERAS 
            ///

            //Check if any cameras are pointed at the current speaker && IS NOT BUSY
                
                //NO - No camera is pointed at current speaker
                    
                    //Get available cameras (Cameras that have a position enabled for the speaker, but is NOT in PGM)
                        
                        //YES Camera available
                            
                            //Move camera to position


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
    }
}
