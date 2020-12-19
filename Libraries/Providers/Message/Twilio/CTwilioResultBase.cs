using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twilio
{
    public abstract class CTwilioResultBase
    {
        /// <summary>
        /// Exception encountered during API request
        /// </summary>
        public RestException RestException { get; set; }
        /// <summary>
        /// The URI for this resource, relative to https://api.twilio.com
        /// </summary>
        public Uri Uri { get; set; }
    }

    /// <summary>
    /// Exceptions returned in the HTTP response body when something goes wrong.
    /// </summary>
    public class RestException
    {
        /// <summary>
        /// The HTTP status code for the exception.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// (Conditional) The Url of Twilio's documentation for the error code.
        /// </summary>
        public string MoreInfo { get; set; }
        /// <summary>
        /// (Conditional) An error code to find help for the exception.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// A more descriptive message regarding the exception.
        /// </summary>
        public string Message { get; set; }
    }
}
