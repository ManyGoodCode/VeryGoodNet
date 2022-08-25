// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Runtime.InteropServices;
using SharpDX.Win32;

namespace SharpDX.XInput
{
    /// <summary>
    /// A XInput controller.
    /// </summary>
    public class Controller
    {
        private readonly UserIndex userIndex;
        private static readonly IXInput xinput;

        public Controller(UserIndex userIndex = UserIndex.Any)
        {
            if(xinput == null)
            {
                throw new NotSupportedException("XInput 1.4 or 1.3 or 9.1.0 is not installed");
            }
            this.userIndex = userIndex;
        }

        // Gets the <see cref="UserIndex"/> associated with this controller.
        public UserIndex UserIndex { get { return this.userIndex; } }

        // Gets the battery information.
        public BatteryInformation GetBatteryInformation(BatteryDeviceType batteryDeviceType)
        {
            BatteryInformation temp;
            var result = ErrorCodeHelper.ToResult(xinput.XInputGetBatteryInformation((int)userIndex, batteryDeviceType, out temp));
            result.CheckError();
            return temp;
        }

        // Gets the capabilities.
        public Capabilities GetCapabilities(DeviceQueryType deviceQueryType)
        {
            Capabilities temp;
            var result = ErrorCodeHelper.ToResult(xinput.XInputGetCapabilities((int)userIndex, deviceQueryType, out temp));
            result.CheckError();
            return temp;
        }

        // Gets the capabilities.
        public bool GetCapabilities(DeviceQueryType deviceQueryType, out Capabilities capabilities)
        {
            return xinput.XInputGetCapabilities((int)userIndex, deviceQueryType, out capabilities) == 0;
        }

        // Gets the keystroke.
        public Result GetKeystroke(DeviceQueryType deviceQueryType, out Keystroke keystroke)
        {
            var result = ErrorCodeHelper.ToResult(xinput.XInputGetKeystroke((int)userIndex, (int)deviceQueryType, out keystroke));
            return result;
        }

        // Gets the state.
        public State GetState()
        {
            State temp;
            var result = ErrorCodeHelper.ToResult(xinput.XInputGetState((int)userIndex, out temp));
            result.CheckError();
            return temp;
        }

        // Gets the state.
        //if the controller is connected, <c>false</c> otherwise.</returns>
        public bool GetState(out State state)
        {
            return xinput.XInputGetState((int)userIndex, out state) == 0;
        }

        // Sets the reporting.
        public static void SetReporting(bool enableReporting)
        {
            if(xinput != null)
            {
                xinput.XInputEnable(enableReporting);
            }
        }

        // Sets the vibration 
        public Result SetVibration(Vibration vibration)
        {
            var result = ErrorCodeHelper.ToResult(xinput.XInputSetState((int)userIndex, vibration));
            result.CheckError();
            return result;
        }

        // Gets a value indicating whether this instance is connected.
        public bool IsConnected
        {
            get
            {
                State temp;
                return xinput.XInputGetState((int)userIndex, out temp) == 0;
            }
        }

        static Controller()
        {
            if(LoadLibrary("xinput1_4.dll") != IntPtr.Zero)
            {
                xinput = new XInput14();
            }
            else if (LoadLibrary("xinput1_3.dll") != IntPtr.Zero)
            {
                xinput = new XInput13();
            }
            else if (LoadLibrary("xinput9_1_0.dll") != IntPtr.Zero)
            {
                xinput = new XInput910();
            }
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode, EntryPoint = "LoadLibrary", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);
    }
}