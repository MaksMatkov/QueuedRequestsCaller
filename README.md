
# QueuedRequestsCallerProject

QueuedRequestsCaller is a .NET library that allows you to call requests in a queryable manner with the ability to respond to request data from one request to another. The main class, QueuedRequestsCallerService, takes a list of QueuedRequestItem objects in its constructor.

## Functionality

This project enables you to call requests in a queryable manner. It can also respond to request data from one request to another. The QueuedRequestsCallerService class takes a list of QueuedRequestItem objects as its constructor. Each QueuedRequestItem contains a RequestModel object, which is used to send the request and a list of MapCouple objects, which can be used to map the response from one request to the next.

The RequestModel object contains the method to use for the request (GET, POST, etc.), the URL of the request, parameters and headers to send with the request. The MapCouple objects can be used to map the response from one request to the parameters and headers of the next request.

The QueuedRequestsCallerService class will then execute the requests in the order they were added to the list and return the response of each request in order.

## Usage

You need to create a list of QueuedRequestItem objects and pass it to the constructor of the QueuedRequestsCallerService. The following example shows how to create a list of QueuedRequestItem objects and pass it to the constructor:

```C#
// create a new list of QueuedRequestItem objects
var callsList = new System.Collections.Generic.List<QueuedRequestsCaller.Models.QueuedRequestItem>();

// add a new QueuedRequestItem object to the list
callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem() { 
    // create a new RequestModel object and set its properties
    model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
    "https://api.namefake.com/", 
    new Dictionary<string, string>(),
    new Dictionary<string, string>(), null),
    // create a new list of MapCouple objects and set its properties
    mappingList = new List<QueuedRequestsCaller.Models.MapCouple>()
    {
        // create a new MapCouple object and set its properties
        new MapCouple()
        {
            // set the From property to a new RequestValue object
            From = new RequestValue()
            {
                FullName = "name",
                location = QueuedRequestsCaller.Enums.MappingValueLocation.Body
            },
            // set the To property to a new RequestValue object
            To = new RequestValue()
            {
                FullName = "name",
                location = QueuedRequestsCaller.Enums.MappingValueLocation.QueryParam
            }
        }
    }
});

// add another QueuedRequestItem object to the list
callsList.Add(new QueuedRequestsCaller.Models.QueuedRequestItem()
{
    // create a new RequestModel object and set its properties
    model = new QueuedRequestsCaller.Models.RequestModel(RestSharp.Method.Get,
   "https://api.nationalize.io/",
   new Dictionary<string, string>(),
   new Dictionary<string, string>(), null)
});

// create a new QueuedRequestsCallerService object with the list of QueuedRequestItem objects
QueuedRequestsCallerService caller = new QueuedRequestsCallerService(callsList);

// make the requests and store the response
var response = caller.MakeRequests();

// print the content of the response to the console
Console.WriteLine(response.Content);
```

Once you have created the list of QueuedRequestItem objects, you can call the MakeRequests method of the QueuedRequestsCallerService to execute the requests in the order they were added. The Execute method will return a list of responses in the order they were called.

## Building the Project

To build the project, you need .NET Core 3.1 or later. Once you have .NET Core installed, you can simply run `dotnet build` from the root of the project to build the project. After the build is complete, you can run the tests by running `dotnet test` from the root of the project.
