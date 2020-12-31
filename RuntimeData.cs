using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision_Automix
{
    class RuntimeData
    {
        // This is all realtime data that does not need to be stored in the project

        //Cam Pos Matrix - Used to quickly idenfify what positions are enabled for what cameras
        public bool[,] camposMatrix = new bool[8, 9];

        //Speakers
        public int speaker1Volume = 0;
        public int speaker2Volume = 0;
        public int speaker3Volume = 0;
        public int speaker4Volume = 0;
        public int speaker5Volume = 0;
        public int speaker6Volume = 0;
        public int speaker7Volume = 0;
        public int speaker8Volume = 0;
        public int[] speakersOpen = { 0, 0, 0, 0, 0, 0, 0, 0 };

        //Director

        public int currentSpeaker = 1;
        public int nextSpeaker = 1;
        public int longestSpeaker = 1;

        public bool multipleSpeakers = false;
        public bool noSpeakers = false;

        public int nextSpeakerVotePercent = 0;

        //Switcher

        public int cameraPGM = 1;
        public int cameraPRW = 1;

        public Int64 lastCutTime = 0;
        public Int64 currentShotTime = 0;

        public Int64 lastTalkingTime = 0;

        //CameraOperator
        public int[] cameraPosition = new int[] { 99, 99, 99, 99, 99, 99, 99, 99 };
        public bool[] cameraBusy = new bool[] {true, true, true, true, true, true, true, true};
        public Int64[] camerBusyTime = new Int64[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        


        //Triggers
        public bool changedPGM = false;
        public bool changedPRW = false;
        public bool changedCurrentSpeaker = false;
        public bool changedNextSpeaker = false;

        public bool changePRW = false;
        public int changePRWcam = 0;


    }
}
