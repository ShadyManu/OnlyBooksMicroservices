global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Mvc;
global using System.Net;
global using AuthSrv.DTOs;
global using AuthSrv.Services.Interfaces;
global using AuthSrv.Entities;
global using AuthSrv.DTOs.UserDetails;
global using AuthSrv.DTOs.Auth;
global using AuthSrv.DTOs.User;
global using AuthSrv.Services;
global using Microsoft.EntityFrameworkCore;
global using AuthSrv.Database;
global using System.Security.Claims;
global using System.IdentityModel.Tokens.Jwt;
global using Microsoft.IdentityModel.Tokens;
global using AutoMapper;
global using AuthSrv.Mapper;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Swashbuckle.AspNetCore.Filters;
global using System.Net.Mail;
global using Microsoft.AspNetCore.Authorization;
global using BooksManagementSrv.Entities;

namespace AuthSrv
{
    public class GlobalUsing
    {
        
    }
}