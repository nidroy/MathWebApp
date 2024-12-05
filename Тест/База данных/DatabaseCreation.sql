-- Создание базы данных
CREATE DATABASE TestWebApplication;

-- Подключение к базе данных
\c TestWebApplication

-- Создание таблицы product
CREATE TABLE product (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    price DECIMAL(10, 2) NOT NULL
);

-- Создание таблицы counterparty
CREATE TABLE counterparty (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    phone VARCHAR(50),
    email VARCHAR(255)
);

-- Создание таблицы document_header
CREATE TABLE document_header (
    id SERIAL PRIMARY KEY,
    document_number VARCHAR(50) NOT NULL,
    counterparty_id INTEGER REFERENCES counterparty(id) ON DELETE SET NULL,
    document_date DATE NOT NULL,
    document_amount DECIMAL(10, 2) NOT NULL
);

-- Создание таблицы document_line
CREATE TABLE document_line (
    id SERIAL PRIMARY KEY,
    document_header_id INTEGER REFERENCES document_header(id) ON DELETE SET NULL,
    product_id INTEGER REFERENCES product(id) ON DELETE SET NULL,
    quantity INT NOT NULL,
    reserved_quantity INT NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    discount INT NOT NULL
);

-- Создание таблицы product_stock
CREATE TABLE product_stock (
    id SERIAL PRIMARY KEY,
    product_id INTEGER REFERENCES product(id) ON DELETE SET NULL,
    actual_quantity INT NOT NULL,
    reserved_quantity INT NOT NULL
);

-- Добавление новых полей для таблицы document_header
ALTER TABLE document_header
ADD COLUMN document_type VARCHAR(50) NOT NULL,
ADD COLUMN document_status VARCHAR(50) NOT NULL;