
## Package manager 
Add-Migration Initial -Context VirtoCommerce.CartModule.Data.Repositories.CartDbContext  -Verbose -OutputDir Migrations -Project VirtoCommerce.CartModule.Data.MySql -StartupProject VirtoCommerce.CartModule.Data.MySql  -Debug



### Entity Framework Core Commands
```
dotnet tool install --global dotnet-ef --version 6.*
```

**Generate Migrations**

```
dotnet ef migrations add Initial
dotnet ef migrations add Update1
dotnet ef migrations add Update2
```

etc..

**Apply Migrations**

`dotnet ef database update`
