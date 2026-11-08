# RaceDay Part 3 – Starter package

This package is based on your uploaded Part 2 source. The MVC project and initial API image-storage changes have been added.

## Open in Visual Studio
1. Extract the ZIP to a normal development folder.
2. Open `RaceDay.Api.slnx` in Visual Studio 2026 / a Visual Studio version supporting .NET 10.
3. Set `RaceDay.Api` and `RaceDay.Mvc` as startup projects (or start the API first, then MVC).
4. Configure the API SQL Server connection and JWT key with User Secrets/environment variables.
5. Configure `AzureBlobStorage:ConnectionString` on the API only.
6. Run the EF migration commands in README before using an existing database.
7. Run API on its configured URL and set MVC `ApiSettings:BaseUrl` to that exact URL.

## Important status
This is the first implementation milestone, not a claim that every Part 3 rubric item is complete. Organiser enrolment listing/results entry, event edit/delete, route creation in the MVC workflow, secure image delivery for private Azure containers, database migration generation, and full integration testing still need completion. The build could not be executed in this environment because the .NET SDK is not installed here.

## Create the Part 3 branch locally
```bash
git checkout -b part3-mvc-azure-docker
git add .
git commit -m "feat: add RaceDay MVC project and session authentication"
```
