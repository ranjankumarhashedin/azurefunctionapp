#r "Newtonsoft.Json"
#r "Microsoft.Azure.WebJobs.Extensions.CosmosDB"
#r "Microsoft.Azure.DocumentDB.Core"


using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Configuration; 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
public class Employee
{  
  public string id { get; set; }
  public string name { get; set; }
  public string dept { get; set; }
  public string mobileno { get; set; }
}

public static async Task<IActionResult> Run(  HttpRequest req , ILogger log, string Id )
{      
string connectionString = "CONNECTIONSTRING";
string endpoint = Environment.GetEnvironmentVariable(connectionString);
string collectionName = "records";
string databaseName = "employeedb";

log.LogInformation(endpoint);
string uri = endpoint.Substring(endpoint.IndexOf("https://"),endpoint.IndexOf("443/") - endpoint.IndexOf("https://") + 4);  
string accesskey = endpoint.Substring(endpoint.IndexOf("AccountKey=")+11).Remove(endpoint.Substring(endpoint.IndexOf("AccountKey=")+11).Length - 1, 1); 

 DocumentClient client = new DocumentClient(new Uri(uri),accesskey); 
 string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
  var updated = JsonConvert.DeserializeObject<Employee>(requestBody);
  
  var option = new FeedOptions { EnableCrossPartitionQuery = true };
  var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

  var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == Id).AsEnumerable().FirstOrDefault();
  
  if (document == null)
  {
    return new NotFoundResult();
  }
  //document.SetPropertyValue("employeeId", updated.employeeId); 
  document.SetPropertyValue("name", updated.name); 
  document.SetPropertyValue("dept", updated.dept);
  document.SetPropertyValue("mobileno", updated.mobileno);
  await client.ReplaceDocumentAsync(document);
  return (ActionResult)new OkObjectResult(document);
}
