-- Seed nalozi za Identity modul (za rucno pokretanje nad 'identity' semom).
-- BCrypt hash-evi generisani lokalno; lozinke su dokumentovane u opisu PR-a, ne ovde.

INSERT INTO identity."Users"
    ("Id", "Username", "PasswordHash", "Role", "FullName", "IsActive")
VALUES
(-1, 'admin',   '$2a$11$HZmQkk/MKJ.9BJj9yWPTb.Y40W7kAsS2BJcGNthslucwXgY6qjGx2', 'Admin',    'Administrator lige', TRUE),
(-2, 'delegat', '$2a$11$8I8o9364Cd.9W1cv/Tz2dOQQakCzg8rYfznfKm8p1DqDTbuxZh.tG', 'Delegate', 'Delegat na terenu',  TRUE)
ON CONFLICT ("Id") DO NOTHING;
