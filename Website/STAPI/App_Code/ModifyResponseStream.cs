using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ModifyResponseStream : System.IO.Stream
{
    private System.IO.Stream baseStream;

    public ModifyResponseStream(System.IO.Stream responseStream)
    {
        if (responseStream == null)
        {
            throw new ArgumentNullException("ResponseStream");
        }

        //Assign stream to class variable for class-wide access.
        baseStream = responseStream;
    }

    public override bool CanRead
    {
        get { return baseStream.CanRead; }
    }

    public override bool CanSeek
    {
        get { return baseStream.CanSeek; }
    }

    public override bool CanWrite
    {
        get { return baseStream.CanWrite; }
    }

    public override void Flush()
    {
        baseStream.Flush();
    }

    public override long Length
    {
        get { return baseStream.Length; }
    }

    public override long Position
    {
        get
        {
            return baseStream.Position;
        }
        set
        {
            baseStream.Position = value;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return baseStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, System.IO.SeekOrigin origin)
    {
        return baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        baseStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        //Get text from response stream.
        string originalText = System.Text.Encoding.UTF8.GetString(buffer, offset, count);

        if (originalText.StartsWith("<?"))
        {
            originalText = originalText.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            originalText = originalText.Replace("<string xmlns=\"http://tempuri.org/\">", "");
            originalText = originalText.Replace("</string>", "");
        }
        else
        {
            // Replace default json data wrapper "d:" and related character elements
            originalText = originalText.Replace("{\"d\":\"", "");
            // replase the closeing d wrapper "}
            originalText = originalText.Substring(0, originalText.Length - 2);

            //originalText = originalText.Replace("}}\"}", "}}");
        }
        originalText = originalText.Replace(@"\", "");

        // Clean out any linebreaks
        originalText = originalText.Replace(Environment.NewLine, "");

        //Write the altered text to the response stream.
        buffer = System.Text.Encoding.UTF8.GetBytes(originalText);
        this.baseStream.Write(buffer, 0, buffer.Length);

    }
}