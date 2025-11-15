using CleanArch.API;

var builder = WebApplication.CreateBuilder(args);

var api = new Api(builder);

await api.RunAsync();
