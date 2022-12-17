using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Models
{
	public class ExtractedNumbersResponse
	{
		public bool IsValid { get; set; }
		public List<ExtractedNumberInfo> Numbers { get; set; }
	}
}
