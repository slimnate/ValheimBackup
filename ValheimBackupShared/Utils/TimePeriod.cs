using System;
using ValheimBackup.BO;

namespace ValheimBackup.Utils
{
    /// <summary>
    /// Utility class for calculating/converting time periods from differing
    /// units.
    /// </summary>
    public static class TimePeriod
    {
        private static readonly int SECONDS = 1000;
        private static readonly int MINUTES = SECONDS * 60;
        private static readonly int HOURS = MINUTES * 60;
        private static readonly int DAYS = HOURS * 24;
        private static readonly int WEEKS = DAYS * 7;

        /// <summary>
        /// Convert a <see cref="BackupFrequency"/> object to it's period value
        /// in milliseconds.
        /// </summary>
        /// <param name="frequency">backup frequency to convert</param>
        /// <returns>the frequency's period in milliseconds.</returns>
        public static double ToMilliseconds(BackupFrequency frequency)
        {
            return frequency.Amount * Multiplier(frequency.Period);
        }

        /// <summary>
        /// Convert a <see cref="CleanupFrequency"/> object to it's period value
        /// in milliseconds.
        /// <br/><br/>
        /// Throws an <see cref="ArgumentException"/> if the cleanup period
        /// is <see cref="CleanupPeriod.Copies"/>
        /// </summary>
        /// <param name="frequency">cleanup frequency to convert</param>
        /// <returns>the frequency's period in milliseconds.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double ToMilliseconds(CleanupFrequency frequency)
        {
            if (frequency.Period == CleanupPeriod.Copies)
            {
                throw new ArgumentException("frequency period is CleanupPeriod.Copies, cannot convert copies to seconds.");
            }
            return frequency.Amount * Multiplier(frequency.Period);
        }

        /// <summary>
        /// Calculates a <see cref="DateTime"/> object by subtracting the
        /// supplied <see cref="CleanupFrequency"/>'s time from the current time.
        /// Used to determine when files should be cleaned up.
        /// <br/><br/>
        /// Throws an <see cref="Exception"/> if the CleanupFrequency is not
        /// valid (because <see cref="CleanupPeriod.Copies"/> is selected, or
        /// some other invalid value was supplied.)
        /// </summary>
        /// <param name="frequency">frequency to calculate from</param>
        /// <returns>
        /// The date/time that was exactly one <paramref name="frequency"/>
        /// period before the current date/time.
        /// </returns>
        /// <exception cref="Exception"></exception>
        public static DateTime TimeAgo(CleanupFrequency frequency)
        {
            TimeSpan span = TimeSpan.FromSeconds(0);
            switch (frequency.Period)
            {
                case CleanupPeriod.Minutes:
                    span = TimeSpan.FromMinutes(frequency.Amount);
                    break;
                case CleanupPeriod.Hours:
                    span = TimeSpan.FromHours(frequency.Amount);
                    break;
                case CleanupPeriod.Days:
                    span = TimeSpan.FromHours(frequency.Amount);
                    break;
                case CleanupPeriod.Weeks:
                    span = TimeSpan.FromHours(frequency.Amount);
                    break;
                default:
                    throw new Exception("Can't get TimeAgo - Invalid CleanupPeriod - could be CleanupPeriod.Copies, or an invalid value");
            }

            return DateTime.Now.Subtract(span);
        }

        /// <summary>
        /// Get's the numeric multiplier needed to convert a frequency's amount
        /// from the specified <paramref name="period"/> to milliseconds.
        /// <br/><br/>
        /// Throws an <see cref="Exception"/> if the <paramref name="period"/>
        /// is not valid.
        /// </summary>
        /// <param name="period">period to get multiplier for</param>
        /// <returns>amount multiplier for period</returns>
        /// <exception cref="Exception"></exception>
        public static double Multiplier(BackupPeriod period)
        {
            switch (period)
            {
                case BackupPeriod.Minutes:
                    return MINUTES;
                case BackupPeriod.Hours:
                    return HOURS;
                case BackupPeriod.Days:
                    return DAYS;
                case BackupPeriod.Weeks:
                    return WEEKS;
                default:
                    throw new Exception("Can't get multiplier - Invalid BackupPeriod - this should NEVER happen");
            }
        }

        /// <summary>
        /// Get's the numeric multiplier needed to convert a frequency's amount
        /// from the specified <paramref name="period"/> to milliseconds.
        /// <br/><br/>
        /// Throws an <see cref="Exception"/> if the <paramref name="period"/>
        /// is not valid (because <see cref="CleanupPeriod.Copies"/> is selected, or
        /// some other invalid value was supplied.)
        /// </summary>
        /// <param name="period">period to get multiplier for</param>
        /// <returns>amount multiplier for period</returns>
        /// <exception cref="Exception"></exception>
        public static double Multiplier(CleanupPeriod period)
        {
            switch (period)
            {
                case CleanupPeriod.Minutes:
                    return MINUTES;
                case CleanupPeriod.Hours:
                    return HOURS;
                case CleanupPeriod.Days:
                    return DAYS;
                case CleanupPeriod.Weeks:
                    return WEEKS;
                default:
                    throw new Exception("Can't get multiplier - Invalid CleanupPeriod - this COULD be CleanupPeriod.Copies, or an invalid value");
            }
        }
    }
}
