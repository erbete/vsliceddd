# VSliceDDD

A deliberately over-engineered vertical-slice + (selective) DDD playground. The domain is trivial on purpose and the architecture is the experiment, not the features.

## Setup
docker compose up -d
dotnet ef migrations add <Name> --project src/Database --startup-project src/WebAPI
dotnet ef database update --project src/Database --startup-project src/WebAPI

## Run
dotnet run --project src/WebAPI

## OpenAPI
http://localhost:5243/scalar