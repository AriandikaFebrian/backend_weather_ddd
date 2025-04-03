// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

global using System;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Ardalis.GuardClauses;
global using AutoMapper;
global using AutoMapper.QueryableExtensions;
global using FluentValidation;
global using MediatR;
global using Microsoft.EntityFrameworkCore;
global using NetCa.Application.Common.Dtos;
global using NetCa.Application.Common.Exceptions;
global using NetCa.Application.Common.Extensions;
global using NetCa.Application.Common.Interfaces;
global using NetCa.Application.Common.Models;
global using NetCa.Domain.Constants;
global using NetCa.Domain.Entities;
global using Z.EntityFramework.Plus;
