-- Seed nalozi za Identity modul (za rucno pokretanje nad 'identity' semom).
-- Lozinke (BCrypt hash-evi generisani lokalno):
--   admin   / admin123
--   delegat / delegat123

INSERT INTO identity."Users"
    ("Id", "Username", "PasswordHash", "Role", "FullName", "IsActive")
VALUES
(-1, 'admin',   '$2a$11$CtDF4kggiFNYnz5lAe/RR.dGfv8fARbdD5Gvh2izB5QAVx9d/w1Tm', 'Admin',    'Administrator lige', TRUE),
(-2, 'delegat', '$2a$11$uD4s5y1nLf93hns9l6Q1Fus.ggWihxa1ImB.VXCwB7LqZZ1xSR6i.', 'Delegate', 'Delegat na terenu',  TRUE)
ON CONFLICT ("Id") DO NOTHING;
