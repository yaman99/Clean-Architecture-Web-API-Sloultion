using Adsbility.Appilication.Common.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adsbility.Appilication.Common.Models
{
    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors, string message, JsonWebToken Tokens)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors.ToArray();
            JsonTokens = Tokens;
        }

        public string Message { get; set; }
        public bool Succeeded { get; set; }
        public JsonWebToken JsonTokens { get; set; }
        
        public string[] Errors { get; set; }

        public static Result Success(string message)
        {
            return new Result(true, new string[] { }, message , null);
        }
        public static Result Success()
        {
            return new Result(true, new string[] { }, null, null);
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors , null, null);
        }
        public static Result GeneralFailure(string message)
        {
            return new Result(false, new string[] { } , message , null);
        }
        public static Result ReturnToken(JsonWebToken Tokens)
        {
            return new Result(true, new string[] { }, null, Tokens);
        }
    }
}
