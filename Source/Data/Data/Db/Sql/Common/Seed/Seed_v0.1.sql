-- Product Seeding Script
-- Seeded with 1,000,000 products in case we need to test performance
DO
$$
    DECLARE
        currencies      TEXT[] := ARRAY ['USD', 'SGD', 'MYR'];
        category_ids    UUID[];
        seller_ids      UUID[];
        i               INT;
        random_text     TEXT;
    BEGIN
        -- Fetch Category and Seller IDs
        SELECT ARRAY_AGG("Id") INTO category_ids FROM "Categories";
        SELECT ARRAY_AGG("Id") INTO seller_ids FROM "Sellers";

        -- Ensure data exists
        IF category_ids IS NULL THEN
            RAISE EXCEPTION 'No categories found!';
        END IF;

        IF seller_ids IS NULL THEN
            RAISE EXCEPTION 'No sellers found!';
        END IF;

        -- Insert Products
        FOR i IN 1..1000000 LOOP
                SELECT 'Description ' || i || ' ' || string_agg(substr(md5(random()::text), 1, 8), ' ')
                INTO random_text
                FROM generate_series(1, 100); -- 110 * 8 ~ 900 chars

                INSERT INTO "Products" ("Id", "Name", "Description", "Amount", "Currency", "Quantity", "ImageUrl", "CategoryId", "SellerId", "CreatedAt", "UpdatedAt")
                VALUES (
                           gen_random_uuid(),
                           'Product ' || i || ' ' || substr(md5(random()::text), 1, 8),
                           random_text,
                           (random() * 1000)::INT + 1,
                           currencies[ceil(random() * 3)],
                           (random() * 100)::INT + 1,
                           null,
                           category_ids[ceil(random() * array_length(category_ids, 1))],
                           seller_ids[ceil(random() * array_length(seller_ids, 1))],
                           now(),
                           now()
                       );
            END LOOP;
    END;
$$;
