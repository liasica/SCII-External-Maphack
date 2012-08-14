using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Data.Data
{
	class OffsetReader //obviously not complete. This will be used for reading offsets from a file instead of doing a pattern scan every time.
	{
		XElement _File;
		string _Version;
		OffsetReader(string Filename)
		{
			FileStream fs = new FileStream(Filename, FileMode.Create, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(fs, Encoding.UTF8);
			_File = XElement.Parse(sr.ReadToEnd());
			sr.Close();
			if (_File.Element("Version") != null)
				_Version = _File.Element("Version").Value;
			else
				_Version = "-1.-1.-1.-1";
		}
	}
}
