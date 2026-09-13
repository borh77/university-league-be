-- Uzorak podataka za rucno testiranje unosa rezultata i admin akcija.
-- Pokrenuti nad 'unileague' semom posle EnsureCreated. Identity nalozi se seed-uju posebno
-- (SeedData/identity-users.sql).
--
-- Cetiri lige (fudbal, kosarka, odbojka muska, odbojka zenska), po cetiri tima u svakoj
-- (sa redovima u tabeli na nuli), roster od 9 igraca po timu, i po DVA neodigrana meca
-- po ligi (kolo 1 i kolo 2) - jedan za unos rezultata, drugi da ostane za probu admin akcija.

DELETE FROM unileague."PlayerStatLines";
DELETE FROM unileague."QuarterScores";
DELETE FROM unileague."SetScores";
DELETE FROM unileague."GoalEvents";
DELETE FROM unileague."Standings";
DELETE FROM unileague."Matches";
DELETE FROM unileague."Players";
DELETE FROM unileague."Teams";
DELETE FROM unileague."Leagues";

INSERT INTO unileague."Leagues" ("Id", "Sport", "LeagueGender") VALUES
(-1, 'Football',   NULL),
(-2, 'Basketball', NULL),
(-3, 'Volleyball', 'Male'),
(-4, 'Volleyball', 'Female');

INSERT INTO unileague."Teams" ("Id", "Name", "LogoUrl") VALUES
(-10, 'FK Zvezda',           '/logos/zvezda.png'),
(-11, 'FK Partizan',         '/logos/partizan.png'),
(-12, 'FK Vojvodina',        '/logos/vojvodina.png'),
(-13, 'FK Cukaricki',        '/logos/cukaricki.png'),
(-20, 'KK Zvezda',           '/logos/zvezda.png'),
(-21, 'KK Partizan',         '/logos/partizan.png'),
(-22, 'KK Mega',             '/logos/mega.png'),
(-23, 'KK FMP',              '/logos/fmp.png'),
(-30, 'OK Vojvodina',        '/logos/vojvodina.png'),
(-31, 'OK Partizan',         '/logos/partizan.png'),
(-32, 'OK Crvena zvezda',    '/logos/zvezda.png'),
(-33, 'OK Ribnica',          '/logos/ribnica.png'),
(-40, 'ZOK Vizura',          '/logos/vizura.png'),
(-41, 'ZOK Crvena zvezda',   '/logos/zvezda.png'),
(-42, 'ZOK Radnicki',        '/logos/radnicki.png'),
(-43, 'ZOK Jedinstvo',       '/logos/jedinstvo.png');

