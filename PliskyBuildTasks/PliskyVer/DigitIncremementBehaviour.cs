namespace Plisky.Build {

    /// <summary>
    /// The VersionIncrementBehaviour enum is the determining factor for how each element is incremented within the VersionSupport class.  Each element
    /// of the version when incrememented will do so in a different way depending on its revision behaviour.
    /// </summary>
    public enum VersionIncrementBehaviour{
        /// <summary>
        /// Fixed values do not change.  They remain constant throuought a version increment.
        /// </summary>
        Fixed,
        /// <summary>
        /// DaysSinceDate will reflect the number of days that have elapsed since the BaseDate
        /// </summary>
        DaysSinceDate,
        /// <summary>
        /// DailyAutoIncrement will increment each time that the increment is called for the current build date.  Therefore multiple builds
        /// on the same day will have incrementing versions but the next day the number resets to zero.
        /// </summary>
        DailyAutoIncrement,
        /// <summary>
        /// AutoIncrementWithReset will increment continually unless the next digit up has changed.  It therefore behaves exactly like continual
        /// increment for the major version part.  For the minor version part it will increment until the major changes then it will reset to zero. 
        /// For the build the build will increment until the minor version changes. Finally for the revision it will increment until the build version
        /// changes.
        /// </summary>
        AutoIncrementWithReset,
        /// <summary>
        /// AutoIncrementWithResetAny will increment continually unless any of the higher order digits have changed.  It therefore continually
        /// increments untill a more significant digit changes then it resets to zero.  Major digits will continally incrememnt.  Minor digits
        /// will continue to incrememnt until the major changes.  Build will continue to increment until either Major or Minor changes and finally
        /// the revision digit wil continue to increment until any of Major / Minor or Build changes.
        /// </summary>
        AutoIncrementWithResetAny,
        /// <summary>
        /// Continual increment will increment non stop until an overflow occurs. When an overflow occurs the digit is reset to 0.
        /// </summary>
        ContinualIncrement,
        /// <summary>
        /// This will return the number of whole or partial weeks since the base date.
        /// </summary>
        WeeksSinceDate,
        /// <summary>
        /// This will prompt the user for a new version identifier. If the process is not running from a GUI environment, where a prompt
        /// would cause a fault the application will treat the behaviour as fixed.
        /// </summary>
        Prompt
    };

}
