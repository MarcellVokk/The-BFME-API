using System.Drawing;
using System.Runtime.InteropServices;

namespace The_BFME_API.BFME1
{
    internal static class InputHelper
    {
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern bool SetMousePos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        public static void SetMousePos(Point position)
        {
            SetMousePos(position.X, position.Y);
        }

        public static void Click(Point position, int moveTimeMs = 80)
        {
            SetMousePos(position);

            Thread.Sleep(moveTimeMs);

            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(30);
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(20);
        }
    }
}
