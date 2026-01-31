global using System.Security.Claims;
global using System.Text;
global using MediatR;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

// Contract References
global using PIED_LMS.Contract.Services.Identity.Responses;
global using PIED_LMS.Contract.Services.Compiler.Requests;
global using PIED_LMS.Contract.Services.Compiler.Responses;

// Domain References
global using PIED_LMS.Domain.Entities;

// Application Abstractions
global using PIED_LMS.Application.Abstractions;
