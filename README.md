
eCommrceUsers

Minimal API project using MySQL eCommerce database.

docker stop eCommerceUsers
docker rm eCommerceUsers
docker build -t ecommerceusers:dev . --progress=plain --no-cache
docker run -it -p 8089:8089 --name eCommerceUsers   -e ASPNETCORE_ENVIRONMENT=Development   ecommerceusers:dev![image](https://github.com/user-attachments/assets/fed73b11-beb2-4ebd-8c8a-40d755aefce8)
