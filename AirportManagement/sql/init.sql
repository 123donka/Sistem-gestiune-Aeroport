-- SQL init script for AirportManagement
CREATE DATABASE IF NOT EXISTS `airportdb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `airportdb`;

CREATE TABLE IF NOT EXISTS `utilizatori` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nume` VARCHAR(255) NOT NULL,
  `username` VARCHAR(100) NOT NULL UNIQUE,
  `parola` VARCHAR(255) NOT NULL,
  `rol` ENUM('admin','operator') NOT NULL DEFAULT 'operator'
);

CREATE TABLE IF NOT EXISTS `zboruri` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `numar_zbor` VARCHAR(50),
  `companie_aeriana` VARCHAR(150),
  `tip_zbor` VARCHAR(50),
  `oras_origine` VARCHAR(255),
  `oras_destinatie` VARCHAR(255),
  `data_ora_programata` DATETIME,
  `data_ora_estimata` DATETIME,
  `data_ora_reala` DATETIME,
  `status` VARCHAR(50),
  `poarta_id` INT,
  `pista_id` INT,
  `numar_total_pasageri` INT,
  `observatii` TEXT,
  `creat_de` INT,
  `data_creare` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `data_modificare` DATETIME NULL,
  `cod` VARCHAR(50),
  `sursa` VARCHAR(255),
  `destinatie` VARCHAR(255),
  `plecare` DATETIME,
  `sosire` DATETIME,
  `CONSTRAINT fk_zboruri_creat_de FOREIGN KEY (creat_de) REFERENCES utilizatori(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS `pasageri` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nume` VARCHAR(255),
  `ticketnr` VARCHAR(100),
  `zborid` INT,
  `checkedin` TINYINT(1) DEFAULT 0,
  `boarded` TINYINT(1) DEFAULT 0,
  FOREIGN KEY (zborid) REFERENCES zboruri(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS `resurse` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nume` VARCHAR(255),
  `tip` VARCHAR(100),
  `disponibila` TINYINT(1) DEFAULT 1
);

CREATE TABLE IF NOT EXISTS `resurse_alocari` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `resursa_id` INT NOT NULL,
  `zbor_id` INT NOT NULL,
  `assigned_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (resursa_id) REFERENCES resurse(id) ON DELETE CASCADE,
  FOREIGN KEY (zbor_id) REFERENCES zboruri(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `alerte` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `mesaj` TEXT,
  `citita` TINYINT(1) DEFAULT 0,
  `data` DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS `logactivitati` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `utilizator` VARCHAR(255),
  `actiune` TEXT,
  `data` DATETIME DEFAULT CURRENT_TIMESTAMP
);
