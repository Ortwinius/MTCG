DROP TABLE IF EXISTS cards;
DROP TABLE IF EXISTS users;
DROP TABLE IF exists packages;
DROP TABLE IF exists package_cards;
DROP TABLE IF exists decks;
DROP TABLE IF exists deck_cards;

-- Create Users table WITHOUT STATS
CREATE TABLE IF NOT EXISTS users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    auth_token VARCHAR(255),
);

-- Create Users table WITH stats
CREATE TABLE IF NOT EXISTS users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    auth_token VARCHAR(255),
    coins INT DEFAULT 20 NOT NULL,
    elo INT DEFAULT 100 NOT NULL,
    wins INT DEFAULT 0,
    losses INT DEFAULT 0,
    name VARCHAR(50),
    bio VARCHAR(255),
    img VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS cards (
    card_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    type VARCHAR(20) NOT NULL,
    element VARCHAR(20) NOT NULL,
    damage INT NOT NULL,
    owned_by INT,
    FOREIGN KEY (owned_by) REFERENCES users(user_id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS packages (
	package_id SERIAL PRIMARY KEY,
	price INT DEFAULT 5 NOT NULL
);

CREATE TABLE IF NOT EXISTS package_cards (
    package_id INT,
    card_id UUID,
    PRIMARY KEY (package_id, card_id),
    FOREIGN KEY (package_id) REFERENCES packages(package_id) ON DELETE CASCADE,
    FOREIGN KEY (card_id) REFERENCES cards(card_id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS decks (
    deck_id INT PRIMARY KEY, 
    FOREIGN KEY (deck_id) REFERENCES users(user_id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS deck_cards(
    deck_id INT, 
    card_id UUID,
    PRIMARY KEY (deck_id, card_id), 
    FOREIGN KEY (deck_id) REFERENCES decks(deck_id) ON DELETE CASCADE,
    FOREIGN KEY (card_id) REFERENCES cards(card_id) ON DELETE CASCADE
);