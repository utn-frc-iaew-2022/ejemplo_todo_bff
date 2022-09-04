using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json.Linq;
using TodoBFF.Models;
using Newtonsoft.Json;

namespace TodoBFF.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoBFFController : ControllerBase
{
    private readonly ILogger<TodoBFFController> _logger;

    public TodoBFFController(ILogger<TodoBFFController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTodo")]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
    {
        //----------------------------------------------
        // Generamos token B2b
        //----------------------------------------------

        var clientAuth0 = new RestClient("https://dev-utn-frc-iaew.auth0.com");
        var requestAuth0 = new RestRequest("/oauth/token", Method.Post);
        requestAuth0.AddHeader("content-type", "application/json");
        requestAuth0.AddHeader("cache-control", "no-cache");
        requestAuth0.AddParameter("application/json", "{\"client_id\":\"1L0oJ7XzqlA8ENHt7cfeAucsJsJJITQe\",\"client_secret\":\"sVYn0LRFJdcRRsS5wy1T6Ba7Cm5ZQCM2fnlYD9PudkxaYEGHLr_-9UmRNiAHtd8s\",\"audience\":\"https://api.example.com\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
        var responseAuth0 = clientAuth0.Execute(requestAuth0);

        dynamic respAuth0 = JObject.Parse(responseAuth0.Content);
        
        string token = respAuth0.access_token;

        //----------------------------------------------
        // Consumimos la API
        //----------------------------------------------

        var client = new RestClient("https://localhost:7181");
        var request = new RestRequest("/api/todoitems", Method.Get);
        request.AddParameter("accept: application/json", ParameterType.HttpHeader);
        request.AddHeader("authorization", "Bearer " + token);

        IEnumerable<TodoItemDTO> listTodo = await client.GetAsync<IEnumerable<TodoItemDTO>>(request);

        return listTodo.ToArray();
    }
}
