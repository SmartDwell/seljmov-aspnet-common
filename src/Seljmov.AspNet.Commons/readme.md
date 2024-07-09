# Seljmov.AspNet.Commons

## Description

This library contains common classes and utilities for ASP.NET Core applications.

## Installation

1. Import the NuGet package: 
```bash
dotnet add package Seljmov.AspNet.Commons
```
2. Use custom Build method in your Program.cs file:
```csharp
... other usings
using Seljmov.AspNet.Commons;

... other code
var app = builder.BuildWebApplication();
```

## Features

The BuildWebApplication method has default parameter type of `BuildOptions` which can be used to configure the application.

```csharp
/// <summary>
/// Build options for web application.
/// </summary>
public class BuildOptions
{
    /// <summary>
    /// Use JWT authentication. If true, you need to add <see cref="JwtOptions"/> to configuration.
    /// </summary>
    public bool UseJwtAuthentication { get; set; } = true;
    
    /// <summary>
    /// Use CORS.
    /// </summary>
    public bool UseCors { get; set; } = true;

    /// <summary>
    /// Authentication policies.
    /// </summary>
    public IReadOnlyCollection<string> AuthenticationPolicies { get; set; } = [];
}
```

# [Ru]

## Описание

Эта библиотека содержит общие классы и утилиты для приложений ASP.NET Core.

## Установка

1. Импортируйте NuGet пакет: 
```bash
dotnet add package Seljmov.AspNet.Commons
```
2. Используйте кастомный метод Build в вашем файле Program.cs:
```csharp
... другие using
using Seljmov.AspNet.Commons;

... другой код
var app = builder.BuildWebApplication();
```

## Возможности

Метод BuildWebApplication имеет параметр по умолчанию типа `BuildOptions`, который можно использовать для настройки приложения.

```csharp
/// <summary>
/// Опции сборки для веб-приложения.
/// </summary>
public class BuildOptions
{
    /// <summary>
    /// Использовать JWT-аутентификацию. Если true, необходимо добавить <see cref="JwtOptions"/> в конфигурацию.
    /// </summary>
    public bool UseJwtAuthentication { get; set; } = true;
    
    /// <summary>
    /// Использовать CORS.
    /// </summary>
    public bool UseCors { get; set; } = true;

    /// <summary>
    /// Политики аутентификации.
    /// </summary>
    public IReadOnlyCollection<string> AuthenticationPolicies { get; set; } = [];
}
```