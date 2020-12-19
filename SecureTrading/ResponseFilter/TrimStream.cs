using System;
using System.IO;
using System.Text;

namespace RemovingWhiteSpacesAspNet
{
	/// <summary>
	/// Class used for trimming white chars from code sent to client.
	/// </summary>
	public class TrimStream : Stream
	{
		private Stream stream;
		private StreamWriter streamWriter;

		private Decoder dec;

		public TrimStream(Stream stm)
		{
			stream = stm;
			streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);

			dec = Encoding.UTF8.GetDecoder();
		}

		/// <summary>
		/// Flag - write '\n' before next line
		/// </summary>
		private bool bNewLine = false;
		/// <summary>
		/// Flag - lash non-blank char in line was '>'
		/// </summary>
		private bool bLastCharGT = false;
		/// <summary>
		/// Array holding white chars from end of last line between Write() calls
		/// </summary>
		private char[] arBlanks = null;

		public override void Write(byte[] buffer, int offset, int count)
		{
			int nCharCnt = dec.GetCharCount(buffer, offset, count);
			char[] result = new char[nCharCnt];
			int nDecoded = dec.GetChars(buffer, offset, count, result, 0);
			
			if (nDecoded <= 0)
				return;

			int nFirstNonBlank = -1; //position of first non-black line char
			int nLastNonBlank = -1; //position of last non-black line char
			int nFirstLineChar = 0; //position of first line char (any)

			bool bFirstLine = true;

			for (int nPos=0; nPos<=nDecoded; ++nPos)
			{
				bool bLastLine = nPos == nDecoded;

				char c = (nPos < nDecoded) ? result[nPos] : '\n';
				if (c == '\n') //handle new line
				{
					//first line, and we have important white chars from previous Write() call
					if (bFirstLine && (arBlanks != null))
					{
						//current line contains non-blank chars - write white chars from previous call
						if (nFirstNonBlank >=0)
						{
							if (arBlanks.Length > 0)
								streamWriter.Write(arBlanks, 0, arBlanks.Length);

							arBlanks = null;
							nFirstNonBlank = 0;
							bNewLine = false;
						}
					}
					bFirstLine = false;

					//current line contains any non-white chars - write them
					if (nFirstNonBlank >= 0)
					{
						if (bNewLine && (result[nFirstNonBlank] != '<')) //write new line char ?
							streamWriter.WriteLine();

						//write current line (trimmed)
						streamWriter.Write(result, nFirstNonBlank, nLastNonBlank - nFirstNonBlank + 1);

						//setting variables...
						if (!bLastLine)
						{
							nFirstNonBlank = -1;
							nLastNonBlank = -1;
							nFirstLineChar = nPos + 1;
						}
						bNewLine = !bLastCharGT;
						bLastCharGT = false;
					}

					if (bLastLine) //last line - optionally remember white chars from its end
					{
						if ((arBlanks == null) && (nFirstNonBlank < 0))
						{
							//empty line and we don't have any white chars from previous call - nothing to do
						}
						else if (nLastNonBlank < nDecoded-1) //there was white chars at end of this line
						{
							int nNumBlanks, nFirstBlank;
							if (nLastNonBlank < 0)
							{
								nNumBlanks = nDecoded - nFirstLineChar;
								nFirstBlank = nFirstLineChar;
							}
							else
							{
								nNumBlanks = nDecoded - nLastNonBlank - 1;
								nFirstBlank = nLastNonBlank + 1;
							}

							if ((arBlanks == null) || (arBlanks.Length <= 0)) //create new array?
							{
								arBlanks = new char[nNumBlanks];
								Array.Copy(result, nFirstBlank, arBlanks, 0, nNumBlanks);
							}
							else //append at end of existsing array
							{
								char[] ar = new char[arBlanks.Length + nNumBlanks];
								arBlanks.CopyTo(ar, 0);
								Array.Copy(result, nFirstBlank, ar, arBlanks.Length, nNumBlanks);
								arBlanks = ar;
							}
						}
						else //line without white-chars at end - mark this using zero-sized array
						{
							arBlanks = new char[0];
						}

						//set variable...
						bNewLine = false;
					}
				}
				else if (!Char.IsWhiteSpace(c)) //handle non-white chars
				{
					if (nFirstNonBlank < 0)
						nFirstNonBlank = nPos;
					nLastNonBlank = nPos;
					bLastCharGT = (c == '>');
				}
			}

			streamWriter.Flush();
		}

		#region Rest of Stream's function - our stream is write-only

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	
		public override bool CanRead
		{ get { return false; } }
	
		public override bool CanSeek
		{ get { return false; } }
	
		public override bool CanWrite
		{ get { return true; } }
	
		public override long Length
		{ get { throw new NotSupportedException(); } }
	
		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
	
		public override void Flush()
		{
			stream.Flush();
		}
	
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}
	
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
