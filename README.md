# Web aplikacija za praćenje informacija o univerzitetskoj ligi u Novom Sadu

# Na projektu radili:
 Ognjen Damnjanović i Boriša Hrnjez

## Environment promenljive

Aplikacija ne čuva tajne u `appsettings.json` - sve dolazi iz env promenljivih.

### Baza (`DbConnectionStringBuilder`)

| Promenljiva | Default | Opis |
|---|---|---|
| `DATABASE_HOST` | `localhost` | Host Postgres servera |
| `DATABASE_PORT` | `5432` | Port Postgres servera |
| `DATABASE_SCHEMA` | `unileaguedb` | Ime baze |
| `DATABASE_USERNAME` | `postgres` | Korisnik |
| `DATABASE_PASSWORD` | `root` | Lozinka |
| `DATABASE_POOLING` | `true` | Npgsql connection pooling |

### JWT (`JwtSettingsBuilder`)

| Promenljiva | Default | Opis |
|---|---|---|
| `JWT_KEY` | **nema u produkciji** | Ključ za potpisivanje tokena, min 32 karaktera. **Obavezna van `Development` okruženja** - `Program.cs` baca izuzetak na startu ako nedostaje. Fallback vrednost je javna konstanta u repou i sme se koristiti samo lokalno. |
| `JWT_ISSUER` | `unileague` | Issuer klejm u tokenu |
| `JWT_AUDIENCE` | `unileague-web` | Audience klejm u tokenu |
| `JWT_EXPIRY_HOURS` | `12` | Trajanje tokena u satima |

### CORS (`CorsConfiguration`)

| Promenljiva | Default | Opis |
|---|---|---|
| `UNILEAGUE_CORS_ORIGINS` | `http://localhost:4200` | **Putanja do fajla** sa dozvoljenim origin-ima, po jedan po liniji (ne lista origin-a direktno) |
