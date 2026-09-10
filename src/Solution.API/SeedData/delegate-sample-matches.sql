-- Uzorak podataka za rucno testiranje unosa rezultata (faza 2a).
-- Pokrenuti nad 'unileague' semom posle EnsureCreated. Identity nalozi se seed-uju posebno
-- (SeedData/identity-users.sql).
--
-- Po jedna liga za sva tri sporta, CETIRI tima po ligi (sa redovima u tabeli na nuli),
-- i po jedan mec bez rezultata izmedju prva dva tima: -100 (fudbal), -200 (kosarka),
-- -300 (odbojka). Tako se posle unosa jednog meca vidi da tabela i dalje ima sva cetiri
-- tima (dva sa nulama), i da PlayoffService ima >= 4 tima.

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
(-3, 'Volleyball', 'Male');

INSERT INTO unileague."Teams" ("Id", "Name", "LogoUrl") VALUES
(-10, 'FK Zvezda',      '/logos/zvezda.png'),
(-11, 'FK Partizan',    '/logos/partizan.png'),
(-12, 'FK Vojvodina',   '/logos/vojvodina.png'),
(-13, 'FK Cukaricki',   '/logos/cukaricki.png'),
(-20, 'KK Zvezda',      '/logos/zvezda.png'),
(-21, 'KK Partizan',    '/logos/partizan.png'),
(-22, 'KK Mega',        '/logos/mega.png'),
(-23, 'KK FMP',         '/logos/fmp.png'),
(-30, 'OK Vojvodina',   '/logos/vojvodina.png'),
(-31, 'OK Partizan',    '/logos/partizan.png'),
(-32, 'OK Crvena zvezda','/logos/zvezda.png'),
(-33, 'OK Ribnica',     '/logos/ribnica.png');

-- Rosteri za timove koji igraju mec (kosarka i odbojka biraju iz rostera)
INSERT INTO unileague."Players" ("Id", "TeamId", "FirstName", "LastName", "JerseyNumber", "ImageUrl") VALUES
(-2001, -20, 'Marko',      'Guduric',      4,  NULL),
(-2002, -20, 'Stefan',     'Jovic',        5,  NULL),
(-2003, -20, 'Nikola',     'Kalinic',      33, NULL),
(-2101, -21, 'Petar',      'Petrovic',     7,  NULL),
(-2102, -21, 'Milan',      'Milanovic',    8,  NULL),
(-2103, -21, 'Ognjen',     'Dobric',       11, NULL),
(-3001, -30, 'Uros',       'Kovacevic',    17, NULL),
(-3002, -30, 'Marko',      'Podrascanin',  12, NULL),
(-3003, -30, 'Aleksandar', 'Atanasijevic', 14, NULL),
(-3101, -31, 'Drazen',     'Luburic',      9,  NULL),
(-3102, -31, 'Neven',      'Majstorovic',  3,  NULL),
(-3103, -31, 'Nemanja',    'Petric',       6,  NULL);

-- Tabela: sva cetiri tima po ligi na nuli (recalc ih prepisuje, ali su polazni spisak)
INSERT INTO unileague."Standings"
    ("LeagueId", "TeamId", "TeamName", "LogoUrl", "Played", "Won", "Drawn", "Lost", "Points", "Scored", "Conceded", "SetWon", "SetLost")
VALUES
(-1, -10, 'FK Zvezda',       '/logos/zvezda.png',    0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-1, -11, 'FK Partizan',     '/logos/partizan.png',  0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-1, -12, 'FK Vojvodina',    '/logos/vojvodina.png', 0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-1, -13, 'FK Cukaricki',    '/logos/cukaricki.png', 0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -20, 'KK Zvezda',       '/logos/zvezda.png',    0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -21, 'KK Partizan',     '/logos/partizan.png',  0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -22, 'KK Mega',         '/logos/mega.png',      0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-2, -23, 'KK FMP',          '/logos/fmp.png',       0, 0, 0, 0, 0, 0, 0, NULL, NULL),
(-3, -30, 'OK Vojvodina',    '/logos/vojvodina.png', 0, 0, 0, 0, 0, 0, 0, 0, 0),
(-3, -31, 'OK Partizan',     '/logos/partizan.png',  0, 0, 0, 0, 0, 0, 0, 0, 0),
(-3, -32, 'OK Crvena zvezda','/logos/zvezda.png',    0, 0, 0, 0, 0, 0, 0, 0, 0),
(-3, -33, 'OK Ribnica',      '/logos/ribnica.png',   0, 0, 0, 0, 0, 0, 0, 0, 0);

INSERT INTO unileague."Matches"
    ("Id", "LeagueId", "RoundNumber",
     "HomeTeamId", "HomeTeamName", "HomeTeamLogoUrl",
     "AwayTeamId", "AwayTeamName", "AwayTeamLogoUrl",
     "ScheduledAt", "Stage", "HomeSeed", "AwaySeed", "HomeScore", "AwayScore")
VALUES
(-100, -1, 1, -10, 'FK Zvezda',    '/logos/zvezda.png',    -11, 'FK Partizan', '/logos/partizan.png',
        '2026-03-01 18:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-200, -2, 1, -20, 'KK Zvezda',    '/logos/zvezda.png',    -21, 'KK Partizan', '/logos/partizan.png',
        '2026-03-01 19:00:00', 'RegularSeason', NULL, NULL, NULL, NULL),
(-300, -3, 1, -30, 'OK Vojvodina', '/logos/vojvodina.png', -31, 'OK Partizan', '/logos/partizan.png',
        '2026-03-01 20:00:00', 'RegularSeason', NULL, NULL, NULL, NULL);
