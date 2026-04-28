CREATE TABLE IF NOT EXISTS tasks (
    id uuid PRIMARY KEY,
    title text NOT NULL,
    owner_id text NOT NULL,
    created timestamp with time zone NOT NULL,
    created_by text NOT NULL
);
