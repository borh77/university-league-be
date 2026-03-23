-- Liga: Fudbal
INSERT INTO unileague."Leagues" ("Id", "Sport", "LeagueGender")
VALUES (-1, 'Football', NULL);

-- Timovi
-- Red Lions:   24 pts, Diff=+13  → 1. mesto
-- Blue Eagles: 21 pts, Diff=+7   → 2. mesto
-- Green Wolves:12 pts, Diff=-4   → 3. mesto
-- Yellow Tigers: 3 pts, Diff=-16 → 4. mesto
INSERT INTO unileague."Standings"
    ("LeagueId", "TeamId", "TeamName", "LogoUrl", "Played", "Won", "Drawn", "Lost", "Points", "Scored", "Conceded", "SetWon", "SetLost")
VALUES
    (-1, 1, 'Red Lions FC', '/logos/zvezda.png',   10, 7, 3, 0, 24, 22,  9, NULL, NULL),
    (-1, 2, 'Blue Eagles', '/logos/zvezda.png' ,    10, 6, 3, 1, 21, 18, 11, NULL, NULL),
    (-1, 3, 'Green Wolves', '/logos/zvezda.png',   10, 3, 3, 4, 12, 14, 18, NULL, NULL),
    (-1, 4, 'Yellow Tigers', '/logos/zvezda.png',  10, 1, 0, 9,  3,  6, 22, NULL, NULL);

    