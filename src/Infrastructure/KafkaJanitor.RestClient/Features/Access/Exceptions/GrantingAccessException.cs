using System;
using System.Net;

namespace KafkaJanitor.RestClient.Features.Access.Exceptions
{
    public class GrantingAccessException : Exception
    {
        public GrantingAccessException(string capabilityRootId, HttpStatusCode httpStatusCode) : base(
            $"Failed to grant access to capabilityRootId: '{capabilityRootId}', httpstatus code returned: '({(int)httpStatusCode}) {httpStatusCode}'"){}
    }
}