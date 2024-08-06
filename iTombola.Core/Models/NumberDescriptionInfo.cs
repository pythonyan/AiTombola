using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Models
{
	public class NumberDescriptionInfo
	{
		public string? Content { get; set; }
		public DescriptionType Type { get; set; } = DescriptionType.Text;
		public string? Culture { get; set; }
		public string? Dialect { get; set; }

		public string? VoiceName { get; set; }

        public bool IsValid() 
		{
			return !string.IsNullOrWhiteSpace(Content);
		}

	}
}
