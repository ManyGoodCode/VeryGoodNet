using System;

namespace SharpDX.Direct3D
{
    public partial struct ShaderMacro : IEquatable<ShaderMacro>
    {
        public ShaderMacro(string name, object definition)
        {
            Name = name;
            Definition = definition == null ? null : definition.ToString();
        }

        public bool Equals(ShaderMacro other)
        {
            return string.Equals(this.Name, other.Name) && string.Equals(this.Definition, other.Definition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ShaderMacro && Equals((ShaderMacro)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name != null ? this.Name.GetHashCode() : 0) * 397) ^ (this.Definition != null ? this.Definition.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ShaderMacro left, ShaderMacro right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderMacro left, ShaderMacro right)
        {
            return !left.Equals(right);
        }
    }
}