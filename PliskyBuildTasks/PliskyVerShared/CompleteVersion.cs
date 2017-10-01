using Plisky.Plumbing;
using System;

namespace Plisky.Build {

    public class CompleteVersion {
        private const int NUMBERDIGITS_FORSHORT = 2;
        public VersionUnit[] units;
        private DisplayType[] displayTypes;
        private const int NUMBERDISPLAYTYPES = 5;

        public CompleteVersion() {
            Bilge.Log("Complete Version Created");
            displayTypes = new DisplayType[NUMBERDISPLAYTYPES];
            displayTypes[(int)FileUpdateType.Assembly4] = DisplayType.Full;
            displayTypes[(int)FileUpdateType.AssemblyInformational] = DisplayType.FullIncludeText;
            displayTypes[(int)FileUpdateType.AssemblyFile] = DisplayType.Full;            
            displayTypes[(int)FileUpdateType.Wix] = DisplayType.FullIncludeText;
            displayTypes[(int)FileUpdateType.Assembly2] = DisplayType.Short;
            
        }

        public void SetDisplayTypeForVersion(FileUpdateType fut, DisplayType dt) {
            int vfut = ValidateFileUpdateType(fut);
            displayTypes[vfut] = dt;
        }

        public DisplayType GetDisplayType(FileUpdateType fut) {
            int fai = ValidateFileUpdateType(fut);
/*
            Bilge.Log($"{0} >> {displayTypes[0]} >> {displayTypes[(int)FileUpdateType.Assembly4]}");
            Bilge.Log($"{1} >> {displayTypes[1]} >> {displayTypes[(int)FileUpdateType.AssemblyInformational]}");
            Bilge.Log($"{2} >> {displayTypes[2]} >> {displayTypes[(int)FileUpdateType.AssemblyFile]}");
            Bilge.Log($"{3} >> {displayTypes[3]} >> {displayTypes[(int)FileUpdateType.Wix]}");
            Bilge.Log($"{4} >> {displayTypes[4]} >> {displayTypes[(int)FileUpdateType.Assembly2]}");
            Bilge.Log("Done");
            Bilge.Log($"Getting {fai}");
            */
            return displayTypes[fai];
        }

        private static int ValidateFileUpdateType(FileUpdateType fut) {
            int fai = (int)fut;
#if DEBUG
            if (fai < 0 || fai > NUMBERDISPLAYTYPES-1) {
                throw new ArgumentOutOfRangeException("fut", $"The value {fut},{fai} for fut does not map to a known type.");
            }
#endif
            return fai;
        }

        public CompleteVersion(params VersionUnit[] versionUnits) : this() {
            units = versionUnits;
        }

        public string GetVersionString(DisplayType dt = DisplayType.Full) {
            string result = string.Empty;
            int stopPoint = units.Length;
            if ((dt == DisplayType.Short) && (units.Length > NUMBERDIGITS_FORSHORT)) {
                stopPoint = NUMBERDIGITS_FORSHORT;
            }

            for (int i = 0; i < stopPoint; i++) {
                result += units[i].GetStringValue(dt);
            }
            return result;
        }

        public override string ToString() {
            return GetVersionString(DisplayType.FullIncludeText);
        }

        public void PerformIncrement() {
            bool lastChanged = false;
            bool anyChanged = false;
            DateTime t1 = DateTime.Now;

            foreach (var un in units) {                
                lastChanged = un.PerformIncrement(lastChanged, anyChanged, t1, t1);
                if (lastChanged) { anyChanged = true; }
            }
        }
    }
}