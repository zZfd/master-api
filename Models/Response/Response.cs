using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Response
{
    public class SuccessResponse
    {
        private readonly string status = "success";
        private string msg;
        private object content;
        public SuccessResponse(string _msg, object _content = null)
        {
            msg = _msg;
            content = _content;
        }


    }
    public class FailResponse
    {
        private readonly string status = "fail";
        private string msg;
        private object content;
        public FailResponse(string _msg, object _content = null)
        {
            msg = _msg;
            content = _content;
        }
    }
}