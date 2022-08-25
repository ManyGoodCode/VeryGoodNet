using System;

namespace SharpDX.DXGI
{
    public partial class Output
    {
        public void GetClosestMatchingMode(SharpDX.ComObject device, SharpDX.DXGI.ModeDescription modeToMatch, out SharpDX.DXGI.ModeDescription closestMatch)
        {
            FindClosestMatchingMode(ref modeToMatch, out closestMatch, device);
        }

        public ModeDescription[] GetDisplayModeList(Format format, DisplayModeEnumerationFlags flags)
        {
            int numberOfDisplayModes = 0;
            GetDisplayModeList(format, (int) flags, ref numberOfDisplayModes, null);
            ModeDescription[] list = new ModeDescription[numberOfDisplayModes];
            if (numberOfDisplayModes > 0)
                GetDisplayModeList(format, (int) flags, ref numberOfDisplayModes, list);
            return list;
        }
    }
}