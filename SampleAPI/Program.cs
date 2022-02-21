using Microsoft.EntityFrameworkCore;
using SampleAPI;

var builder = WebApplication.CreateBuilder(args);

var data = new[]
{
   new {Id = 1,Name="Helo"}
};

var res = data.GroupBy(x => x.Id).ToLookup();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StackOverflow2010Context>(
    o => o
    .UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    .EnableSensitiveDataLogging());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/user/list",async (StackOverflow2010Context db) => {
    var query = (
    from u in db.Users
        //from p in db.Posts.Where(x => x.OwnerUserId == u.Id).Take(1)
        //join p in db.Posts on u.Id equals p.OwnerUserId into _p from p in _p.Take(1)
        join c in db.Comments on u.Id equals c.UserId
        //join v in db.Votes on p.Id equals v.PostId
    group new {u.AboutMe,c.Text} by new {UserId =u.Id,CommentId =c.Id} into g
    //select new {UserId = u.Id,CommentId = c.Id,Postid = p.Id}
    select new {g.Key,Count = g.Count()}
    ).Take(10);
    
    var response = await query.ToListAsync();
    return response;
});

app.Run();