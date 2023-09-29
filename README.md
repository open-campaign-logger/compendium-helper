
# Compendium Helper

## Description:

**Compendium Helper** is a stand-alone application designed to assist users in creating
[Campaign Logger](https://app.campaign-logger.com/) campaign files from publicly available
websites.

At is core, **Compendium Helper** is a web scraper; you provide the configuration and 
it will scrape and format the content for import into the Campaign Logger application.

## Configuration Items:

### Compendiums

> **Compendium** = A collection of websites to scrape.

A compendium is a collection of sources that are related to a specific topic.  For example,
a compendium could be created for the topic of "Dwarves", and contain all of the dwarves that
are found in the various source materials that you have access to.

Once you've configured a compendium you can:
1. Execute the web scraper to scrape the sources that are contained in the compendium and generate a Campaign Logger file, or
2. Download the compendium configuration file so that you can share it with others.
 
### Sources

> **Source** = A website that contains content that you want to scrape.

Sources are the individual items that are found in a compendium.  For example, an entity
could be a specific dwarf, such as "Gimli", or it could be a group of dwarves, such as "The
Dwarves of the Lonely Mountain".

### Labels

> **Label** = A meta-data label to used to group sources.

Labels are used to categorize sources. For example, you could create labels called "Mountain",
"Hill" and "Deep" to categorize the various dwarves that are found in your compendium.

Labels are used throughout the Campaign Logger application as meta-data to help you find
content more quickly.

## Configuration:

### Compendiums

![img.png](doc/compendium-properties.png)

### SourceS

### Labels

## Dependencies:

1. **Microsoft.AspNetCore.Components**
   - **Description**: A collection of packages that provide ASP.NET Core components, allowing developers to build interactive web UIs using C#.
   - **Link**: [ASP.NET Core Components](https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-5.0)

2. **Microsoft.JSInterop**
   - **Description**: Provides interoperability between JavaScript and Blazor.
   - **Link**: [JavaScript interop in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/blazor/javascript-interop?view=aspnetcore-5.0)

3. **Radzen.Blazor**
   - **Description**: A set of native Blazor components.
   - **Link**: [Radzen Blazor Components](https://blazor.radzen.com/)

4. **Markdig**
   - **Description**: A fast, powerful, and extensible Markdown processor for .NET.
   - **Link**: [Markdig on GitHub](https://github.com/lunet-io/markdig)

5. **ReverseMarkdown**
   - **Description**: Converts HTML to Markdown.
   - **Link**: [ReverseMarkdown on GitHub](https://github.com/mysticmind/reversemarkdown-net)

6. **Newtonsoft.Json**
   - **Description**: A popular high-performance JSON framework for .NET.
   - **Link**: [Json.NET](https://www.newtonsoft.com/json)

7. **StyleCop.Analyzers**
   - **Description**: Uses Roslyn to analyze C# code to ensure it adheres to StyleCop's coding rules.
   - **Link**: [StyleCop.Analyzers on GitHub](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)

8. **Radzen.Blazor**
   - **Description**: A set of native Blazor components.
   - **Link**: [Radzen Blazor Components](https://blazor.radzen.com/)

9. **HtmlAgilityPack**
   - **Description**: A .NET library that parses HTML.
   - **Link**: [HtmlAgilityPack on GitHub](https://html-agility-pack.net/)
