﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danorjuela.Utils.ResponseProcessor.Interface
{
    public interface IResponseService
    {
        IActionResult ProcessException(Exception ex);
        IActionResult ProcessException(Exception ex, int? code = null);
        IActionResult ProcessResponse<T>(T data, int? code = 200);

    }
}
