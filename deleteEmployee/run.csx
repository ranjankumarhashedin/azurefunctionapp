#r "Newtonsoft.Json"
#r "Microsoft.Azure.DocumentDB.Core"
#r "Microsoft.Azure.WebJobs.Extensions.CosmosDB"

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
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

public static async Task<IActionResult> Run(  HttpRequest req, string Id )
{

string connectionString = "CONNECTIONSTRING";
string collectionName = "COLLECTIONNAME";
string databaseName = "DBNAME";


string endpoint = Environment.GetEnvironmentVariable(connectionString);
string uri = endpoint.Substring(endpoint.IndexOf("https://"),endpoint.IndexOf("443/") - endpoint.IndexOf("https://") + 4);  
string accesskey = endpoint.Substring(endpoint.IndexOf("AccountKey=")+11).Remove(endpoint.Substring(endpoint.IndexOf("AccountKey=")+11).Length - 1, 1); 

DocumentClient client = new DocumentClient(new Uri(uri),accesskey);
  var option = new FeedOptions { EnableCrossPartitionQuery = true };
  var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
  
  var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == Id).AsEnumerable().FirstOrDefault();
  if (document == null)
  {
    return new NotFoundResult();
  }

  await client.DeleteDocumentAsync(document.SelfLink ,new RequestOptions {PartitionKey = new Microsoft.Azure.Documents.PartitionKey(document.Id)});
  return new OkResult();
}