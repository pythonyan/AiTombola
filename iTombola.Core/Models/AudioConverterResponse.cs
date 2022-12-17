using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Models
{
	public class AudioConverterResponse
	{
		public string InputText { get; set; }
		public string Culture { get; set; }
		public Stream? Audio { get; set; }
		public string? AudioFilePath { get; set; }
	}
}
