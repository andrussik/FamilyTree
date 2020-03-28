# Sugupuu

### Ärireeglid
- Laps ei tohi olla vanem kui tema ema või isa.
- Lapsel saab olla ainult üks ema ja üks isa.
- Lisatava isiku sünnikuupäev ei tohi olla tulevikus.

~~~
dotnet ef migrations add InitialDbCreation --project DAL.App.EF --startup-project WebApp
dotnet ef database update --project DAL.App.EF --startup-project WebApp
dotnet ef database drop --project DAL.App.EF --startup-project WebApp
~~~

Generate Identity UI
~~~
dotnet aspnet-codegenerator identity -dc DAL.App.EF.AppDbContext  -f  
~~~

run in WebApp folder
~~~
dotnet aspnet-codegenerator controller -name FamilyTreesController           -actions -m FamilyTree          -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name PersonsController               -actions -m Person              -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name RelationshipsController         -actions -m Relationship        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name RelationshipTypesController     -actions -m RelationshipType    -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name GendersController               -actions -m Gender              -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
~~~
