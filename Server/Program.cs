
using System.Text.RegularExpressions;

List<Human> users = new List<Human>
{
    new (){Id=Guid.NewGuid().ToString(), Name="Ted",Age=35},
    new (){Id=Guid.NewGuid().ToString(), Name="Bred",Age=40},
    new (){Id=Guid.NewGuid().ToString(), Name="Jon",Age=23}
};
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (context) =>
{
    var response=context.Response;
    var request=context.Request;
    var path=request.Path;
    //2r769uty-6545-8o09-7ef2t174e89c
    string expressionForGuid = @"^/api/users/\w{8}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/users" && request.Method == "GET")
    {
        await GetAllHuman(response);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
    {
        string? id = path.Value?.Split("/")[3];
        await GetHuman(id, response);
    }
    else if (path == "/api/users" && request.Method == "POST")
    {
        await CreateHuman(response, request);
    }
    else if (path == "/api/users" && request.Method == "PUT")
    {
        await UpdateHuman(response, request);
    }
    else if (path == "/api/users" && request.Method == "DELETE")
    {
        string? id=path.Value?.Split("/")[3];
        await DeleteHuman(id, response);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();
async Task GetAllHuman(HttpResponse response)
{
    await response.WriteAsJsonAsync(users);
}
async Task GetHuman(string? id, HttpResponse response)
{
    Human? user=users.FirstOrDefault((u)=>u.Id == id);
    if(user != null)
    {
        await response.WriteAsJsonAsync(user);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
    }
}
async Task DeleteHuman(string? id, HttpResponse response)
{
    Human? user=users.FirstOrDefault(u=>u.Id == id);
    if (user != null) 
    {
        users.Remove(user);
        await response.WriteAsJsonAsync(user);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
    }
}
async Task CreateHuman(HttpResponse response, HttpRequest request)
{
    try
    {
        var user = await request.ReadFromJsonAsync<Human>();
        if (user != null)
        {
            user.Id = Guid.NewGuid().ToString();
            users.Add(user);
            await response.WriteAsJsonAsync(user);
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}
async Task UpdateHuman(HttpResponse response, HttpRequest request)
{
    try
    {
        Human? userData = await request.ReadFromJsonAsync<Human>();
        if (userData != null)
        {
            var user = users.FirstOrDefault(u => u.Id == userData.Id);
            if (user != null)
            {
                user.Age = userData.Age;
                user.Name = userData.Name;
                await response.WriteAsJsonAsync(user);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
            }
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
 }
public class Human
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Age {  get; set; }

}