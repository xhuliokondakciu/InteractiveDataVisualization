﻿using DataVisualization.Domain.Contracts;
using DataVisualization.Helper;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DataVisualization.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding)
        {
            return new JsonNetResult
            {
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                Data = data
            };
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                Data = data,
                JsonRequestBehavior = behavior
            };
        }
    }
}