using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.StudentService.Data;
using PRN232.LMS.StudentService.Middleware;
using PRN232.LMS.StudentService.Protos;
using PRN232.LMS.StudentService.Services;
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

// Repositories & Services
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentServiceWithGrpc>();

// Register gRPC Client for Course Service
var courseGrpcUrl = builder.Configuration["GrpcClients:CourseService"] ?? "http://course-service:8080";
builder.Services.AddGrpcClient<CourseGrpc.CourseGrpcClient>(o =>
{
    o.Address = new Uri(courseGrpcUrl);
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Student Service API v1", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "Student Service API v2", Version = "v2" });

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

    if (!await db.Students.AnyAsync())
    {
        var names = new[]
        {
            "Nguyen Van An", "Tran Thi Bao", "Pham Minh Chau", "Le Hoang Duc", "Vu Thi Huong",
            "Dang Van Kien", "Hoang Thi Linh", "Ta Quoc Minh", "Bui Thi Nga", "Cao Van Phong",
            "Duong Thi Quynh", "Dinh Van Son", "Giang Thi Suong", "Hy Van Tam", "Khanh Thi Uyen",
            "Luong Van Vu", "Manh Thi Xuong", "Nhu Van Yen", "On Thi Zoa", "Phuc Van A",
            "Quan Thi Binh", "Rap Van Chi", "Sau Thi Dat", "Tan Van Em", "Ung Thi Phat",
            "Viet Van Gai", "Xuyen Thi Hao", "Yen Van Hai", "Zing Thi Ich", "An Van Ky",
            "Ba Thi Loi", "Co Van Mau", "Dam Thi Noi", "Eu Van On", "Phe Thi Phich",
            "Que Van Quang", "Re Thi Rieng", "So Van Sanh", "Tac Thi Tan", "Uc Van Ung",
            "Vo Thi Van", "Xa Van Xuong", "Yeu Thi Yem", "Zai Van Zau", "A Thi An",
            "Bang Van Bau", "Cat Thi Cuu", "Dac Van Dang", "E Thi En", "Pha Van Pheo"
        };

        var emails = new[]
        {
            "nguyenan@email.com", "tranbao@email.com", "phamminhchau@email.com", "lehoangduc@email.com", "vuthuong@email.com",
            "dangvankien@email.com", "hoangthilinh@email.com", "taquocminh@email.com", "buithinga@email.com", "caovanphong@email.com",
            "duongthiquynh@email.com", "dinhvanson@email.com", "giangthisuong@email.com", "hyvantam@email.com", "khanhthiuyen@email.com",
            "luongvanvu@email.com", "manhthixuong@email.com", "nhuvanyen@email.com", "onthizoa@email.com", "phucvana@email.com",
            "quanthibinh@email.com", "rapvanchi@email.com", "sauthidat@email.com", "tanvanem@email.com", "ungthiphat@email.com",
            "vietvangai@email.com", "xuyenthihao@email.com", "yenvanhai@email.com", "zingthiich@email.com", "anvanky@email.com",
            "bathiloi@email.com", "covanmau@email.com", "damthinoi@email.com", "euvanon@email.com", "phethiphich@email.com",
            "quevanguang@email.com", "rethirieng@email.com", "sovansanh@email.com", "tacthitan@email.com", "ucvanung@email.com",
            "vothivan@email.com", "xavanxuong@email.com", "yeuthiyem@email.com", "zaivanzau@email.com", "athian@email.com",
            "bangvanbau@email.com", "cathicuu@email.com", "dacvandang@email.com", "ethien@email.com", "phavanpheo@email.com"
        };

        var dobStart = new DateTime(2003, 1, 1);
        for (int i = 0; i < 50; i++)
        {
            db.Students.Add(new Student
            {
                StudentId = i + 1,
                FullName = names[i],
                Email = emails[i],
                DateOfBirth = dobStart.AddDays(i * 12),
                StudentCode = $"HE18{1000 + i:D4}"
            });
        }
        
        // Use raw SQL insert or enable identity insert if needed:
        await db.Database.OpenConnectionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students ON");
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students OFF");
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
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
            $"Student Service API {description.GroupName.ToUpperInvariant()}");
    }
    c.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<StudentGrpcService>();

app.Run();
