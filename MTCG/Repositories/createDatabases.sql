DROP TABLE IF EXISTS cards;
DROP TABLE IF EXISTS users;

-- Create Users table
CREATE TABLE IF NOT EXISTS users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50),
    password VARCHAR(255) NOT NULL,
    coin INT DEFAULT 20 NOT NULL,
    elo INT DEFAULT 100 NOT NULL
);

-- Create Cards table
CREATE TABLE IF NOT EXISTS cards (
    card_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    type VARCHAR(50) NOT NULL,
    damage INT NOT NULL,
    owner_user_id VARCHAR(50),
    FOREIGN KEY (owner_user_id) REFERENCES users(user_id) ON DELETE SET NULL
);

-- Create Packages table
CREATE TABLE IF NOT EXISTS packages (
	package_id SERIAL PRIMARY KEY,
	price INT DEFAULT 5 NOT NULL
);

-- Create Package_Cards (junction table)
CREATE TABLE IF NOT EXISTS package_cards (
    package_id INT,
    card_id INT,
    PRIMARY KEY (package_id, card_id),
    FOREIGN KEY (package_id) REFERENCES packages(package_id) ON DELETE CASCADE,
    FOREIGN KEY (card_id) REFERENCES cards(card_id) ON DELETE CASCADE
);

INSERT INTO users (username, password) VALUES ('admin', '$2y$10$3Q');
INSERT INTO users (username, password) VALUES ('ortwinius', 'jnfjkewfhwe9491ß043+´1o3eqw0ewiq4r39wreqrew$2y$10$3Q');