-- Rosteri - 9 igraca po timu, jedinstveni brojevi dresa unutar tima
INSERT INTO unileague."Players" ("Id", "TeamId", "FirstName", "LastName", "JerseyNumber", "ImageUrl") VALUES
-- FK Zvezda (-10)
(-1001, -10, 'Marko',     'Jovanovic',   1,  NULL),
(-1002, -10, 'Nikola',    'Petrovic',    4,  NULL),
(-1003, -10, 'Stefan',    'Nikolic',     5,  NULL),
(-1004, -10, 'Aleksandar','Markovic',    7,  NULL),
(-1005, -10, 'Milos',     'Djordjevic',  8,  NULL),
(-1006, -10, 'Dusan',     'Stojanovic',  9,  NULL),
(-1007, -10, 'Vladimir',  'Ilic',        10, NULL),
(-1008, -10, 'Nemanja',   'Stankovic',   11, NULL),
(-1009, -10, 'Petar',     'Pavlovic',    17, NULL),
-- FK Partizan (-11)
(-1101, -11, 'Uros',      'Milosevic',   1,  NULL),
(-1102, -11, 'Filip',     'Simic',       3,  NULL),
(-1103, -11, 'Ognjen',    'Ristic',      6,  NULL),
(-1104, -11, 'Luka',      'Kovacevic',   7,  NULL),
(-1105, -11, 'Vuk',       'Todorovic',   9,  NULL),
(-1106, -11, 'Djordje',   'Zivkovic',    10, NULL),
(-1107, -11, 'Bogdan',    'Radovanovic', 11, NULL),
(-1108, -11, 'Lazar',     'Popovic',     14, NULL),
(-1109, -11, 'Mihailo',   'Vasic',       23, NULL),
-- FK Vojvodina (-12)
(-1201, -12, 'Ivan',      'Antic',       1,  NULL),
(-1202, -12, 'Dejan',     'Mitrovic',    4,  NULL),
(-1203, -12, 'Marko',     'Jankovic',    5,  NULL),
(-1204, -12, 'Nikola',    'Obradovic',   7,  NULL),
(-1205, -12, 'Stefan',    'Lukic',       8,  NULL),
(-1206, -12, 'Aleksandar','Savic',       9,  NULL),
(-1207, -12, 'Milos',     'Kostic',      10, NULL),
(-1208, -12, 'Dusan',     'Peric',       11, NULL),
(-1209, -12, 'Vladimir',  'Blagojevic',  20, NULL),
-- FK Cukaricki (-13)
(-1301, -13, 'Nemanja',   'Milic',       1,  NULL),
(-1302, -13, 'Petar',     'Zoric',       4,  NULL),
(-1303, -13, 'Uros',      'Gavrilovic',  5,  NULL),
(-1304, -13, 'Filip',     'Vukovic',     7,  NULL),
(-1305, -13, 'Ognjen',    'Cvetkovic',   8,  NULL),
(-1306, -13, 'Luka',      'Nedeljkovic', 9,  NULL),
(-1307, -13, 'Vuk',       'Filipovic',   10, NULL),
(-1308, -13, 'Djordje',   'Simovic',     11, NULL),
(-1309, -13, 'Bogdan',    'Radic',       19, NULL),
-- KK Zvezda (-20)
(-2001, -20, 'Marko',     'Guduric',     4,  NULL),
(-2002, -20, 'Stefan',    'Jovic',       5,  NULL),
(-2003, -20, 'Nikola',    'Kalinic',     33, NULL),
(-2004, -20, 'Vasilije',  'Micic',       0,  NULL),
(-2005, -20, 'Ognjen',    'Dobric',      11, NULL),
(-2006, -20, 'Nemanja',   'Dangubic',    22, NULL),
(-2007, -20, 'Alen',      'Smailagic',   6,  NULL),
(-2008, -20, 'Petar',     'Rakicevic',   14, NULL),
(-2009, -20, 'Filip',     'Petrusev',    13, NULL),
-- KK Partizan (-21)
(-2101, -21, 'Petar',     'Petrovic',    7,  NULL),
(-2102, -21, 'Milan',     'Milanovic',   8,  NULL),
(-2103, -21, 'Ognjen',    'Dobric',      11, NULL),
(-2104, -21, 'Karlik',    'Jones',       3,  NULL),
(-2105, -21, 'Dante',     'Exum',        1,  NULL),
(-2106, -21, 'Yago',      'Dos Santos',  9,  NULL),
(-2107, -21, 'Mathias',   'Lessort',     15, NULL),
(-2108, -21, 'Zach',      'Leday',       23, NULL),
(-2109, -21, 'Sterling',  'Brown',       27, NULL),
-- KK Mega (-22)
(-2201, -22, 'Nikola',    'Djurisic',    5,  NULL),
(-2202, -22, 'Filip',     'Bulatovic',   10, NULL),
(-2203, -22, 'Bogdan',    'Dejanovic',   12, NULL),
(-2204, -22, 'Marko',     'Ljubicic',    18, NULL),
(-2205, -22, 'Stefan',    'Todorovic',   21, NULL),
(-2206, -22, 'Uros',      'Trifunovic',  24, NULL),
(-2207, -22, 'Aleksa',    'Radanov',     8,  NULL),
(-2208, -22, 'Vanja',     'Marinkovic',  17, NULL),
(-2209, -22, 'Lazar',     'Djokovic',    9,  NULL),
-- KK FMP (-23)
(-2301, -23, 'Dusan',     'Ristic',      15, NULL),
(-2302, -23, 'Marko',     'Simonovic',   6,  NULL),
(-2303, -23, 'Nikola',    'Tanaskovic',  7,  NULL),
(-2304, -23, 'Ivan',      'Paunic',      11, NULL),
(-2305, -23, 'Stefan',    'Sivacki',     13, NULL),
(-2306, -23, 'Petar',     'Aleksic',     19, NULL),
(-2307, -23, 'Ognjen',    'Jaramaz',     22, NULL),
(-2308, -23, 'Filip',     'Vukasin',     25, NULL),
(-2309, -23, 'Nemanja',   'Gordic',      30, NULL),
-- OK Vojvodina (-30)
(-3001, -30, 'Uros',      'Kovacevic',   17, NULL),
(-3002, -30, 'Marko',     'Podrascanin', 12, NULL),
(-3003, -30, 'Aleksandar','Atanasijevic',14, NULL),
(-3004, -30, 'Nikola',    'Rosic',       3,  NULL),
(-3005, -30, 'Aleksa',    'Brdar',       6,  NULL),
(-3006, -30, 'Marko',     'Ivovic',      1,  NULL),
(-3007, -30, 'Petar',     'Krsmanovic',  9,  NULL),
(-3008, -30, 'Drazen',    'Bosnjak',     11, NULL),
(-3009, -30, 'Srecko',    'Lisinac',     19, NULL),
-- OK Partizan (-31)
(-3101, -31, 'Drazen',    'Luburic',     9,  NULL),
(-3102, -31, 'Neven',     'Majstorovic', 3,  NULL),
(-3103, -31, 'Nemanja',   'Petric',      6,  NULL),
(-3104, -31, 'Marko',     'Ubovic',      4,  NULL),
(-3105, -31, 'Aleksandar','Okolic',      7,  NULL),
(-3106, -31, 'Uros',      'Nikic',       10, NULL),
(-3107, -31, 'Vasilije',  'Simic',       13, NULL),
(-3108, -31, 'Ognjen',    'Vasic',       16, NULL),
(-3109, -31, 'Bogdan',    'Vujanovic',   21, NULL),
-- OK Crvena zvezda (-32)
(-3201, -32, 'Milos',     'Nikic',       5,  NULL),
(-3202, -32, 'Vlado',     'Petkovic',    8,  NULL),
(-3203, -32, 'Sasa',      'Starovic',    2,  NULL),
(-3204, -32, 'Marko',     'Basta',       11, NULL),
(-3205, -32, 'Ivan',      'Zaric',       15, NULL),
(-3206, -32, 'Nikola',    'Melic',       18, NULL),
(-3207, -32, 'Filip',     'Rasic',       20, NULL),
(-3208, -32, 'Aleksa',    'Milutinovic', 23, NULL),
(-3209, -32, 'Dusan',     'Micanovic',   26, NULL),
-- OK Ribnica (-33)
(-3301, -33, 'Vuk',       'Todic',       1,  NULL),
(-3302, -33, 'Nemanja',   'Djuric',      4,  NULL),
(-3303, -33, 'Petar',     'Vranjes',     7,  NULL),
(-3304, -33, 'Marko',     'Klipa',       9,  NULL),
(-3305, -33, 'Stefan',    'Kaludjerovic',12, NULL),
(-3306, -33, 'Nikola',    'Jokic',       14, NULL),
(-3307, -33, 'Ognjen',    'Cebic',       17, NULL),
(-3308, -33, 'Filip',     'Stojanovic',  22, NULL),
(-3309, -33, 'Aleksandar','Veljic',      29, NULL),
-- ZOK Vizura (-40)
(-4001, -40, 'Jovana',    'Brakocevic',  1,  NULL),
(-4002, -40, 'Bojana',    'Drca',        3,  NULL),
(-4003, -40, 'Ana',       'Bjelica',     5,  NULL),
(-4004, -40, 'Milica',    'Rankovic',    7,  NULL),
(-4005, -40, 'Teodora',   'Pusic',       9,  NULL),
(-4006, -40, 'Sladjana',  'Mirkovic',    11, NULL),
(-4007, -40, 'Ivana',     'Djilas',      14, NULL),
(-4008, -40, 'Anđela',    'Simic',       17, NULL),
(-4009, -40, 'Nadja',     'Dukic',       21, NULL),
-- ZOK Crvena zvezda (-41)
(-4101, -41, 'Jovana',    'Stevanovic',  2,  NULL),
(-4102, -41, 'Milica',    'Vukovic',     4,  NULL),
(-4103, -41, 'Marija',    'Kecman',      6,  NULL),
(-4104, -41, 'Tijana',    'Boskovic',    8,  NULL),
(-4105, -41, 'Sofija',    'Rasic',       10, NULL),
(-4106, -41, 'Natasa',    'Krsmanovic',  12, NULL),
(-4107, -41, 'Ana',       'Antonijevic', 15, NULL),
(-4108, -41, 'Dragana',   'Jaksic',      19, NULL),
(-4109, -41, 'Isidora',   'Minic',       24, NULL),
-- ZOK Radnicki (-42)
(-4201, -42, 'Jovana',    'Ognjenovic',  1,  NULL),
(-4202, -42, 'Milica',    'Ivovic',      5,  NULL),
(-4203, -42, 'Teodora',   'Vukasinovic', 7,  NULL),
(-4204, -42, 'Bojana',    'Radulovic',   9,  NULL),
(-4205, -42, 'Ana',       'Milutinovic', 11, NULL),
(-4206, -42, 'Sofija',    'Maletic',     13, NULL),
(-4207, -42, 'Natasa',    'Djordjevic',  16, NULL),
(-4208, -42, 'Anđela',    'Blagojevic',  18, NULL),
(-4209, -42, 'Marija',    'Petkovic',    27, NULL),
-- ZOK Jedinstvo (-43)
(-4301, -43, 'Ivana',     'Nesovic',     2,  NULL),
(-4302, -43, 'Jovana',    'Micic',       6,  NULL),
(-4303, -43, 'Milica',    'Stanic',      8,  NULL),
(-4304, -43, 'Teodora',   'Radovic',     10, NULL),
(-4305, -43, 'Sofija',    'Jankovic',    13, NULL),
(-4306, -43, 'Ana',       'Vranjes',     15, NULL),
(-4307, -43, 'Natasa',    'Cvijic',      20, NULL),
(-4308, -43, 'Dragana',   'Simic',       22, NULL),
(-4309, -43, 'Isidora',   'Vuckovic',    28, NULL);

