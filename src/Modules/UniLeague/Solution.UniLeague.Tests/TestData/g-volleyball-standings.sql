-- Liga: Odbojka muška
INSERT INTO unileague."Leagues" ("Id", "Sport", "LeagueGender")
VALUES (-2, 'Volleyball', 'Male');

-- Liga: Odbojka ženska
INSERT INTO unileague."Leagues" ("Id", "Sport", "LeagueGender")
VALUES (-3, 'Volleyball', 'Female');

-- Muška liga — timovi
INSERT INTO unileague."Standings"
    ("LeagueId", "TeamId", "TeamName", "Played", "Won", "Drawn", "Lost", "Points", "Scored", "Conceded", "SetWon", "SetLost")
VALUES
    (-2, 10, 'Ace Spikers',   8, 7, 0, 1, 21, 890, 710, 21, 6),
    (-2, 11, 'Block Masters', 8, 5, 0, 3, 15, 820, 790, 17, 11),
    (-2, 12, 'Net Crushers',  8, 2, 0, 6,  6, 730, 840,  9, 19);

-- Ženska liga — timovi (drugačiji TeamId-evi od muških)
INSERT INTO unileague."Standings"
    ("LeagueId", "TeamId", "TeamName", "Played", "Won", "Drawn", "Lost", "Points", "Scored", "Conceded", "SetWon", "SetLost")
VALUES
    (-3, 20, 'Flame Setters',  8, 6, 0, 2, 18, 870, 780, 19, 9),
    (-3, 21, 'Swift Diggers',  8, 4, 0, 4, 12, 810, 810, 14, 14),
    (-3, 22, 'Power Servers',  8, 1, 0, 7,  3, 700, 850,  6, 20);