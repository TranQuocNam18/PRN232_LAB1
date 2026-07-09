using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Middleware;
using PRN232.LMS.CourseService.Protos;
using PRN232.LMS.CourseService.Services;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Database Context
builder.Services.AddDbContext<LmsDbContext, CourseDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Repositories
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// Services
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentServiceWithGrpc>();

// Register gRPC Client for Student Service
var studentGrpcUrl = builder.Configuration["GrpcClients:StudentService"] ?? "http://student-service:8080";
builder.Services.AddGrpcClient<StudentGrpc.StudentGrpcClient>(o =>
{
    o.Address = new Uri(studentGrpcUrl);
});

// Enable gRPC Server
builder.Services.AddGrpc();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? "PRN232-LMS-SuperSecretKey-ChangeInProduction-AtLeast32Chars!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "PRN232.LMS.API";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "PRN232.LMS.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Course Service API v1", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });

    c.DocInclusionPredicate((docName, apiDesc) => apiDesc.GroupName == docName);
});

var app = builder.Build();

// Run Migrations and Seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Semesters.AnyAsync())
    {
        // Resolve path to seed-data.sql
        DirectoryInfo? currentDirectory = new(AppContext.BaseDirectory);
        string? seedScriptPath = null;
        while (currentDirectory != null && seedScriptPath == null)
        {
            var candidatePath = Path.Combine(currentDirectory.FullName, "seed-data.sql");
            if (File.Exists(candidatePath)) { seedScriptPath = candidatePath; break; }
            currentDirectory = currentDirectory.Parent;
        }

        if (seedScriptPath != null)
        {
            var seedSql = await File.ReadAllTextAsync(seedScriptPath);
            // Split by lines and remove Students/Enrollments insertions/deletions/reseeds
            var lines = seedSql.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var filteredLines = lines.Where(line => 
                !line.Contains("Students") && 
                !line.Contains("STUDENTS") && 
                !line.Contains("Enrollments") && 
                !line.Contains("ENROLLMENTS")
            ).ToList();

            var filteredSql = string.Join("\n", filteredLines);
            await db.Database.ExecuteSqlRawAsync(filteredSql);
        }
    }

    if (!await db.Enrollments.AnyAsync())
    {
        var random = new Random(42);
        var statuses = new[] { "Active", "Completed", "Dropped", "Pending" };
        var courses = await db.Courses.ToListAsync();
        for (int i = 0; i < 500; i++)
        {
            var studentId = random.Next(1, 51); // 50 students
            var course = courses[random.Next(courses.Count)];
            db.Enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                CourseId = course.CourseId,
                EnrollDate = DateTime.UtcNow.AddDays(-random.Next(1, 300)),
                Status = statuses[random.Next(statuses.Length)]
            });
        }
        await db.SaveChangesAsync();
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            $"Course Service API {description.GroupName.ToUpperInvariant()}");
    }
    c.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<CourseGrpcService>();

app.Run();
