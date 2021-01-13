#r "Newtonsoft.Json"
#r "Microsoft.Azure.WebJobs.Extensions.CosmosDB"
#r "Microsoft.Azure.DocumentDB.Core"


using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Configuration; 
using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

public class Employee
{  
  public string employeeId { get; set; }
  public string name { get; set; }
  public string dept { get; set; }
  public string mobileno { get; set; }
}
//public static async Task<IActionResult> Run(  HttpRequest req, IEnumerable<Employee> employeeDocument, ILogger log, string Id ){
public static async Task<IActionResult> Run(  HttpRequest req, ILogger log, string Id ){
string connectionString = "cf-cmp-cosmosdb_DOCUMENTDB";
string collectionString = "COLLECTIONNAME";
string databaseString = "DBNAME";

string endpoint = Environment.GetEnvironmentVariable(connectionString);
string collectionName = Environment.GetEnvironmentVariable(collectionString);
string databaseName = Environment.GetEnvironmentVariable(databaseString);

log.LogInformation(endpoint);
string uri = endpoint.Substring(endpoint.IndexOf("https://"),endpoint.IndexOf("443/") - endpoint.IndexOf("https://") + 4);  
string accesskey = endpoint.Substring(endpoint.IndexOf("AccountKey=")+11).Remove(endpoint.Substring(endpoint.IndexOf("AccountKey=")+11).Length - 1, 1); 

 DocumentClient client = new DocumentClient(new Uri(uri),accesskey); 
 string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
  var updated = JsonConvert.DeserializeObject<Employee>(requestBody);
  
  var option = new FeedOptions { EnableCrossPartitionQuery = true };
  var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

 // var employees = (List<Employee>) employeeDocument;

  // foreach (Employee obj in employees){
  //   log.LogInformation(obj.employeeId);

// SQL querying allows dynamic property access
var query = new SqlQuerySpec(
    "SELECT * FROM books b WHERE b.title = @title", 
    new SqlParameterCollection(new SqlParameter[] { new SqlParameter { Name = "@title", Value = "War and Peace" }}));

dynamic document = client.CreateDocumentQuery<dynamic>(collectionLink, query).AsEnumerable().FirstOrDefault();

 //var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.employeeId == id).AsEnumerable().FirstOrDefault();
 //var empdocument = (List<Employee>) employeeDocument;
 //var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == empdocument.id).AsEnumerable().FirstOrDefault();
  var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == Id)
        .AsEnumerable().FirstOrDefault();
  if (document == null)
  {
    return new NotFoundResult();
  }

  document.SetPropertyValue("name", updated.name); 
  document.SetPropertyValue("dept", updated.dept);
  document.SetPropertyValue("mobileno", updated.mobileno);
  
  await client.ReplaceDocumentAsync(document);
  return (ActionResult)new OkObjectResult(document);
}
