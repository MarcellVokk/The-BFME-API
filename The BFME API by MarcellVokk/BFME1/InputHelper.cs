using Gma.System.MouseKeyHook;
using System.Drawing;
using System.Runtime.InteropServices;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public static class InputHelper
    {
        private static IKeyboardMouseEvents GlobalMouseHook = Hook.GlobalEvents();
        private static Point AllowedMousePosition = new Point(0, 0);
        private static bool AllowNextMouseDown = false;
        private static bool AllowNextMouseUp = false;

        public static Action? OnEnterDown;

        public static DateTime LastInput = DateTime.Now;
        public static bool IsMouseLocked { get; private set; } = false;
        public static bool IsKeyboardLocked { get; private set; } = false;

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

        public static void Init()
        {
            GlobalMouseHook.MouseMoveExt += M_GlobalHook_MouseMoveExt;
            GlobalMouseHook.MouseDownExt += M_GlobalHook_MouseDownExt;
            GlobalMouseHook.MouseUpExt += M_GlobalHook_MouseUpExt;
            GlobalMouseHook.KeyDown += M_GlobalHook_KeyDown;
            GlobalMouseHook.KeyUp += GlobalMouseHook_KeyUp;
        }

        public static void Dispose()
        {
            GlobalMouseHook.MouseMoveExt -= M_GlobalHook_MouseMoveExt;
            GlobalMouseHook.MouseDownExt -= M_GlobalHook_MouseDownExt;
            GlobalMouseHook.MouseUpExt -= M_GlobalHook_MouseUpExt;
            GlobalMouseHook.KeyDown -= M_GlobalHook_KeyDown;
            GlobalMouseHook.KeyUp -= GlobalMouseHook_KeyUp;
            GlobalMouseHook.Dispose();
        }

        public static void SetMousePos(Point position)
        {
            AllowedMousePosition = position;
            SetMousePos(position.X, position.Y);
        }

        public static void Click(Point position, int moveTimeMs = 80)
        {
            SetMousePos(position);

            Thread.Sleep(moveTimeMs);

            AllowNextMouseDown = true;
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(30);
            AllowNextMouseUp = true;
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(20);
        }

        public static void LockInput()
        {
            SetMousePos(new Point(0, 0));
            IsMouseLocked = true;
            IsKeyboardLocked = true;
        }

        public static void UnlockInput()
        {
            IsMouseLocked = false;
            IsKeyboardLocked = false;
        }

        private static void M_GlobalHook_MouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            if (IsMouseLocked && e.Location != AllowedMousePosition)
            {
                e.Handled = true;
                SetMousePos(AllowedMousePosition.X, AllowedMousePosition.Y);
            }

            LastInput = DateTime.Now;
        }

        private static void M_GlobalHook_MouseDownExt(object? sender, MouseEventExtArgs e)
        {
            if (IsMouseLocked)
            {
                if (!AllowNextMouseDown)
                {
                    e.Handled = true;
                }
                else
                {
                    AllowNextMouseDown = false;
                }

                SetMousePos(AllowedMousePosition);
            }

            LastInput = DateTime.Now;
        }

        private static void M_GlobalHook_MouseUpExt(object? sender, MouseEventExtArgs e)
        {
            if (IsMouseLocked)
            {
                if (!AllowNextMouseUp)
                {
                    e.Handled = true;
                }
                else
                {
                    AllowNextMouseUp = false;
                }

                SetMousePos(AllowedMousePosition);
            }

            LastInput = DateTime.Now;
        }

        public static string InputBuffer = "";

        private static void M_GlobalHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            InputBuffer += e.KeyCode.ToString();

            if(InputBuffer.Length > 100)
            {
                InputBuffer = InputBuffer.Remove(0, 1);
            }

            if (IsKeyboardLocked)
            {
                e.Handled = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Enter || e.KeyCode == System.Windows.Forms.Keys.F1 || e.KeyCode == System.Windows.Forms.Keys.F2)
            {
                OnEnterDown?.Invoke();
            }

            LastInput = DateTime.Now;
        }

        private static void GlobalMouseHook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (IsKeyboardLocked)
            {
                e.Handled = true;
            }

            LastInput = DateTime.Now;
        }
    }
}
