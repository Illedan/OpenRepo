using System;
using System.Runtime.InteropServices;
using Avalonia.Threading;

namespace OpenRepo.Desktop.MacOS
{
    /// <summary>
    /// A system wide keyboard shortcut, using Carbon's RegisterEventHotKey. Unlike keyboard monitoring,
    /// it needs no accessibility permission, and other apps do not receive the key press.
    /// </summary>
    internal static class GlobalHotKey
    {
        private const string Carbon = "/System/Library/Frameworks/Carbon.framework/Carbon";
        private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
        private const uint KeyboardEventClass = 0x6B657962; // 'keyb'
        private const uint HotKeyPressedEvent = 5;
        private const uint CommandModifier = 1 << 8;
        private const uint ShiftModifier = 1 << 9;
        private const ushort UsQuoteKeyCode = 0x27;
        private const ushort KeyActionDisplay = 3;
        private const uint NoDeadKeys = 1;

        private static EventHandlerCallback s_callback; // Carbon holds a pointer to this, so it must not be collected.
        private static Action s_pressed;

        /// <summary>
        /// Registers ⌘ and the key that types <paramref name="character"/> on the current keyboard layout.
        /// </summary>
        /// <returns>null when registered, otherwise a message saying why not.</returns>
        public static string Register(char character, Action pressed)
        {
            var (keyCode, modifiers) = FindKey(character) ?? (UsQuoteKeyCode, 0u);
            s_pressed = pressed;
            s_callback = OnHotKeyPressed;

            var target = GetApplicationEventTarget();
            var eventTypes = new[] { new EventTypeSpec { EventClass = KeyboardEventClass, EventKind = HotKeyPressedEvent } };
            var status = InstallEventHandler(target, s_callback, (nuint)eventTypes.Length, eventTypes, IntPtr.Zero, out _);
            if (status == 0)
            {
                var id = new EventHotKeyId { Signature = 0x4F505250, Id = 1 }; // 'OPRP'
                status = RegisterEventHotKey(keyCode, CommandModifier | modifiers, id, target, 0, out _);
            }

            return status == 0 ? null : $"⌘{character} could not be registered (error {status}), another app may use it";
        }

        private static int OnHotKeyPressed(IntPtr callRef, IntPtr eventRef, IntPtr userData)
        {
            Dispatcher.UIThread.Post(() => s_pressed());
            return 0;
        }

        /// <summary>
        /// Finds the key code, and whether shift is needed, for typing the character on the current keyboard layout.
        /// </summary>
        private static (ushort KeyCode, uint Modifiers)? FindKey(char character)
        {
            var source = TISCopyCurrentKeyboardLayoutInputSource();
            if (source == IntPtr.Zero) return null;
            try
            {
                var layoutData = TISGetInputSourceProperty(source, GetUnicodeKeyLayoutDataKey());
                if (layoutData == IntPtr.Zero) return null;

                var layout = CFDataGetBytePtr(layoutData);
                var keyboardType = LMGetKbdType();
                var typed = new ushort[4];
                foreach (var modifiers in new[] { 0u, ShiftModifier })
                {
                    for (ushort keyCode = 0; keyCode < 128; keyCode++)
                    {
                        uint deadKeyState = 0;
                        var status = UCKeyTranslate(layout, keyCode, KeyActionDisplay, (modifiers >> 8) & 0xFF, keyboardType,
                            NoDeadKeys, ref deadKeyState, (nuint)typed.Length, out var length, typed);
                        if (status == 0 && length == 1 && typed[0] == character) return (keyCode, modifiers);
                    }
                }

                return null;
            }
            finally
            {
                CFRelease(source);
            }
        }

        private static IntPtr GetUnicodeKeyLayoutDataKey()
        {
            var carbon = NativeLibrary.Load(Carbon);
            return Marshal.ReadIntPtr(NativeLibrary.GetExport(carbon, "kTISPropertyUnicodeKeyLayoutData"));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EventTypeSpec
        {
            public uint EventClass;
            public uint EventKind;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EventHotKeyId
        {
            public uint Signature;
            public uint Id;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int EventHandlerCallback(IntPtr callRef, IntPtr eventRef, IntPtr userData);

        [DllImport(Carbon)]
        private static extern IntPtr GetApplicationEventTarget();

        [DllImport(Carbon)]
        private static extern int InstallEventHandler(IntPtr target, EventHandlerCallback handler, nuint typeCount,
            EventTypeSpec[] types, IntPtr userData, out IntPtr handlerRef);

        [DllImport(Carbon)]
        private static extern int RegisterEventHotKey(uint keyCode, uint modifiers, EventHotKeyId id, IntPtr target,
            uint options, out IntPtr hotKeyRef);

        [DllImport(Carbon)]
        private static extern IntPtr TISCopyCurrentKeyboardLayoutInputSource();

        [DllImport(Carbon)]
        private static extern IntPtr TISGetInputSourceProperty(IntPtr inputSource, IntPtr propertyKey);

        [DllImport(Carbon)]
        private static extern byte LMGetKbdType();

        [DllImport(Carbon)]
        private static extern int UCKeyTranslate(IntPtr keyLayout, ushort virtualKeyCode, ushort keyAction,
            uint modifierKeyState, uint keyboardType, uint options, ref uint deadKeyState, nuint maxLength,
            out nuint actualLength, [Out] ushort[] unicodeString);

        [DllImport(CoreFoundation)]
        private static extern IntPtr CFDataGetBytePtr(IntPtr data);

        [DllImport(CoreFoundation)]
        private static extern void CFRelease(IntPtr cf);
    }
}
