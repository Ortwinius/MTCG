DROP TABLE IF EXISTS cards;
DROP TABLE IF EXISTS users;

-- Create Users table
CREATE TABLE IF NOT EXISTS users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    auth_token VARCHAR(255),
    coin INT DEFAULT 20 NOT NULL,
    elo INT DEFAULT 100 NOT NULL
);

-- Create Cards table
CREATE TABLE IF NOT EXISTS cards (
    card_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    type VARCHAR(20) NOT NULL,
    element VARCHAR(20) NOT NULL,
    damage INT NOT NULL,
    owned_by INT,
    FOREIGN KEY (owned_by) REFERENCES users(user_id) ON DELETE SET NULL
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
INSERT INTO users (username, password) VALUES ('kienboec', 'AQQQAAAAAUAAagaAAAAEgeiwwwü&%rxCvbkausVauserATRu&asewa039853?=Q§$?§$)I$%§0');
INSERT INTO users (username, password) VALUES ('ortwinius', 'BBCCXEEQAAAAAUAAagaAAAAEgeiwwwü&%rxCvbkausVauserATRu&asewa039853?=Q§$?§$)I$%§0');

INSERT INTO cards (name, type, element, damage, owned_by) VALUES ('Goblin', 'Monster', 'Fire', 30, 2);
