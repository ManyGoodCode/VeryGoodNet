namespace Fclp.Internals.Parsing
{
	/// <summary>
	/// 解析的原始数据
	/// </summary>
	public class ParsedOption
	{
		public ParsedOption(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public ParsedOption()
		{
		}

		public string RawKey { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
		public string[] Values { get; set; }
		public string[] AdditionalValues { get; set; }
		public string Prefix { get; set; }
		public string Suffix { get; set; }

        internal ICommandLineOption SetupCommand { get; set; }

        internal int SetupOrder { get; set; }

        internal int Order { get; set; }
        public bool HasValue
		{
			get { return string.IsNullOrEmpty(Value) == false; }
		}

		public bool HasSuffix
		{
			get { return string.IsNullOrEmpty(Suffix) == false; }
		}

		protected bool Equals(ParsedOption other)
		{
			return string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) 
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType()) 
				return false;
			return Equals((ParsedOption) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Key != null ? Key.GetHashCode() : 0)*397) ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}

		public ParsedOption Clone()
		{
			return new ParsedOption
			{
				Key = Key,
				Prefix = Prefix,
				Suffix = Suffix,
				Value = Value,
				AdditionalValues = AdditionalValues,
				RawKey = RawKey,
				Values = Values
			};
		}
	}
}