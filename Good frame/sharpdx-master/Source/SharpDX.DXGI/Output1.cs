using System;

namespace SharpDX.DXGI
{
    public partial class Output1
    {
        public ModeDescription1[] GetDisplayModeList1(Format enumFormat, DisplayModeEnumerationFlags flags)
        {
            int numberOfDisplayModes = 0;
            GetDisplayModeList1(enumFormat, (int)flags, ref numberOfDisplayModes, null);
            ModeDescription1[] list = new ModeDescription1[numberOfDisplayModes];
            if (numberOfDisplayModes > 0)
                GetDisplayModeList1(enumFormat, (int)flags, ref numberOfDisplayModes, list);
            return list;
        }
    }
}