using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace eCommerceUsers.Models
{
    public class User
    {
        public int id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? email { get; set; }
        public int? age { get; set; }
        public string? gender { get; set; }
        public string? state { get; set; }
        public string? street_address { get; set; }
        public string? postal_code { get; set; }
        public string? city { get; set; }
        public string? country { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? traffic_source { get; set; }
        public DateTime? created_at { get; set; }
    }

    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/User").WithTags(nameof(User));

            // GET /api/User - Get all users
            group.MapGet("/", async (UsersDbContext db, int page = 1, int pageSize = 10) =>
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Limit max page size

                // Get total count
                var totalUsers = await db.Users.CountAsync();
                
                // Calculate pagination info
                var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
                var skip = (page - 1) * pageSize;

                // Get paginated data
                var users = await db.Users
                    .OrderBy(u => u.id) // Always order for consistent pagination
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();
                // Create response with pagination metadata
                var response = new
                {
                    Data = users,
                    Pagination = new
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalUsers = totalUsers,
                        TotalPages = totalPages,
                        HasNext = page < totalPages,
                        HasPrevious = page > 1
                    }
                };
                return Results.Ok(response);
            })
            .WithName("GetAllUsers")
            .WithOpenApi();

            // GET /api/User/{id} - Get user by ID
            group.MapGet("/{id}", async (int id, UsersDbContext db) =>
            {
                var user = await db.Users.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound($"User with ID {id} not found");
                }

                return Results.Ok(user);
            })
            .WithName("GetUserById")
            .WithOpenApi();

            // POST /api/User - Create new user
            group.MapPost("/", async (User model, UsersDbContext db) =>
            {
                // Set creation timestamp
                model.created_at = DateTime.UtcNow;

                db.Users.Add(model);
                await db.SaveChangesAsync();

                return Results.Created($"/api/User/{model.id}", model);
            })
            .WithName("CreateUser")
            .WithOpenApi();

            // PUT /api/User/{id} - Update existing user
            group.MapPut("/{id}", async (int id, User input, UsersDbContext db) =>
            {
                var existingUser = await db.Users.FindAsync(id);

                if (existingUser == null)
                {
                    return Results.NotFound($"User with ID {id} not found");
                }

                // Update properties
                existingUser.first_name = input.first_name;
                existingUser.last_name = input.last_name;
                existingUser.email = input.email;
                existingUser.age = input.age;
                existingUser.gender = input.gender;
                existingUser.state = input.state;
                existingUser.street_address = input.street_address;
                existingUser.postal_code = input.postal_code;
                existingUser.city = input.city;
                existingUser.country = input.country;
                existingUser.latitude = input.latitude;
                existingUser.longitude = input.longitude;
                existingUser.traffic_source = input.traffic_source;

                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdateUser")
            .WithOpenApi();

            // DELETE /api/User/{id} - Delete user
            group.MapDelete("/{id}", async (int id, UsersDbContext db) =>
            {
                var user = await db.Users.FindAsync(id);

                if (user == null)
                {
                    return Results.NotFound($"User with ID {id} not found");
                }

                db.Users.Remove(user);
                await db.SaveChangesAsync();

                return Results.Ok(user);
            })
            .WithName("DeleteUser")
            .WithOpenApi();

            // Additional useful endpoints:

            // GET /api/User/search?email={email} - Find user by email
            group.MapGet("/search", async (string? email, UsersDbContext db) =>
            {
                if (string.IsNullOrEmpty(email))
                {
                    return Results.BadRequest("Email parameter is required");
                }

                var user = await db.Users
                    .FirstOrDefaultAsync(u => u.email == email);

                if (user == null)
                {
                    return Results.NotFound($"User with email {email} not found");
                }

                return Results.Ok(user);
            })
            .WithName("SearchUserByEmail")
            .WithOpenApi();

            // GET /api/User/by-state/{state} - Get users by state
            group.MapGet("/by-state/{state}", async (string state, UsersDbContext db) =>
            {
                var users = await db.Users
                    .Where(u => u.state == state)
                    .ToListAsync();

                return Results.Ok(users);
            })
            .WithName("GetUsersByState")
            .WithOpenApi();

            // GET /api/User/count - Get total user count
            group.MapGet("/count", async (UsersDbContext db) =>
            {
                var count = await db.Users.CountAsync();
                return Results.Ok(new { TotalUsers = count });
            })
            .WithName("GetUserCount")
            .WithOpenApi();
        }
    }
}