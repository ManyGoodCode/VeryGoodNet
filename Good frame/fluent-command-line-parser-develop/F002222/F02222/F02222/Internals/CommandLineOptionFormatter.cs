using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Fclp.Internals.Extensions;

namespace Fclp.Internals
{
	public class CommandLineOptionFormatter : ICommandLineOptionFormatter
	{
		public CommandLineOptionFormatter()
		{
			this.ValueText = "Value";
			this.DescriptionText = "Description";
			this.NoOptionsText = "No options have been setup";
		}


		public const string TextFormat = "\t{0}\t\t{1}\n";
		private bool ShowHeader
		{
			get { return Header != null; }
		}

		public string Header { get; set; }
		public string ValueText { get; set; }
		public string DescriptionText { get; set; }
		public string NoOptionsText { get; set; }
		public string Format(IEnumerable<ICommandLineOption> options)
		{
			if (options == null) 
				throw new ArgumentNullException("options");

			IEnumerable<ICommandLineOption> list = options.ToList();
			if (!list.Any()) 
				return this.NoOptionsText;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();

			if (ShowHeader)
			{
				sb.AppendLine(Header);
				sb.AppendLine();
			}

			IEnumerable<ICommandLineOption> ordered = (from option in list
						  orderby option.ShortName.IsNullOrWhiteSpace() == false descending , option.ShortName
						  select option).ToList();

			foreach (ICommandLineOption cmdOption in ordered)
				sb.AppendFormat(CultureInfo.CurrentUICulture, TextFormat, FormatValue(cmdOption), cmdOption.Description);
			return sb.ToString();
		}

		static string FormatValue(ICommandLineOption cmdOption)
		{
			if (cmdOption.ShortName.IsNullOrWhiteSpace())
			{
				return cmdOption.LongName;
			}
			
			if (cmdOption.LongName.IsNullOrWhiteSpace())
			{
				return cmdOption.ShortName;
			}

			return cmdOption.ShortName + ":" + cmdOption.LongName;
		}
	}
}
