**To test locally**

execute `dotnet run` and open a browser on http://localhost:5259

Use Swagger UI and `preprocess` API to preprocess a graphQL file content and only return the requested operation with 
the required fragment.

**To test in docker**

In main folder execute `docker compose build` and then `docker compose run`

Access the deployed container on http://localhost:8080