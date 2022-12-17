using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Models
{
	public class ExtractedNumberInfo
	{
		public string? Number { get; set; }
		public NumberDescriptionInfo? Description { get; set; }
		public Stream? NumberAudio { get; set; }
		public string? NumberAudioFilePath { get; set; }

		public Stream? DescriptionAudio { get; set; }
		public string? DescriptionAudioFilePath { get; set; }

		public double Confidence { get; set; }

		public bool HasDescription()
		{
			return Description != null && Description.IsValid();
		}
	}
}
