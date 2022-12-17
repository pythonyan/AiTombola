using iTombola.Core.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Cognitive.Configurations
{
	internal class FileSystemAudioConverterConfiguration
	{
		const string ConfigRootName = "AudioConverter";
		public string SpeechKey { get; set; }
		public string SpeechRegion { get; set; }
		public string AudioFilePath { get; set; }

		// For info about voice name look at https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=stt-tts#text-to-speech

		public string VoiceName { get; set; }

		public static FileSystemAudioConverterConfiguration Load(IConfiguration config)
		{
			var retVal = new FileSystemAudioConverterConfiguration();
			retVal.SpeechKey = config[$"{ConfigRootName}:SpeechKey"];
			retVal.SpeechRegion = config[$"{ConfigRootName}:SpeechRegion"];
			retVal.AudioFilePath = config[$"{ConfigRootName}:AudioFilePath"];
			retVal.VoiceName = config[$"{ConfigRootName}:VoiceName"];
			return retVal;
		}

		public string GetAudioFullPath() => FilePathUtility.GetAbsolutePath(this.AudioFilePath);
	}
}
