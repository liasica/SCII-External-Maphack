using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Updater
{
	public class DownloadStatus : EventArgs
	{
		public int BytesDownloaded
		{
			get;
			set;
		}
		public int BytesTotal
		{
			get;
			set;
		}
	}
	
	public delegate void BytesDownloadedEventHandler(DownloadStatus e);

	public class Downloader
	{
		private bool Downloading = false;

		public event BytesDownloadedEventHandler BytesDownloaded;

		public static int GetSize(string URL)
		{
			WebRequest req = WebRequest.Create(URL);
			using (WebResponse resp = req.GetResponse())
			{
				int ContentLength;
				if (int.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
				{
					return ContentLength;
				}
			}
			return -1;
		}

		/// <summary>
		/// Downloads a file to a byte array
		/// </summary>
		/// <param name="URL">The URL of the file to download</param>
		/// <returns>A byte array containing the downloaded file, or null if there was an error</returns>
		public byte[] Download(string URL)
		{
			try
			{
				if (Downloading)
					return null;

				Downloading = true;
				byte[] FileData;

				WebRequest Req = WebRequest.Create(URL);
				WebResponse Response = Req.GetResponse();
				Stream DataStream = Response.GetResponseStream();

				byte[] Buffer = new byte[1024];

				int TotalLength = (int)Response.ContentLength;

				DownloadStatus status = new DownloadStatus();
				status.BytesDownloaded = 0;
				status.BytesTotal = TotalLength;
				if (BytesDownloaded != null)
					BytesDownloaded(status);

				using (MemoryStream ms = new MemoryStream())
				{
					while (true)
					{
						int BytesRead = DataStream.Read(Buffer, 0, Buffer.Length);

						if (BytesRead == 0)
						{

							status.BytesDownloaded = TotalLength;
							status.BytesTotal = TotalLength;
							if (BytesDownloaded != null)
								BytesDownloaded(status);
							break;
						}
						else
						{
							ms.Write(Buffer, 0, BytesRead);

							status.BytesDownloaded += BytesRead;
							status.BytesTotal = TotalLength;
							if (BytesDownloaded != null)
								BytesDownloaded(status);
						}
					}

					FileData = ms.ToArray();
				}

				DataStream.Close();
				Response.Close();
				
				return FileData;

			}
			catch (Exception ex)
			{
				return null;
			}
			finally
			{
				Downloading = false;
			}
		}

		/// <summary>
		/// Downloads a file and saves it
		/// </summary>
		/// <param name="URL">The URL of the file to download</param>
		/// <param name="Destination">The filename to use when saving the downloaded file</param>
		/// <returns>True if the download was successful, False if there was an error</returns>
		public bool DownloadToFile(string URL, string Destination)
		{
			byte[] Result = Download(URL);
			if (Result == null)
				return false;
			try
			{
				using (FileStream file = new FileStream(Destination, FileMode.Create))
					file.Write(Result, 0, Result.Length);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

	}
}