using Plisky.Plumbing;
using System;

namespace Plisky.Build {

    public class CompleteVersion {
        private const int NUMBERDIGITS_FORSHORT = 2;
        public VersionUnit[] units;
        public DisplayType[] displayTypes;

        public CompleteVersion() {
            displayTypes = new DisplayType[5];
            displayTypes[(int)FileUpdateType.Assembly4] = DisplayType.Full;
            displayTypes[(int)FileUpdateType.AssemblyFile] = DisplayType.Full;
            displayTypes[(int)FileUpdateType.AssemblyInformational] = DisplayType.FullIncludeText;
            displayTypes[(int)FileUpdateType.Wix] = DisplayType.FullIncludeText;
            displayTypes[(int)FileUpdateType.Assembly2] = DisplayType.Short;
        }

        public void SetDisplayTypeForVersion(FileUpdateType fut, DisplayType dt) {
            displayTypes[(int)fut] = dt;
        }

        public DisplayType GetDisplayType(FileUpdateType fut) {
            return displayTypes[(int)fut];
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