-- Tabela: sva cetiri tima po ligi na nuli (recalc ih prepisuje, ali su polazni spisak)
INSERT INTO unileague."Standings"
    ("LeagueId", "TeamId", "TeamName", "LogoUrl", "Played", "Won", "Drawn", "Lost", "Points", "Scored", "Conceded", "SetWon", "SetLost")
VALUES
(-1, -10, 'FK Zvezda',           '/logos/zvezda.png',    0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-1, -11, 'FK Partizan',         '/logos/partizan.png',  0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-1, -12, 'FK Vojvodina',        '/logos/vojvodina.png', 0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-1, -13, 'FK Cukaricki',        '/logos/cukaricki.png', 0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -20, 'KK Zvezda',           '/logos/zvezda.png',    0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -21, 'KK Partizan',         '/logos/partizan.png',  0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -22, 'KK Mega',             '/logos/mega.png',      0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -23, 'KK FMP',              '/logos/fmp.png',       0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-3, -30, 'OK Vojvodina',        '/logos/vojvodina.png', 0, 0, 0, 0, 0, 0, 0, 0, 0),
(-3, -31, 'OK Partizan',         '/logos/partizan.png',  0, 0, 0, 0, 0, 0, 0, 0, 0),
(-3, -32, 'OK Crvena zvezda',    '/logos/zvezda.png',    0, 0, 0, 0, 0, 0, 0, 0, 0),
(-3, -33, 'OK Ribnica',          '/logos/ribnica.png',   0, 0, 0, 0, 0, 0, 0, 0, 0),
(-4, -40, 'ZOK Vizura',          '/logos/vizura.png',    0, 0, 0, 0, 0, 0, 0, 0, 0),
(-4, -41, 'ZOK Crvena zvezda',   '/logos/zvezda.png',    0, 0, 0, 0, 0, 0, 0, 0, 0),
(-4, -42, 'ZOK Radnicki',        '/logos/radnicki.png',  0, 0, 0, 0, 0, 0, 0, 0, 0),
(-4, -43, 'ZOK Jedinstvo',       '/logos/jedinstvo.png', 0, 0, 0, 0, 0, 0, 0, 0, 0);

