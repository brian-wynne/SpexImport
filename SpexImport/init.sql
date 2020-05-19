DROP DATABASE IF EXISTS spex;
CREATE DATABASE spex;
USE spex;

-- Index tables
CREATE TABLE IF NOT EXISTS locales 
(
	localeid SMALLINT,
	isactive TINYINT,
	languagecode VARCHAR(5),
	countrycode VARCHAR(5),
	name TEXT
);

CREATE TABLE IF NOT EXISTS unitnames
(
	unitid INT,
	baseunitid INT,
	multiple DOUBLE
);

CREATE TABLE IF NOT EXISTS product 
(
	productid INT NOT NULL, 
	mfgid VARCHAR(16), 
	mfgpn VARCHAR(128) NOT NULL, 
	categoryid INT, 
	is_active CHAR(1), 
	equivalency TEXT, 
	create_date TIMESTAMP, 
	modify_date TIMESTAMP, 
	last_update TIMESTAMP
);

CREATE TABLE IF NOT EXISTS categorynames
(
	categoryid INT,
	name TEXT,
	localeid SMALLINT
);

CREATE TABLE IF NOT EXISTS headernames
(
	headerid INT,
	name TEXT,
	localeid SMALLINT
);

CREATE TABLE IF NOT EXISTS productfeaturebullets 
(
	uniqueid BIGINT, 
	productid INT, 
	localeid SMALLINT, 
	orderid SMALLINT, 
	text TEXT, 
	modifieddate TIMESTAMP
);

CREATE TABLE IF NOT EXISTS attributenames
(
	attributeid INT,
	name TEXT,
	localeid SMALLINT
);

-- Non-Index

CREATE TABLE IF NOT EXISTS productattributes 
(
	productid INT, 
	attributeid INT, 
	setnumber SMALLINT, 
	text TEXT, 
	absolutevalue DOUBLE, 
	unitid INT, 
	isabsolute SMALLINT, 
	isactive SMALLINT, 
	localeid SMALLINT, 
	type INT
);

CREATE TABLE IF NOT EXISTS productdescriptions 
(
	productid INT, 
	description TEXT, 
	isdefault CHAR(1), 
	type CHAR(1), 
	localeid SMALLINT
);

CREATE TABLE IF NOT EXISTS productlocales 
(
	productid INT, 
	isactive CHAR(1), 
	published TINYTEXT
);

CREATE TABLE IF NOT EXISTS productaccessories 
(
	productid INT, 
	accessoryid INT, 
	localeid SMALLINT
);

CREATE TABLE IF NOT EXISTS searchattributes 
(
	productid INT, 
	categoryid INT, 
	unknownid INT, 
	isactive SMALLINT, 
	localeid SMALLINT
);

CREATE TABLE IF NOT EXISTS productkeywords 
(
	productid INT, 
	text TEXT, 
	localeid SMALLINT
);