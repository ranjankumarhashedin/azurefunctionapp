#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;


public class Employee
{
 public string ID { get; set; }
  public string employeeId { get; set; }
  public string name { get; set; }
  public string dept { get; set; }
  public string mobileno { get; set; } 
}

public static async Task<IActionResult> Run(HttpRequest req, IEnumerable<Employee> employeeDocument, ILogger log)
{
  var employees = (List<Employee>) employeeDocument;
  return new OkObjectResult(employees);
}