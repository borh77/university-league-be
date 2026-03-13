-- Mečevi bez rezultata
INSERT INTO unileague."Matches"
    ("Id", "LeagueId", "RoundNumber",
     "HomeTeamId", "HomeTeamName", "HomeTeamLogoUrl",
     "AwayTeamId", "AwayTeamName", "AwayTeamLogoUrl",
     "ScheduledAt", "HomeScore", "AwayScore")
VALUES
(-1, -1, 1, -10, 'Crvena zvezda', '/logos/zvezda.png',
         -11, 'Partizan',         '/logos/partizan.png',
         '2025-09-14 18:00:00', NULL, NULL),
(-2, -1, 1, -12, 'Vojvodina',    '/logos/vojvodina.png',
         -13, 'Čukarički',        '/logos/cukaricki.png',
         '2025-09-14 20:00:00', NULL, NULL),

-- Fudbalski mečevi sa rezultatom bez cetvrtina i setova
(-3, -1, 2, -11, 'Partizan',     '/logos/partizan.png',
         -12, 'Vojvodina',        '/logos/vojvodina.png',
         '2025-09-21 18:00:00', 2, 1),
(-4, -1, 2, -13, 'Čukarički',   '/logos/cukaricki.png',
         -10, 'Crvena zvezda',   '/logos/zvezda.png',
         '2025-09-21 20:00:00', 0, 3),

-- Košarkaški mečevi sa četvrtinama
(-5, -2, 1, -20, 'Crvena zvezda KK', '/logos/zvezda.png',
         -21, 'Partizan KK',          '/logos/partizan.png',
         '2025-09-14 20:00:00', 102, 95),
(-6, -2, 1, -22, 'Mega MIS',         '/logos/mega.png',
         -23, 'FMP',                   '/logos/fmp.png',
         '2025-09-14 22:00:00', 88, 91),

-- Odbojkaški mečevi sa setovima 
(-7, -3, 1, -30, 'Vojvodina Ribarska', '/logos/vojvodina.png',
         -31, 'Spartak Subotica',        '/logos/spartak.png',
         '2025-09-14 19:00:00', 3, 1),
(-8, -3, 1, -32, 'OK Beograd',  '/logos/okbeograd.png',
         -33, 'Mladost',          '/logos/mladost.png',
         '2025-09-14 21:00:00', 3, 2);

-- Četvrtine za meč -5 (102:95)
INSERT INTO unileague."QuarterScores"
    ("MatchId", "QuarterNumber", "HomeScore", "AwayScore")
VALUES
(-5, 1, 25, 21),
(-5, 2, 22, 25),
(-5, 3, 30, 28),
(-5, 4, 25, 21);

-- Četvrtine za meč -6 (88:91)
INSERT INTO unileague."QuarterScores"
    ("MatchId", "QuarterNumber", "HomeScore", "AwayScore")
VALUES
(-6, 1, 20, 24),
(-6, 2, 22, 21),
(-6, 3, 25, 23),
(-6, 4, 21, 23);

-- Setovi za odbojkaški meč -7 (3:1 — Vojvodina Ribarska pobedila)
INSERT INTO unileague."SetScores"
    ("MatchId", "SetNumber", "HomeScore", "AwayScore")
VALUES
(-7, 1, 25, 21),
(-7, 2, 22, 25),
(-7, 3, 25, 18),
(-7, 4, 25, 19);

-- Setovi za odbojkaški meč -8 (3:2 — OK Beograd pobedio)
INSERT INTO unileague."SetScores"
    ("MatchId", "SetNumber", "HomeScore", "AwayScore")
VALUES
(-8, 1, 25, 22),
(-8, 2, 21, 25),
(-8, 3, 25, 23),
(-8, 4, 23, 25),
(-8, 5, 15, 12);