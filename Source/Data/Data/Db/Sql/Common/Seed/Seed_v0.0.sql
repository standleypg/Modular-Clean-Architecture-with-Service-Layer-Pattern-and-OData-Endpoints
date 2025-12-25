-- Seed Users and Roles Function
CREATE OR REPLACE FUNCTION seed_users_and_roles() RETURNS void AS
$$
DECLARE
seller_role_id UUID;
    user_role_id UUID;
    admin_role_id UUID;
    seller_id UUID;
    user_id UUID;
    user_only_id UUID;
    admin_only_id UUID;
    business_name TEXT := 'SellerJohn Sdn. Bhd.';
BEGIN
    -- Fetch role IDs
SELECT "Id" INTO seller_role_id FROM "Roles" WHERE "Name" = 'Seller';
SELECT "Id" INTO user_role_id FROM "Roles" WHERE "Name" = 'User';
SELECT "Id" INTO admin_role_id FROM "Roles" WHERE "Name" = 'Admin';

-- Not the best apprroach for password insertion, but just for seeding purposes
-- Password: P@ssw0rd (for all users, admin, and seller)

-- Seed Users (need to seed one by one to get the IDs - otherwise, returning multiple IDs is not supported)
-- Insert Seller User
INSERT INTO "Users" ("Id", "FirstName", "LastName", "Email", "SellerId", "CreatedAt", "UpdatedAt", "PasswordHash", "PasswordSalt")
VALUES (gen_random_uuid(), 'SellerJohn', 'Doe', 'seller@email.com', NULL, now(), now(), E'\\x2AD6CAFC54764BAB95007CA0EE4195BC3CEAE57580BC0024CD0BC582DCE20ED62EB3DA43BBF81F582E9C6D6F1007997682A23B617DE3533D218E7C392E706B4D', E'\\x01D5C65E2C3769D009B9981C560E3FC22753595418BD4224A711CE8136B6FE98B41BD296D325E65BC1553BA82CEBA4030F7889857CB94C0978A3219250C5E9B5FBE6BA40C6C5CA375849A98C8518F5D4E190CDFBC07F4F3B7AC15FB9FACB39354BF5BB69C6E01432FD6871CA288DB69CE3ED2D62BE2A90AD414B7A17318591C3')
    RETURNING "Id" INTO user_id;
INSERT INTO "Sellers" ("Id", "BusinessName", "UserId", "CreatedAt", "UpdatedAt")
VALUES (gen_random_uuid(), business_name, NULL, now(), now())
    RETURNING "Id" INTO seller_id;


-- Insert User-only User
INSERT INTO "Users" ("Id", "FirstName", "LastName", "Email", "SellerId", "CreatedAt", "UpdatedAt", "PasswordHash", "PasswordSalt")
VALUES (gen_random_uuid(), 'UserJane', 'Doe', 'user@email.com', NULL, now(), now(), E'\\x2AD6CAFC54764BAB95007CA0EE4195BC3CEAE57580BC0024CD0BC582DCE20ED62EB3DA43BBF81F582E9C6D6F1007997682A23B617DE3533D218E7C392E706B4D', E'\\x01D5C65E2C3769D009B9981C560E3FC22753595418BD4224A711CE8136B6FE98B41BD296D325E65BC1553BA82CEBA4030F7889857CB94C0978A3219250C5E9B5FBE6BA40C6C5CA375849A98C8518F5D4E190CDFBC07F4F3B7AC15FB9FACB39354BF5BB69C6E01432FD6871CA288DB69CE3ED2D62BE2A90AD414B7A17318591C3')
    RETURNING "Id" INTO user_only_id;

-- Insert Admin User
INSERT INTO "Users" ("Id", "FirstName", "LastName", "Email", "SellerId", "CreatedAt", "UpdatedAt", "PasswordHash", "PasswordSalt")
VALUES (gen_random_uuid(), 'AdminJake', 'Smith', 'admin@email.com', NULL, now(), now(), E'\\x2AD6CAFC54764BAB95007CA0EE4195BC3CEAE57580BC0024CD0BC582DCE20ED62EB3DA43BBF81F582E9C6D6F1007997682A23B617DE3533D218E7C392E706B4D', E'\\x01D5C65E2C3769D009B9981C560E3FC22753595418BD4224A711CE8136B6FE98B41BD296D325E65BC1553BA82CEBA4030F7889857CB94C0978A3219250C5E9B5FBE6BA40C6C5CA375849A98C8518F5D4E190CDFBC07F4F3B7AC15FB9FACB39354BF5BB69C6E01432FD6871CA288DB69CE3ED2D62BE2A90AD414B7A17318591C3')
    RETURNING "Id" INTO admin_only_id;


-- Assign Roles
-- 7. Assign the role to the seler user
INSERT INTO "UserRoles" ("UserId", "RoleId")
VALUES (user_id, seller_role_id);
INSERT INTO "UserRoles" ("UserId", "RoleId")
VALUES (user_id, user_role_id);

-- 8. Assign the role to the user
INSERT INTO "UserRoles" ("UserId", "RoleId")
VALUES (user_only_id, user_role_id);

-- 9. Assign the role to the admin
INSERT INTO "UserRoles" ("UserId", "RoleId")
VALUES
    (admin_only_id, admin_role_id);
END;
$$ LANGUAGE plpgsql;

-- Core Seeding Script: Categories, Roles, Users
DO
$$
BEGIN
        -- Seed Categories
INSERT INTO "Categories" ("Id", "Name", "CreatedAt", "UpdatedAt")
VALUES (gen_random_uuid(), 'Electronics', now(), now()),
       (gen_random_uuid(), 'Books', now(), now()),
       (gen_random_uuid(), 'Clothing', now(), now()),
       (gen_random_uuid(), 'Home & Kitchen', now(), now()),
       (gen_random_uuid(), 'Toys & Games', now(), now())
    ON CONFLICT DO NOTHING;

-- Seed Roles
INSERT INTO "Roles" ("Id", "Name", "Description", "CreatedAt", "UpdatedAt")
VALUES (gen_random_uuid(), 'Admin', 'Administrator role with full access', now(), now()),
       (gen_random_uuid(), 'User', 'Regular user role with limited access', now(), now()),
       (gen_random_uuid(), 'Seller', 'Seller role with permissions to manage products', now(), now())
    ON CONFLICT DO NOTHING;

-- Seeding Users and Roles
PERFORM seed_users_and_roles();
        RAISE NOTICE 'Categories, Roles, and Users seeded successfully.';
END;
$$ LANGUAGE plpgsql;
