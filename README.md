# aelf Elastic Search Client

A C# module for Elastic Search.

- [About The Project](#about-the-project)
- [Getting Started](#getting-started)
    - [Adding Package](#adding-package)
    - [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## About The Project

This project is a C# module for Elastic Search. It provides a simple way to interact with Elastic Search from C# code for aelf related Elastic Search queries.

## Getting Started

### Adding Package

Run the following command to install it in the current project:

```sh
dotnet add package AElf.Indexing.Elasticsearch
```

### Usage

```csharp
using AElf.Indexing.Elasticsearch;

[DependsOn(
    typeof(AElfIndexingElasticsearchModule)
)]
public class YourModule : AElfModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<IndexCreateOption>(x => { x.AddModule(typeof(YourModule)); });
        //...
    }
}
```

## Contributing

If you encounter a bug or have a feature request, please use the [Issue Tracker](https://github.com/AElfProject/aelf-elastic-search-client/issues/new). The project is also open to contributions, so feel free to fork the project and open pull requests.

## License

Distributed under the Apache License. See [License](LICENSE) for more information.
Distributed under the MIT License. See [License](LICENSE) for more information.