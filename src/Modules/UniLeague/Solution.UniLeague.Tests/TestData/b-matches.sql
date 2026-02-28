-- Mečevi bez rezultata (raspored)
INSERT INTO uni_league."Matches"
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
-- Mečevi sa rezultatom (odigrani)
(-3, -1, 2, -11, 'Partizan',      '/logos/partizan.png',
         -12, 'Vojvodina',         '/logos/vojvodina.png',
         '2025-09-21 18:00:00', 2, 1),
(-4, -1, 2, -13, 'Čukarički',    '/logos/cukaricki.png',
         -10, 'Crvena zvezda',    '/logos/zvezda.png',
         '2025-09-21 20:00:00', 0, 3);