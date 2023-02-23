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
					Description = $"Freeswitch ��װ8080 �ӿ�,���Խӿڲ��� �ҵ��ӿ�-���tryitout-������-���Execute"
				});
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				//	IncludeXmlComments �ڶ����� true ����ʾ ������ ע��
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