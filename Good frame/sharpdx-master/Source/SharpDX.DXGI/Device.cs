using System;

namespace SharpDX.DXGI
{
    public partial class Device
    {
        public Residency[] QueryResourceResidency(params ComObject[] comObjects) 
        {
            int numResources = comObjects.Length;
            Residency[] residencies = new Residency[numResources];
            QueryResourceResidency(comObjects, residencies, numResources);
            return residencies;
        }
    }
}