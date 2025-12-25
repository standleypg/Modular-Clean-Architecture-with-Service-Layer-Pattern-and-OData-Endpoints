-- Product Seeding Script (Updated for numeric IDs and new schema)
-- Seeded with 1,000,000 products in case we need to test performance
DO
$$
DECLARE
    currencies      TEXT[] := ARRAY ['USD', 'SGD', 'MYR'];
    user_ids        NUMERIC(20,0)[];
    i               BIGINT;
    random_text     TEXT;
    category_val    INT;
BEGIN
    -- Fetch User IDs (replacing Seller IDs)
    SELECT ARRAY_AGG("Id") INTO user_ids FROM "Users" WHERE EXISTS (
        SELECT 1 FROM "UserRoles" ur
        INNER JOIN "Roles" r ON ur."RoleId" = r."Id"
        WHERE ur."UserId" = "Users"."Id" AND r."Name" = 'Seller'
    );

    -- Ensure data exists
    IF user_ids IS NULL OR array_length(user_ids, 1) = 0 THEN
        RAISE EXCEPTION 'No seller users found! Please run Seed_v1.0.sql first.';
    END IF;

    RAISE NOTICE 'Starting to seed % products...', 1000000;

    -- Insert Products
    FOR i IN 1..1000000 LOOP
        -- Generate random description
        SELECT 'Description ' || i || ' ' || string_agg(substr(md5(random()::text), 1, 8), ' ')
        INTO random_text
        FROM generate_series(1, 100); -- 100 * 8 ~ 800 chars

        -- Random category value (0-4 for Electronics, Books, Clothing, HomeKitchen, ToysGames)
        category_val := floor(random() * 5)::INT;

        INSERT INTO "Products" ("Id", "Guid", "Name", "Description","Amount", "Currency", "Quantity", "ImageUrl", "Category", "UserId", "CreatedAt", "UpdatedAt")
        VALUES (
            i,
            gen_random_uuid(),
            'Product ' || i || ' ' || substr(md5(random()::text), 1, 8),
            random_text,
            (random() * 100)::INT + 1,
            currencies[ceil(random() * 3)],
            (random() * 100)::INT + 1,
            null,
            category_val,
            user_ids[1 + (random() * (array_length(user_ids, 1) - 1))::INT],
            now(),
            now()
        );

        -- Progress indicator
        IF i % 100000 = 0 THEN
            RAISE NOTICE '% products seeded...', i;
        END IF;
    END LOOP;

    RAISE NOTICE 'Successfully seeded % products.', 1000000;
END;
$$;
