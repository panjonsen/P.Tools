using System.Reflection;
using Microsoft.OpenApi.Models;
using P.Core.FreeswitchApi.Serivice;

namespace P.Core.FreeswitchApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			var webHost = builder.Configuration.GetSection("WebHoST").Value;

			var freeswitchUsername = builder.Configuration.GetSection("FreeSwitchUserName").Value;

			var freeswitchPassword = builder.Configuration.GetSection("FreeSwitchPassWord").Value;

			var freeswitchWebApiUrl = builder.Configuration.GetSection("FreeSwitchWebApiUrl").Value;



			builder.Services.AddScoped<FreeswitchService>(
				m=>new FreeswitchService(freeswitchWebApiUrl,freeswitchUsername, freeswitchPassword)
				);

			builder.Services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "FreeswitchWebApi",
					Description = $"Freeswitch 封装8080 接口,测试接口步骤 找到接口-点击tryitout-填充参数-点击Execute"
				});
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				//	IncludeXmlComments 第二参数 true 则显示 控制器 注释
				options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
			});
			var app = builder.Build();

			// Configure the HTTP request pipeline.
		
				app.UseSwagger();
				app.UseSwaggerUI();
		

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}