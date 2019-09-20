
CREATE DATABASE `cloud_viz` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */;
use cloud_viz;

CREATE TABLE `canvas_resolution` (
  `x` tinyint(4) DEFAULT NULL,
  `y` tinyint(4) DEFAULT NULL,
  `viewValue` char(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `taginfo` (
  `id` char(36) NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  `alias` varchar(45) DEFAULT NULL,
  `units` varchar(45) DEFAULT NULL,
  `max` decimal(20,5) DEFAULT NULL,
  `min` decimal(20,5) DEFAULT NULL,
  `dataType` char(10) DEFAULT NULL,
  `source` varchar(45) DEFAULT NULL,
  `description` varchar(1000) DEFAULT NULL,
  `location` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `user` (
  `id` char(36) NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `password` varbinary(1000) NOT NULL,
  `roles` varchar(45) DEFAULT 'user',
  `note` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name_UNIQUE` (`name`),
  UNIQUE KEY `email_UNIQUE` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `background` (
  `id` char(36) NOT NULL,
  `graphicChartId` char(36) NOT NULL,
  `width` tinyint(4) DEFAULT NULL,
  `height` tinyint(4) DEFAULT NULL,
  `bgSizeOption` char(10) DEFAULT NULL,
  `image` mediumblob,
  `imgContentType` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `graphicChartId_UNIQUE` (`graphicChartId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `symbolinfo` (
  `id` char(36) NOT NULL,
  `graphicChartId` char(36) NOT NULL,
  `type` varchar(30) NOT NULL,
  `tagId` char(36) NOT NULL,
  `tagName` varchar(45) NOT NULL,
  `viewBox` varchar(45) NOT NULL,
  `viewBoxWidth` smallint(6) NOT NULL,
  `viewBoxHeight` smallint(6) NOT NULL,
  `positionXRatio` decimal(7,5) NOT NULL,
  `positionYRatio` decimal(7,5) NOT NULL,
  `widthRatio` decimal(7,5) NOT NULL,
  `strokeRGB` char(13) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

