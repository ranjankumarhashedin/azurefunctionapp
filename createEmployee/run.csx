#r "Newtonsoft.Json"

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

public static IActionResult Run(HttpRequest req, out object employeeDocument, ILogger log)
{
  
  var body = new StreamReader(req.Body).ReadToEnd();
  dynamic data = JsonConvert.DeserializeObject(body);  
  employeeDocument = new
  {
    employeeId = data.employeeId,
    name = data.name,
    dept = data.dept,
    mobileno = data.mobileno
  };
  return (ActionResult)new OkObjectResult(employeeDocument);
}

