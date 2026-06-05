using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Services;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Repositories.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LmsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PRN232 LMS API", Version = "v1", Description = "Learning Management System RESTful API" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Semesters.AnyAsync())
    {
        DirectoryInfo? currentDirectory = new(AppContext.BaseDirectory);
        string? seedScriptPath = null;

        while (currentDirectory != null && seedScriptPath == null)
        {
            var candidatePath = Path.Combine(currentDirectory.FullName, "seed-data.sql");
            if (File.Exists(candidatePath))
            {
                seedScriptPath = candidatePath;
                break;
            }

            currentDirectory = currentDirectory.Parent;
        }

        if (seedScriptPath != null)
        {
            var seedSql = await File.ReadAllTextAsync(seedScriptPath);
            await db.Database.ExecuteSqlRawAsync(seedSql);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1"));

//app.UseHttpsRedirection();
app.MapControllers();
app.Run();