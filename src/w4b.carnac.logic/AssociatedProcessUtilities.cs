using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Carnac.Logic {
    public static class AssociatedProcessUtilities {
        private static readonly ConcurrentDictionary<int, Process> processes = new();

        [DllImport("User32.dll")]
        private static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static Process GetAssociatedProcess() {
            int handle = GetForegroundWindow();

            if (processes.TryGetValue(handle, out Process cached)) {
                return cached;
            }

            _ = GetWindowThreadProcessId(new IntPtr(handle), out uint processId);
            try {
                Process p = Process.GetProcessById(Convert.ToInt32(processId));
                processes.TryAdd(handle, p);
                return p;
            } catch (ArgumentException) {
                return null;
            }
        }
    }
}
