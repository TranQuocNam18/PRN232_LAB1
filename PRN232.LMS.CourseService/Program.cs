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
builder.Services.AddDbContext<LmsDbContext>(opt =>
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
    var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Semesters.AnyAsync())
    {
        db.Semesters.AddRange(
            new Semester { SemesterId = 1, SemesterName = "Fall 2024", StartDate = DateTime.Parse("2024-09-01"), EndDate = DateTime.Parse("2024-12-31") },
            new Semester { SemesterId = 2, SemesterName = "Spring 2025", StartDate = DateTime.Parse("2025-01-15"), EndDate = DateTime.Parse("2025-05-31") },
            new Semester { SemesterId = 3, SemesterName = "Summer 2025", StartDate = DateTime.Parse("2025-06-01"), EndDate = DateTime.Parse("2025-08-31") },
            new Semester { SemesterId = 4, SemesterName = "Fall 2025", StartDate = DateTime.Parse("2025-09-01"), EndDate = DateTime.Parse("2025-12-31") },
            new Semester { SemesterId = 5, SemesterName = "Spring 2026", StartDate = DateTime.Parse("2026-01-15"), EndDate = DateTime.Parse("2026-05-31") }
        );
        
        await db.Database.OpenConnectionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Semesters ON");
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Semesters OFF");
        }
        finally { await db.Database.CloseConnectionAsync(); }

        db.Subjects.AddRange(
            new Subject { SubjectId = 1, SubjectCode = "CS101", SubjectName = "Introduction to Programming", Credit = 3 },
            new Subject { SubjectId = 2, SubjectCode = "CS201", SubjectName = "Data Structures", Credit = 4 },
            new Subject { SubjectId = 3, SubjectCode = "CS202", SubjectName = "Web Development", Credit = 3 },
            new Subject { SubjectId = 4, SubjectCode = "CS301", SubjectName = "Database Design", Credit = 4 },
            new Subject { SubjectId = 5, SubjectCode = "CS302", SubjectName = "Software Engineering", Credit = 3 },
            new Subject { SubjectId = 6, SubjectCode = "MATH101", SubjectName = "Calculus I", Credit = 4 },
            new Subject { SubjectId = 7, SubjectCode = "MATH201", SubjectName = "Linear Algebra", Credit = 3 },
            new Subject { SubjectId = 8, SubjectCode = "ENG101", SubjectName = "English Communication", Credit = 3 },
            new Subject { SubjectId = 9, SubjectCode = "BUS101", SubjectName = "Business Management", Credit = 3 },
            new Subject { SubjectId = 10, SubjectCode = "PHY101", SubjectName = "Physics I", Credit = 4 }
        );

        await db.Database.OpenConnectionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects ON");
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects OFF");
        }
        finally { await db.Database.CloseConnectionAsync(); }

        db.Courses.AddRange(
            new Course { CourseId = 1, CourseName = "Programming Basics A", SemesterId = 1, SubjectId = 1 },
            new Course { CourseId = 2, CourseName = "Programming Basics B", SemesterId = 1, SubjectId = 1 },
            new Course { CourseId = 3, CourseName = "Calculus I A", SemesterId = 1, SubjectId = 6 },
            new Course { CourseId = 4, CourseName = "Calculus I B", SemesterId = 1, SubjectId = 6 },
            new Course { CourseId = 5, CourseName = "English Communication A", SemesterId = 1, SubjectId = 8 },
            new Course { CourseId = 6, CourseName = "Data Structures A", SemesterId = 2, SubjectId = 2 },
            new Course { CourseId = 7, CourseName = "Web Development A", SemesterId = 2, SubjectId = 3 },
            new Course { CourseId = 8, CourseName = "Database Design A", SemesterId = 2, SubjectId = 4 },
            new Course { CourseId = 9, CourseName = "Linear Algebra A", SemesterId = 2, SubjectId = 7 },
            new Course { CourseId = 10, CourseName = "Business Management A", SemesterId = 2, SubjectId = 9 },
            new Course { CourseId = 11, CourseName = "Software Engineering A", SemesterId = 3, SubjectId = 5 },
            new Course { CourseId = 12, CourseName = "Physics I A", SemesterId = 3, SubjectId = 10 },
            new Course { CourseId = 13, CourseName = "Programming Advanced", SemesterId = 3, SubjectId = 1 },
            new Course { CourseId = 14, CourseName = "Web Development B", SemesterId = 3, SubjectId = 3 },
            new Course { CourseId = 15, CourseName = "Data Structures B", SemesterId = 4, SubjectId = 2 },
            new Course { CourseId = 16, CourseName = "Database Design B", SemesterId = 4, SubjectId = 4 },
            new Course { CourseId = 17, CourseName = "Software Engineering B", SemesterId = 4, SubjectId = 5 },
            new Course { CourseId = 18, CourseName = "Physics I B", SemesterId = 4, SubjectId = 10 },
            new Course { CourseId = 19, CourseName = "Advanced Programming", SemesterId = 5, SubjectId = 1 },
            new Course { CourseId = 20, CourseName = "Machine Learning Basics", SemesterId = 5, SubjectId = 2 }
        );
        
        await db.Database.OpenConnectionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Courses ON");
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Courses OFF");
        }
        finally { await db.Database.CloseConnectionAsync(); }
    }

    if (!await db.Students.AnyAsync())
    {
        for (int i = 1; i <= 50; i++)
        {
            db.Students.Add(new Student
            {
                StudentId = i,
                FullName = $"Dummy Student {i}",
                Email = $"dummy{i}@email.com",
                DateOfBirth = DateTime.UtcNow
            });
        }
        await db.Database.OpenConnectionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students ON");
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students OFF");
        }
        finally { await db.Database.CloseConnectionAsync(); }
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
