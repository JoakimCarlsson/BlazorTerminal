﻿global using Azure.Identity;

global using System.Net;
global using System.Text.Json;

global using BlazorTerminal.Api.Persistence.Entites;
global using BlazorTerminal.Api.Persistence.Services;
global using BlazorTerminal.Api.Persistence.Repository;
global using BlazorTerminal.Api.Persistence.Extensions;

global using BlazorTerminal.Api.Feature.Game;
global using BlazorTerminal.Api.Feature.Game.Create;
global using BlazorTerminal.Api.Feature.Game.Guess;
global using BlazorTerminal.Api.Feature.Game.Get;

global using BlazorTerminal.Api.Extensions;

global using Microsoft.Extensions.Configuration.AzureAppConfiguration;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Azure.Cosmos;
global using Microsoft.Azure.Cosmos.Fluent;
global using Microsoft.Extensions.Caching.Distributed;

global using Broker.SourceGenerator;
global using Broker.Abstractions;