-- Po dva neodigrana meca po ligi (kolo 1 i kolo 2) - prvi za unos rezultata, drugi za probu admin akcija
INSERT INTO unileague."Matches"
    ("Id", "LeagueId", "RoundNumber",
     "HomeTeamId", "HomeTeamName", "HomeTeamLogoUrl",
     "AwayTeamId", "AwayTeamName", "AwayTeamLogoUrl",
     "ScheduledAt", "Stage", "HomeSeed", "AwaySeed", "HomeScore", "AwayScore")
VALUES
(-100, -1, 1, -10, 'FK Zvezda',         '/logos/zvezda.png',    -11, 'FK Partizan',       '/logos/partizan.png',
        '2026-03-01 18:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-101, -1, 2, -12, 'FK Vojvodina',      '/logos/vojvodina.png', -13, 'FK Cukaricki',      '/logos/cukaricki.png',
        '2026-03-08 18:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-200, -2, 1, -20, 'KK Zvezda',         '/logos/zvezda.png',    -21, 'KK Partizan',       '/logos/partizan.png',
        '2026-03-01 19:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-201, -2, 2, -22, 'KK Mega',           '/logos/mega.png',      -23, 'KK FMP',            '/logos/fmp.png',
        '2026-03-08 19:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-300, -3, 1, -30, 'OK Vojvodina',      '/logos/vojvodina.png', -31, 'OK Partizan',       '/logos/partizan.png',
        '2026-03-01 20:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-301, -3, 2, -32, 'OK Crvena zvezda',  '/logos/zvezda.png',    -33, 'OK Ribnica',        '/logos/ribnica.png',
        '2026-03-08 20:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-400, -4, 1, -40, 'ZOK Vizura',        '/logos/vizura.png',    -41, 'ZOK Crvena zvezda', '/logos/zvezda.png',
        '2026-03-01 21:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-401, -4, 2, -42, 'ZOK Radnicki',      '/logos/radnicki.png',  -43, 'ZOK Jedinstvo',     '/logos/jedinstvo.png',
        '2026-03-08 21:00:00', 'RegularSeason', NULL, NULL, NULL, NULL);
