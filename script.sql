/*

CREATE SEQUENCE seq_emp START 1;
CREATE OR REPLACE FUNCTION generate_matricule() RETURNS VARCHAR AS $$
BEGIN
  RETURN 'EMP' || nextval('seq_emp'::regclass);
END;
$$ LANGUAGE plpgsql;


-- Create a tsvector column
ALTER TABLE employers ADD COLUMN text_vector tsvector;

-- Update the tsvector column with indexed text data
UPDATE employers SET text_vector = to_tsvector('french', Nom || ' ' || Prenom);

-- Create a GIN index on the tsvector column
CREATE INDEX text_vector_idx ON employers USING GIN (text_vector);

-- Search for employers with 'John' in their name
SELECT * FROM employers WHERE text_vector @@ to_tsquery('french', 'John');

-- Search for employers with 'Doe' in their name or 'John' in their first name
SELECT * FROM employers WHERE text_vector @@ to_tsquery('french', 'Doe | John');

SELECT *,
       ts_rank(to_tsvector(e."Prenom"), to_tsquery('french', 'randy')) as probabilite
FROM employers e
WHERE to_tsvector(e."Nom" || ' ' || e."Prenom" || ' ' || e."Email" ||' ' || e."Mdp")
      @@ to_tsquery('french', 'randy | Ny');


----------------------------------------------------------------------------------------------------
DELETE FROM personnes RETURNING 1 AS isSuccessful;

WITH deleted_rows AS (
    DELETE FROM personnes
    RETURNING 1 AS isSuccessful
)
SELECT etat
FROM deleted_rows
UNION ALL
SELECT 0 AS etat
WHERE NOT EXISTS (SELECT 1 FROM deleted_rows);











*/