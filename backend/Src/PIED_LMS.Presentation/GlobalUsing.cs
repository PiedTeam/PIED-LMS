global using System.Security.Claims;
global using Carter;
global using Mapster;
global using MapsterMapper;
global using MediatR;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

// Contract References
global using PIED_LMS.Contract.Services.Identity.Requests;
global using PIED_LMS.Contract.Services.Identity.Responses;
global using PIED_LMS.Contract.Services.Compiler.Requests;
global using PIED_LMS.Contract.Services.Compiler.Responses;

// Application References - Commands & Queries
global using PIED_LMS.Application.UserCases.Identity.Commands.Login;
global using PIED_LMS.Application.UserCases.Identity.Commands.Register;
global using PIED_LMS.Application.UserCases.Identity.Commands.Logout;
global using PIED_LMS.Application.UserCases.Identity.Commands.RefreshToken;
global using PIED_LMS.Application.UserCases.Identity.Commands.ChangePassword;
global using PIED_LMS.Application.UserCases.Identity.Commands.AssignRole;
global using PIED_LMS.Application.UserCases.Identity.Queries.GetUserById;
global using PIED_LMS.Application.UserCases.Identity.Queries.GetAllUsers;
global using PIED_LMS.Application.UserCases.Compilers.Commands.Compile;
global using PIED_LMS.Application.UserCases.Compilers.Commands.JudgeSubmission;
global using PIED_LMS.Application.UserCases.Compilers.Commands.JudgeFromFile;
