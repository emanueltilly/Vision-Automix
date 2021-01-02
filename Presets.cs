using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision_Automix
{
    class Presets
    {
        public static bool LoadPreset(ProjectData data, string requestedPreset)
        {
            switch (requestedPreset)
            {
                case "instant":
                    data.voteLength = 5;
                    data.minimumShotTime = 1;
                    data.maximumQuietTime = 10;
                    data.enableCutToWideOnQuiet = true;
                    return true;
                    
                case "fast":
                    data.voteLength = 10;
                    data.minimumShotTime = 3;
                    data.maximumQuietTime = 10;
                    data.enableCutToWideOnQuiet = true;
                    return true;
                    
                case "normal":
                    data.voteLength = 10;
                    data.minimumShotTime = 6;
                    data.maximumQuietTime = 10;
                    data.enableCutToWideOnQuiet = true;
                    return true;
                    
                case "careful":
                    data.voteLength = 100;
                    data.minimumShotTime = 10;
                    data.maximumQuietTime = 10;
                    data.enableCutToWideOnQuiet = false;
                    return true;
                    
                case "slow":
                    data.voteLength = 100;
                    data.minimumShotTime = 30;
                    data.maximumQuietTime = 30;
                    data.enableCutToWideOnQuiet = true;
                    return true;
                    
            }

            return false;
        }
    }
}
