using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommanderGQL.Data;
using CommanderGQL.GraphQL;
using CommanderGQL.GraphQL.Commands;
using CommanderGQL.GraphQL.Platforms;
using GraphQL.Server.Ui.Voyager;
using HotChocolate.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommanderGQL
{
  public class Startup
  {
    private readonly IConfiguration _config;
    public Startup(IConfiguration configuration)
    {
      _config = configuration;
    }
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      //No need to setup the IConfig service as we're using the IHostBuilder which implements the service already


      services.AddPooledDbContextFactory<AppDbContext>(opt => opt.UseSqlServer
        (_config.GetConnectionString("CommandConStr")));

      services
          .AddGraphQLServer()
          .AddQueryType<Query>()
          .AddType<PlatformType>()
          .AddType<CommandType>()
          .AddMutationType<Mutation>()
          .AddSubscriptionType<Subscription>()
          .AddInMemorySubscriptions()
          .AddFiltering()
          .AddSorting();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseWebSockets();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGraphQL();
      });

      app.UseGraphQLVoyager(new GraphQLVoyagerOptions()
      {
        GraphQLEndPoint = "/graphql",
        Path = "/graphql-voyager"
      });
    }
  }
}
