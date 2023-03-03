<img src="logo.png" alt= “” width="300">

# :bell:QueuedRequestsCaller - manage and execute your API requests

QueuedRequestsCaller is a .NET library that allows you to call requests in a queryable manner with the ability to :star2::grey_exclamation:respond to request data from one request to another.:star2::grey_exclamation:

## Functionality

- [x] :mailbox_with_mail: Make HTTP requests.
- [x] :football: Map data from Body to Header, Header to Query param and etc.
- [x] :horse_racing: Catch Errors in every iteration. (https://github.com/MaksMatkov/QueuedRequestsCallerProject/issues/1) 
- [x] :chart_with_upwards_trend: Get logs for every iteration. (https://github.com/MaksMatkov/QueuedRequestsCallerProject/issues/1) 
- [x] :triangular_ruler: Unit Tests. (https://github.com/MaksMatkov/QueuedRequestsCallerProject/issues/3)
- [x] :golf: Post request callbacks. (https://github.com/MaksMatkov/QueuedRequestsCallerProject/issues/2) 
- [ ] :calling: Markup parser for generating Requests List. (https://github.com/MaksMatkov/QueuedRequestsCallerProject/issues/6) 
- [ ] :mortar_board: Wiki.

## Usage

The QueuedRequestsCallerService class takes a list of QueuedRequestItem objects as its constructor. Each QueuedRequestItem contains a RequestModel object, which is used to send the request and a list of MapCouple objects, which can be used to map the response from one request to the next.

The RequestModel object contains the method to use for the request (GET, POST, etc.), the URL of the request, parameters and headers to send with the request. The MapCouple objects can be used to map the response from one request to the parameters and headers of the next request.

The QueuedRequestsCallerService class will then execute the requests in the order they were added to the list and return the response of each request in order.

You need to create a list of QueuedRequestItem objects and pass it to the constructor of the QueuedRequestsCallerService. The following example shows how to create a list of QueuedRequestItem objects and pass it to the constructor:

```C#
static void Main(string[] args)
{
  Console.WriteLine("Start");

  // Create a list of QueuedRequestItem objects, each containing a request model, mapping list, and post-request actions list
  var callsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

  // Add a request to the namefake API with a mapping from the response body to a query parameter, and two post-request actions to log some information
  callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
  {
    Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
    "https://api.namefake.com/",
    new Dictionary<string, string>(),
    new Dictionary<string, string>(), null),

    MappingList = new List<QueuedRequestsCaller.Models.MapCouple>()
    {
      new MapCouple()
      {
        From = new RequestValue()
        {
          FullName = "name",
          Location = QueuedRequestsCaller.Enums.MappingValueLocation.Body
         },
         To = new RequestValue()
         {
           FullName = "name",
           Location = QueuedRequestsCaller.Enums.MappingValueLocation.QueryParam
          }
        }
      },

      PostRequestActionsList = new List<Action<RequestModel, RequestModel>>()
      {
        LogSomeInfo,
        LogSomeInfo
      }
    });

    callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
    {
      Model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
      "https://api.nationalize.io/",
      new Dictionary<string, string>(),
      new Dictionary<string, string>(), null)
     });

   // Create a QueuedRequestsCallerService instance with the callsList as a parameter
   QueuedRequestsCallerService caller = new QueuedRequestsCallerService(new QueuedRequestsCallerSettings() { RequestsList = callsList });

   // Call MakeRequests method of QueuedRequestsCallerService to execute all requests sequentially
   var result = caller.MakeRequests();

   // Print the result status and response content or last exception message
   Console.WriteLine("Result Status: " + result.IsSuccessfully);
   if (result.IsSuccessfully)
     Console.WriteLine("Result: " + result.Response.Content);
   else
     Console.Write(result.LastException);

   Console.WriteLine("Finish");

   Console.ReadLine();
}

  // A method to log some information about the current request
  public static void LogSomeInfo(RequestModel current, RequestModel next)
  {
    Console.WriteLine("");
    Console.WriteLine(JObject.Parse(current.RequestResponse.Content));
    Console.WriteLine("");
  }
```

Once you have created the list of QueuedRequestItem objects, you can call the MakeRequests method of the QueuedRequestsCallerService to execute the requests in the order they were added. The Execute method will return a list of responses in the order they were called.

### :bulb: Use JSON for prepare requests faster

You have the ability to use JSON data to create an input list of requests with mapped data, which is a better approach for using dynamic requests. Additionally, you can save JSON as a reusable template, which can greatly improve your workflow efficiency.

```C#
public static void MakeRequestFromJson()
{
    // Read the contents of the "data.json" file into a string variable.
    var json = File.ReadAllText("data.json");

    // Create a QueuedRequestsCallerService instance with the parsed JSON data and the default settings.
    QueuedRequestsCallerService caller = new QueuedRequestsCallerService(json, new QueuedRequestsCallerSettingsParser());

    // Call the MakeRequests method to send the queued requests and get the results.
    var result = caller.MakeRequests();
}
```

## Building the Project

To build the project, you need .NET Core 3.1 or later. Once you have .NET Core installed, you can simply run `dotnet build` from the root of the project to build the project. After the build is complete, you can run the tests by running `dotnet test` from the root of the project.

## Credits

This project is built using the following open-source libraries:

- [Newtonsoft.Json.Schema](https://github.com/JamesNK/Newtonsoft.Json.Schema)
- [Remote.Linq.Newtonsoft](https://github.com/6bee/Remote.Linq)
- [RestSharp](https://github.com/restsharp/RestSharp)

We would like to thank the contributors of these projects for their amazing work.
