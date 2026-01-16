# PIED-LMS

## Prerequisites

This project requires **.NET 10 SDK** for local development and CI/CD builds.

### Installing .NET 10 SDK

- **Download**: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Verify installation**:

  ```bash
  dotnet --version
  ```

  Should output `10.0.x` or higher.

The repository includes a `global.json` file in the `Backend/` directory that pins the SDK version to ensure consistent builds across all environments.

## Development

Navigate to the Backend directory before running any .NET commands:

```bash
cd Backend
dotnet restore
dotnet build
```
