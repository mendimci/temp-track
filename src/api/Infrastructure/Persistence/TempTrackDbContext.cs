using Microsoft.EntityFrameworkCore;

namespace TempTrack.Api.Infrastructure.Persistence;

public class TempTrackDbContext(DbContextOptions<TempTrackDbContext> options) : DbContext(options);
