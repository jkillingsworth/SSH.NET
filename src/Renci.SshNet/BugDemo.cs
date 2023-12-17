using System;
using System.Diagnostics;

namespace Renci.SshNet
{
    /// <summary>
    /// The bug demo helper.
    /// </summary>
    public static class BugDemo
    {
        /// <summary>
        /// Gets or sets a value indicating whether the trace flag is set.
        /// </summary>
        public static bool Flag { get; set; }

        /// <summary>
        /// Execute debug break.
        /// </summary>
        [DebuggerHidden]
        public static void DebugBreak()
        {
            if (Flag)
            {
                Debugger.Break();
            }
        }

        /// <summary>
        /// Execute debug write.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public static void DebugWrite(string value)
        {
            if (Flag)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Execute write.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public static void Write(string value)
        {
            Console.WriteLine("[thread {0}] {1}", Environment.CurrentManagedThreadId, value);
        }
    }
}
